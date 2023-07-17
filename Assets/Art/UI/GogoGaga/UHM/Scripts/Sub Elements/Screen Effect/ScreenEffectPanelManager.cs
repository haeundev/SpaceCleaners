using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class ScreenEffectPanelManager : MonoBehaviour
    {
        AnimationController animCont;
        Coroutine EffectCorotuine;
        public GameObject effectGameobject;

        private void Awake()
        {
            animCont = GetComponent<AnimationController>();
        }

        public void ShowDamageEffect(float startingDelay, float displayTime = 0.2f)
        {
            EffectCorotuine = StartCoroutine(ShowingDamageEffect(startingDelay, displayTime));
        }

        public void ShowHealEffect(float startingDelay, float displayTime = 0.2f)
        {
            EffectCorotuine = StartCoroutine(ShowingDamageEffect(startingDelay, displayTime, true));
        }

        public void HideEffect()
        {
            if (EffectCorotuine != null)
                StopCoroutine(EffectCorotuine);

            if (animCont != null)
                animCont.Play("Idle");
        }


        IEnumerator ShowingDamageEffect(float startingDelay, float displayTime, bool heal = false)
        {
            yield return new WaitForSeconds(startingDelay);

            //DamageEffectPanel.gameObject.SetActive(true);

            if (animCont != null)
            {
                if (heal)
                    animCont.Play("Heal");
                else
                    animCont.Play("Damage");
            }

            if (displayTime < 0)
            {
                yield break;
            }

            yield return new WaitForSeconds(displayTime);

            if (animCont != null)
                animCont.Play("Idle");
        }
    }
}