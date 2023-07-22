using System;

namespace LiveLarson.GameMode
{
    public interface IGameModeService
    {
        event Action<Enums.GameMode> OnGameModeEnter;
        event Action<Enums.GameMode> OnGameModeExit;

        void TryEnterGameMode(Enums.GameMode gameMode);
        Enums.GameMode GetGameMode();
    }
}