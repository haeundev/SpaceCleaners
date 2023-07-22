using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Editor
{
    [InitializeOnLoad]
    public static class GameStarter
    {
        private const string BOOTING = "Assets/Scenes/Booting.unity";
        private const string LOADING = "Assets/Scenes/Loading.unity";
        private const string OUTER_SPACE = "Assets/Scenes/OuterSpace.unity";
        private const string JUNGLE = "Assets/Scenes/JunglePlanet.unity";
        private const string MONUMENT = "Assets/Scenes/MonumentPlanet.unity";
        private const string CRYSTAL_DEPOT = "Assets/Scenes/CrystalDepot.unity";

        static GameStarter()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("Is editor closed or crashed while playing?");
            }
        }

        [MenuItem("AlienBusters/Start Game (Not implemented) %&r", false, 999)] //%&
        private static void StartGame() => StartGame_impl(true, false);
        
        private static void StartGame_impl(bool server, bool noLimit = false)
        {
            // if (SceneManager.GetActiveScene().isDirty)
            // {
            //     if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            //     {
            //         EditorApplication.isPlaying = false;
            //         return;
            //     }
            // }
            //
            // EditorPrefs.SetBool("IsUsedServers", server);
            // EditorPrefs.SetBool("NoLimit", noLimit);
            // EditorSceneManager.OpenScene(MasterScene);
            // EditorApplication.EnterPlaymode();
        }
        
        [MenuItem("AlienBusters/Booting", false, 98)]
        private static void LoadPlanetJungleScene() => OpenScene(BOOTING);
        [MenuItem("AlienBusters/Loading", false, 99)]
        private static void LoadLoadingScene() => OpenScene(LOADING);
        [MenuItem("AlienBusters/OuterSpace", false, 100)]
        private static void LoadOuterSpaceScene() => OpenScene(OUTER_SPACE);
        
        [MenuItem("AlienBusters/Jungle", false, 101)]
        private static void LoadJungleScene() => OpenScene(JUNGLE);
        [MenuItem("AlienBusters/Monument", false, 102)]
        private static void LoadMonumentScene() => OpenScene(MONUMENT);
        [MenuItem("AlienBusters/CrystalDepot", false, 103)]
        private static void LoadCrystalDepotScene() => OpenScene(CRYSTAL_DEPOT);

        private static void OpenScene(string scenePath)
        {
            if (SceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}