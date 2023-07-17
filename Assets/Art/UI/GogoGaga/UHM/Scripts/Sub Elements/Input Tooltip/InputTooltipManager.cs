using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class InputTooltipManager : MonoBehaviour
    {

        public TextMeshProUGUI tooltipText;

        public TMP_SpriteAsset keyboardLight;
        public TMP_SpriteAsset keyboardDark;

        public Image Shadow;
        AnimationController animCont;

        Coroutine HideCorotuine;

        private void Awake()
        {
            animCont = GetComponent<AnimationController>();
        }


        public void Set(string text)
        {
            tooltipText.text = text;

            StartCoroutine(SetShadow());

            if (animCont != null)
                animCont.Play("Show");

            if (HideCorotuine != null)
                StopCoroutine(HideCorotuine);
        }



        public void Hide(float hideTime = 0)
        {
            HideCorotuine = StartCoroutine(Hiding(hideTime));
        }

        IEnumerator SetShadow()
        {
            yield return new WaitForEndOfFrame();
            
            Shadow.rectTransform.sizeDelta = tooltipText.textBounds.extents * 2;
            Shadow.rectTransform.sizeDelta = new Vector2(Shadow.rectTransform.sizeDelta.x + 60, Shadow.rectTransform.sizeDelta.y + 50);
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