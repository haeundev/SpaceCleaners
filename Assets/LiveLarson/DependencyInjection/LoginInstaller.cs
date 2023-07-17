using Zenject;

namespace LiveLarson.DependencyInjection
{
    public class LoginInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CommonInstaller.Install(Container);
            // Container.Bind<IUiService>().WithId(GameMode.None).To<NoneUiService>().AsSingle().NonLazy();
            Container.Bind<InventorySaveService>().AsSingle().NonLazy();

        }
    }
}