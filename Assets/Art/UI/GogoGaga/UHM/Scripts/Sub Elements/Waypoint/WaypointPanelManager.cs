using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class WaypointPanelManager : MonoBehaviour
    {
        public WaypointMarkerUI uiPrefab;

        public Transform uiParent;

        List<WaypointWorldMarker> allMarkers = new List<WaypointWorldMarker>();
        public WaypointWorldMarker CreateWaypoint(WaypointMarkerProperties properties, Vector3 worldPos)
        {
            GameObject marker = new GameObject("Waypoint");
            marker.transform.position = worldPos;
            marker.transform.rotation = Quaternion.identity;
            WaypointWorldMarker w = marker.AddComponent<WaypointWorldMarker>();
            w.WaypointProperties = properties;
            return w;
        }

        public void AddMarker(WaypointWorldMarker marker)
        {
            if (!allMarkers.Contains(marker))
                allMarkers.Add(marker);
        }

        public void removeMarker(WaypointWorldMarker m)
        {
            allMarkers.Remove(m);
        }

        public WaypointMarkerUI CreateWaypointUI(WaypointWorldMarker marker)
        {
            WaypointMarkerUI w = Instantiate(uiPrefab, uiParent);
            w.SetMarker(marker);

            return w;
        }

        public void RemoveAll()
        {
            for (int i = 0; i < allMarkers.Count; i++)
            {
                if (allMarkers[i] != null)
                    Destroy(allMarkers[i].gameObject);
            }
        }
    }
}