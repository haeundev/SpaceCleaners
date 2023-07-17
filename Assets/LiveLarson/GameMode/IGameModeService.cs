using System;

namespace LiveLarson.GameMode
{
    public interface IGameModeService
    {
        event Action<Enums.GameMode> OnGameModeEnter;
        event Action<Enums.GameMode> OnGameModeExit;

        void EnterLoadingGameMode(Enums.GameMode gameMode);
        void EnterGameMode(Enums.GameMode gameMode);
        void LeaveGameMode(Enums.GameMode gameMode);
        void ResetGameMode();
        void ForceSetGameMode(Enums.GameMode gameMode);
        Enums.GameMode GetGameMode();
        Enums.GameMode GetNextGameMode();
        bool IsCurrentlyInBaseGameMode(Enums.GameMode mode);
        bool IsClassifiedAsBaseGameMode(Enums.GameMode mode);
    }
}