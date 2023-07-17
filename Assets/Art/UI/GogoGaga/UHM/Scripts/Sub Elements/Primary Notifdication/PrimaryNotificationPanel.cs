using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class PrimaryNotificationPanel : MonoBehaviour
    {

        public TextMeshProUGUI topTitleText;
        public TextMeshProUGUI bottomTitleText;
        public RectTransform bottomDivider;


        AnimationController AnimCont;
        CanvasGroup canvasGroup;

        private void Awake()
        {
            AnimCont = GetComponent<AnimationController>();
            canvasGroup = GetComponent<CanvasGroup>();
        }


        public void CreatePrimaryNotification(string topText, string bottomText, Color topTextColor, Color bottomTextColor)
        {


            topTitleText.text = topText;
            topTitleText.color = topTextColor;


            bottomTitleText.text = bottomText;
            bottomTitleText.color = bottomTextColor;

            StartCoroutine(SetBottomDivider());
        }

        IEnumerator SetBottomDivider()
        {
            yield return new WaitForEndOfFrame();
            RectTransform tRect = topTitleText.GetComponent<RectTransform>();
            RectTransform bRect = bottomTitleText.GetComponent<RectTransform>();

            float newWidth = 50;

            newWidth += Mathf.Max(tRect.sizeDelta.x, bRect.sizeDelta.x);

            newWidth = Mathf.Clamp(newWidth, 150, 800);


            bottomDivider.sizeDelta = new Vector2(newWidth, bottomDivider.sizeDelta.y);
        }


        public void Show()
        {
            if (AnimCont != null)
                AnimCont.Play("Show");

            if (canvasGroup != null)
                canvasGroup.alpha = 1;
        }

        public void Hide()
        {

            if (AnimCont != null)
                AnimCont.Play("Hide");

        }

    }
}