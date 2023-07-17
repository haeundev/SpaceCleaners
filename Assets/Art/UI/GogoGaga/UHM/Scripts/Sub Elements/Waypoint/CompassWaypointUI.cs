using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace GogoGaga.UHM
{
    public class CompassWaypointUI : MonoBehaviour
    {

        public WaypointWorldMarker worldMarker { get; set; }

        public Image Icon;
        public TextMeshProUGUI DistanceText;

        public RectTransform parentRect { get; set; }
        public RectTransform compass { get; set; }
        RectTransform myRectTr;
        Camera cam;
        CanvasGroup canvasGroup;
        void Start()
        {
            myRectTr = GetComponent<RectTransform>();
            cam = UltimateHudManager.Instance.mainCam;
            canvasGroup = GetComponent<CanvasGroup>();
        }


        void Update()
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            if(worldMarker == null)
            {
                Destroy(gameObject);
                return;
            }

            float Dis = Vector3.Distance(worldMarker.transform.position, cam.transform.position);

            Vector3 dir = (worldMarker.transform.position - cam.transform.position).normalized;
            dir.y = 0;

            Vector3 forward = cam.transform.forward;
            forward.y = 0;

            float Angle = UltimateHudManager.GetAngle(forward, dir, Vector3.up);

            //Angle = UltimateHudManager.RemapValue(Angle, 0, 180, -90, 90);

            //Debug.Log(Angle);

            float Xpos = compass.sizeDelta.x * (Angle / 90);

            float LimitLeftX = -parentRect.sizeDelta.x / 2 + myRectTr.sizeDelta.x;
            float LimitRightX = parentRect.sizeDelta.x / 2 - myRectTr.sizeDelta.x;

            Xpos = Mathf.Clamp(Xpos, LimitLeftX, LimitRightX);

            myRectTr.anchoredPosition = new Vector2(Xpos, myRectTr.anchoredPosition.y);



            //Visuals 

            Sprite iconSprite = worldMarker.WaypointProperties.onScreenImage;

            if (!UltimateHudManager.CheckIfMissingOrNull(iconSprite))
                Icon.sprite = iconSprite;

            float IconSize = worldMarker.WaypointProperties.onScreenSize * 25;

            Icon.rectTransform.sizeDelta = new Vector2(IconSize, IconSize);

            Icon.color = worldMarker.WaypointProperties.onScreenImageColor;



            TMP_FontAsset font = worldMarker.WaypointProperties.textMeshTextPreset;

            if (!UltimateHudManager.CheckIfMissingOrNull(font))
                DistanceText.font = font;

            DistanceText.color = worldMarker.WaypointProperties.distanceTextColor;

            DistanceText.text = Dis.ToString("f0") + "m";



            float lowestDis = worldMarker.WaypointProperties.HideWhenCloseDistance;
            float fading = worldMarker.WaypointProperties.HideFadingSmoothing;

            fading = Mathf.Clamp(fading, 0.1f, 10f);

            if (lowestDis > 0)
            {
                float last = lowestDis + fading;
                canvasGroup.alpha = UltimateHudManager.RemapValue(Dis, lowestDis, last, 0, 1);
            }
            else
            {
                canvasGroup.alpha = 1;
            }

        }
    }
}