using TMPro;
using UnityEngine;

namespace LiveLarson.BootAndLoad
{
    public abstract class LoadingProgressBarDisplay : MonoBehaviour
    {
        [SerializeField] protected RectTransform progressBar;
        [SerializeField] protected TextMeshProUGUI progressText;

        private string _postfix;

        protected void Awake()
        {
            if (progressText == null)
                return;
            _postfix = progressText.text;
        }

        protected void OnEnable()
        {
            ResetPosition();
            ResetProgressText();
            ResetCheckPoints();
        }
        
        private void ResetPosition()
        {
            UpdateDisplay(0f);
        }

        private void ResetProgressText()
        {
            UpdateProgress(0f);
        }

        protected abstract void ResetCheckPoints();
        
        public void UpdateProgress(float normalized)
        {
            UpdateDisplay(normalized);
            
            if (progressText == null || progressBar == null)
                return;
            progressText.text = $"{normalized * 100.0f:0}{_postfix}";
        }

        protected abstract void UpdateDisplay(float normalized);
    }
}