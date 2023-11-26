/// <summary>
/// Developed by DaiMangou 
/// programmer and developer : Ruchmair Dixon
/// Copytight 2017 DaiMangou Limited
/// </summary>

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace DaiMangou.ProRadarBuilder
{

    public class ProRadarBuilderEditorWindow : EditorWindow
    {

        [MenuItem("Tools/Pro Radar Builder/Standard 2D System")]
        private static void MakeStandard2D()
        {
            Create2DRadar();
        }
        [MenuItem("Tools/Pro Radar Builder/Unity UI 2D System")]
        private static void MakeUI2D()
        {
            Create2DRadar(true);
        }

        [MenuItem("Tools/Pro Radar Builder/Standard 3D System")]
        private static void MakeStandard3D()
        {
            Create3DRadar();
        }


        #region Create 2D Radar
        static void Create2DRadar(bool UseUI = false)
        {

            if (!UseUI)
            {
                GameObject RadarInstance = new GameObject("2D Radar");
                RadarInstance.AddComponent<_2DRadar>();
                RadarInstance.AddComponent<Visualization2D>();

                Selection.activeGameObject = RadarInstance;
                GameObject DesignsParent = new GameObject("Designs");
                DesignsParent.transform.parent = RadarInstance.transform;
                GameObject BlipsContainer = new GameObject("Blips");
                BlipsContainer.transform.parent = RadarInstance.transform;
                GameObject DefaultDesign = new GameObject("DefaultRadarSprite");
                DefaultDesign.transform.parent = DesignsParent.transform;


                DefaultDesign.AddComponent<SpriteRenderer>();
                DefaultDesign.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Default Sprites/Radar/Default2DRadarSprite");
                CreateRenderCamera(RadarInstance);
                BlipsContainer.AddComponent<Canvas>();
                var canvasComponent = BlipsContainer.GetComponent<Canvas>();
                canvasComponent.renderMode = RenderMode.WorldSpace;
                canvasComponent.worldCamera = RadarInstance.GetComponent<_2DRadar>().RadarDesign.renderingCamera;
                BlipsContainer.AddComponent<CanvasScaler>();
                BlipsContainer.AddComponent<GraphicRaycaster>();
                BlipsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            }
            else
            {
                GameObject RadarCanvasInstance = new GameObject("2D Radar Canvas");
                RadarCanvasInstance.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                RadarCanvasInstance.AddComponent<CanvasScaler>();
                RadarCanvasInstance.GetComponent<CanvasScaler>().scaleFactor = 2.0f;
                RadarCanvasInstance.AddComponent<GraphicRaycaster>();
                GameObject RadarInstance = new GameObject("2D Radar");
                RadarInstance.AddComponent<RectTransform>();

                RadarInstance.AddComponent<_2DRadar>();
                RadarInstance.AddComponent<Visualization2D>();
                RadarInstance.AddComponent<Image>().sprite = Resources.Load<Sprite>("Default Sprites/Radar/DefaultMaskSprite");
                RadarInstance.AddComponent<Mask>().showMaskGraphic = false;
                RadarInstance.transform.SetParent(RadarCanvasInstance.transform);
                Selection.activeGameObject = RadarInstance;
                GameObject DesignsParent = new GameObject("Designs");
                DesignsParent.AddComponent<RectTransform>();
                DesignsParent.transform.SetParent(RadarInstance.transform);
                GameObject BlipsContainer = new GameObject("Blips");
                BlipsContainer.AddComponent<RectTransform>();
                BlipsContainer.transform.SetParent(RadarInstance.transform);
                GameObject DefaultDesign = new GameObject("DefaultRadarSprite");
                DefaultDesign.AddComponent<RectTransform>();
                DefaultDesign.transform.SetParent(DesignsParent.transform);
                DefaultDesign.AddComponent<Image>();
                DefaultDesign.GetComponent<Image>().sprite = Resources.Load<Sprite>("Default Sprites/Radar/Default2DRadarSprite");
                RadarInstance.GetComponent<_2DRadar>().RadarDesign.UseUI = true;
                RadarInstance.GetComponent<RectTransform>().anchorMin = Vector2.zero;
                RadarInstance.GetComponent<RectTransform>().anchorMax = Vector2.zero;
                RadarInstance.GetComponent<RectTransform>().position = new Vector2(50, 50);

            }
            // Tab_Selection = 0;

        }
        #endregion

        #region Create 3D Radar
        static void Create3DRadar()
        {



            GameObject RadarInstance = new GameObject("3D Radar");
            RadarInstance.AddComponent<_3DRadar>();
            RadarInstance.AddComponent<Visualization3D>();
            Selection.activeGameObject = RadarInstance;
            GameObject DesignsParent = new GameObject("Designs");
            DesignsParent.transform.parent = RadarInstance.transform;
            GameObject BlipsContainer = new GameObject("Blips");
            BlipsContainer.transform.parent = RadarInstance.transform;
            GameObject DefaultDesign = new GameObject("Default3DRadarSprite");
            DefaultDesign.transform.parent = DesignsParent.transform;
            DefaultDesign.transform.Rotate(new Vector2(90, 0), Space.World);


            DefaultDesign.AddComponent<SpriteRenderer>();
            DefaultDesign.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Default Sprites/Radar/Default3DRadarSprite");
            CreateRenderCamera(RadarInstance);
            BlipsContainer.AddComponent<Canvas>();
            var canvasComponent = BlipsContainer.GetComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.WorldSpace;
            canvasComponent.worldCamera = RadarInstance.GetComponent<_3DRadar>().RadarDesign.renderingCamera;
            BlipsContainer.AddComponent<CanvasScaler>();
            BlipsContainer.AddComponent<GraphicRaycaster>();
            BlipsContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);



            //   Tab_Selection = 0;

        }
        #endregion

        #region CreateRender Camera
        static void CreateRenderCamera(GameObject radar)
        {
            GameObject RenderCamera = new GameObject("Render Camera");
            RenderCamera.AddComponent<Camera>();
            RenderCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
            RenderCamera.GetComponent<Camera>().farClipPlane = 265;
            RenderCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
            RenderCamera.GetComponent<Camera>().depth = 100;
            RenderCamera.GetComponent<Camera>().cullingMask = 1;
            RenderCamera.GetComponent<Camera>().orthographicSize = 2.3f;

            if (radar.GetComponent<_2DRadar>() != null)
                radar.GetComponent<_2DRadar>().RadarDesign.renderingCamera = RenderCamera.GetComponent<Camera>();
            else
                radar.GetComponent<_3DRadar>().RadarDesign.renderingCamera = RenderCamera.GetComponent<Camera>();


        }
        #endregion




    }

}