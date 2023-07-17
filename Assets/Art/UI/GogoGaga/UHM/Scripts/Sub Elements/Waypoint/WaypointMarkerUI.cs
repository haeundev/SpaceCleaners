using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class WaypointMarkerUI : MonoBehaviour
    {

        public Image onScreenImage;
        public Image offScreenImage;
        public Transform offScreenParent;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI distanceText;

        WaypointWorldMarker worldMarker;

        RectTransform rectT;
        CanvasGroup canvasGroup;


        void Start()
        {
            rectT = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

        }


        public void SetMarker(WaypointWorldMarker marker)
        {
            worldMarker = marker;
        }

        void Update()
        {
            if (worldMarker == null)
            {
                Destroy(gameObject);
                return;
            }
            UpdateUI();
        }

        void UpdateUI()
        {
            Camera myCam = UltimateHudManager.Instance.mainCam;

            Vector3 offsetFromEdges = worldMarker.WaypointProperties.UIOffsetFromEdge;

            float minX = rectT.sizeDelta.x / 2 + offsetFromEdges.x;
            float maxX = Screen.width - minX;

            float minY = rectT.sizeDelta.y / 2 + offsetFromEdges.y;
            float maxY = Screen.height - minY;

            Vector3 targetPos = worldMarker.transform.position + worldMarker.WaypointProperties.PostionOffset;

            Vector2 newPos = myCam.WorldToScreenPoint(targetPos);

            float dotProduct = Vector3.Dot(targetPos - myCam.transform.position, myCam.transform.forward);

            // Debug.Log(dotProduct);

            if (dotProduct < 0)
            {
                //Marker is behind the camera

                if (newPos.x < Screen.width / 2)
                {
                    newPos.x = maxX;
                }
                else
                {
                    newPos.x = minX;
                }

            }


            // Checking if out of view
            bool OutOfX = newPos.x > (Screen.width - offsetFromEdges.x) || newPos.x < 0;
            bool OutOfY = newPos.y > (Screen.height - offsetFromEdges.y) || newPos.y < 0;


            bool outOfscreen = (OutOfX || OutOfY) || dotProduct < 0;


            if (outOfscreen)
            {
                onScreenImage.gameObject.SetActive(false);
                offScreenParent.gameObject.SetActive(worldMarker.WaypointProperties.showOffScreen);

                offScreenParent.rotation = Quaternion.identity;
                

                if (worldMarker.WaypointProperties.showOffScreen && worldMarker.WaypointProperties.rotateToTarget)
                {
      
                    Vector3 Pos = myCam.WorldToScreenPoint(targetPos);
                    //Debug.Log(Pos);

                    offScreenParent.LookAt(Pos, Vector3.back);

                    if (dotProduct < 0)
                        offScreenParent.Rotate(new Vector3(0, 0, 180));

                }

                
                offScreenParent.rotation = new Quaternion(0, 0, offScreenParent.rotation.z, offScreenParent.rotation.w);
                offScreenParent.Rotate(new Vector3(0, 0, worldMarker.WaypointProperties.offScreenRotation));


                float offScreenImageSizeMultiplier = worldMarker.WaypointProperties.offScreenSize * 100;
                offScreenImage.rectTransform.sizeDelta = new Vector2(offScreenImageSizeMultiplier, offScreenImageSizeMultiplier);

                offScreenImage.color = worldMarker.WaypointProperties.offScreenImageColor;

                Sprite offScreenSprite = worldMarker.WaypointProperties.offScreenImage;
                if (!UltimateHudManager.CheckIfMissingOrNull(offScreenSprite))
                    offScreenImage.sprite = offScreenSprite;
            }
            else
            {
                onScreenImage.gameObject.SetActive(worldMarker.WaypointProperties.showOnScreen);
                offScreenParent.gameObject.SetActive(false);

                onScreenImage.transform.rotation = Quaternion.identity;
                onScreenImage.transform.Rotate(new Vector3(0, 0, worldMarker.WaypointProperties.onScreenRotation));

                onScreenImage.color = worldMarker.WaypointProperties.onScreenImageColor;
                float onScreenImageSizeMultiplier = worldMarker.WaypointProperties.onScreenSize * 100;

                onScreenImage.rectTransform.sizeDelta = new Vector2(onScreenImageSizeMultiplier, onScreenImageSizeMultiplier);

                Sprite onScreenSprite = worldMarker.WaypointProperties.onScreenImage;
                if (!UltimateHudManager.CheckIfMissingOrNull(onScreenSprite))
                    onScreenImage.sprite = onScreenSprite;
            }


            //Setting postion

            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

            transform.position = newPos;



            //setting visuals

            TMP_FontAsset fontAsset = worldMarker.WaypointProperties.textMeshTextPreset;
            if (fontAsset != null)
            {
                distanceText.font = fontAsset;
                titleText.font = fontAsset;
            }

            titleText.color = worldMarker.WaypointProperties.titleTextColor;
            distanceText.color = worldMarker.WaypointProperties.distanceTextColor;







            //Title Text
            bool ShowTitle = worldMarker.WaypointProperties.showTitleOnScreen;

            if (outOfscreen)
                ShowTitle = worldMarker.WaypointProperties.showTitleOffScreen;

            titleText.gameObject.SetActive(ShowTitle);

            if (ShowTitle)
                titleText.text = worldMarker.WaypointProperties.Title;


            //Distance
            float distance = Vector3.Distance(targetPos, myCam.transform.position);

            float lowestDis = worldMarker.WaypointProperties.HideWhenCloseDistance;
            float fading = worldMarker.WaypointProperties.HideFadingSmoothing;

            fading = Mathf.Clamp(fading, 0.1f, 10f);

            if (lowestDis > 0)
            {
                float last = lowestDis + fading;
                canvasGroup.alpha = UltimateHudManager.RemapValue(distance, lowestDis, last, 0, 1);
            }
            else
            {
                canvasGroup.alpha = 1;
            }


            bool ShowDistance = worldMarker.WaypointProperties.showDistanceOnScreen;

            if (OutOfX || OutOfY)
                ShowDistance = worldMarker.WaypointProperties.showDistanceOffScreen;

            distanceText.gameObject.SetActive(ShowDistance);

            if (ShowDistance)
            {
                distanceText.text = distance.ToString("f0") + "m";
            }
        }





    }
}