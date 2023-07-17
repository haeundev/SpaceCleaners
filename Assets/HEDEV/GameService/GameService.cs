using Proto.GameModeSystem;
using UnityEngine;

namespace Proto.Service
{
    public class GameService
    {
        public static readonly int FrameCountPerSec = Application.targetFrameRate;
        private static IGameModeService _gameModeService = default;
        public static IGameModeService GameModeService => _gameModeService ??= ZenjectService.Resolve<IGameModeService>();
    }
}