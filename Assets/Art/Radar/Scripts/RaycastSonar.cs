using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kataner
{

    public class RaycastSonar : MonoBehaviour
    {
        [Header("General Settings")]
        public GameObject targetPlane;
        [Range(0.01f, 2.0f)]
        public float repeatRate = 0.1f;
        [Range(1, 100)]
        public int viewDistance = 100;
        [Range(0, 90)]
        public float viewAngle;
        [Range(1, 10)]
        public int angleResolution = 2;

        public LayerMask obstacleMask;
        public Color color = Color.green;
        public Boolean drawNoise = false;
        public Boolean increaseEchoStrength = true;

        [Header("Ray Level Settings")]
        [Range(1, 100)]
        public int rayLevelCount = 10;
        public AnimationCurve stepCurve;
        [Range(0.0f, 50.0f)]
        public float maxStep;

        [Header("Range Display Settings")]
        public Boolean drawLines = false;
        public Color lineColor = Color.green;
        public int range = 5;

        private readonly Dictionary<int, List<ViewCastInfo>> viewCasts = new Dictionary<int, List<ViewCastInfo>>();

        private Color32[] resetColorArray;
        private Color32[] lineColorArray;
        private Texture2D tex2D;
        private Texture2D tex2DLines;

        private int actualStartAngle = 0;
        private int actualEndAngle = 0;

        //private int oldAngleRangePerUpdate = 10;
        private float oldRepeatRate = 0.0f;
        private int oldMaxLinesCount = 0;
        private Color oldLineColor = Color.green;

        private readonly ConcurrentDictionary<string, IdentifierInfo> identifier = new ConcurrentDictionary<string, IdentifierInfo>();
        private GameObject identifierParent;

        private void OnEnable()
        {
            tex2D = TextureHelper.CreateClearTexture(1000, 1000);
            resetColorArray = tex2D.GetPixels32();

            oldMaxLinesCount = (int)viewDistance / 5;
            DrawRangeLines(color, oldMaxLinesCount);
            lineColorArray = tex2DLines.GetPixels32();

            targetPlane.GetComponent<Renderer>().material.mainTexture = tex2D;

            actualEndAngle = actualStartAngle + (int)viewAngle;
            //oldAngleRangePerUpdate = angleRangePerUpdate;
            InvokeRepeating(nameof(ExecuteSonar), 0f, repeatRate);
        }

        void ExecuteSonar()
        {
            if (oldRepeatRate != repeatRate)
            {
                CancelInvoke();
                InvokeRepeating(nameof(ExecuteSonar), 0f, repeatRate);
                oldRepeatRate = repeatRate;
            }
            if (drawLines)
            {
                int maxLinesCount = (int)viewDistance / range;
                if (oldMaxLinesCount != maxLinesCount || oldLineColor != lineColor)
                {
                    DrawRangeLines(lineColor, maxLinesCount);
                    lineColorArray = tex2DLines.GetPixels32();
                    oldMaxLinesCount = maxLinesCount;
                    oldLineColor = lineColor;

                }
                tex2D.SetPixels32(0, 0, tex2D.width, tex2D.height, lineColorArray);
            }
            else
            {
                tex2D.SetPixels32(0, 0, tex2D.width, tex2D.height, resetColorArray);
            }

            CalculateViewCasts(actualStartAngle, actualEndAngle);

           

            tex2D.Apply();

           
            actualStartAngle = 0;
            actualEndAngle = (int)viewAngle;
            
        }

        void CalculateViewCasts(int StartAngle, int EndAngle)
        {
            if (tex2D == null)
                return;
            int stepCount = Mathf.RoundToInt((EndAngle - StartAngle) * angleResolution);
            float stepAngleSize = (float)(EndAngle - StartAngle) / (float)stepCount;

            Transform originTransform = GetComponentInParent<Transform>();

            List<ViewCastInfo> tmpViewCastInfo = new List<ViewCastInfo>();

            float scaleFactor = (float)tex2D.width / viewDistance;
            float offsetX = tex2D.width/2.0f;
            float offsetY = tex2D.height/8.0f;

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.localEulerAngles.y - StartAngle - stepAngleSize * i;
                tmpViewCastInfo.AddRange(ViewcastHelper.ViewCast(angle, originTransform, viewDistance, obstacleMask, rayLevelCount, maxStep, stepCurve));
            }
            viewCasts.Remove(StartAngle);
            viewCasts.Add(StartAngle, tmpViewCastInfo);

            //TextureHelper.DrawCircle(tex2D, 0, 0,50, Color.green);
            //TextureHelper.DrawCircle(tex2D, 0, (int)tex2D.height, 50, Color.red);
            //TextureHelper.DrawCircle(tex2D, (int)tex2D.width, (int)0, 50, Color.yellow);
            //TextureHelper.DrawCircle(tex2D, (int)tex2D.width, (int)tex2D.height, 50, Color.blue);

            


            for (int i = 0; i < viewAngle; i++)
            {
                viewCasts.TryGetValue(i, out List<ViewCastInfo> allViewCasts);
                if (allViewCasts == null)
                {
                    continue;
                }

                for (int j = 0; j < allViewCasts.Count; j++)
                {
                    ViewCastInfo vci = allViewCasts[j];
                    if (vci.dst > viewDistance) //out of range
                        continue;
                    //calculate point on texture as 
                    float a = Mathf.PI * (45 - vci.angle) / 180.0f;
                    float x = Mathf.Cos(a) * vci.dst * scaleFactor + offsetX;
                    float y = Mathf.Sin(a) * vci.dst * scaleFactor + offsetY;

                    tex2D.SetPixel((int)x, (int)y, drawNoise ? color * UnityEngine.Random.Range(0.7f, 1.0f) : color);
                    if (increaseEchoStrength)
                    {
                        float pixelFactor = UnityEngine.Random.Range(0.4f, 0.7f);
                        tex2D.SetPixel((int)x - 1, (int)y, drawNoise ? color * pixelFactor : color);
                        tex2D.SetPixel((int)x, (int)y - 1, drawNoise ? color * pixelFactor : color);
                        tex2D.SetPixel((int)x + 1, (int)y, drawNoise ? color * pixelFactor : color);
                        tex2D.SetPixel((int)x, (int)y + 1, drawNoise ? color * pixelFactor : color);
                    }
                }
            }
        }

        

        void DrawRangeLines(Color color, int numberOfLines)
        {
            tex2DLines = TextureHelper.CreateClearTexture(tex2D.width, tex2D.height);
            int delta = tex2DLines.height / numberOfLines;
            for (int i = 0; i < numberOfLines; i++)
            {
                int y = tex2DLines.height - (i * delta);
                TextureHelper.DrawLine(tex2DLines, new Vector2(0,y), new Vector2(tex2DLines.width, y), color);
            }
            tex2DLines.Apply();
        }



#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {

            Matrix4x4 matrix = transform.localToWorldMatrix;
            float positionY = 0;
            float radius = viewDistance;
            UnityEditor.Handles.color = Color.yellow;
            for (int i = 0; i < rayLevelCount; i++)
            {
                float percentage = (float)i / (float)rayLevelCount;
                float value = stepCurve.Evaluate(percentage);
                positionY += value * maxStep;
                UnityEditor.Handles.matrix = matrix;
                var position = new Vector3(0, positionY, 0);
                UnityEditor.Handles.DrawWireArc(position, new Vector3(0, -1, 0), new Vector3(0, 0, 1), viewAngle, radius);
            }
        }
#endif
    }
}