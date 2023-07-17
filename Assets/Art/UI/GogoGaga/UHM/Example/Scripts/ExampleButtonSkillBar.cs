using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM {
    public class ExampleButtonSkillBar : MonoBehaviour, ExampleButtonInterface
    {
        Animator anim;

        public int valIncrease = 5;
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        int value;
        public void OnButtonPress()
        {
            value += valIncrease;
            UltimateHudManager.Instance.ShowSkillProgress("Progress", value);
            anim.Play("Press");
        }

        
    }
}