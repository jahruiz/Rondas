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
        }
        public MainPageViewModel MainPageViewModel => _container.Resolve<MainPageViewModel>();
        public BajarRondaViewModel BajarRondaViewModel => _container.Resolve<BajarRondaViewModel>();
        public IniciarSesionViewModel IniciarSesionViewModel => _container.Resolve<IniciarSesionViewModel>();

        public HacerRondaViewModel HacerRondaViewModel => _container.Resolve<HacerRondaViewModel>();
    }
}
