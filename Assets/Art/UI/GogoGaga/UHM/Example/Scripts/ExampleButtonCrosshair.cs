using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class ExampleButtonCrosshair : MonoBehaviour , ExampleButtonInterface
    {
        public int Effect;

        Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void OnButtonPress()
        {
            anim.Play("Press");

            switch (Effect)
            {
                case 0:
                    UltimateHudManager.Instance.IdleEffectCrossHair();
                    break;

                case 1:
                    UltimateHudManager.Instance.HighlightEffectCrossHair();
                    break;

                case 2:
                    UltimateHudManager.Instance.ShootEffectCrossHair();
                    break;

                case 3:
                    UltimateHudManager.Instance.HitEfectCrossHair();
                    break;
            }
        }        
    }
}