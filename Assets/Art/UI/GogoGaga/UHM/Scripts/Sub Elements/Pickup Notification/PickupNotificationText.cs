using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GogoGaga.UHM
{
    public class PickupNotificationText : MonoBehaviour
    {

        public TextMeshProUGUI pickuptext;
        public Image Icon;




        public void Set(string text, Sprite icon , float delay)
        {
            pickuptext.text = text;

            if (icon != null)
                Icon.sprite = icon;
            else
                Icon.gameObject.SetActive(false);

            Destroy(gameObject, delay);
        }


    }
}