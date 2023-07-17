using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GogoGaga.UHM
{
    public class CrosshairGraphics : MonoBehaviour
    {

        public Transform CrosshairParent;
        public Transform CrosshairHitParent;


        public void SetColor(Color newColor)
        {
            foreach (Image item in CrosshairParent.GetComponentsInChildren<Image>())
            {
                item.color = newColor;
            }

        }

        public void SetHitColor(Color newColor)
        {
            foreach (Image item in CrosshairHitParent.GetComponentsInChildren<Image>())
            {
                item.color = newColor;
            }

        }



    }
}