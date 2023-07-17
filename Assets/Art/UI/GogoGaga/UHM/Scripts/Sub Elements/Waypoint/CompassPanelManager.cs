using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GogoGaga.UHM
{
    public class CompassPanelManager : MonoBehaviour
    {
        public CompassWaypointUI uiPrefab;
        public RectTransform compassMarkerParent;
        public RectTransform Compass1;
        public RectTransform Compass2;



        Camera cam;
        void Start()
        {


            cam = UltimateHudManager.Instance.mainCam;
        }


        void Update()
        {
            UpdateUI();
        }


        public CompassWaypointUI CreateUI(WaypointWorldMarker marker)
        {
            CompassWaypointUI c = Instantiate(uiPrefab, compassMarkerParent);

            c.worldMarker = marker;

            c.parentRect = compassMarkerParent;

            c.compass = Compass1;

            return c;
        }

        void UpdateUI()
        {

            float Angle = UltimateHudManager.RemapValue((cam.transform.eulerAngles.y) % 360, 0f, 360f, -180, 180);

            //Debug.Log(Angle);\\\\\\\\\

            float XPos = Compass1.sizeDelta.x * (Angle / 360f);

            Compass1.anchoredPosition = new Vector2(XPos, 0);

            if (Angle <= 0)
                XPos += Compass2.sizeDelta.x;
            else
                XPos -= Compass2.sizeDelta.x;

            Compass2.anchoredPosition = new Vector2(XPos, 0);
        }
    }
}