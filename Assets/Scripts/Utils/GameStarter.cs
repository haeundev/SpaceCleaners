using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Editor
{
    [InitializeOnLoad]
    public static class GameStarter
    {
        private const string OUTER_SPACE = "Assets/Scenes/OuterSpaceScene.unity";
        private const string PLANET_JUNGLE = "Assets/Scenes/Planet Jungle.unity";

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
        
        [MenuItem("AlienBusters/Outer Space", false, 100)]
        private static void LoadOuterSpaceScene() => OpenScene(OUTER_SPACE);
        
        [MenuItem("AlienBusters/Planet Jungle", false, 101)]
        private static void LoadPlanetJungleScene() => OpenScene(PLANET_JUNGLE);

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