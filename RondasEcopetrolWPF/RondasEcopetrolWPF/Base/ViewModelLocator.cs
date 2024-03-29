﻿namespace RondasEcopetrolWPF.Base
{
    using RondasEcopetrolWPF.ViewModels;
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
            _container.RegisterType<CapturaDatos1ViewModel>();
            _container.RegisterType<CapturaDatos2ViewModel>();
            _container.RegisterType<EnviarRondaViewModel>();
            _container.RegisterType<CortarRondaViewModel>();
        }
        public MainPageViewModel MainPageViewModel => _container.Resolve<MainPageViewModel>();
        public BajarRondaViewModel BajarRondaViewModel => _container.Resolve<BajarRondaViewModel>();
        public IniciarSesionViewModel IniciarSesionViewModel => _container.Resolve<IniciarSesionViewModel>();
        public HacerRondaViewModel HacerRondaViewModel => _container.Resolve<HacerRondaViewModel>();
        public CapturaDatos1ViewModel CapturaDatos1ViewModel => _container.Resolve<CapturaDatos1ViewModel>();
        public CapturaDatos2ViewModel CapturaDatos2ViewModel => _container.Resolve<CapturaDatos2ViewModel>();
        public EnviarRondaViewModel EnviarRondaViewModel => _container.Resolve<EnviarRondaViewModel>();
        public CortarRondaViewModel CortarRondaViewModel => _container.Resolve<CortarRondaViewModel>();
    }
}