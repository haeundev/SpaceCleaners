using Proto.Enums;
using Proto.UISystem;
using Zenject;

namespace Proto.Service
{
    public class NoneUiService : IUiService
    {
        public void Enter()
        {
            //SoundService.PlayBGM();
        }

        public void Exit()
        {
            
        }
        
        [Inject]
        public void Register(IUiServiceFactory factory)
        {
            factory.RegisterUiService(GameMode.None, this);
        }
    }
}