using UnityEngine;

namespace LiveLarson.BootAndLoad
{
    public class LoadingDisplay : MonoBehaviour
    {
        [SerializeField] private LoadingProgressBarDisplay _progressBarDisplay = null;

        
        public enum LoadingThemeType
        {
            GamePlayTip,
            SpaceDebrisInfo,
        }

        public string UnityPreviousSceneName { get; set; }
        public string UnitySceneName { get; set; }

        public void UpdateProgress(float progress)
        {
            _progressBarDisplay.UpdateProgress(progress);

        }
    }
}