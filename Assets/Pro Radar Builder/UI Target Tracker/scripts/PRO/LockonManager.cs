using UnityEngine;

namespace DaiMangou.ProRadarBuilder
{
    public enum LockonConditions
    {
        mouseOver,
        functionTrigger
    }

    public class LockonManager : MonoBehaviour
    {
        private RaycastHit hit;
        public LockonConditions lockonConditions = LockonConditions.mouseOver;

        /// <summary>
        ///     from any script, assign a gameobject to ObjectToLockonTo
        /// </summary>
        [HideInInspector] [SerializeField] public GameObject ObjectToLockonTo;

        private Ray ray;

        public UITargetTracker uITargetTracker;

        private void Reset()
        {
            uITargetTracker = GetComponent<UITargetTracker>();
        }


        public void Update()
        {
            if (uITargetTracker.useLockon && uITargetTracker._3dRadar)
                Lockon3D();

            if (uITargetTracker.useLockon && uITargetTracker._2dRadar)
                Lockon2D();
        }


        private void Lockon3D()
        {
            switch (lockonConditions)
            {
                case LockonConditions.mouseOver:

                    for (var b = 0; b < uITargetTracker._3dRadar.Blips.Count; b++)
                    for (var i = 0; i < uITargetTracker._3dRadar.Blips[b].gos.Count; i++)
                    {
                        var theTrackedGameObject = uITargetTracker._3dRadar.Blips[b].gos[i];

                        ray = uITargetTracker.radarcamera.ScreenPointToRay(Input.mousePosition);


                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform == theTrackedGameObject.transform)
                                uITargetTracker.trackers[b].uIData[i].lockedOn = true;
                            else
                                uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                        }
                        else
                        {
                            uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                        }
                    }

                    break;
                case LockonConditions.functionTrigger:
                    for (var b = 0; b < uITargetTracker._3dRadar.Blips.Count; b++)
                    for (var i = 0; i < uITargetTracker._3dRadar.Blips[b].gos.Count; i++)
                    {
                        var theTrackedGameObject = uITargetTracker._3dRadar.Blips[b].gos[i];

                        if (ObjectToLockonTo == theTrackedGameObject)
                            uITargetTracker.trackers[b].uIData[i].lockedOn = true;

                        if (ObjectToLockonTo == null) uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                    }


                    break;
            }
        }

        private void Lockon2D()
        {
            switch (lockonConditions)
            {
                case LockonConditions.mouseOver:

                    for (var b = 0; b < uITargetTracker._2dRadar.Blips.Count; b++)
                    for (var i = 0; i < uITargetTracker._2dRadar.Blips[b].gos.Count; i++)
                    {
                        var theTrackedGameObject = uITargetTracker._2dRadar.Blips[b].gos[i];

                        ray = uITargetTracker.radarcamera.ScreenPointToRay(Input.mousePosition);


                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform == theTrackedGameObject.transform)
                                uITargetTracker.trackers[b].uIData[i].lockedOn = true;
                            else
                                uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                        }
                        else
                        {
                            uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                        }
                    }

                    break;
                case LockonConditions.functionTrigger:
                    for (var b = 0; b < uITargetTracker._2dRadar.Blips.Count; b++)
                    for (var i = 0; i < uITargetTracker._2dRadar.Blips[b].gos.Count; i++)
                    {
                        var theTrackedGameObject = uITargetTracker._2dRadar.Blips[b].gos[i];

                        if (ObjectToLockonTo == theTrackedGameObject)
                            uITargetTracker.trackers[b].uIData[i].lockedOn = true;
                        if (ObjectToLockonTo == null) uITargetTracker.trackers[b].uIData[i].lockedOn = false;
                    }


                    break;
            }
        }
    }
}