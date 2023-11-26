using UnityEngine;
using UnityEngine.UI;

//Not Exported in builds
namespace DaiMangou.ProRadarBuilder
{
#if (UNITY_EDITOR)
    public class Visualization2D : MonoBehaviour
    {
        private _2DRadar Radar;

        internal RectTransform  rectTransform;
        private CanvasScaler canvasScaler;
        public void OnDrawGizmos()
        {
            transform.hideFlags = HideFlags.None;

            if (!Radar) Radar = this.GetComponent<_2DRadar>();



            if (Radar.RadarDesign == null) return;
            if (!Radar.RadarDesign.Visualize) return;

            if (Radar.RadarDesign.UseUI)
            {
                if (gameObject.GetComponent<RectTransform>() != null)
                    rectTransform = gameObject.GetComponent<RectTransform>();

                if (transform.parent.GetComponent<CanvasScaler>() != null)
                    canvasScaler = transform.parent.GetComponent<CanvasScaler>();
            }

            if (Radar.RadarDesign.radarStyle == Editor.RadarStyle.Round)
            {
                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, Radar.RadarDesign.UseUI ? (Radar.RadarDesign.TrackingBounds * 8.5f) * canvasScaler.scaleFactor : Radar.RadarDesign.TrackingBounds * transform.lossyScale.x);
                UnityEditor.Handles.color = Color.cyan;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, Radar.RadarDesign.UseUI ? (Radar.RadarDesign.InnerCullingZone * 8.5f) * canvasScaler.scaleFactor : Radar.RadarDesign.InnerCullingZone * transform.lossyScale.x);
                if (Radar.RadarDesign.UseLocalScale) return;
                UnityEditor.Handles.color = Color.red;
                UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, Radar.RadarDesign.RadarDiameter);
            }
            else
            {
               

            }

            // Gizmos.DrawFrustum(transform.position, 60, 20, 0.3f, 1);
        }

    }
#endif
}
