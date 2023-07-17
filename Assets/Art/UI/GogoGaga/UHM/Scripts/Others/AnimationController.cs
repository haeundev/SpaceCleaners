using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class AnimationController : MonoBehaviour
    {
        Animator animCont;
        private void Awake()
        {
            animCont = GetComponent<Animator>();
            animCont.enabled = false;
        }

        public void Play(string name)
        {
            if (animCont == null)
                return;

            animCont.enabled = true;
            animCont.Play(name, 0);
        }



        public void SetTrigger(string name)
        {
            if (animCont == null)
                return;

            Debug.Log("sasa");
            animCont.enabled = true;
            animCont.SetTrigger(name);
        }

        public void StopAnimator()
        {
            // animCont.enabled = false;
        }

    }
}