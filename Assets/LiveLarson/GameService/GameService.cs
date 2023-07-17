using LiveLarson.DependencyInjection;
using LiveLarson.GameMode;
using UnityEngine;

namespace LiveLarson.GameService
{
    public class GameService
    {
        public static readonly int FrameCountPerSec = Application.targetFrameRate;
        private static IGameModeService _gameModeService = default;
        public static IGameModeService GameModeService => _gameModeService ??= ZenjectService.Resolve<IGameModeService>();
    }
}