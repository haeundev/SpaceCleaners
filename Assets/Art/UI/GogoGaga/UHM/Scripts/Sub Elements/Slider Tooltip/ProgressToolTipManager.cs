using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class ProgressToolTipManager : MonoBehaviour
    {
        [Header("Setting")]
        public Color NormalColor = Color.white;
        public Color CompleteColor = Color.green;
        [Header("Refs")]
        public Slider slider;
        public TextMeshProUGUI titleText;


        AnimationController animCont;


        private void Start()
        {
            animCont = GetComponent<AnimationController>();
        }

        
        public void Show(float max,string text, bool autoHideOnComplete = true)
        {
            titleText.text = text;
            StopAllCoroutines();
            StartCoroutine(Showing(max, autoHideOnComplete));
        }


        public void Hide()
        {
            StopAllCoroutines();
            StartCoroutine(Hiding());
        }
        IEnumerator Hiding()
        {
            animCont.Play("Hide");

            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);
        }

        const float progressDelay = 0.2f;
        IEnumerator Showing(float _max, bool autoHide)
        {
            yield return new WaitForEndOfFrame();

            _max -= progressDelay;

            animCont.Play("Show");

            slider.maxValue = _max;
            slider.value = 0;

            Image i = slider.fillRect.GetComponent<Image>();

            i.color = NormalColor;
            titleText.color = NormalColor;

            yield return new WaitForSeconds(progressDelay);

            while (slider.value < _max)
            {
                yield return new WaitForEndOfFrame();
                slider.value += Time.deltaTime;
            }

            i.color = CompleteColor;
            titleText.color = CompleteColor;

            yield return new WaitForSeconds(1);

            animCont.Play("Hide");

            if (!autoHide)
                yield break;

            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);
        }
    }
}