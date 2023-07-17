using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class ExampleButtonMission : MonoBehaviour, ExampleButtonInterface
    {

        public int TaskNo = 0;
        public TaskProgressType taskType;
        
        Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void OnButtonPress()
        {
            UltimateHudManager.Instance.ChangeTaskStatus(taskType, TaskNo);
            anim.Play("Press");
        }


        void Update()
        {

        }
    }
}