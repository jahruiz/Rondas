namespace RondasEcopetrol.Base
{
    using RondasEcopetrol.ViewModels;
    using Unity;
    public class ViewModelLocator
    {
        private readonly IUnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();

            _container.RegisterType<MainPageViewModel>();
            _container.RegisterType<BajarRondaViewModel>();
            _container.RegisterType<IniciarSesionViewModel>();
            _container.RegisterType<HacerRondaViewModel>();
<<<<<<< HEAD
            _container.RegisterType<CapturaEquipoViewModel>();
            _container.RegisterType<CapturaTareaViewModel>();
=======
            _container.RegisterType<CapturaDatos1ViewModel>();
            _container.RegisterType<CapturaDatos2ViewModel>();
            _container.RegisterType<EnviarRondaViewModel>();
            _container.RegisterType<CambiarContrasenaViewModel>();
>>>>>>> 154817767d09bb40001b0ffa14b99aba26aa8a6f
        }
        public MainPageViewModel MainPageViewModel => _container.Resolve<MainPageViewModel>();
        public BajarRondaViewModel BajarRondaViewModel => _container.Resolve<BajarRondaViewModel>();
        public IniciarSesionViewModel IniciarSesionViewModel => _container.Resolve<IniciarSesionViewModel>();

        public HacerRondaViewModel HacerRondaViewModel => _container.Resolve<HacerRondaViewModel>();

        public CapturaEquipoViewModel CapturaEquipoViewModel => _container.Resolve<CapturaEquipoViewModel>();

<<<<<<< HEAD
        public CapturaTareaViewModel CapturaTareaViewModel => _container.Resolve<CapturaTareaViewModel>();
=======
        public CapturaDatos2ViewModel CapturaDatos2ViewModel => _container.Resolve<CapturaDatos2ViewModel>();

        public EnviarRondaViewModel EnviarRondaViewModel => _container.Resolve<EnviarRondaViewModel>();

        public CambiarContrasenaViewModel CambiarContrasenaViewModel => _container.Resolve<CambiarContrasenaViewModel>();
>>>>>>> 154817767d09bb40001b0ffa14b99aba26aa8a6f
    }
}
