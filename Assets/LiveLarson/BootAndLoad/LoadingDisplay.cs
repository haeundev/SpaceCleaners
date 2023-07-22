using UnityEngine;

namespace LiveLarson.BootAndLoad
{
    public class LoadingDisplay : MonoBehaviour
    {
        [SerializeField] private LoadingProgressBarDisplay _progressBarDisplay;
        private Transform _camTransform;

        public enum LoadingThemeType
        {
            GamePlayTip,
            SpaceDebrisInfo
        }

        public void UpdateProgress(float progress)
        {
            _progressBarDisplay.UpdateProgress(progress);
        }

        private void Start()
        {
            var mainCam = UnityEngine.Camera.main;
            if (mainCam == default)
                return;
            _camTransform = mainCam.transform;
        }

        private void Update()
        {
            if (_camTransform != default)
                transform.LookAt(_camTransform);
        }
    }
}