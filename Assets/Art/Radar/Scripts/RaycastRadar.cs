using UnityEngine;
using System.Collections.Generic;
using System;

namespace Kataner
{

    public class RaycastRadar : MonoBehaviour
    {
        [Header("General Settings")]
        public GameObject targetPlane;
        [Range(0.01f, 2.0f)]
        public float repeatRate = 0.1f;
        [Range(100, 10000)]
        public int viewDistance = 1000;
        [Range(0, 360)]
        public float viewAngle;
        [Range(0, 360)]
        public int angleRangePerUpdate = 10;
        [Range(1, 10)]
        public int angleResolution = 2;

        public LayerMask obstacleMask;
        public Color color = Color.green;
        public Boolean drawNoise = false;
        public Boolean increaseEchoStrength = true;

        [Header("Height Ray Settings")]
        [Range(1, 100)]
        public int heightRaysCount = 10;
        public AnimationCurve stepHeightCurve;
        [Range(0.0f, 50.0f)]
        public float maxStepHeight;

        [Header("Sweep Settings")]
        public Boolean drawSweep = true;
        [Range(0, 40)]
        public int sweepShadowAngle = 40;
        public Color sweepColor = Color.green;

        [Header("Range Display Settings")]
        public Boolean drawRings = false;
        public Color ringColor = Color.green;
        public int range = 100;

        [Header("Identifiable Objects Settings")]
        public Boolean drawIdentifiers = false;
        public int timeBeforeDeactivation = 10;
        public Color fontColor = Color.green;
        public int fontSize = 20;



        private readonly Dictionary<int, List<ViewCastInfo>> viewCasts = new Dictionary<int, List<ViewCastInfo>>();

        private Color32[] resetColorArray;
        private Color32[] ringColorArray;
        private Texture2D tex2D;
        private Texture2D tex2DRings;

        private int actualStartAngle = 0;
        private int actualEndAngle = 0;

        private int oldAngleRangePerUpdate = 10;
        private float currTime = 0.0f;
        private int oldMaxRingsCount = 0;
        private Color oldRingColor = Color.green;

        private readonly ConcurrentDictionary<string, IdentifierInfo> identifier = new ConcurrentDictionary<string, IdentifierInfo>();
        private GameObject identifierParent;

        private void Awake()
        {
            tex2D = TextureHelper.CreateClearTexture(1000, 1000);
            resetColorArray = tex2D.GetPixels32();

            oldMaxRingsCount = (int)viewDistance / 100;
            DrawRangeRings(color, tex2D.width / 2, tex2D.height / 2, tex2D.width / 2, oldMaxRingsCount);
            ringColorArray = tex2DRings.GetPixels32();

            targetPlane.GetComponent<Renderer>().material.mainTexture = tex2D;

            if (identifierParent != null)
            {
                Destroy(identifierParent);
            }
            identifierParent = new GameObject("identifiers");
            identifierParent.transform.parent = targetPlane.transform;

            actualEndAngle = actualStartAngle + angleRangePerUpdate;
            oldAngleRangePerUpdate = angleRangePerUpdate;
        }

        void Update()
        {
            if (currTime < repeatRate)
            {
                currTime += Time.deltaTime;
                return;
            }
            currTime = 0;

            if (drawRings)
            {
                int maxRingsCount = (int)viewDistance / range;
                if (oldMaxRingsCount != maxRingsCount || oldRingColor != ringColor)
                {
                    DrawRangeRings(ringColor, tex2D.width / 2, tex2D.height / 2, tex2D.width / 2, maxRingsCount);
                    ringColorArray = tex2DRings.GetPixels32();
                    oldMaxRingsCount = maxRingsCount;
                    oldRingColor = ringColor;

                }
                tex2D.SetPixels32(0, 0, tex2D.width, tex2D.height, ringColorArray);
            }
            else
            {
                tex2D.SetPixels32(0, 0, tex2D.width, tex2D.height, resetColorArray);
            }

            CalculateViewCasts(actualStartAngle, actualEndAngle);

            if (drawSweep)
            {
                DrawSweep(actualStartAngle, actualEndAngle);
            }

            tex2D.Apply();

            if (drawIdentifiers)
            {
                //Check for destroyable identifiers
                foreach (IdentifierInfo child in identifier.Values)
                {
                    if (Time.time - child.lastKnownActive > timeBeforeDeactivation)
                    {
                        identifier.TryRemove(child.originObject.name, out IdentifierInfo identifierInfo);
                        GameObject.Destroy(child.identifier);
                    }
                }
            }

            actualStartAngle += angleRangePerUpdate;
            actualEndAngle += angleRangePerUpdate;
            if (actualEndAngle > viewAngle)
            {
                actualStartAngle = 0;
                actualEndAngle = angleRangePerUpdate;
            }

        }

        void CalculateViewCasts(int StartAngle, int EndAngle)
        {
            if (tex2D == null)
                return;
            int stepCount = Mathf.RoundToInt((EndAngle - StartAngle) * angleResolution);
            float stepAngleSize = (float)(EndAngle - StartAngle) / (float)stepCount;

            Transform originTransform = GetComponentInParent<Transform>();
            if (oldAngleRangePerUpdate != angleRangePerUpdate)
            {
                viewCasts.Clear();
                oldAngleRangePerUpdate = angleRangePerUpdate;
            }
            else
            {
                viewCasts.Remove(StartAngle);
            }
            List<ViewCastInfo> tmpViewCastInfo = new List<ViewCastInfo>();

            float scaleFactor = (float)tex2D.width / viewDistance;
            float offsetX = tex2D.width / 2.0f;
            float offsetY = tex2D.height / 2.0f;

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.localEulerAngles.y - StartAngle - stepAngleSize * i;
                tmpViewCastInfo.AddRange(ViewcastHelper.ViewCast(angle, originTransform, viewDistance, obstacleMask, heightRaysCount, maxStepHeight, stepHeightCurve));
            }

            viewCasts.Add(StartAngle, tmpViewCastInfo);

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
                    if (vci.dst > viewDistance / 2) //out of range
                        continue;
                    //calculate point on texture as 
                    float a = Mathf.PI * (270 - vci.angle) / 180.0f;
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
                    if (drawIdentifiers)
                    {
                        if (vci.transform.gameObject.CompareTag("identifiable_by_radar"))
                        {
                            if (!identifier.ContainsKey(vci.transform.name))
                            {
                                GameObject textObject = new GameObject("identifier_" + vci.transform.name);
                                textObject.AddComponent<TextMesh>();
                                textObject.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
                                identifier.TryAdd(vci.transform.name, new IdentifierInfo(textObject, vci.transform.gameObject));
                                Quaternion quaternion = Quaternion.Euler(90, textObject.transform.rotation.eulerAngles.y, textObject.transform.rotation.eulerAngles.z);
                                textObject.transform.rotation = quaternion;
                            }
                            identifier.TryGetValue(vci.transform.name, out IdentifierInfo identifierInfo);
                            if (identifierInfo.identifier == null)
                            {
                                continue;
                            }
                            TextMesh textMeshComponent = identifierInfo.identifier.GetComponent(typeof(TextMesh)) as TextMesh;
                            textMeshComponent.fontSize = fontSize;
                            textMeshComponent.color = fontColor;
                            textMeshComponent.text = vci.transform.name;
                            float localX = (x / (float)tex2D.width * 10.0f) - 5f;
                            float localY = (y / (float)tex2D.height * 10.0f) - 5f;
                            Vector3 textWorldPosition = targetPlane.transform.TransformPoint(new Vector3(-localX, 0, -localY));
                            identifierInfo.identifier.transform.position = textWorldPosition;
                            identifierInfo.identifier.transform.parent = identifierParent.transform;
                            identifierInfo.lastKnownActive = Time.time;

                        }
                    }
                    else
                    {
                        //Clear all Identifier gameobjects
                        identifier.Clear();
                        foreach (Transform child in identifierParent.transform)
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                    }
                }
            }
        }

        void DrawSweep(int StartAngle, int EndAngle)
        {
            if (tex2D == null)
                return;

            float offsetX = tex2D.width / 2.0f;
            float offsetY = tex2D.height / 2.0f;
            for (int i = StartAngle; i < EndAngle; i++)
            {
                float a1 = Mathf.PI * (270 + i) / 180.0f;
                float x1 = Mathf.Cos(a1) * tex2D.width / 2 + offsetX;
                float y1 = Mathf.Sin(a1) * tex2D.height / 2 + offsetY;
                TextureHelper.DrawLine(tex2D, new Vector2(tex2D.width / 2, tex2D.height / 2), new Vector2(x1, y1), sweepColor);
            }
            for (int i = StartAngle; i > StartAngle - sweepShadowAngle; i--)
            {
                Color sweepShadowColor = sweepColor * 0.5f;
                float a1 = Mathf.PI * (270 + i) / 180.0f;
                float x1 = Mathf.Cos(a1) * tex2D.width / 2 + offsetX;
                float y1 = Mathf.Sin(a1) * tex2D.height / 2 + offsetY;
                TextureHelper.DrawLine(tex2D, new Vector2(tex2D.width / 2, tex2D.height / 2), new Vector2(x1, y1), sweepShadowColor);
            }
        }

        void DrawRangeRings(Color color, int x, int y, int maxRadius, int numberOfRings)
        {
            tex2DRings = TextureHelper.CreateClearTexture(tex2D.width, tex2D.height);
            int radiusDelta = maxRadius / numberOfRings;
            for (int i = 0; i < numberOfRings; i++)
            {
                int radius = maxRadius - (i * radiusDelta);
                TextureHelper.DrawCircle(tex2DRings, x, y, radius, color);
            }
            tex2DRings.Apply();
        }



#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {

            Matrix4x4 matrix = transform.localToWorldMatrix;
            float positionY = 0;
            float radius = viewDistance / 2;
            UnityEditor.Handles.color = Color.green;
            for (int i = 0; i < heightRaysCount; i++)
            {
                float percentage = (float)i / (float)heightRaysCount;
                float value = stepHeightCurve.Evaluate(percentage);
                positionY += value * maxStepHeight;
                UnityEditor.Handles.matrix = matrix;
                var position = new Vector3(0, positionY, 0);
                UnityEditor.Handles.DrawWireArc(position, new Vector3(0, -1, 0), new Vector3(0, 0, 1), viewAngle, radius);
            }
        }
#endif
    }
}