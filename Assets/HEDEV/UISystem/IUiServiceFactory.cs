using Proto.Enums;

namespace Proto.UISystem
{
    public interface IUiServiceFactory
    {
        IUiService GetUiService(GameMode gameMode);
        void RegisterUiService(GameMode gameMode, IUiService uiService);
    }
}