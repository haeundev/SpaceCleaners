using LiveLarson.UISystem;
using Zenject;

namespace LiveLarson.DependencyInjection
{
    public class CommonInstaller : Installer<CommonInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<ZenjectService>().AsSingle().NonLazy();
            Container.Bind<GameService.GameService>().AsSingle().NonLazy();
            Container.Bind<UiServiceFactory>().AsSingle().NonLazy();
            //Container.Bind<InventorySaveService>().AsSingle().NonLazy();
            // Container.Bind<FirebaseFirestore>().AsSingle().NonLazy();
            Container.Bind<IUiService>().WithId(Enums.GameMode.None).To<NoneUiService>().AsSingle();
        }
    }
}