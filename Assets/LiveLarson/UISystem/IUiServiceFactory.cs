namespace LiveLarson.UISystem
{
    public interface IUiServiceFactory
    {
        IUiService GetUiService(Enums.GameMode gameMode);
        void RegisterUiService(Enums.GameMode gameMode, IUiService uiService);
    }
}