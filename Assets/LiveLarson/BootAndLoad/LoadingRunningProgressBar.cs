using UnityEngine;
using UnityEngine.UI;

namespace LiveLarson.BootAndLoad
{
    public class LoadingRunningProgressBar : LoadingProgressBarDisplay
    {
        [SerializeField] private Slider slider;

        protected override void ResetCheckPoints()
        {
        }

        protected override void UpdateDisplay(float normalized)
        {
            slider.value = normalized;
        }
    }
}