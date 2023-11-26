using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DaiMangou.ProRadarBuilder;
using DaiMangou.ProRadarBuilder.Editor;
using DaiMangou.ProRadarBuilderEditor;


[CustomEditor(typeof(_2DRadar))]
[CanEditMultipleObjects]
public class _2DRadarBuilderInspector : Editor
{
    public void OnEnable()
    {
        selected2DRadar = (_2DRadar)target;
        edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];

        // _RadarCenterObject2D = serializedObject.FindProperty("_RadarCenterObject2D");

        skin = Resources.Load<GUISkin>("PRBSkin");

    }
    public override bool RequiresConstantRepaint()
    {
        return true;
    }
    public override void OnInspectorGUI()
    {
        // GUI.skin = skin;



        if (edwin.Length == 0)
        {
            Repaint();
            edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];
        }

        ScreenRect.size = new Vector2(edwin[0].position.width, edwin[0].position.height);

        serializedObject.Update();

        #region Radar and Minimap Design

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selected2DRadar.RadarDesign.ShowDesignsArea = GUILayout.Toggle(selected2DRadar.RadarDesign.ShowDesignsArea, new GUIContent("Design", "This section will allows you to begin the first phase of your system design"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected2DRadar.RadarDesign.ShowDesignsArea)
        {
            #region Camera Section

            HelpMessage("Here you set up your rendering camera and main camera");

            selected2DRadar.RadarDesign.ShowRenderCameraSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowRenderCameraSettings, new GUIContent("Cameras", "Here you set up your rendering camera and main camera \n The Render camera is used to ONLY render the radar and the other camers is used to render your scene and as a reference for the transform and rotation of the Render camera"));

            if (selected2DRadar.RadarDesign.ShowRenderCameraSettings)
            {
                HelpMessage("This is the cameras in front of which the radar will be displayed");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Manual Camera Setup");
                selected2DRadar.RadarDesign.ManualCameraSetup = GUILayout.Toggle(selected2DRadar.RadarDesign.ManualCameraSetup, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                if (selected2DRadar.RadarDesign.ManualCameraSetup)
                {
                    HelpMessage("When selected , the scale of the radar once greater than or less than 1 ; will be ignored");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Camera");
                    selected2DRadar.RadarDesign.camera = (Camera)EditorGUILayout.ObjectField("", selected2DRadar.RadarDesign.camera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    HelpMessage("Will always find and set the scenes Main Camera as the default Camera  for the radar");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Always use main camera");
                    selected2DRadar.RadarDesign.UseMainCamera = GUILayout.Toggle(selected2DRadar.RadarDesign.UseMainCamera, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Finds a camera with the selected tag and uses it as the radars camera");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use camera with tag");
                    selected2DRadar.RadarDesign.CameraTag = EditorGUILayout.TagField("", selected2DRadar.RadarDesign.CameraTag, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }
                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("YOU MUST HAVE A RENDERING CAMERA");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Render Camera");
                    selected2DRadar.RadarDesign.renderingCamera = (Camera)EditorGUILayout.ObjectField("", selected2DRadar.RadarDesign.renderingCamera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }


            }
            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();

            #endregion

            #region Minimap Settings      

            GUILayout.BeginHorizontal();
            selected2DRadar.RadarDesign.ShowMinimapSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowMinimapSettings, "Minimap " + (selected2DRadar.RadarDesign._2DSystemsWithMinimapFunction ? " is on" : " is off"));
            selected2DRadar.RadarDesign._2DSystemsWithMinimapFunction = GUILayout.Toggle(selected2DRadar.RadarDesign._2DSystemsWithMinimapFunction, selected2DRadar.RadarDesign._2DSystemsWithMinimapFunction ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18));
            GUILayout.EndHorizontal();


            EditorGUI.BeginDisabledGroup(!selected2DRadar.RadarDesign._2DSystemsWithMinimapFunction);

            if (selected2DRadar.RadarDesign.ShowMinimapSettings)
            {
                HelpMessage("When set to Realtime, the minimap texture will be drawn from the current scene . If static, then your predesigned map will be used");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Minimap Type");
                selected2DRadar.minimapModule.mapType = (MapType)EditorGUILayout.EnumPopup(selected2DRadar.minimapModule.mapType, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                if (selected2DRadar.minimapModule.mapType != MapType.Realtime)
                {
                    HelpMessage("The static texture2D image that will be your map");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Minimap Texture");
                    selected2DRadar.minimapModule.MapTexture = (Sprite)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.MapTexture, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Sets the ratio which the radars internal system will use to ensure consistency in your minimap");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Calibrate Minimap");
                    selected2DRadar.minimapModule.calibrate = EditorGUILayout.Toggle("", selected2DRadar.minimapModule.calibrate, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    HelpMessage("Empty gameobject which is to be placed at the center of where your minimp image is located in game");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Map Center Reference Object");
                    selected2DRadar.minimapModule.MapCenterReference = (Transform)EditorGUILayout.ObjectField("", selected2DRadar.minimapModule.MapCenterReference, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    HelpMessage("The order of the map sprite in the layer");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Order In layer");
                    selected2DRadar.minimapModule.OrderInLayer = EditorGUILayout.IntField(selected2DRadar.minimapModule.OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                }
                else
                {
                    HelpMessage("The render texture which will be used to pass the data from Realtime Minimap Camera to the Map");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Render Texture");
                    selected2DRadar.minimapModule.renderTexture = (RenderTexture)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.renderTexture, typeof(RenderTexture), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    if (!selected2DRadar.minimapModule.RealtimeMinimapCamera)
                    {
                        if (GUILayout.Button("Create Minimap Camera", GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                        {
                            selected2DRadar.minimapModule.RealtimeMinimapCamera = CreateRealtimeMinimapCamera(typeof(_2DRadar));

                        }

                    }
                    else
                    {
                        HelpMessage("Your minimap camera ");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Minimap Camera");
                        selected2DRadar.minimapModule.RealtimeMinimapCamera = (Camera)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.RealtimeMinimapCamera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();
                    }

                    HelpMessage("The y position of your Realtime Minimap Camera");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Camera Height");
                    selected2DRadar.minimapModule.CameraHeight = EditorGUILayout.FloatField(selected2DRadar.minimapModule.CameraHeight, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                }

                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("Create radars of any shape using sprites, use 100 x 100 sprites and turn on Ignore diametere scale and ensure that your blips use a material that can be masked like our example 'Masked' material");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use Custom Mask Sprite Shape");
                    selected2DRadar.minimapModule.UseCustomMapMaskShape = EditorGUILayout.Toggle("", selected2DRadar.minimapModule.UseCustomMapMaskShape, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (selected2DRadar.minimapModule.UseCustomMapMaskShape)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("custom Mask sprite shape");
                        selected2DRadar.minimapModule.CustomMapMaskShape = (Sprite)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.CustomMapMaskShape, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                    }
                }

                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("the Material to be placed on the Minimap. This must be a material that must be able to be masked and it's shader must allow for Texture images");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Map Material");
                    selected2DRadar.minimapModule.MapMaterial = (Material)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.MapMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("The material that will Mask the Map in a radar");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Mask Material");
                    selected2DRadar.minimapModule.MaskMaterial = (Material)EditorGUILayout.ObjectField(selected2DRadar.minimapModule.MaskMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("On Layer"); selected2DRadar.minimapModule.layer = EditorGUILayout.LayerField(selected2DRadar.minimapModule.layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();


                if (selected2DRadar.minimapModule.calibrate && selected2DRadar.minimapModule.mapType == MapType.Static)
                {


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Map Scale");
                    selected2DRadar.minimapModule.MapScale = EditorGUILayout.FloatField("", selected2DRadar.minimapModule.MapScale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUI.EndDisabledGroup();


            Separator();
            EditorGUILayout.Space();

            #endregion

            #region Scale Settings
            HelpMessage("The scale setting of your radar");

            selected2DRadar.RadarDesign.ShowScaleSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowScaleSettings, "Scale");

            if (selected2DRadar.RadarDesign.ShowScaleSettings)
            {
                HelpMessage("Use the Canvas Scaler component Scale Factor to rescale");

                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("This will override the Radar Diameter value to make the radar be set to the default scale of the DESIGNS child objects of the radar");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use Local Scale");
                    selected2DRadar.RadarDesign.UseLocalScale = GUILayout.Toggle(selected2DRadar.RadarDesign.UseLocalScale, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (!selected2DRadar.RadarDesign.UseLocalScale)
                    {
                        HelpMessage("Radar Diameter is the diameter of the radar");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Radar Diameter");
                        selected2DRadar.RadarDesign.RadarDiameter = EditorGUILayout.FloatField(Mathf.Clamp(selected2DRadar.RadarDesign.RadarDiameter, 0, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();
                    }
                }
                HelpMessage("Anything outside of the tracking bounds will not be seen");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Tracking Bounds");
                selected2DRadar.RadarDesign.TrackingBounds = EditorGUILayout.FloatField(Mathf.Clamp(selected2DRadar.RadarDesign.TrackingBounds, 0, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();






                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("When selected, the scale of the radar once greater than or less than 1 ; will be ignored");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ignore Diameter Scale");
                    selected2DRadar.RadarDesign.IgnoreDiameterScale = GUILayout.Toggle(selected2DRadar.RadarDesign.IgnoreDiameterScale, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }

                HelpMessage("Scene Scale is the zoom of the radar; it represents how much units of space the radar can 'observe'");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scene Scale");
                selected2DRadar.RadarDesign.SceneScale = EditorGUILayout.FloatField(Mathf.Clamp(selected2DRadar.RadarDesign.SceneScale, 1, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                HelpMessage("Anything inside this area will be culled (not seen)");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Inner Culling Zone");
                selected2DRadar.RadarDesign.InnerCullingZone = EditorGUILayout.FloatField(Mathf.Clamp(selected2DRadar.RadarDesign.InnerCullingZone, 0, selected2DRadar.RadarDesign.TrackingBounds), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();
            #endregion

            #region Rotation Settings
            HelpMessage("The rotation setting of your radar");

            selected2DRadar.RadarDesign.ShowRotationSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowRotationSettings, "Rotation");

            if (selected2DRadar.RadarDesign.ShowRotationSettings)
            {
                if (selected2DRadar.RadarDesign.DontRotateMapAndContent)
                    HelpMessage("When set to true, will prevent the map from roting itself or content inside it by default.");


                GUILayout.BeginHorizontal();
                GUILayout.Label("Don't Rotate Map And Content");
                selected2DRadar.RadarDesign.DontRotateMapAndContent = GUILayout.Toggle(selected2DRadar.RadarDesign.DontRotateMapAndContent, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();



            }

            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();
            #endregion

            #region Positioning  Section
            HelpMessage("Position setting of your radar in screen space");

            selected2DRadar.RadarDesign.ShowPositioningSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowPositioningSettings, "Position");
            if (selected2DRadar.RadarDesign.ShowPositioningSettings)
            {

                #region Positioning settings
                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("If you turn on  Use Custom Positioning all internal positioning systems will be disabled, and you will be free to position the radar manually");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use Custom Positioning");
                    selected2DRadar.RadarDesign.UseCustomPositioning = EditorGUILayout.Toggle(selected2DRadar.RadarDesign.UseCustomPositioning, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Choose between 9 point snapping or Manual positioning");
                    EditorGUI.BeginDisabledGroup(selected2DRadar.RadarDesign.UseCustomPositioning);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Radar Positioning");
                    selected2DRadar.RadarDesign.radarPositioning = (RadarPositioning)EditorGUILayout.EnumPopup("", selected2DRadar.RadarDesign.radarPositioning, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    switch (selected2DRadar.RadarDesign.radarPositioning)
                    {
                        case RadarPositioning.Manual:
                            HelpMessage("Position the radar manually on the x and y axis");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("X and Y position");
                            selected2DRadar.RadarDesign.RadarRect.position = EditorGUILayout.Vector2Field("", selected2DRadar.RadarDesign.RadarRect.position, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            break;
                        case RadarPositioning.Snap:
                            HelpMessage("Use our 9 point snapping  to snap the position of your radar to 9 dirent points on your screen");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Snap to the");
                            selected2DRadar.RadarDesign.snapPosition = (SnapPosition)EditorGUILayout.EnumPopup("", selected2DRadar.RadarDesign.snapPosition, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                            break;

                    }
                }
                #endregion

                #region FrontIs Settings
                HelpMessage("Determine what the front facing direction of the radar is ");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Front is");
                selected2DRadar.RadarDesign.frontIs = (FrontIs)EditorGUILayout.EnumPopup("", selected2DRadar.RadarDesign.frontIs, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();
                #endregion

                if (!selected2DRadar.RadarDesign.UseUI)
                {
                    HelpMessage("Pading on the X axis");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("X Pading");
                    selected2DRadar.RadarDesign.xPadding = EditorGUILayout.FloatField("", selected2DRadar.RadarDesign.xPadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Pading on the Y axis");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Y Pading");
                    selected2DRadar.RadarDesign.yPadding = EditorGUILayout.FloatField("", selected2DRadar.RadarDesign.yPadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();
                }

            }
            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();
            #endregion


        }
        #endregion

        Separator3();

        #region Rotation Targets

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selected2DRadar.RadarDesign.ShowRotationTargetsArea = GUILayout.Toggle(selected2DRadar.RadarDesign.ShowRotationTargetsArea, new GUIContent("Rotation Targets", "This section will allows you to set rotation targets for specific objects"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected2DRadar.RadarDesign.ShowRotationTargetsArea)
        {
            #region Setting and creating Rotation Targets
            HelpMessage("You are able to target layers blips or parts of your radar and have them rotate in various ways");

            #endregion



            if (selected2DRadar.RadarDesign.RotationTargets.Count == 0)
            {
                if (GUILayout.Button("Add a Rotation target ?"))
                    selected2DRadar.RadarDesign.RotationTargets.Add(new RotationTarget());

            }

            for (int i = 0; i < selected2DRadar.RadarDesign.RotationTargets.Count; i++)
            {
                var rotationTarget = selected2DRadar.RadarDesign.RotationTargets[i];

                var contentBodyArea = GUILayoutUtility.GetLastRect().PlaceUnder(ScreenRect.width - 35, ShowHelpMessages ? 450 : 340);
                GUI.Box(contentBodyArea, "", skin.customStyles[2]);


                if (rotationTarget == null) return;

                var foldoutArea = contentBodyArea.ToUpperLeft(100, 20);
                string Target = (!rotationTarget.TargetedObject) ? (rotationTarget.target == TargetBlip.InstancedBlip) ? rotationTarget.InstancedTargetBlipname : "nothing " : rotationTarget.TargetedObject.name;
                string Target2 = (!rotationTarget.Target) ? "nothing " : rotationTarget.Target.name;

                GUILayout.FlexibleSpace();

                var upButtonArea = contentBodyArea.ToUpperLeft(15, 10, 130, 2.5f);

                if (ClickEvent.Click(0, upButtonArea, ImageLibrary.upArrow))
                {
                    if (i != 0)
                    {
                        var temptarget = rotationTarget;
                        selected2DRadar.RadarDesign.RotationTargets[i] = selected2DRadar.RadarDesign.RotationTargets[i - 1];
                        selected2DRadar.RadarDesign.RotationTargets[i - 1] = temptarget;
                    }
                }

                var downButtonArea = upButtonArea.PlaceToRight(0, 0, 20);
                if (ClickEvent.Click(0, downButtonArea, ImageLibrary.downArrow))
                {
                    if (i != selected2DRadar.RadarDesign.RotationTargets.Count - 1)
                    {
                        var temptarget = rotationTarget;
                        selected2DRadar.RadarDesign.RotationTargets[i] = selected2DRadar.RadarDesign.RotationTargets[i + 1];
                        selected2DRadar.RadarDesign.RotationTargets[i + 1] = temptarget;
                    }
                }

                var powerButtonArea = downButtonArea.PlaceToRight(15, 15, 20, -2.5f);

                if (ClickEvent.Click(0, powerButtonArea, rotationTarget.UseRotationTarget ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro))
                    rotationTarget.UseRotationTarget = !rotationTarget.UseRotationTarget;



                EditorGUI.BeginDisabledGroup(!rotationTarget.UseRotationTarget);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();

                GUILayout.Space(20);

                GUI.DrawTexture(contentBodyArea.ToUpperLeft(0, 3, 0, 20), Textures.DuskLighter);

                #region Draw Radar Design Interface
                HelpMessage("Here you will set the objet (blip or otherwise) which you wish to have rotate ");


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Rotate");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.target = (TargetBlip)EditorGUILayout.EnumPopup(rotationTarget.target);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                switch (rotationTarget.target)
                {

                    case TargetBlip.ThisObject:

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.TargetedObject = (GameObject)EditorGUILayout.ObjectField(rotationTarget.TargetedObject, typeof(GameObject), true);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        break;
                    case TargetBlip.InstancedBlip:

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.InstancedTargetBlipname = EditorGUILayout.TextField(rotationTarget.InstancedTargetBlipname);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        break;

                }



                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.rotations = (Rotations)EditorGUILayout.EnumPopup(rotationTarget.rotations);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("To");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.ObjectToTrack = (TargetObject)EditorGUILayout.EnumPopup(rotationTarget.ObjectToTrack);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                switch (rotationTarget.ObjectToTrack)
                {
                    case TargetObject.FindObject:
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.FindingName = EditorGUILayout.TextField(rotationTarget.FindingName);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        break;
                    case TargetObject.ObjectWithTag:
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.tag = EditorGUILayout.TextField(rotationTarget.tag);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        break;
                    case TargetObject.ThisObject:
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.Target = (GameObject)EditorGUILayout.ObjectField(rotationTarget.Target, typeof(GameObject), true);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        break;
                    case TargetObject.InstancedBlip:
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        rotationTarget.InstancedObjectToTrackBlipName = EditorGUILayout.TextField(rotationTarget.InstancedObjectToTrackBlipName);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        break;

                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                HelpMessage("By what percent will rotation be reduced");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("With % damping of");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.RotationDamping = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.RotationDamping, 0, 100));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("And use " + Target2 + "'s Y rotation");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                HelpMessage("Use the Y rotation of the targeted object as the rotation of the target");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.UseY = EditorGUILayout.Toggle(rotationTarget.UseY, GUILayout.MaxWidth(10), GUILayout.MinWidth(10));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                #endregion


                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Separator();


                GUILayout.Space(40);

                var conditionBodyFooter = contentBodyArea.PlaceUnder(0, 5);
                GUI.Box(conditionBodyFooter, "", skin.customStyles[3]);
                var buttonArea = conditionBodyFooter.ToLowerRight(55, 14, 0, 14);
                GUI.Box(buttonArea, "", skin.customStyles[4]);

                var addConditionButtonArea = buttonArea.ToCenterLeft(8, 8, 10);
                if (ClickEvent.Click(0, addConditionButtonArea, ImageLibrary.addConditionIcon, "Add a new Rotation Target"))
                {
                    selected2DRadar.RadarDesign.RotationTargets.Insert(i + 1, new RotationTarget());
                }

                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                //  if(i!=0)
                if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon, "Delete this Rotation Target"))
                {
                    selected2DRadar.RadarDesign.RotationTargets.RemoveAt(i);
                    return;
                }

            }
        }
        #endregion

        Separator3();

        #region Blip Design

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selected2DRadar.RadarDesign.ShowBlipsArea = GUILayout.Toggle(selected2DRadar.RadarDesign.ShowBlipsArea, new GUIContent("Blips", "This section will allows you to design all your systems blips"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected2DRadar.RadarDesign.ShowBlipsArea)
        {
            #region Setting Blips

            HelpMessage("This blip; if set to active, will always appear at the center of your radar.");
            selected2DRadar._RadarCenterObject2D.State = selected2DRadar._RadarCenterObject2D.IsActive ? selected2DRadar._RadarCenterObject2D.Tag + " is Active" : selected2DRadar._RadarCenterObject2D.Tag + " is Inactive";

            Separator2();
            GUILayout.BeginHorizontal();
            selected2DRadar._RadarCenterObject2D.ShowCenterBLipSettings = EditorGUILayout.Foldout(selected2DRadar._RadarCenterObject2D.ShowCenterBLipSettings, selected2DRadar._RadarCenterObject2D.State);

            if (selected2DRadar._RadarCenterObject2D.BlipSprite)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Space(80);
                GUILayout.Box(selected2DRadar._RadarCenterObject2D.BlipSprite.texture, "Label", GUILayout.Height(18), GUILayout.Width(18));
                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button((selected2DRadar._RadarCenterObject2D.IsActive) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18))) selected2DRadar._RadarCenterObject2D.IsActive = !selected2DRadar._RadarCenterObject2D.IsActive;
            GUILayout.EndHorizontal();

            if (selected2DRadar._RadarCenterObject2D.ShowCenterBLipSettings)
            {
                Separator();
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                #region sprite

                HelpMessage("If using sprites as blips; set the Sprite and Material here");
                selected2DRadar._RadarCenterObject2D.ShowSpriteBlipSettings = EditorGUILayout.Foldout(selected2DRadar._RadarCenterObject2D.ShowSpriteBlipSettings, selected2DRadar._RadarCenterObject2D.Tag + " Sprite");

                if (selected2DRadar._RadarCenterObject2D.ShowSpriteBlipSettings)
                {
                    #region Sprite Blip Design


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Sprite");
                    selected2DRadar._RadarCenterObject2D.BlipSprite = (Sprite)EditorGUILayout.ObjectField(selected2DRadar._RadarCenterObject2D.BlipSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    selected2DRadar._RadarCenterObject2D.CanUseNullMaterial = selected2DRadar.RadarDesign.UseUI;


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material");
                    selected2DRadar._RadarCenterObject2D.BlipMaterial = (Material)EditorGUILayout.ObjectField("", selected2DRadar._RadarCenterObject2D.BlipMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Colour");
                    selected2DRadar._RadarCenterObject2D.colour = EditorGUILayout.ColorField("", selected2DRadar._RadarCenterObject2D.colour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Order In Layer");
                    selected2DRadar._RadarCenterObject2D.OrderInLayer = EditorGUILayout.IntField(selected2DRadar._RadarCenterObject2D.OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();



                    if (selected2DRadar._RadarCenterObject2D.BlipSprite)
                    {
                        GUILayout.Box(selected2DRadar._RadarCenterObject2D.BlipSprite.texture, "Label", GUILayout.Height(50), GUILayout.Width(50));
                    }

                    #endregion
                }


                Separator();

                EditorGUILayout.Space();
                #endregion

                #region Prefab


                HelpMessage("If using prefab as blips; set the prefab here");
                selected2DRadar._RadarCenterObject2D.ShowPrefabBlipSettings = EditorGUILayout.Foldout(selected2DRadar._RadarCenterObject2D.ShowPrefabBlipSettings, selected2DRadar._RadarCenterObject2D.Tag + " Prefab");
                if (selected2DRadar._RadarCenterObject2D.ShowPrefabBlipSettings)
                {
                    #region Prefab BLip Design

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Prefab");
                    selected2DRadar._RadarCenterObject2D.prefab = (Transform)EditorGUILayout.ObjectField("", selected2DRadar._RadarCenterObject2D.prefab, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    #endregion
                }


                Separator();

                EditorGUILayout.Space();
                #endregion

                #region Additional Options



                HelpMessage("Displaying additional options for your blip");
                selected2DRadar._RadarCenterObject2D.ShowAdditionalOptions = EditorGUILayout.Foldout(selected2DRadar._RadarCenterObject2D.ShowAdditionalOptions, "Additional Options");
                if (selected2DRadar._RadarCenterObject2D.ShowAdditionalOptions)
                {
                    HelpMessage("When enabled, all " + selected2DRadar._RadarCenterObject2D.Tag + "blips will not disaqppear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on it's distance from the center object");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Always Show " + selected2DRadar._RadarCenterObject2D.Tag + " in radar");
                    selected2DRadar._RadarCenterObject2D.AlwaysShowCenterObject = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.AlwaysShowCenterObject, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                }





                Separator();

                EditorGUILayout.Space();

                #endregion

                #region Scale And Rotation Settings




                selected2DRadar._RadarCenterObject2D.ShowGeneralSettings = EditorGUILayout.Foldout(selected2DRadar._RadarCenterObject2D.ShowGeneralSettings, "Rotation and Scale");
                if (selected2DRadar._RadarCenterObject2D.ShowGeneralSettings)
                {
                    #region Blip Scale Settings
                    if (!selected2DRadar._RadarCenterObject2D.CenterObjectCanScaleByDistance)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale");
                        selected2DRadar._RadarCenterObject2D.BlipSize = EditorGUILayout.FloatField(selected2DRadar._RadarCenterObject2D.BlipSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();
                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scale By Distance");
                    selected2DRadar._RadarCenterObject2D.CenterObjectCanScaleByDistance = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.CenterObjectCanScaleByDistance, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (selected2DRadar._RadarCenterObject2D.CenterObjectCanScaleByDistance)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Min Scale");
                        selected2DRadar._RadarCenterObject2D.BlipMinSize = EditorGUILayout.FloatField("", selected2DRadar._RadarCenterObject2D.BlipMinSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max Scale");
                        selected2DRadar._RadarCenterObject2D.BlipMaxSize = EditorGUILayout.FloatField("", selected2DRadar._RadarCenterObject2D.BlipMaxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                    }
                    #endregion


                    #region Blip Rotation Settings
                    HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Track Rotation");
                    selected2DRadar._RadarCenterObject2D.IsTrackRotation = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.IsTrackRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    if (selected2DRadar._RadarCenterObject2D.IsTrackRotation)
                    {

                        HelpMessage("choose the way in which you want your blip to rotate with respect to its tracked object");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Rotation Method");
                        selected2DRadar._RadarCenterObject2D.rotatingMethod = (RotatingMethod)EditorGUILayout.EnumPopup(selected2DRadar._RadarCenterObject2D.rotatingMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        switch (selected2DRadar._RadarCenterObject2D.rotatingMethod)
                        {
                            case RotatingMethod.singleAxis:


                                HelpMessage("choose a specific rotation axis of the tracked object to use to rotate the blip through the Y axis");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Target Axis");
                                selected2DRadar._RadarCenterObject2D.targetRotationAxis = (TargetRotationAxis)EditorGUILayout.EnumPopup(selected2DRadar._RadarCenterObject2D.targetRotationAxis, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                break;
                            case RotatingMethod.multiAxis:

                                HelpMessage("Freeze rotation through the x,y, or z axis");

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Freeze");
                                selected2DRadar._RadarCenterObject2D.lockX = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.lockX, "X");
                                selected2DRadar._RadarCenterObject2D.lockY = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.lockY, "Y");
                                selected2DRadar._RadarCenterObject2D.lockZ = GUILayout.Toggle(selected2DRadar._RadarCenterObject2D.lockZ, "Z");
                                GUILayout.EndHorizontal();
                                break;
                        }






                    }


                    #endregion

                }


                Separator();


                GUILayout.Space(20);

                var blipSettingsArea = GUILayoutUtility.GetLastRect().PlaceUnder(ScreenRect.width - 35, 180);
                GUI.Box(blipSettingsArea, "", skin.customStyles[2]);
                GUI.DrawTexture(blipSettingsArea.ToUpperLeft(0, 3, 0, 15), Textures.DuskLighter);
                #endregion

                #region Universal Settings

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Create blip");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                selected2DRadar._RadarCenterObject2D._CreateBlipAs = (CreateBlipAs)EditorGUILayout.EnumPopup(selected2DRadar._RadarCenterObject2D._CreateBlipAs, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("From gameObjects tagged");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                selected2DRadar._RadarCenterObject2D.Tag = EditorGUILayout.TagField(selected2DRadar._RadarCenterObject2D.Tag, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("And place on this layer");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                selected2DRadar._RadarCenterObject2D.Layer = EditorGUILayout.LayerField(selected2DRadar._RadarCenterObject2D.Layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (selected2DRadar._RadarCenterObject2D._CreateBlipAs == CreateBlipAs.AsMesh)
                {
                    EditorGUILayout.HelpBox("Meshes are not supporeted for 2D blips, will fallback to Sprite", MessageType.Warning);
                }


                #endregion

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            



            GUILayout.Space(15);
            Separator();




            #region Setting and creating All other Blips

            if (selected2DRadar.Blips.Count == 0)
                if (GUILayout.Button("Add a blip"))
                    selected2DRadar.Blips.Add(new RadarBlips2D());

            for (int i = 0; i < selected2DRadar.Blips.Count; i++)
            {
                var _Blip = selected2DRadar.Blips[i];


                if (_Blip == null) return;


                _Blip.State = _Blip.IsActive ? _Blip.Tag + " is Active" : _Blip.Tag + " is Inactive";

                Separator2();

                GUILayout.BeginHorizontal();
                _Blip.ShowBLipSettings = EditorGUILayout.Foldout(_Blip.ShowBLipSettings, _Blip.State);




                if (_Blip.BlipSprite)
                {
                    GUILayout.Box(_Blip.BlipSprite.texture, "Label", GUILayout.Width(18), GUILayout.Height(18));
                }


                if (GUILayout.Button((_Blip.IsActive) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18))) _Blip.IsActive = !_Blip.IsActive;

                GUILayout.Space(5);
                if (GUILayout.Button(new GUIContent(ImageLibrary.CopyIcon, "Copy"), "Label", GUILayout.Width(16), GUILayout.Height(16)))
                {
                    // copiedBlipData2D.DoRemoval = _Blip.DoRemoval;
                    //  copiedBlipData2D.Instanced = _Blip.Instanced;
                    copiedBlipData2D.IsActive = _Blip.IsActive;
                    copiedBlipData2D.ShowBLipSettings = _Blip.ShowBLipSettings;
                    copiedBlipData2D.ShowSpriteBlipSettings = _Blip.ShowSpriteBlipSettings;
                    copiedBlipData2D.ShowPrefabBlipSettings = _Blip.ShowPrefabBlipSettings;
                    copiedBlipData2D.IsTrackRotation = _Blip.IsTrackRotation;
                    copiedBlipData2D.lockX = _Blip.lockX;
                    copiedBlipData2D.lockY = _Blip.lockY;
                    copiedBlipData2D.lockZ = _Blip.lockZ;
                    copiedBlipData2D.BlipCanScleBasedOnDistance = _Blip.BlipCanScleBasedOnDistance;
                    copiedBlipData2D.ShowGeneralSettings = _Blip.ShowGeneralSettings;
                    copiedBlipData2D.ShowAdditionalOptions = _Blip.ShowAdditionalOptions;
                    copiedBlipData2D.AlwaysShowBlipsInRadarSpace = _Blip.AlwaysShowBlipsInRadarSpace;
                    copiedBlipData2D.ShowOptimizationSettings = _Blip.ShowOptimizationSettings;
                    copiedBlipData2D.SmoothScaleTransition = _Blip.SmoothScaleTransition;

                    copiedBlipData2D.rotatingMethod = _Blip.rotatingMethod;
                    copiedBlipData2D.targetRotationAxis = _Blip.targetRotationAxis;

                    copiedBlipData2D.icon = _Blip.icon;

                    copiedBlipData2D.State = _Blip.State;
                    copiedBlipData2D.Tag = _Blip.Tag;

                    copiedBlipData2D.SpriteMaterial = _Blip.SpriteMaterial;

                    copiedBlipData2D.colour = _Blip.colour;
                    copiedBlipData2D.BlipSize = _Blip.BlipSize;
                    // copiedBlipData2D.DynamicBlipSize = _Blip.DynamicBlipSize;
                    copiedBlipData2D.BlipMinSize = _Blip.BlipMinSize;
                    copiedBlipData2D.BlipMaxSize = _Blip.BlipMaxSize;

                    copiedBlipData2D.Layer = _Blip.Layer;
                    copiedBlipData2D.prefab = _Blip.prefab;
                    copiedBlipData2D._CreateBlipAs = _Blip._CreateBlipAs;
                    copiedBlipData2D.OrderInLayer = _Blip.OrderInLayer;
                    copiedBlipData2D.sortingLayer = _Blip.sortingLayer;
                    copiedBlipData2D.ObjectCount = _Blip.ObjectCount;

                    copiedBlipData2D.optimization = new OptimizationModule();

                    copiedBlipData2D.optimization.poolSize = _Blip.optimization.poolSize;
                    copiedBlipData2D.optimization.SetPoolSizeManually = _Blip.optimization.SetPoolSizeManually;
                    copiedBlipData2D.optimization.objectFindingMethod = _Blip.optimization.objectFindingMethod;
                    copiedBlipData2D.optimization.RemoveBlipsOnTagChange = _Blip.optimization.RemoveBlipsOnTagChange;
                    copiedBlipData2D.optimization.RemoveBlipsOnDisable = _Blip.optimization.RemoveBlipsOnDisable;
                    copiedBlipData2D.optimization.RequireInstanceObjectCheck = _Blip.optimization.RequireInstanceObjectCheck;
                    copiedBlipData2D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = _Blip.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;


                }

                GUILayout.Space(5);

                if (GUILayout.Button(new GUIContent(ImageLibrary.PasteIcon, "Paste"), "Label", GUILayout.Width(16), GUILayout.Height(16)))
                {
                    // copiedBlipData2D.DoRemoval = _Blip.DoRemoval;
                    //  copiedBlipData2D.Instanced = _Blip.Instanced;
                    _Blip.IsActive = copiedBlipData2D.IsActive;
                    _Blip.ShowBLipSettings = copiedBlipData2D.ShowBLipSettings;
                    _Blip.ShowSpriteBlipSettings = copiedBlipData2D.ShowSpriteBlipSettings;
                    _Blip.ShowPrefabBlipSettings = copiedBlipData2D.ShowPrefabBlipSettings;
                    _Blip.IsTrackRotation = copiedBlipData2D.IsTrackRotation;
                    _Blip.lockX = copiedBlipData2D.lockX;
                    _Blip.lockY = copiedBlipData2D.lockY;
                    _Blip.lockZ = copiedBlipData2D.lockZ;
                    _Blip.BlipCanScleBasedOnDistance = copiedBlipData2D.BlipCanScleBasedOnDistance;
                    _Blip.ShowGeneralSettings = copiedBlipData2D.ShowGeneralSettings;
                    _Blip.ShowAdditionalOptions = copiedBlipData2D.ShowAdditionalOptions;
                    _Blip.AlwaysShowBlipsInRadarSpace = copiedBlipData2D.AlwaysShowBlipsInRadarSpace;
                    _Blip.ShowOptimizationSettings = copiedBlipData2D.ShowOptimizationSettings;
                    _Blip.SmoothScaleTransition = copiedBlipData2D.SmoothScaleTransition;

                    _Blip.rotatingMethod = copiedBlipData2D.rotatingMethod;
                    _Blip.targetRotationAxis = copiedBlipData2D.targetRotationAxis;

                    _Blip.icon = copiedBlipData2D.icon;

                    _Blip.State = copiedBlipData2D.State;
                    _Blip.Tag = copiedBlipData2D.Tag;

                    _Blip.SpriteMaterial = copiedBlipData2D.SpriteMaterial;

                    _Blip.colour = copiedBlipData2D.colour;
                    _Blip.BlipSize = copiedBlipData2D.BlipSize;
                    // copiedBlipData2D.DynamicBlipSize = copiedBlipData2D.DynamicBlipSize;
                    _Blip.BlipMinSize = copiedBlipData2D.BlipMinSize;
                    _Blip.BlipMaxSize = copiedBlipData2D.BlipMaxSize;
                    _Blip.Layer = copiedBlipData2D.Layer;
                    _Blip.prefab = copiedBlipData2D.prefab;
                    _Blip._CreateBlipAs = copiedBlipData2D._CreateBlipAs;
                    _Blip.OrderInLayer = copiedBlipData2D.OrderInLayer;
                    _Blip.sortingLayer = copiedBlipData2D.sortingLayer;
                    _Blip.ObjectCount = copiedBlipData2D.ObjectCount;

                    _Blip.optimization = new OptimizationModule();

                    _Blip.optimization.poolSize = copiedBlipData2D.optimization.poolSize;
                    _Blip.optimization.SetPoolSizeManually = copiedBlipData2D.optimization.SetPoolSizeManually;
                    _Blip.optimization.objectFindingMethod = copiedBlipData2D.optimization.objectFindingMethod;
                    _Blip.optimization.RemoveBlipsOnTagChange = copiedBlipData2D.optimization.RemoveBlipsOnTagChange;
                    _Blip.optimization.RemoveBlipsOnDisable = copiedBlipData2D.optimization.RemoveBlipsOnDisable;
                    _Blip.optimization.RequireInstanceObjectCheck = copiedBlipData2D.optimization.RequireInstanceObjectCheck;
                    _Blip.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = copiedBlipData2D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;
                }

                GUILayout.EndHorizontal();


                if (_Blip.ShowBLipSettings)
                {
                    Separator();
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(5);
                    GUILayout.BeginVertical();

                    #region Sprite




                    HelpMessage("If using sprites as blips; set the Sprite and Material here");

                    _Blip.ShowSpriteBlipSettings = EditorGUILayout.Foldout(_Blip.ShowSpriteBlipSettings, _Blip.Tag + " Sprite");
                    if (_Blip.ShowSpriteBlipSettings)
                    {

                        #region Sprite Blip Design


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Sprite");
                        _Blip.BlipSprite = (Sprite)EditorGUILayout.ObjectField(_Blip.BlipSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        selected2DRadar.Blips[i].CanUseNullMaterial = selected2DRadar.RadarDesign.UseUI;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material");
                        selected2DRadar.Blips[i].BlipMaterial = (Material)EditorGUILayout.ObjectField(selected2DRadar.Blips[i].BlipMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Colour");
                        selected2DRadar.Blips[i].colour = EditorGUILayout.ColorField("", selected2DRadar.Blips[i].colour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Order In Layer");
                        selected2DRadar.Blips[i].OrderInLayer = EditorGUILayout.IntField(selected2DRadar.Blips[i].OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                        #endregion

                        if (_Blip.BlipSprite)
                        {
                            GUILayout.Box(_Blip.BlipSprite.texture, "Label", GUILayout.Width(50), GUILayout.Height(50));
                        }

                    }


                    Separator();

                    EditorGUILayout.Space();
                    #endregion

                    #region Prefab


                    HelpMessage("If using prefabs as blips; set the Prefab here");

                    _Blip.ShowPrefabBlipSettings = EditorGUILayout.Foldout(_Blip.ShowPrefabBlipSettings, _Blip.Tag + " Prefab");
                    if (_Blip.ShowPrefabBlipSettings)
                    {
                        #region Prefab BLip Design

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Prefab");
                        _Blip.prefab = (Transform)EditorGUILayout.ObjectField("", _Blip.prefab, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        #endregion
                    }


                    Separator();

                    EditorGUILayout.Space();
                    #endregion

                    #region Additional Options



                    HelpMessage("Displaying additional options for your blip");
                    selected2DRadar.Blips[i].ShowAdditionalOptions = EditorGUILayout.Foldout(selected2DRadar.Blips[i].ShowAdditionalOptions, "AdditionalOptions");
                    if (selected2DRadar.Blips[i].ShowAdditionalOptions)
                    {
                        HelpMessage("When eabled all " + _Blip.Tag + "blips will not disappear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on it's distance from the center object");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Always Show " + _Blip.Tag + " in radar");
                        selected2DRadar.Blips[i].AlwaysShowBlipsInRadarSpace = GUILayout.Toggle(selected2DRadar.Blips[i].AlwaysShowBlipsInRadarSpace, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                    }



                    Separator();

                    EditorGUILayout.Space();

                    #endregion

                    #region Optimization 



                    HelpMessage("Options for optimization the radar proceses");
                    GUILayout.BeginHorizontal();
                    selected2DRadar.Blips[i].ShowOptimizationSettings = EditorGUILayout.Foldout(selected2DRadar.Blips[i].ShowOptimizationSettings, "Optimization Options");
                    //GUILayout.Box(ImageLibrary.optimizeIcon, "Label", GUILayout.Width(120), GUILayout.Height(20));
                    GUILayout.EndHorizontal();
                    if (selected2DRadar.Blips[i].ShowOptimizationSettings)
                    {
                        if (selected2DRadar.Blips[i].optimization.objectFindingMethod == ObjectFindingMethod.Pooling)
                        {
                            HelpMessage("If you are spawning any new objects into the scene then call radar2D.DoInstanceObjectCheck() at instance or at the end of instancing");
                        }

                        if (selected2DRadar.Blips[i].optimization.objectFindingMethod != ObjectFindingMethod.Recursive)
                        {
                            HelpMessage("This requires that you call ' _2DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");

                            HelpMessage(" If you know exactly ow many scene objects this blip should represet then you can set the pool size manually");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Set Pool Size");
                            selected2DRadar.Blips[i].optimization.SetPoolSizeManually = EditorGUILayout.Toggle(selected2DRadar.Blips[i].optimization.SetPoolSizeManually, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                            if (selected2DRadar.Blips[i].optimization.SetPoolSizeManually)
                            {
                                HelpMessage("The mumber of scene objects that this blip will represent");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Pool Size");
                                selected2DRadar.Blips[i].optimization.poolSize = EditorGUILayout.IntField(selected2DRadar.Blips[i].optimization.poolSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                HelpMessage("If your pool size is too large then the count will be calculated DOWN");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Recalculate Pool Size On Start");
                                selected2DRadar.Blips[i].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = EditorGUILayout.Toggle(selected2DRadar.Blips[i].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();
                            }

                        }




                        HelpMessage("This method allows you to use object pooling to store your blips");
                        GUILayout.BeginHorizontal();
                        var info = selected2DRadar.Blips[i].optimization.objectFindingMethod == ObjectFindingMethod.Pooling ? "Pooling (Fast)" : (selected2DRadar.Blips[i].optimization.RequireInstanceObjectCheck) ? "Recursive (Fast)" : "Recursive (Slower)";
                        GUILayout.Label(info);
                        selected2DRadar.Blips[i].optimization.objectFindingMethod = (ObjectFindingMethod)EditorGUILayout.EnumPopup(selected2DRadar.Blips[i].optimization.objectFindingMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected2DRadar.Blips[i].optimization.objectFindingMethod == ObjectFindingMethod.Recursive)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Require Instance Object Check");
                            if (selected2DRadar.Blips[i].optimization.RequireInstanceObjectCheck)
                                HelpMessage("This requires that you call ' _2DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");
                            selected2DRadar.Blips[i].optimization.RequireInstanceObjectCheck = EditorGUILayout.Toggle(selected2DRadar.Blips[i].optimization.RequireInstanceObjectCheck, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }

                        HelpMessage("Allows blips to be removed whenever the object it represent changes it's tag");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Remove Blip On Tag Change");
                        selected2DRadar.Blips[i].optimization.RemoveBlipsOnTagChange = EditorGUILayout.Toggle(selected2DRadar.Blips[i].optimization.RemoveBlipsOnTagChange, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        HelpMessage("Allows for the blip to be turned off when the object it represents is disabled");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Remove Blip On Disable");
                        selected2DRadar.Blips[i].optimization.RemoveBlipsOnDisable = EditorGUILayout.Toggle(selected2DRadar.Blips[i].optimization.RemoveBlipsOnDisable, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                    }



                    Separator();

                    EditorGUILayout.Space();

                    #endregion

                    #region Scale and Rotation Settings



                    _Blip.ShowGeneralSettings = EditorGUILayout.Foldout(_Blip.ShowGeneralSettings, "Rotation and Scale");
                    if (_Blip.ShowGeneralSettings)
                    {
                        HelpMessage("Set the scale of the blip. If 'Scale by distance' is being used then the blips will scale in the radar based on their distance from the center object . the visibility of the scaling varies based on the size of the radar ");

                        #region Blip Scale Settings
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale by distance");
                        selected2DRadar.Blips[i].BlipCanScleBasedOnDistance = GUILayout.Toggle(selected2DRadar.Blips[i].BlipCanScleBasedOnDistance, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected2DRadar.Blips[i].BlipCanScleBasedOnDistance)
                        {
                            HelpMessage("This will make your blip not scale by distance o nthe Y axiz if you are also not using Track Y Position");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Don't Scale by Y distance");
                            selected2DRadar.Blips[i].IgnoreYDistanceScaling = GUILayout.Toggle(selected2DRadar.Blips[i].IgnoreYDistanceScaling, new GUIContent("", "This will make your blip not scale by distance o nthe Y axiz if you are also not using Track Y Position"), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }

                        if (!selected2DRadar.Blips[i].BlipCanScleBasedOnDistance)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            selected2DRadar.Blips[i].BlipSize = EditorGUILayout.FloatField(selected2DRadar.Blips[i].BlipSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Min Scale");
                            selected2DRadar.Blips[i].BlipMinSize = EditorGUILayout.FloatField("", selected2DRadar.Blips[i].BlipMinSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max Scale");
                            selected2DRadar.Blips[i].BlipMaxSize = EditorGUILayout.FloatField("", selected2DRadar.Blips[i].BlipMaxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                        }

                        #endregion

                        #region Rotation Settings
                        HelpMessage("Tells blips to face forward by ignoring the rotation of the radar and its content");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Ignore Radar Rotationn");
                        selected2DRadar.Blips[i].IgnoreRadarRotation = GUILayout.Toggle(selected2DRadar.Blips[i].IgnoreRadarRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected2DRadar.Blips[i].IgnoreRadarRotation)
                            selected2DRadar.Blips[i].IsTrackRotation = false;

                        HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Track Rotation");
                        selected2DRadar.Blips[i].IsTrackRotation = GUILayout.Toggle(selected2DRadar.Blips[i].IsTrackRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected2DRadar.Blips[i].IsTrackRotation)
                        {
                            selected2DRadar.Blips[i].IgnoreRadarRotation = false;

                            HelpMessage("Inverts the rotation value on all axis");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Invert Rotation");
                            selected2DRadar.Blips[i].InvertRotation = GUILayout.Toggle(selected2DRadar.Blips[i].InvertRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            HelpMessage("choose the way in which you want your blip to rotate with respect to its tracked object");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Rotation Method");

                            selected2DRadar.Blips[i].rotatingMethod = (RotatingMethod)EditorGUILayout.EnumPopup(selected2DRadar.Blips[i].rotatingMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            switch (selected2DRadar.Blips[i].rotatingMethod)
                            {
                                case RotatingMethod.singleAxis:


                                    HelpMessage("choose a specific rotation axis of the tracked object to use to rotate the blip through the Y axis");
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Target Axis");
                                    selected2DRadar.Blips[i].targetRotationAxis = (TargetRotationAxis)EditorGUILayout.EnumPopup(selected2DRadar.Blips[i].targetRotationAxis, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                    GUILayout.EndHorizontal();

                                    break;
                                case RotatingMethod.multiAxis:

                                    HelpMessage("Freeze rotation through the x,y, or z axis");

                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Freeze");
                                    selected2DRadar.Blips[i].lockX = GUILayout.Toggle(selected2DRadar.Blips[i].lockX, "X");
                                    selected2DRadar.Blips[i].lockY = GUILayout.Toggle(selected2DRadar.Blips[i].lockY, "Y");
                                    selected2DRadar.Blips[i].lockZ = GUILayout.Toggle(selected2DRadar.Blips[i].lockZ, "Z");
                                    GUILayout.EndHorizontal();
                                    break;
                            }






                        }

                        #endregion

                    }



                    Separator();

                    GUILayout.Space(20);
                    #endregion

                    var blipSettingsArea = GUILayoutUtility.GetLastRect().PlaceUnder(ScreenRect.width - 35, 180);
                    GUI.Box(blipSettingsArea, "", skin.customStyles[2]);
                    GUI.DrawTexture(blipSettingsArea.ToUpperLeft(0, 3, 0, 15), Textures.DuskLighter);


                    #region Universal Settings

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginVertical();

                    GUILayout.Space(20);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Create blip");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    _Blip._CreateBlipAs = (CreateBlipAs)EditorGUILayout.EnumPopup(_Blip._CreateBlipAs, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("From gameObjects tagged");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    _Blip.Tag = EditorGUILayout.TagField(_Blip.Tag, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Box(ImageLibrary.DownFlowArrow, "Label", GUILayout.Width(15));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("And place on this layer");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    _Blip.Layer = EditorGUILayout.LayerField(_Blip.Layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    #endregion

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();


                }

                var conditionBodyFooter = GUILayoutUtility.GetLastRect().ToLowerRight(0, 5, 5, 10);
                GUI.Box(conditionBodyFooter, "", skin.customStyles[3]);
                var buttonArea = conditionBodyFooter.ToLowerRight(55, 14, 0, 14);
                GUI.Box(buttonArea, "", skin.customStyles[4]);

                var addConditionButtonArea = buttonArea.ToCenterLeft(8, 8, 10);
                if (ClickEvent.Click(0, addConditionButtonArea, ImageLibrary.addConditionIcon, "Add a new blip"))
                {
                    selected2DRadar.Blips.Insert(i + 1, new RadarBlips2D());
                }

                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                //  if(i!=0)
                if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon, "Delete this blip"))
                {
                    selected2DRadar.Blips.RemoveAt(i);
                    return;
                }

                if (_Blip._CreateBlipAs == CreateBlipAs.AsMesh)
                {
                    GUILayout.Space(25);
                    EditorGUILayout.HelpBox("Meshes are not supporeted for 2D blips, will fallback to Sprite", MessageType.Warning);
                }


                #endregion

                GUILayout.Space(40);
                Separator();

            }

            selected2DRadar.RadarDesign.ShowGeneralBlipSettings = EditorGUILayout.Foldout(selected2DRadar.RadarDesign.ShowGeneralBlipSettings, "General Blip Settings");

            GUILayout.Space(10);

            if (selected2DRadar.RadarDesign.ShowGeneralBlipSettings)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                selected2DRadar.RadarDesign.TrackYPosition = GUILayout.Toggle(selected2DRadar.RadarDesign.TrackYPosition, new GUIContent(selected2DRadar.RadarDesign.TrackYPosition ? "Using X,Y Position" : "Using X,Z Position", " This will make the radar track the x y position of blips instead of X Z position"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            #endregion

            GUILayout.Space(20);
        }
        #endregion

        Separator3();

        #region Visualize and HelpMessage
        HelpMessage("Turn on 'Visualize'. This show you your radar settings.");
        selected2DRadar.RadarDesign.Visualize = GUILayout.Toggle(selected2DRadar.RadarDesign.Visualize, "Visualize");
        HelpMessage("These help messages will help you to set up your radar");
        ShowHelpMessages = GUILayout.Toggle(ShowHelpMessages, "Show Help Messages");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button(new GUIContent("Add UI Target Tracker"," The Ui Target tracker allows you to setup on screen and off screen UI that will hover over and point at tracked gameobjects")))
        {
            if (!selected2DRadar.gameObject.GetComponent<UITargetTracker>())
                selected2DRadar.gameObject.AddComponent<UITargetTracker>();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        #endregion
    }

    #region Help Messages
    void HelpMessage(string message)
    {
        if (ShowHelpMessages) { EditorGUILayout.HelpBox(message, MessageType.Info); }
    }
    #endregion

    #region Separator
    void Separator()
    {
        var area = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(0, area.position.y + area.height, ScreenRect.width, 1), Textures.DuskLight);
    }

    void Separator2()
    {
        var area = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(0, area.position.y + area.height, ScreenRect.width, 20), Textures.lightTransBlue);
        GUILayout.Space(20);
    }

    private void Separator3(Rect rect, float width, float height)
    {
        GUILayout.Space(height);

        var repeatArea = rect.ToLowerLeft(width, height);
        GUI.DrawTextureWithTexCoords(repeatArea, ImageLibrary.RepeatableStipe, new Rect(0, 0, repeatArea.width / height, 1));
    }

    private void Separator3()
    {
        var area = GUILayoutUtility.GetLastRect().AddRect(-15, 20);
        Separator3(area, ScreenRect.width, 20);
        // GUILayout.Space(20);

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

        radar.GetComponent<_2DRadar>().RadarDesign.renderingCamera = RenderCamera.GetComponent<Camera>();



    }
    #endregion

    #region Create Realitime Minimap Camera

    Camera CreateRealtimeMinimapCamera(Type radarType)
    {
        GameObject RenderCamera = new GameObject("Realtime Minimap Camera");
        RenderCamera.transform.Rotate(new Vector2(90, 0), Space.World);
        RenderCamera.AddComponent<Camera>();
        RenderCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        RenderCamera.GetComponent<Camera>().farClipPlane = 50;
        RenderCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
        RenderCamera.GetComponent<Camera>().depth = 0;
        RenderCamera.GetComponent<Camera>().orthographic = true;
        try
        {

            RenderCamera.GetComponent<Camera>().targetTexture = selected2DRadar.minimapModule.renderTexture;

        }
        catch
        {
            Debug.LogWarning("You have no set up a render texture your Realtime Minimap Camera yet");
        }
        return RenderCamera.GetComponent<Camera>();
    }
    #endregion

    #region variables
    private Rect ScreenRect = new Rect(0, 0, 0, 0);
    private _2DRadar selected2DRadar;
    private EditorWindow[] edwin;

    // SerializedProperty _RadarCenterObject2D;
    //  SerializedProperty Blips;
    //  SerializedProperty RadarDesign;
    // SerializedProperty minimapModule;

    bool ShowHelpMessages;
    private RadarBlips2D copiedBlipData2D = new RadarBlips2D();
    GUISkin skin;

    //   SerializedProperty
    #endregion
}