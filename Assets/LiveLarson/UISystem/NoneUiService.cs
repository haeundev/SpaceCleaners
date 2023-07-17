namespace LiveLarson.UISystem
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
        
        public void Register(IUiServiceFactory factory)
        {
            factory.RegisterUiService(Enums.GameMode.None, this);
        }
    }
}