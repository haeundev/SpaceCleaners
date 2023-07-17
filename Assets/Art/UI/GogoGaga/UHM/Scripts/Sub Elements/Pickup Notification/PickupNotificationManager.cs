using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class PickupNotificationManager : MonoBehaviour
    {

        public PickupNotificationText textPrefab;
        public Transform listParent;


        public void Create(string text, Sprite icon = null, float delay = 0, float stayTime = 4)
        {
            StartCoroutine(CreateText(text, icon, delay,stayTime));
        }

        IEnumerator CreateText(string text, Sprite icon, float delay,float stayTime)
        {
            yield return new WaitForSeconds(delay);

            Instantiate(textPrefab, listParent).Set(text, icon,stayTime);
        }
    }
}