using Proto.Enums;
using Proto.Service;
using Proto.UISystem;
using Zenject;

namespace Proto.DependencyInjection
{
    public class CommonInstaller : Installer<CommonInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ZenjectService>().AsSingle().NonLazy();
            Container.Bind<GameService>().AsSingle().NonLazy();
            Container.Bind<UiServiceFactory>().AsSingle().NonLazy();
            //Container.Bind<InventorySaveService>().AsSingle().NonLazy();
            // Container.Bind<FirebaseFirestore>().AsSingle().NonLazy();
            Container.Bind<IUiService>().WithId(GameMode.None).To<NoneUiService>().AsSingle();
        }
    }
}