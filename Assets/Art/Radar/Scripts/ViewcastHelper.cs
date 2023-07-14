using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kataner
{
    
    public class ViewcastHelper : MonoBehaviour
    {

        public static List<ViewCastInfo> ViewCast(float globalAngle, Transform originTransform, int viewDistance, LayerMask obstacleMask, int heightRaysCount, float maxStepHeight, AnimationCurve stepHeightCurve)
        {
            List<ViewCastInfo> viewCastInfos = new List<ViewCastInfo>();


            Vector3 transform_position = originTransform.position;
            Vector3 dir = originTransform.rotation * DirFromAngle(originTransform, globalAngle, false);

            float y = 0;
            for (int h = 0; h < heightRaysCount; h++)
            {
                float percentage = (float)h / (float)heightRaysCount;
                float value = stepHeightCurve.Evaluate(percentage);
                y += value * maxStepHeight;
                transform_position = new Vector3(0, y, 0);
                transform_position = originTransform.TransformPoint(transform_position);
                //Debug.DrawRay(transform_position, dir * viewDistance);
                //Debug.Log("Y = " + transform_position.y);
                if (Physics.Raycast(transform_position, dir, out RaycastHit hit, viewDistance, obstacleMask))
                {
                    viewCastInfos.Add(new ViewCastInfo(hit.transform, hit.point, hit.distance, globalAngle));
                }
            }

            return viewCastInfos;
        }

        private static Vector3 DirFromAngle(Transform originTransform, float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += originTransform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

    }
}