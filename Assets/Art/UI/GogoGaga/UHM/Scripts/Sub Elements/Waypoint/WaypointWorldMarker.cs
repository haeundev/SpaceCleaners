using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class WaypointWorldMarker : MonoBehaviour
    {
        public WaypointMarkerProperties WaypointProperties;
        public WaypointPanelManager waypointPanelManager { get; set; }
        public CompassPanelManager compassPanelManager { get; set; }

        WaypointMarkerUI markerUI;

        CompassWaypointUI compassUI;


        private void Start()
        {
            Setup();

            if (waypointPanelManager != null)
                waypointPanelManager.AddMarker(this);
        }

        void Setup()
        {
            if(UltimateHudManager.Instance != null)
            {
                waypointPanelManager = UltimateHudManager.Instance.waypointPanelManager;
                compassPanelManager = UltimateHudManager.Instance.compassPanelManager;
            }
        }

        private void Update()
        {
            if(waypointPanelManager == null || compassPanelManager == null)
            {
                Setup();
                return;
            }

            if (WaypointProperties.ShowAsWorldMarker && waypointPanelManager != null && markerUI == null)
            {
                markerUI = waypointPanelManager.CreateWaypointUI(this);
            }

            else if (!WaypointProperties.ShowAsWorldMarker && markerUI != null)
            {
                Destroy(markerUI.gameObject);
            }

            if (WaypointProperties.ShowAsCompassMarker && compassPanelManager != null && compassUI == null)
            {
                compassUI = compassPanelManager.CreateUI(this);
            }
            else if (!WaypointProperties.ShowAsCompassMarker && compassUI != null)
            {
                Destroy(compassUI.gameObject);
            }

        }

        private void OnDestroy()
        {
            waypointPanelManager.removeMarker(this);
        }
    }




    [System.Serializable]
    public class WaypointMarkerProperties
    {
        [Header("Commons")]
        public string Title = "Title";


        public Vector3 PostionOffset;

        [Tooltip("This is the offset from the edges of the waypoint UI in canvas")]
        public Vector2 UIOffsetFromEdge = new Vector2(70,100);

        [Tooltip("Use this if you want to hide waypoint when to close to target. Put 0 or less if you want no effect")]
        public float HideWhenCloseDistance;


        [Range(0.1f, 10), Tooltip("Use this to smooth out the hiding of waypoint UI when to close to target")]
        public float HideFadingSmoothing = 1;

        [Space(20)]

        [Header("On screen")]
        public bool showOnScreen = true;
        public Sprite onScreenImage;
        public Color onScreenImageColor = Color.white;
        [Range(0, 2)] public float onScreenSize = 1;
        [Range(0, 360)] public float onScreenRotation = 0;

        [Space(20)]

        [Header("Off screen")]
        public bool showOffScreen = true;
        public Sprite offScreenImage;
        public Color offScreenImageColor = Color.white;
        [Range(0, 2)] public float offScreenSize = 1;
        [Range(0, 360)] public float offScreenRotation = 0;
        public bool rotateToTarget = true;

        [Space(20)]

        [Header("Text properties")]
        public TMPro.TMP_FontAsset textMeshTextPreset;

        [Space]
        public bool showDistanceOnScreen = true;
        public bool showDistanceOffScreen = false;
        public Color distanceTextColor = Color.white;

        [Space]
        public bool showTitleOnScreen = true;
        public bool showTitleOffScreen = false;
        public Color titleTextColor = Color.white;

        [Space(20)]

        [Header("Other properties")]
        public bool ShowAsWorldMarker = true;
        public bool ShowAsCompassMarker = true;
        //public bool ShowAsMinimapMarker = true;
    }

}