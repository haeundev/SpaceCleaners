using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GogoGaga.UHM
{
    public class SecondaryNotificationManager : MonoBehaviour
    {

        public TextMeshProUGUI topTitleText, bottomTitleText, tooltipText;
        public Image Icon;
        public Sprite defaultIconImage;

        AnimationController animCont;

        Coroutine HideCorotuine;

        private void Awake()
        {
            animCont = GetComponent<AnimationController>();
        }
        
        public void Create(string toptext, string bottomText, string tooltip, float hideDelay, Sprite icon, Color topTextColor, Color bottomTextColor)
        {
            if (string.IsNullOrEmpty(toptext) && string.IsNullOrEmpty(bottomText))
                return;

            if (HideCorotuine != null)
                StopCoroutine(HideCorotuine);

            topTitleText.text = toptext;
            topTitleText.color = topTextColor;

            if (string.IsNullOrEmpty(bottomText))
                bottomTitleText.gameObject.SetActive(false);
            else
                bottomTitleText.gameObject.SetActive(true);

            bottomTitleText.text = bottomText;
            bottomTitleText.color = bottomTextColor;

            tooltipText.text = tooltip;

            if (icon == null)
                Icon.sprite = defaultIconImage;
            else
                Icon.sprite = icon;

            StartCoroutine(Show());

            if (hideDelay > 0)
                Hide(hideDelay);
        }
        
        IEnumerator Show()
        {
            HorizontalLayoutGroup h = GetComponentInChildren<HorizontalLayoutGroup>();
            h.enabled = false;
            yield return new WaitForEndOfFrame();
            h.enabled = true;
            animCont.Play("Show");

        }

        public void Hide(float hideTime = 0)
        {
            HideCorotuine = StartCoroutine(Hiding(hideTime));
        }

        public IEnumerator Hiding(float startwait = 0.1f)
        {
            yield return new WaitForSeconds(startwait);

            animCont.Play("Hide");

            yield return new WaitForSeconds(1);

            gameObject.SetActive(false);
        }

    }
}