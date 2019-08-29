#
# Add-AppxDevPackage.ps1 is a PowerShell script designed to install app
# packages created by Visual Studio for developers.  To run this script from
# Explorer, right-click on its icon and choose "Run with PowerShell".
#
# Visual Studio supplies this script in the folder generated with its
# "Prepare Package" command.  The same folder will also contain the app
# package (a .appx file), the signing certificate (a .cer file), and a
# "Dependencies" subfolder containing all the framework packages used by the
# app.
#
# This script simplifies installing these packages by automating the
# following functions:
#   1. Find the app package and signing certificate in the script directory
#   2. Prompt the user to acquire a developer license and to install the
#      certificate if necessary
#   3. Find dependency packages that are applicable to the operating system's
#      CPU architecture
#   4. Install the package along with all applicable dependencies
#
# All command line parameters are reserved for use internally by the script.
# Users should launch this script from Explorer.
#

# .Link
# http://go.microsoft.com/fwlink/?LinkId=243053

param(
    [switch]$Force = $false,
    [switch]$GetDeveloperLicense = $false,
    [string]$CertificatePath = $null
)

$ErrorActionPreference = "Stop"

# The language resources for this script are placed in the
# "Add-AppDevPackage.resources" subfolder alongside the script.  Since the
# current working directory might not be the directory that contains the
# script, we need to create the full path of the resources directory to
# pass into Import-LocalizedData
$ScriptPath = $null
try
{
    $ScriptPath = (Get-Variable MyInvocation).Value.MyCommand.Path
    $ScriptDir = Split-Path -Parent $ScriptPath
}
catch {}

if (!$ScriptPath)
{
    PrintMessageAndExit $UiStrings.ErrorNoScriptPath $ErrorCodes.NoScriptPath
}

$LocalizedResourcePath = Join-Path $ScriptDir "Add-AppDevPackage.resources"
Import-LocalizedData -BindingVariable UiStrings -BaseDirectory $LocalizedResourcePath

$ErrorCodes = Data {
    ConvertFrom-StringData @'
    Success = 0
    NoScriptPath = 1
    NoPackageFound = 2
    ManyPackagesFound = 3
    NoCertificateFound = 4
    ManyCertificatesFound = 5
    BadCertificate = 6
    PackageUnsigned = 7
    CertificateMismatch = 8
    ForceElevate = 9
    LaunchAdminFailed = 10
    GetDeveloperLicenseFailed = 11
    InstallCertificateFailed = 12
    AddPackageFailed = 13
    ForceDeveloperLicense = 14
    CertUtilInstallFailed = 17
    CertIsCA = 18
    BannedEKU = 19
    NoBasicConstraints = 20
    NoCodeSigningEku = 21
    InstallCertificateCancelled = 22
    BannedKeyUsage = 23
    ExpiredCertificate = 24
'@
}

function PrintMessageAndExit($ErrorMessage, $ReturnCode)
{
    Write-Host $ErrorMessage
    if (!$Force)
    {
        Pause
    }
    exit $ReturnCode
}

#
# Warns the user about installing certificates, and presents a Yes/No prompt
# to confirm the action.  The default is set to No.
#
function ConfirmCertificateInstall
{
    $Answer = $host.UI.PromptForChoice(
                    "", 
                    $UiStrings.WarningInstallCert, 
                    [System.Management.Automation.Host.ChoiceDescription[]]@($UiStrings.PromptYesString, $UiStrings.PromptNoString), 
                    1)
    
    return $Answer -eq 0
}

#
# Validates whether a file is a valid certificate using CertUtil.
# This needs to be done before calling Get-PfxCertificate on the file, otherwise
# the user will get a cryptic "Password: " prompt for invalid certs.
#
function ValidateCertificateFormat($FilePath)
{
    # certutil -verify prints a lot of text that we don't need, so it's redirected to $null here
    certutil.exe -verify $FilePath > $null
    if ($LastExitCode -lt 0)
    {
        PrintMessageAndExit ($UiStrings.ErrorBadCertificate -f $FilePath, $LastExitCode) $ErrorCodes.BadCertificate
    }
    
    # Check if certificate is expired
    $cert = Get-PfxCertificate $FilePath
    if (($cert.NotBefore -gt (Get-Date)) -or ($cert.NotAfter -lt (Get-Date)))
    {
        PrintMessageAndExit ($UiStrings.ErrorExpiredCertificate -f $FilePath) $ErrorCodes.ExpiredCertificate
    }
}

#
# Verify that the developer certificate meets the following restrictions:
#   - The certificate must contain a Basic Constraints extension, and its
#     Certificate Authority (CA) property must be false.
#   - The certificate's Key Usage extension must be either absent, or set to
#     only DigitalSignature.
#   - The certificate must contain an Extended Key Usage (EKU) extension with
#     Code Signing usage.
#   - The certificate must NOT contain any other EKU except Code Signing and
#     Lifetime Signing.
#
# These restrictions are enforced to decrease security risks that arise from
# trusting digital certificates.
#
function CheckCertificateRestrictions
{
    Set-Variable -Name BasicConstraintsExtensionOid -Value "2.5.29.19" -Option Constant
    Set-Variable -Name KeyUsageExtensionOid -Value "2.5.29.15" -Option Constant
    Set-Variable -Name EkuExtensionOid -Value "2.5.29.37" -Option Constant
    Set-Variable -Name CodeSigningEkuOid -Value "1.3.6.1.5.5.7.3.3" -Option Constant
    Set-Variable -Name LifetimeSigningEkuOid -Value "1.3.6.1.4.1.311.10.3.13" -Option Constant

    $CertificateExtensions = (Get-PfxCertificate $CertificatePath).Extensions
    $HasBasicConstraints = $false
    $HasCodeSigningEku = $false

    foreach ($Extension in $CertificateExtensions)
    {
        # Certificate must contain the Basic Constraints extension
        if ($Extension.oid.value -eq $BasicConstraintsExtensionOid)
        {
            # CA property must be false
            if ($Extension.CertificateAuthority)
            {
                PrintMessageAndExit $UiStrings.ErrorCertIsCA $ErrorCodes.CertIsCA
            }
            $HasBasicConstraints = $true
        }

        # If key usage is present, it must be set to digital signature
        elseif ($Extension.oid.value -eq $KeyUsageExtensionOid)
        {
            if ($Extension.KeyUsages -ne "DigitalSignature")
            {
                PrintMessageAndExit ($UiStrings.ErrorBannedKeyUsage -f $Extension.KeyUsages) $ErrorCodes.BannedKeyUsage
            }
        }

        elseif ($Extension.oid.value -eq $EkuExtensionOid)
        {
            # Certificate must contain the Code Signing EKU
            $EKUs = $Extension.EnhancedKeyUsages.Value
            if ($EKUs -contains $CodeSigningEkuOid)
            {
                $HasCodeSigningEKU = $True
            }

            # EKUs other than code signing and lifetime signing are not allowed
            foreach ($EKU in $EKUs)
            {
                if ($EKU -ne $CodeSigningEkuOid -and $EKU -ne $LifetimeSigningEkuOid)
                {
                    PrintMessageAndExit ($UiStrings.ErrorBannedEKU -f $EKU) $ErrorCodes.BannedEKU
                }
            }
        }
    }

    if (!$HasBasicConstraints)
    {
        PrintMessageAndExit $UiStrings.ErrorNoBasicConstraints $ErrorCodes.NoBasicConstraints
    }
    if (!$HasCodeSigningEKU)
    {
        PrintMessageAndExit $UiStrings.ErrorNoCodeSigningEku $ErrorCodes.NoCodeSigningEku
    }
}

#
# Performs operations that require administrative privileges:
#   - Prompt the user to obtain a developer license
#   - Install the developer certificate (if -Force is not specified, also prompts the user to confirm)
#
function DoElevatedOperations
{
    if ($GetDeveloperLicense)
    {
        Write-Host $UiStrings.GettingDeveloperLicense

        if ($Force)
        {
            PrintMessageAndExit $UiStrings.ErrorForceDeveloperLicense $ErrorCodes.ForceDeveloperLicense
        }
        try
        {
            Show-WindowsDeveloperLicenseRegistration
        }
        catch
        {
            $Error[0] # Dump details about the last error
            PrintMessageAndExit $UiStrings.ErrorGetDeveloperLicenseFailed $ErrorCodes.GetDeveloperLicenseFailed
        }
    }

    if ($CertificatePath)
    {
        Write-Host $UiStrings.InstallingCertificate

        # Make sure certificate format is valid and usage constraints are followed
        ValidateCertificateFormat $CertificatePath
        CheckCertificateRestrictions

        # If -Force is not specified, warn the user and get consent
        if ($Force -or (ConfirmCertificateInstall))
        {
            # Add cert to store
            certutil.exe -addstore TrustedPeople $CertificatePath
            if ($LastExitCode -lt 0)
            {
                PrintMessageAndExit ($UiStrings.ErrorCertUtilInstallFailed -f $LastExitCode) $ErrorCodes.CertUtilInstallFailed
            }
        }
        else
        {
            PrintMessageAndExit $UiStrings.ErrorInstallCertificateCancelled $ErrorCodes.InstallCertificateCancelled
        }
    }
}

#
# Checks whether the machine is missing a valid developer license.
#
function CheckIfNeedDeveloperLicense
{
    $Result = $true
    try
    {
        $Result = (Get-WindowsDeveloperLicense | Where-Object { $_.IsValid } | Measure-Object).Count -eq 0
    }
    catch {}

    return $Result
}

#
# Launches an elevated process running the current script to perform tasks
# that require administrative privileges.  This function waits until the
# elevated process terminates, and checks whether those tasks were successful.
#
function LaunchElevated
{
    # Set up command line arguments to the elevated process
    $RelaunchArgs = '-ExecutionPolicy Unrestricted -file "' + $ScriptPath + '"'

    if ($Force)
    {
        $RelaunchArgs += ' -Force'
    }
    if ($NeedDeveloperLicense)
    {
        $RelaunchArgs += ' -GetDeveloperLicense'
    }
    if ($NeedInstallCertificate)
    {
        $RelaunchArgs += ' -CertificatePath "' + $DeveloperCertificatePath.FullName + '"'
    }

    # Launch the process and wait for it to finish
    try
    {
        $AdminProcess = Start-Process "$PsHome\PowerShell.exe" -Verb RunAs -ArgumentList $RelaunchArgs -PassThru
    }
    catch
    {
        $Error[0] # Dump details about the last error
        PrintMessageAndExit $UiStrings.ErrorLaunchAdminFailed $ErrorCodes.LaunchAdminFailed
    }

    while (!($AdminProcess.HasExited))
    {
        Start-Sleep -Seconds 2
    }

    # Check if all elevated operations were successful
    if ($NeedDeveloperLicense)
    {
        if (CheckIfNeedDeveloperLicense)
        {
            PrintMessageAndExit $UiStrings.ErrorGetDeveloperLicenseFailed $ErrorCodes.GetDeveloperLicenseFailed
        }
        else
        {
            Write-Host $UiStrings.AcquireLicenseSuccessful
        }
    }
    if ($NeedInstallCertificate)
    {
        $Signature = Get-AuthenticodeSignature $DeveloperPackagePath -Verbose
        if ($Signature.Status -ne "Valid")
        {
            PrintMessageAndExit ($UiStrings.ErrorInstallCertificateFailed -f $Signature.Status) $ErrorCodes.InstallCertificateFailed
        }
        else
        {
            Write-Host $UiStrings.InstallCertificateSuccessful
        }
    }
}

#
# Finds all applicable dependency packages according to OS architecture, and
# installs the developer package with its dependencies.  The expected layout
# of dependencies is:
#
# <current dir>
#     \Dependencies
#         <Architecture neutral dependencies>.appx\.msix
#         \x86
#             <x86 dependencies>.appx\.msix
#         \x64
#             <x64 dependencies>.appx\.msix
#         \arm
#             <arm dependencies>.appx\.msix
#         \arm64
#             <arm64 dependencies>.appx\.msix
#
function InstallPackageWithDependencies
{
    $DependencyPackagesDir = (Join-Path $ScriptDir "Dependencies")
    $DependencyPackages = @()
    if (Test-Path $DependencyPackagesDir)
    {
        # Get architecture-neutral dependencies
        $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "*.appx") | Where-Object { $_.Mode -NotMatch "d" }
        $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "*.msix") | Where-Object { $_.Mode -NotMatch "d" }

        $ProcessorArchitecture = $Env:Processor_Architecture

        # Getting $Env:Processor_Architecture on arm64 machines will return x86.  So check if the environment
        # variable "ProgramFiles(Arm)" is also set, if it is we know the actual processor architecture is arm64.
        # The value will also be x86 on amd64 machines when running the x86 version of PowerShell.
        if ($ProcessorArchitecture -eq "x86")
        {
            if (${Env:ProgramFiles(Arm)} -ne $null)
            {
                $ProcessorArchitecture = "arm64"
            }
            elseif (${Env:ProgramFiles(x86)} -ne $null)
            {
                $ProcessorArchitecture = "amd64"
            }
        }

        # Get architecture-specific dependencies
        if (($ProcessorArchitecture -eq "x86" -or $ProcessorArchitecture -eq "amd64" -or $ProcessorArchitecture -eq "arm64") -and (Test-Path (Join-Path $DependencyPackagesDir "x86")))
        {
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x86\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x86\*.msix") | Where-Object { $_.Mode -NotMatch "d" }
        }
        if (($ProcessorArchitecture -eq "amd64") -and (Test-Path (Join-Path $DependencyPackagesDir "x64")))
        {
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x64\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "x64\*.msix") | Where-Object { $_.Mode -NotMatch "d" }
        }
        if (($ProcessorArchitecture -eq "arm" -or $ProcessorArchitecture -eq "arm64") -and (Test-Path (Join-Path $DependencyPackagesDir "arm")))
        {
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "arm\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "arm\*.msix") | Where-Object { $_.Mode -NotMatch "d" }
        }
        if (($ProcessorArchitecture -eq "arm64") -and (Test-Path (Join-Path $DependencyPackagesDir "arm64")))
        {
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "arm64\*.appx") | Where-Object { $_.Mode -NotMatch "d" }
            $DependencyPackages += Get-ChildItem (Join-Path $DependencyPackagesDir "arm64\*.msix") | Where-Object { $_.Mode -NotMatch "d" }
        }
    }
    Write-Host $UiStrings.InstallingPackage

    $AddPackageSucceeded = $False
    try
    {
        if ($DependencyPackages.FullName.Count -gt 0)
        {
            Write-Host $UiStrings.DependenciesFound
            $DependencyPackages.FullName
            Add-AppxPackage -Path $DeveloperPackagePath.FullName -DependencyPath $DependencyPackages.FullName -ForceApplicationShutdown
        }
        else
        {
            Add-AppxPackage -Path $DeveloperPackagePath.FullName -ForceApplicationShutdown
        }
        $AddPackageSucceeded = $?
    }
    catch
    {
        $Error[0] # Dump details about the last error
    }

    if (!$AddPackageSucceeded)
    {
        if ($NeedInstallCertificate)
        {
            PrintMessageAndExit $UiStrings.ErrorAddPackageFailedWithCert $ErrorCodes.AddPackageFailed
        }
        else
        {
            PrintMessageAndExit $UiStrings.ErrorAddPackageFailed $ErrorCodes.AddPackageFailed
        }
    }
}

#
# Main script logic when the user launches the script without parameters.
#
function DoStandardOperations
{
    # Check for an .appx or .msix file in the script directory
    $PackagePath = Get-ChildItem (Join-Path $ScriptDir "*.appx") | Where-Object { $_.Mode -NotMatch "d" }
    if ($PackagePath -eq $null)
    {
        $PackagePath = Get-ChildItem (Join-Path $ScriptDir "*.msix") | Where-Object { $_.Mode -NotMatch "d" }
    }
    $PackageCount = ($PackagePath | Measure-Object).Count

    # Check for an .appxbundle or .msixbundle file in the script directory
    $BundlePath = Get-ChildItem (Join-Path $ScriptDir "*.appxbundle") | Where-Object { $_.Mode -NotMatch "d" }
    if ($BundlePath -eq $null)
    {
        $BundlePath = Get-ChildItem (Join-Path $ScriptDir "*.msixbundle") | Where-Object { $_.Mode -NotMatch "d" }
    }
    $BundleCount = ($BundlePath | Measure-Object).Count

    # Check for an .eappx or .emsix file in the script directory
    $EncryptedPackagePath = Get-ChildItem (Join-Path $ScriptDir "*.eappx") | Where-Object { $_.Mode -NotMatch "d" }
    if ($EncryptedPackagePath -eq $null)
    {
        $EncryptedPackagePath = Get-ChildItem (Join-Path $ScriptDir "*.emsix") | Where-Object { $_.Mode -NotMatch "d" }
    }
    $EncryptedPackageCount = ($EncryptedPackagePath | Measure-Object).Count

    # Check for an .eappxbundle or .emsixbundle file in the script directory
    $EncryptedBundlePath = Get-ChildItem (Join-Path $ScriptDir "*.eappxbundle") | Where-Object { $_.Mode -NotMatch "d" }
    if ($EncryptedBundlePath -eq $null)
    {
        $EncryptedBundlePath = Get-ChildItem (Join-Path $ScriptDir "*.emsixbundle") | Where-Object { $_.Mode -NotMatch "d" }
    }
    $EncryptedBundleCount = ($EncryptedBundlePath | Measure-Object).Count

    $NumberOfPackages = $PackageCount + $EncryptedPackageCount
    $NumberOfBundles = $BundleCount + $EncryptedBundleCount

    # There must be at least one package or bundle
    if ($NumberOfPackages + $NumberOfBundles -lt 1)
    {
        PrintMessageAndExit $UiStrings.ErrorNoPackageFound $ErrorCodes.NoPackageFound
    }
    # We must have exactly one bundle OR no bundle and exactly one package
    elseif ($NumberOfBundles -gt 1 -or
            ($NumberOfBundles -eq 0 -and $NumberOfpackages -gt 1))
    {
        PrintMessageAndExit $UiStrings.ErrorManyPackagesFound $ErrorCodes.ManyPackagesFound
    }

    # First attempt to install a bundle or encrypted bundle. If neither exists, fall back to packages and then encrypted packages
    if ($BundleCount -eq 1)
    {
        $DeveloperPackagePath = $BundlePath
        Write-Host ($UiStrings.BundleFound -f $DeveloperPackagePath.FullName)
    }
    elseif ($EncryptedBundleCount -eq 1)
    {
        $DeveloperPackagePath = $EncryptedBundlePath
        Write-Host ($UiStrings.EncryptedBundleFound -f $DeveloperPackagePath.FullName)
    }
    elseif ($PackageCount -eq 1)
    {
        $DeveloperPackagePath = $PackagePath
        Write-Host ($UiStrings.PackageFound -f $DeveloperPackagePath.FullName)
    }
    elseif ($EncryptedPackageCount -eq 1)
    {
        $DeveloperPackagePath = $EncryptedPackagePath
        Write-Host ($UiStrings.EncryptedPackageFound -f $DeveloperPackagePath.FullName)
    }
    
    # The package must be signed
    $PackageSignature = Get-AuthenticodeSignature $DeveloperPackagePath
    $PackageCertificate = $PackageSignature.SignerCertificate
    if (!$PackageCertificate)
    {
        PrintMessageAndExit $UiStrings.ErrorPackageUnsigned $ErrorCodes.PackageUnsigned
    }

    # Test if the package signature is trusted.  If not, the corresponding certificate
    # needs to be present in the current directory and needs to be installed.
    $NeedInstallCertificate = ($PackageSignature.Status -ne "Valid")

    if ($NeedInstallCertificate)
    {
        # List all .cer files in the script directory
        $DeveloperCertificatePath = Get-ChildItem (Join-Path $ScriptDir "*.cer") | Where-Object { $_.Mode -NotMatch "d" }
        $DeveloperCertificateCount = ($DeveloperCertificatePath | Measure-Object).Count

        # There must be exactly 1 certificate
        if ($DeveloperCertificateCount -lt 1)
        {
            PrintMessageAndExit $UiStrings.ErrorNoCertificateFound $ErrorCodes.NoCertificateFound
        }
        elseif ($DeveloperCertificateCount -gt 1)
        {
            PrintMessageAndExit $UiStrings.ErrorManyCertificatesFound $ErrorCodes.ManyCertificatesFound
        }

        Write-Host ($UiStrings.CertificateFound -f $DeveloperCertificatePath.FullName)

        # The .cer file must have the format of a valid certificate
        ValidateCertificateFormat $DeveloperCertificatePath

        # The package signature must match the certificate file
        if ($PackageCertificate -ne (Get-PfxCertificate $DeveloperCertificatePath))
        {
            PrintMessageAndExit $UiStrings.ErrorCertificateMismatch $ErrorCodes.CertificateMismatch
        }
    }

    $NeedDeveloperLicense = CheckIfNeedDeveloperLicense

    # Relaunch the script elevated with the necessary parameters if needed
    if ($NeedDeveloperLicense -or $NeedInstallCertificate)
    {
        Write-Host $UiStrings.ElevateActions
        if ($NeedDeveloperLicense)
        {
            Write-Host $UiStrings.ElevateActionDevLicense
        }
        if ($NeedInstallCertificate)
        {
            Write-Host $UiStrings.ElevateActionCertificate
        }

        $IsAlreadyElevated = ([Security.Principal.WindowsIdentity]::GetCurrent().Groups.Value -contains "S-1-5-32-544")
        if ($IsAlreadyElevated)
        {
            if ($Force -and $NeedDeveloperLicense)
            {
                PrintMessageAndExit $UiStrings.ErrorForceDeveloperLicense $ErrorCodes.ForceDeveloperLicense
            }
            if ($Force -and $NeedInstallCertificate)
            {
                Write-Warning $UiStrings.WarningInstallCert
            }
        }
        else
        {
            if ($Force)
            {
                PrintMessageAndExit $UiStrings.ErrorForceElevate $ErrorCodes.ForceElevate
            }
            else
            {
                Write-Host $UiStrings.ElevateActionsContinue
                Pause
            }
        }

        LaunchElevated
    }

    InstallPackageWithDependencies
}

#
# Main script entry point
#
if ($GetDeveloperLicense -or $CertificatePath)
{
    DoElevatedOperations
}
else
{
    DoStandardOperations
    PrintMessageAndExit $UiStrings.Success $ErrorCodes.Success
}

# SIG # Begin signature block
# MIIhkwYJKoZIhvcNAQcCoIIhhDCCIYACAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCD6ikziRRosq7X2
# kxnfJWDQ4+LPlbpxnQV8CqNWsL29HaCCC3YwggT+MIID5qADAgECAhMzAAACz20s
# xXyqZabYAAAAAALPMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwHhcNMTkwNTAyMjEyNTQyWhcNMjAwNTAyMjEyNTQyWjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQC5WvO1XVj5Qw+VAXn6dOWwyrU48JLK7JuZQh066rWPdXWDqRs2/0CWjtd1CtGW
# rsezdxI5gtnuSHU9R61TOTxp0crY8D8hWkw66Chnw30gU0uci7yT+SBG8FCCym9q
# Tlbx1EOu2Onbvnx3fUZzAzpk6fYtyyVRDZDJFlkpDBUgY6i4T3AknQHlaZRJydZY
# qJuJuC/gbhunzJ11CKbIMWUPo+291saTjGYiv0hXHYyhweatkIEpK4pi9MKh28jB
# eSN0NwiSZeNXAeRPoQr1aJbWCYBKBdyuMNexWvsd+YTeksvywPjqJm7Tc37FZqUF
# rMTrENoMXHw2mZxZicaUR/a7AgMBAAGjggF9MIIBeTAfBgNVHSUEGDAWBgorBgEE
# AYI3PQYBBggrBgEFBQcDAzAdBgNVHQ4EFgQUN/NN7E2vLAQxqRCIC1jAbwO8VQgw
# VAYDVR0RBE0wS6RJMEcxLTArBgNVBAsTJE1pY3Jvc29mdCBJcmVsYW5kIE9wZXJh
# dGlvbnMgTGltaXRlZDEWMBQGA1UEBRMNMjMwODY1KzQ1NDI0NDAfBgNVHSMEGDAW
# gBTm/F97uyIAWORyTrX0IXQjMubvrDBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8v
# Y3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNDb2RTaWdQQ0Ff
# MjAxMC0wNy0wNi5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRw
# Oi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY0NvZFNpZ1BDQV8yMDEw
# LTA3LTA2LmNydDAMBgNVHRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IBAQCqMI8I
# dLEoiAqBDMw46rHAR5p98xk1puiL6RYE5MMy4Sn3zLjwxsMC1bCNC0H+XKkDhiX1
# JJ+wBMRpCieUBLIJ3BmvuwGLkuTYcgQHcoHTicrXeZhB5aAp+5WR2fiMQCNwmrL2
# ywp1C70cctf/mCAB3sDN0NOl/2sMi/mFaSIVCddO8sHp1Hin0eNG9j2pBmYRo54v
# 7+bTxYApyPrTt5gN3KTOhGjrm/sUgyhtIleaRr43ccTs7TiPTW4TVDmHxvQeQd+6
# hyjawsACpYBSPL2a4mbAXsNW6GUdzNTs9U0FrvOblnvyqTuu4Ls7QpvsILrqHG30
# aiSPMQE8igpYYWdYMIIGcDCCBFigAwIBAgIKYQxSTAAAAAAAAzANBgkqhkiG9w0B
# AQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNV
# BAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEyMDAG
# A1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIwMTAw
# HhcNMTAwNzA2MjA0MDE3WhcNMjUwNzA2MjA1MDE3WjB+MQswCQYDVQQGEwJVUzET
# MBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMV
# TWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYDVQQDEx9NaWNyb3NvZnQgQ29kZSBT
# aWduaW5nIFBDQSAyMDEwMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA
# 6Q5kUHlntcTj/QkATJ6UrPdWaOpE2M/FWE+ppXZ8bUW60zmStKQe+fllguQX0o/9
# RJwI6GWTzixVhL99COMuK6hBKxi3oktuSUxrFQfe0dLCiR5xlM21f0u0rwjYzIjW
# axeUOpPOJj/s5v40mFfVHV1J9rIqLtWFu1k/+JC0K4N0yiuzO0bj8EZJwRdmVMkc
# vR3EVWJXcvhnuSUgNN5dpqWVXqsogM3Vsp7lA7Vj07IUyMHIiiYKWX8H7P8O7YAS
# NUwSpr5SW/Wm2uCLC0h31oVH1RC5xuiq7otqLQVcYMa0KlucIxxfReMaFB5vN8sZ
# M4BqiU2jamZjeJPVMM+VHwIDAQABo4IB4zCCAd8wEAYJKwYBBAGCNxUBBAMCAQAw
# HQYDVR0OBBYEFOb8X3u7IgBY5HJOtfQhdCMy5u+sMBkGCSsGAQQBgjcUAgQMHgoA
# UwB1AGIAQwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQY
# MBaAFNX2VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6
# Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1
# dF8yMDEwLTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0
# dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3J0MIGdBgNVHSAEgZUwgZIwgY8GCSsGAQQBgjcuAzCBgTA9Bggr
# BgEFBQcCARYxaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL1BLSS9kb2NzL0NQUy9k
# ZWZhdWx0Lmh0bTBABggrBgEFBQcCAjA0HjIgHQBMAGUAZwBhAGwAXwBQAG8AbABp
# AGMAeQBfAFMAdABhAHQAZQBtAGUAbgB0AC4gHTANBgkqhkiG9w0BAQsFAAOCAgEA
# GnTvV08pe8QWhXi4UNMi/AmdrIKX+DT/KiyXlRLl5L/Pv5PI4zSp24G43B4AvtI1
# b6/lf3mVd+UC1PHr2M1OHhthosJaIxrwjKhiUUVnCOM/PB6T+DCFF8g5QKbXDrMh
# KeWloWmMIpPMdJjnoUdD8lOswA8waX/+0iUgbW9h098H1dlyACxphnY9UdumOUjJ
# N2FtB91TGcun1mHCv+KDqw/ga5uV1n0oUbCJSlGkmmzItx9KGg5pqdfcwX7RSXCq
# tq27ckdjF/qm1qKmhuyoEESbY7ayaYkGx0aGehg/6MUdIdV7+QIjLcVBy78dTMgW
# 77Gcf/wiS0mKbhXjpn92W9FTeZGFndXS2z1zNfM8rlSyUkdqwKoTldKOEdqZZ14y
# jPs3hdHcdYWch8ZaV4XCv90Nj4ybLeu07s8n07VeafqkFgQBpyRnc89NT7beBVaX
# evfpUk30dwVPhcbYC/GO7UIJ0Q124yNWeCImNr7KsYxuqh3khdpHM2KPpMmRM19x
# HkCvmGXJIuhCISWKHC1g2TeJQYkqFg/XYTyUaGBS79ZHmaCAQO4VgXc+nOBTGBpQ
# HTiVmx5mMxMnORd4hzbOTsNfsvU9R1O24OXbC2E9KteSLM43Wj5AQjGkHxAIwlac
# vyRdUQKdannSF9PawZSOB3slcUSrBmrm1MbfI5qWdcUxghVzMIIVbwIBATCBlTB+
# MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVk
# bW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYDVQQDEx9N
# aWNyb3NvZnQgQ29kZSBTaWduaW5nIFBDQSAyMDEwAhMzAAACz20sxXyqZabYAAAA
# AALPMA0GCWCGSAFlAwQCAQUAoIHGMBkGCSqGSIb3DQEJAzEMBgorBgEEAYI3AgEE
# MBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3AgEVMC8GCSqGSIb3DQEJBDEiBCAT
# tUyCW/+3SOQgIHkiHXOai1PTuTv9gFfLdUeLv1xnDjBaBgorBgEEAYI3AgEMMUww
# SqAsgCoAYQBkAGQALQBhAHAAcABkAGUAdgBwAGEAYwBrAGEAZwBlAC4AcABzADGh
# GoAYaHR0cDovL3d3dy5taWNyb3NvZnQuY29tMA0GCSqGSIb3DQEBAQUABIIBAIgU
# z+7Esl6nvhRLwlxKpY/GuNHbTZyORef1V0IhQ9cMEsXxjTnnB93vZX3XYR9VQq26
# EulITHYBQj/1XIEUhAVWX2CmGsn5cobUIuPgPcG1MRZdhvf84YY9y+3m9CcBjU/L
# oGmaOIlkAVd4OeSlrfLPDnFqlJ5F6CAU/Kd+9Y0LtLEXniCA7jcVy4CH5WL3/+Js
# dfb2JM1AGUBuEmmLwpNayMrXQAHh0ipZcoHcU1eIjNhebwNffdns0qG6qtQUsknY
# dFd0PVJvvo/2vtJgF3+RU50ef9JYcqup7ZnEeXNX38FmX99qRjPdOI57b7OPrd+Z
# X1h9SuJYYZaTwPHIKbOhghLlMIIS4QYKKwYBBAGCNwMDATGCEtEwghLNBgkqhkiG
# 9w0BBwKgghK+MIISugIBAzEPMA0GCWCGSAFlAwQCAQUAMIIBUQYLKoZIhvcNAQkQ
# AQSgggFABIIBPDCCATgCAQEGCisGAQQBhFkKAwEwMTANBglghkgBZQMEAgEFAAQg
# MVYIwCLUihX0FnJlZs5tfkatsYsKEM6x/U98OAebdJoCBlzJ5u2oixgTMjAxOTA3
# MTEwMTEyMjAuMTgzWjAEgAIB9KCB0KSBzTCByjELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjElMCMGA1UECxMcTWljcm9zb2Z0IEFtZXJpY2EgT3Bl
# cmF0aW9uczEmMCQGA1UECxMdVGhhbGVzIFRTUyBFU046NDlCQy1FMzdBLTIzM0Mx
# JTAjBgNVBAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2Wggg48MIIE8TCC
# A9mgAwIBAgITMwAAAO74xfU2O4EcjAAAAAAA7jANBgkqhkiG9w0BAQsFADB8MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNy
# b3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDAeFw0xODEwMjQyMTE0MTVaFw0yMDAx
# MTAyMTE0MTVaMIHKMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQ
# MA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9u
# MSUwIwYDVQQLExxNaWNyb3NvZnQgQW1lcmljYSBPcGVyYXRpb25zMSYwJAYDVQQL
# Ex1UaGFsZXMgVFNTIEVTTjo0OUJDLUUzN0EtMjMzQzElMCMGA1UEAxMcTWljcm9z
# b2Z0IFRpbWUtU3RhbXAgU2VydmljZTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCC
# AQoCggEBAKP3VjWE89005KOTSGWykmm4oVHC+1EeCL6lhK/zSEabbAzcj8OCSxIR
# sRFc/yKzPsLet9qsk0WR9Ir/EhZv0i1PyCB2BSE3S5/U/Vd+qfSRIaa5jGm/6SG4
# mAwgt1u0xb9Pw2x0ho0Q/Gj+jQyFZFvwIy3aJAsMULSMj2cCDZc3tB8dr4vN1YeM
# HnDT7t8iyf5UrFf0vs1VEvuboCnhy1YCLk3Lciq/TIwUmoQqJd8RzoCB6hVgYlei
# DhoJdKUYremaiFYsza/SaV0v8YgalhE4UQ6Xvd1sEVLoMqWVF6QnSal+OBqEHejl
# 582fbJwvRGUTXYlagD08Wjd+/V/Gqy0CAwEAAaOCARswggEXMB0GA1UdDgQWBBQm
# d804K0V+vdMzZSj3z+No8g5lTTAfBgNVHSMEGDAWgBTVYzpcijGQ80N7fEYbxTNo
# WoVtVTBWBgNVHR8ETzBNMEugSaBHhkVodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20v
# cGtpL2NybC9wcm9kdWN0cy9NaWNUaW1TdGFQQ0FfMjAxMC0wNy0wMS5jcmwwWgYI
# KwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5odHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20vcGtpL2NlcnRzL01pY1RpbVN0YVBDQV8yMDEwLTA3LTAxLmNydDAMBgNVHRMB
# Af8EAjAAMBMGA1UdJQQMMAoGCCsGAQUFBwMIMA0GCSqGSIb3DQEBCwUAA4IBAQCR
# P+LRFRsgIRzye7zkm1kD34yaXGLEHsHF006WIWD1A+XPg5iUJ01d+BpO7KRZ4yOH
# LCzIcvjEJsNzBlqn/+3+7qGKJ8VlaMj2Fo9aTnLlN/GRgfwY5G4szZSZtdpgoGBf
# djxkRkX/GRYMQCV/Z7svOvwfvxhsaJCMrZojb/2ppj4pslceqoOMoxq1lePXC3jJ
# pNELHLvNxc/jE0VXQw8fwReGJnEvrSduEfOms3JdkKPdoCt0h/d3JtkGj19N/R4P
# eww08CVUIeRuSx+rfZ2aAYAC3/apphn5JNKoFC44+iFZM/6bZ8oQd09mTnyXtwRa
# pQSzVjm2L1Qh4lzFAahGMIIGcTCCBFmgAwIBAgIKYQmBKgAAAAAAAjANBgkqhkiG
# 9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAO
# BgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEy
# MDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIw
# MTAwHhcNMTAwNzAxMjEzNjU1WhcNMjUwNzAxMjE0NjU1WjB8MQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGlt
# ZS1TdGFtcCBQQ0EgMjAxMDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB
# AKkdDbx3EYo6IOz8E5f1+n9plGt0VBDVpQoAgoX77XxoSyxfxcPlYcJ2tz5mK1vw
# FVMnBDEfQRsalR3OCROOfGEwWbEwRA/xYIiEVEMM1024OAizQt2TrNZzMFcmgqNF
# DdDq9UeBzb8kYDJYYEbyWEeGMoQedGFnkV+BVLHPk0ySwcSmXdFhE24oxhr5hoC7
# 32H8RsEnHSRnEnIaIYqvS2SJUGKxXf13Hz3wV3WsvYpCTUBR0Q+cBj5nf/VmwAOW
# RH7v0Ev9buWayrGo8noqCjHw2k4GkbaICDXoeByw6ZnNPOcvRLqn9NxkvaQBwSAJ
# k3jN/LzAyURdXhacAQVPIk0CAwEAAaOCAeYwggHiMBAGCSsGAQQBgjcVAQQDAgEA
# MB0GA1UdDgQWBBTVYzpcijGQ80N7fEYbxTNoWoVtVTAZBgkrBgEEAYI3FAIEDB4K
# AFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAfBgNVHSME
# GDAWgBTV9lbLj+iiXGJo0T2UkFvXzpoYxDBWBgNVHR8ETzBNMEugSaBHhkVodHRw
# Oi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2NybC9wcm9kdWN0cy9NaWNSb29DZXJB
# dXRfMjAxMC0wNi0yMy5jcmwwWgYIKwYBBQUHAQEETjBMMEoGCCsGAQUFBzAChj5o
# dHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY1Jvb0NlckF1dF8y
# MDEwLTA2LTIzLmNydDCBoAYDVR0gAQH/BIGVMIGSMIGPBgkrBgEEAYI3LgMwgYEw
# PQYIKwYBBQUHAgEWMWh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9QS0kvZG9jcy9D
# UFMvZGVmYXVsdC5odG0wQAYIKwYBBQUHAgIwNB4yIB0ATABlAGcAYQBsAF8AUABv
# AGwAaQBjAHkAXwBTAHQAYQB0AGUAbQBlAG4AdAAuIB0wDQYJKoZIhvcNAQELBQAD
# ggIBAAfmiFEN4sbgmD+BcQM9naOhIW+z66bM9TG+zwXiqf76V20ZMLPCxWbJat/1
# 5/B4vceoniXj+bzta1RXCCtRgkQS+7lTjMz0YBKKdsxAQEGb3FwX/1z5Xhc1mCRW
# S3TvQhDIr79/xn/yN31aPxzymXlKkVIArzgPF/UveYFl2am1a+THzvbKegBvSzBE
# JCI8z+0DpZaPWSm8tv0E4XCfMkon/VWvL/625Y4zu2JfmttXQOnxzplmkIz/amJ/
# 3cVKC5Em4jnsGUpxY517IW3DnKOiPPp/fZZqkHimbdLhnPkd/DjYlPTGpQqWhqS9
# nhquBEKDuLWAmyI4ILUl5WTs9/S/fmNZJQ96LjlXdqJxqgaKD4kWumGnEcua2A5H
# moDF0M2n0O99g/DhO3EJ3110mCIIYdqwUB5vvfHhAN/nMQekkzr3ZUd46PioSKv3
# 3nJ+YWtvd6mBy6cJrDm77MbL2IK0cs0d9LiFAR6A+xuJKlQ5slvayA1VmXqHczsI
# 5pgt6o3gMy4SKfXAL1QnIffIrE7aKLixqduWsqdCosnPGUFN4Ib5KpqjEWYw07t0
# MkvfY3v1mYovG8chr1m1rtxEPJdQcdeh0sVV42neV8HR3jDA/czmTfsNv11P6Z0e
# GTgvvM9YBS7vDaBQNdrvCScc1bN+NR4Iuto229Nfj950iEkSoYICzjCCAjcCAQEw
# gfihgdCkgc0wgcoxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAw
# DgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# JTAjBgNVBAsTHE1pY3Jvc29mdCBBbWVyaWNhIE9wZXJhdGlvbnMxJjAkBgNVBAsT
# HVRoYWxlcyBUU1MgRVNOOjQ5QkMtRTM3QS0yMzNDMSUwIwYDVQQDExxNaWNyb3Nv
# ZnQgVGltZS1TdGFtcCBTZXJ2aWNloiMKAQEwBwYFKw4DAhoDFQA4PawmQSKPPMbG
# VdoGCvkybZMuaaCBgzCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEw
# MA0GCSqGSIb3DQEBBQUAAgUA4NCtUjAiGA8yMDE5MDcxMTAyMzA0MloYDzIwMTkw
# NzEyMDIzMDQyWjB3MD0GCisGAQQBhFkKBAExLzAtMAoCBQDg0K1SAgEAMAoCAQAC
# AiKTAgH/MAcCAQACAhIFMAoCBQDg0f7SAgEAMDYGCisGAQQBhFkKBAIxKDAmMAwG
# CisGAQQBhFkKAwKgCjAIAgEAAgMHoSChCjAIAgEAAgMBhqAwDQYJKoZIhvcNAQEF
# BQADgYEAjq45+pT+RUvMtxAY/nCTlvJiQirquYBoOSH7xvIIV+arCi1if/KYqQmj
# okxv1LHmPXHDUCsbY5MFQtfN6w9c9EEiACyUelv/bUz2nlNxesco2CfVELJET9/A
# GhnrO2KjEvbZpbu4XsbE4xHWSMvOlDhVzQOskcFSW8tR/2UvWhQxggMNMIIDCQIB
# ATCBkzB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UE
# BxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYD
# VQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMAITMwAAAO74xfU2O4Ec
# jAAAAAAA7jANBglghkgBZQMEAgEFAKCCAUowGgYJKoZIhvcNAQkDMQ0GCyqGSIb3
# DQEJEAEEMC8GCSqGSIb3DQEJBDEiBCCTjqkKvRJTUBa4tAJSJB98uJgAUt9qruqB
# ENuvPcHJJzCB+gYLKoZIhvcNAQkQAi8xgeowgecwgeQwgb0EID+uxiK3Pvv4tLF3
# kzU+klEKnwshAfKZKxIM0SMvEC7DMIGYMIGApH4wfDELMAkGA1UEBhMCVVMxEzAR
# BgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1p
# Y3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3Rh
# bXAgUENBIDIwMTACEzMAAADu+MX1NjuBHIwAAAAAAO4wIgQgbvfgPRu5Z5aBJuOH
# DCxurgxtZWPIQQ22w4lbHvJg7TUwDQYJKoZIhvcNAQELBQAEggEAMl3st/jPN+l1
# j1HTSmS6nfMGLR0qGkOXgJAbuAwOQ6LqLeOBQlv60RUcwWOxK0CEUAhGUZYC7MEd
# e9NYB/f1NTJyypFd6bs2VrFeNvVES2+NLdZdd9u3T36woqB1gWxg2Z0cTCMd8/zg
# Y7GEL0UOsPKlU2DSsO32izlMBIrNcXeCwYpKMdXmy/9g+zvSZ/2GYfPdO+VcDWTy
# SeTFiI7jYfEDzfPoRUEyfmvC6VJ7wDQWN3FCpR301J9uP+8Sr+DVdejxUfr38WrB
# GdoM2zo0BBK3BXHYBv3uYfniyeXdWTDOiBss+VAs3JYQMR1navrZ/qOEmsxtqfCh
# Ml+wOPTRtw==
# SIG # End signature block
