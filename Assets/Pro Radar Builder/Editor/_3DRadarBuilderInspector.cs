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


[CustomEditor(typeof(_3DRadar))]
[CanEditMultipleObjects]
public class _3DRadarBuilderInspector : Editor
{
    public void OnEnable()
    {
        selected3DRadar = (_3DRadar)target;
        edwin = Resources.FindObjectsOfTypeAll(typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow")) as EditorWindow[];

        skin = Resources.Load<GUISkin>("PRBSkin");

    }
    public override bool RequiresConstantRepaint()
    {
        return true;
    }
    public override void OnInspectorGUI()
    {
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
        selected3DRadar.RadarDesign.ShowDesignsArea = GUILayout.Toggle(selected3DRadar.RadarDesign.ShowDesignsArea, new GUIContent("Design", "This section will allows you to begin the first phase of your system design"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected3DRadar.RadarDesign.ShowDesignsArea)
        {





            #region Camera Section

            GUILayout.BeginHorizontal();
            selected3DRadar.RadarDesign.ShowRenderCameraSettings = EditorGUILayout.Foldout(selected3DRadar.RadarDesign.ShowRenderCameraSettings, new GUIContent( "Cameras", "Here you set up your rendering camera and main camera \n The Render camera is used to ONLY render the radar and the other camers is used to render your scene and as a reference for the transform and rotation of the Render camera"));
            selected3DRadar.RadarDesign._3DSystemsWithScreenSpaceFunction = GUILayout.Toggle(selected3DRadar.RadarDesign._3DSystemsWithScreenSpaceFunction, "Screen Space", EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
            GUILayout.EndHorizontal();

            EditorGUI.BeginDisabledGroup(!selected3DRadar.RadarDesign._3DSystemsWithScreenSpaceFunction);

            if (selected3DRadar.RadarDesign.ShowRenderCameraSettings)
            {
                HelpMessage("The Rendering camera");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Manual Camera Setup");
                selected3DRadar.RadarDesign.ManualCameraSetup = GUILayout.Toggle(selected3DRadar.RadarDesign.ManualCameraSetup, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                if (selected3DRadar.RadarDesign.ManualCameraSetup)
                {
                    HelpMessage("you can drag and drop a camera here or set one via script");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Camera");
                    selected3DRadar.RadarDesign.camera = (Camera)EditorGUILayout.ObjectField("", selected3DRadar.RadarDesign.camera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    HelpMessage("Will always find and set the scenes Main Camera as the default Camera  for the radar");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Always use main camera");
                    selected3DRadar.RadarDesign.UseMainCamera = GUILayout.Toggle(selected3DRadar.RadarDesign.UseMainCamera, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Finds a camera with the selected tag and uses it as the radars camera");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use camera with tag");
                    selected3DRadar.RadarDesign.CameraTag = EditorGUILayout.TagField("", selected3DRadar.RadarDesign.CameraTag, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }

                HelpMessage("YOU MUST HAVE A RENDERING CAMERA");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Render Camera");
                selected3DRadar.RadarDesign.renderingCamera = (Camera)EditorGUILayout.ObjectField("", selected3DRadar.RadarDesign.renderingCamera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();

            #endregion

            #region Minimap Settings

            GUILayout.BeginHorizontal();
            selected3DRadar.RadarDesign.ShowMinimapSettings = EditorGUILayout.Foldout(selected3DRadar.RadarDesign.ShowMinimapSettings, "Minimap " + (selected3DRadar.RadarDesign._3DSystemsWithMinimapFunction ? " is on" : " is off"));
            selected3DRadar.RadarDesign._3DSystemsWithMinimapFunction = GUILayout.Toggle(selected3DRadar.RadarDesign._3DSystemsWithMinimapFunction, selected3DRadar.RadarDesign._3DSystemsWithMinimapFunction ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18));
            GUILayout.EndHorizontal();


            EditorGUI.BeginDisabledGroup(!selected3DRadar.RadarDesign._3DSystemsWithMinimapFunction);

            if (selected3DRadar.RadarDesign.ShowMinimapSettings)
            {

                HelpMessage("When set to Realtime, the minimap texture will be drawn from the curent scene. If static then a your predesigned map will be used");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Minimap Type");
                selected3DRadar.minimapModule.mapType = (MapType)EditorGUILayout.EnumPopup(selected3DRadar.minimapModule.mapType, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                if (selected3DRadar.minimapModule.mapType != MapType.Realtime)
                {
                    HelpMessage("The static texture2D image that will be your map");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Minimap Texture");
                    selected3DRadar.minimapModule.MapTexture = (Sprite)EditorGUILayout.ObjectField(selected3DRadar.minimapModule.MapTexture, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Sets the ratio which the radars internal system will use to ensure consistency in your minimap");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Calibrate Minimap");
                    selected3DRadar.minimapModule.calibrate = EditorGUILayout.Toggle("", selected3DRadar.minimapModule.calibrate, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    HelpMessage("Empty gameobject which is to be placed at the center of where your minimp image is located in game");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Map Center Reference Object");
                    selected3DRadar.minimapModule.MapCenterReference = (Transform)EditorGUILayout.ObjectField("", selected3DRadar.minimapModule.MapCenterReference, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    HelpMessage("The order of the map sprite in the layer");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Order In layer");
                    selected3DRadar.minimapModule.OrderInLayer = EditorGUILayout.IntField(selected3DRadar.minimapModule.OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    HelpMessage("The render texture which will be used to pass the data from Realtime Minimap Camera to the Map");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Render Texture");
                    selected3DRadar.minimapModule.renderTexture = (RenderTexture)EditorGUILayout.ObjectField(selected3DRadar.minimapModule.renderTexture, typeof(RenderTexture), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (!selected3DRadar.minimapModule.RealtimeMinimapCamera)
                    {
                        if (GUILayout.Button("Create Minimap Camera", GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                        {
                            selected3DRadar.minimapModule.RealtimeMinimapCamera = CreateRealtimeMinimapCamera(typeof(_3DRadar));

                        }

                    }
                    else
                    {
                        HelpMessage("Your minimap camera ");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Minimap Camera");
                        selected3DRadar.minimapModule.RealtimeMinimapCamera = (Camera)EditorGUILayout.ObjectField(selected3DRadar.minimapModule.RealtimeMinimapCamera, typeof(Camera), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();
                    }
                    HelpMessage("The y position of your Realtime Minimap Camera");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Camera Height");
                    selected3DRadar.minimapModule.CameraHeight = EditorGUILayout.FloatField(selected3DRadar.minimapModule.CameraHeight, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }



                HelpMessage("the Material to be placed on the Minimap, this material must be able to be masked and it's shader must allow for Texture images");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Map Material");
                selected3DRadar.minimapModule.MapMaterial = (Material)EditorGUILayout.ObjectField(selected3DRadar.minimapModule.MapMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                HelpMessage("The material that will Mask the Map in a radar");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Mask Material");
                selected3DRadar.minimapModule.MaskMaterial = (Material)EditorGUILayout.ObjectField(selected3DRadar.minimapModule.MaskMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                HelpMessage("Places the map components on a specific layer at runtime");
                GUILayout.BeginHorizontal();
                GUILayout.Label("On Layer"); selected3DRadar.minimapModule.layer = EditorGUILayout.LayerField(selected3DRadar.minimapModule.layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();


                if (selected3DRadar.minimapModule.calibrate && selected3DRadar.minimapModule.mapType == MapType.Static)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Map Scale");
                    selected3DRadar.minimapModule.MapScale = EditorGUILayout.FloatField("", selected3DRadar.minimapModule.MapScale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }



            }
            EditorGUI.EndDisabledGroup();


            Separator();
            EditorGUILayout.Space();

            #endregion

            #region Scale Settings
            HelpMessage("The scale setting of your radar");
            selected3DRadar.RadarDesign.ShowScaleSettings = EditorGUILayout.Foldout(selected3DRadar.RadarDesign.ShowScaleSettings, "Scale");

            if (selected3DRadar.RadarDesign.ShowScaleSettings)
            {
                HelpMessage("This will override the Radar Diameter value to make the radar be set to the default scale x,y,z in the inspector");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Use Local Scale");
                selected3DRadar.RadarDesign.UseLocalScale = GUILayout.Toggle(selected3DRadar.RadarDesign.UseLocalScale, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                if (!selected3DRadar.RadarDesign.UseLocalScale)
                {
                    HelpMessage("Radar Diameter is the diameter of the radar");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Radar Diameter");
                    selected3DRadar.RadarDesign.RadarDiameter = EditorGUILayout.FloatField(Mathf.Clamp(selected3DRadar.RadarDesign.RadarDiameter, 0, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }

                HelpMessage("Anything outside of the tracking bounds will not be seen");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Tracking Bounds");
                selected3DRadar.RadarDesign.TrackingBounds = EditorGUILayout.FloatField(Mathf.Clamp(selected3DRadar.RadarDesign.TrackingBounds, 0, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();






                HelpMessage("When selected, the scale of the radar once greater than or less than 1 ; will be ignored");

                if (selected3DRadar.RadarDesign._3DSystemsWithScreenSpaceFunction)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Ignore Diameter Scale");
                    selected3DRadar.RadarDesign.IgnoreDiameterScale = GUILayout.Toggle(selected3DRadar.RadarDesign.IgnoreDiameterScale, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                }

                HelpMessage("Scene Scale is the zoom of the radar or how much units of space the radar can 'read'");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Scene Scale");
                selected3DRadar.RadarDesign.SceneScale = EditorGUILayout.FloatField(Mathf.Clamp(selected3DRadar.RadarDesign.SceneScale, 1, Mathf.Infinity), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();

                HelpMessage("Anything inside this area will be culled (not seen)");

                GUILayout.BeginHorizontal();
                GUILayout.Label("Inner Culling Zone");
                selected3DRadar.RadarDesign.InnerCullingZone = EditorGUILayout.FloatField(Mathf.Clamp(selected3DRadar.RadarDesign.InnerCullingZone, 0, selected3DRadar.RadarDesign.TrackingBounds), GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            Separator();

            EditorGUILayout.Space();
            #endregion

            #region Positioning  Section
            if (selected3DRadar.RadarDesign._3DSystemsWithScreenSpaceFunction)
            {
                HelpMessage("Position setting of your radar in screen space");

                selected3DRadar.RadarDesign.ShowPositioningSettings = EditorGUILayout.Foldout(selected3DRadar.RadarDesign.ShowPositioningSettings, "Position");
                if (selected3DRadar.RadarDesign.ShowPositioningSettings)
                {

                    #region Positioning settings
                    HelpMessage("Choose between 9 point snapping or Manual positioning");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Radar Positioning");
                    selected3DRadar.RadarDesign.radarPositioning = (RadarPositioning)EditorGUILayout.EnumPopup("", selected3DRadar.RadarDesign.radarPositioning, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    switch (selected3DRadar.RadarDesign.radarPositioning)
                    {
                        case RadarPositioning.Manual:
                            HelpMessage("Position the radar manually on the x and y axis");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("X and Y position");
                            selected3DRadar.RadarDesign.RadarRect.position = EditorGUILayout.Vector2Field("", selected3DRadar.RadarDesign.RadarRect.position, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            break;
                        case RadarPositioning.Snap:
                            HelpMessage("Use our 9 point snapping  to snap the position of your radar to 9 dirent points on your screen");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Snap to the");
                            selected3DRadar.RadarDesign.snapPosition = (SnapPosition)EditorGUILayout.EnumPopup("", selected3DRadar.RadarDesign.snapPosition, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            HelpMessage("In order to correct for camera distortion of scene objects closer to the center of the screen , you can turn on this function to make snaps to the Top Left,Right, Bottom Left, Right, Middle Left, Right not distort");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Use Orthographic For Side Snaps");
                            selected3DRadar.RadarDesign.UseOrthographicForSideSnaps = EditorGUILayout.Toggle("", selected3DRadar.RadarDesign.UseOrthographicForSideSnaps, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                            break;

                    }
                    #endregion

                    #region FrontIs Settings
                    HelpMessage("Determine what the front facing direction of the radar is ");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Front is");
                    selected3DRadar.RadarDesign.frontIs = (FrontIs)EditorGUILayout.EnumPopup("", selected3DRadar.RadarDesign.frontIs, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();
                    #endregion

                    HelpMessage("Pading on the X axis");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("X Pading");
                    selected3DRadar.RadarDesign.xPadding = EditorGUILayout.FloatField(selected3DRadar.RadarDesign.xPadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    HelpMessage("Pading on the Y axis");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Y Pading");
                    selected3DRadar.RadarDesign.yPadding = EditorGUILayout.FloatField(selected3DRadar.RadarDesign.yPadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();



                }
                GUILayout.Space(5);
                Separator();

                EditorGUILayout.Space();
            }
            #endregion
        }

        #endregion

        Separator3();

        #region Rotation Targets

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selected3DRadar.RadarDesign.ShowRotationTargetsArea = GUILayout.Toggle(selected3DRadar.RadarDesign.ShowRotationTargetsArea, new GUIContent("Rotation Targets", "This section will allows you to set rotation targets for specific objects"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected3DRadar.RadarDesign.ShowRotationTargetsArea)
        {


            if (selected3DRadar.RadarDesign.RotationTargets.Count == 0)
            {
                if (GUILayout.Button("Add a Rotation target ?"))
                    selected3DRadar.RadarDesign.RotationTargets.Add(new RotationTarget());

            }

            for (int i = 0; i < selected3DRadar.RadarDesign.RotationTargets.Count; i++)
            {

                var rotationTarget = selected3DRadar.RadarDesign.RotationTargets[i];

                var contentBodyArea = GUILayoutUtility.GetLastRect().PlaceUnder(ScreenRect.width - 35, ShowHelpMessages? 610 : 410);
                GUI.Box(contentBodyArea, "", skin.customStyles[2]);


                if (rotationTarget == null) return;

                string Target = (!rotationTarget.TargetedObject) ? (rotationTarget.target == TargetBlip.InstancedBlip) ? rotationTarget.InstancedTargetBlipname : "nothing " : rotationTarget.TargetedObject.name;


                GUILayout.FlexibleSpace();

                var upButtonArea = contentBodyArea.ToUpperLeft(15, 10, 130, 2.5f);

                if (ClickEvent.Click(0, upButtonArea, ImageLibrary.upArrow))
                {
                    if (i != 0)
                    {
                        var temptarget = rotationTarget;
                        selected3DRadar.RadarDesign.RotationTargets[i] = selected3DRadar.RadarDesign.RotationTargets[i - 1];
                        selected3DRadar.RadarDesign.RotationTargets[i - 1] = temptarget;
                    }
                }


                var downButtonArea = upButtonArea.PlaceToRight(0, 0, 20);
                if (ClickEvent.Click(0, downButtonArea, ImageLibrary.downArrow))
                {
                    if (i != selected3DRadar.RadarDesign.RotationTargets.Count - 1)
                    {
                        var temptarget = rotationTarget;
                        selected3DRadar.RadarDesign.RotationTargets[i] = selected3DRadar.RadarDesign.RotationTargets[i + 1];
                        selected3DRadar.RadarDesign.RotationTargets[i + 1] = temptarget;
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

                HelpMessage("This value should usually  set to 90 for sprite objects");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Add Rotation (optional)");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("X");
                rotationTarget.AddedXRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.AddedXRotation, 0, Mathf.Infinity), GUILayout.Width(40));
                GUILayout.Label("Y");
                rotationTarget.AddedYRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.AddedYRotation, 0, Mathf.Infinity), GUILayout.Width(40));
                GUILayout.Label("Z");
                rotationTarget.AddedZRotation = EditorGUILayout.FloatField(Mathf.Clamp(rotationTarget.AddedZRotation, 0, Mathf.Infinity), GUILayout.Width(40));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                HelpMessage("Freze rotation on any axis");



                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Freeze Rotation (optional)");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                rotationTarget.FreezeX = GUILayout.Toggle(rotationTarget.FreezeX, "X");
                rotationTarget.FreezeY = GUILayout.Toggle(rotationTarget.FreezeY, "Y");
                rotationTarget.FreezeZ = GUILayout.Toggle(rotationTarget.FreezeZ, "Z");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                HelpMessage("Replace the rotation on any axis of your target object with that of the targeted object");

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Retargeted Rotation (optional)");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("X");
                rotationTarget.retargetedXRotation = (RetargetX)EditorGUILayout.EnumPopup(rotationTarget.retargetedXRotation, GUILayout.Width(40));
                GUILayout.Label("Y");
                rotationTarget.retargetedYRotation = (RetargetY)EditorGUILayout.EnumPopup(rotationTarget.retargetedYRotation, GUILayout.Width(40));
                GUILayout.Label("Z");
                rotationTarget.retargetedZRotation = (RetargetZ)EditorGUILayout.EnumPopup(rotationTarget.retargetedZRotation, GUILayout.Width(40));
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
                    selected3DRadar.RadarDesign.RotationTargets.Insert(i + 1, new RotationTarget());
                }

                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                //  if(i!=0)
                if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon, "Delete this Rotation Target"))
                {
                    selected3DRadar.RadarDesign.RotationTargets.RemoveAt(i);
                    return;
                }

            }
        }
        #endregion

        Separator3();

        #region Blip Design

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selected3DRadar.RadarDesign.ShowBlipsArea = GUILayout.Toggle(selected3DRadar.RadarDesign.ShowBlipsArea, new GUIContent("Blips", "This section will allows you to design all your systems blips"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (selected3DRadar.RadarDesign.ShowBlipsArea)
        {
            #region Setting Blips

            HelpMessage("This blip ; if set to active will always appear at the center of your radar.");
            selected3DRadar._RadarCenterObject3D.State = selected3DRadar._RadarCenterObject3D.IsActive ? selected3DRadar._RadarCenterObject3D.Tag + " is Active" : selected3DRadar._RadarCenterObject3D.Tag + " is Inactive";

            Separator2();

            GUILayout.BeginHorizontal();
            selected3DRadar._RadarCenterObject3D.ShowBLipSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowBLipSettings, selected3DRadar._RadarCenterObject3D.State);

            if (selected3DRadar._RadarCenterObject3D.BlipSprite)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Space(80);
                GUILayout.Box(selected3DRadar._RadarCenterObject3D.BlipSprite.texture, "Label", GUILayout.Height(18), GUILayout.Width(18));
                GUILayout.FlexibleSpace();
            }

            if (GUILayout.Button((selected3DRadar._RadarCenterObject3D.IsActive) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18))) selected3DRadar._RadarCenterObject3D.IsActive = !selected3DRadar._RadarCenterObject3D.IsActive;
            GUILayout.EndHorizontal();

            if (selected3DRadar._RadarCenterObject3D.ShowBLipSettings)
            {
                Separator();
                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                #region sprite


                HelpMessage("If using sprites as blips; set the Sprite and Material here");
                selected3DRadar._RadarCenterObject3D.ShowSpriteBlipSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowSpriteBlipSettings, selected3DRadar._RadarCenterObject3D.Tag + " Sprite");

                if (selected3DRadar._RadarCenterObject3D.ShowSpriteBlipSettings)
                {
                    #region Sprite Blip Design



                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Sprite");
                    selected3DRadar._RadarCenterObject3D.BlipSprite = (Sprite)EditorGUILayout.ObjectField(selected3DRadar._RadarCenterObject3D.BlipSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material");
                    selected3DRadar._RadarCenterObject3D.BlipMaterial = (Material)EditorGUILayout.ObjectField("", selected3DRadar._RadarCenterObject3D.BlipMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Colour");
                    selected3DRadar._RadarCenterObject3D.colour = EditorGUILayout.ColorField("", selected3DRadar._RadarCenterObject3D.colour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Order In Layer");
                    selected3DRadar._RadarCenterObject3D.OrderInLayer = EditorGUILayout.IntField(selected3DRadar._RadarCenterObject3D.OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (selected3DRadar._RadarCenterObject3D.BlipSprite)
                        GUILayout.Box(selected3DRadar._RadarCenterObject3D.BlipSprite.texture, "Label", GUILayout.Height(50), GUILayout.Width(50));
                    #endregion
                }


                Separator();

                EditorGUILayout.Space();
                #endregion

                #region Mesh

                HelpMessage("If using mesh as blips; set the mesh and materials here");
                selected3DRadar._RadarCenterObject3D.ShowMeshBlipSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowMeshBlipSettings, selected3DRadar._RadarCenterObject3D.Tag + " Mesh");
                if (selected3DRadar._RadarCenterObject3D.ShowMeshBlipSettings)
                {
                    #region MeshBlipDesign

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Mesh");
                    selected3DRadar._RadarCenterObject3D.mesh = (Mesh)EditorGUILayout.ObjectField(selected3DRadar._RadarCenterObject3D.mesh, typeof(Mesh), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material Count");
                    selected3DRadar._RadarCenterObject3D.MatCount = Mathf.Clamp(EditorGUILayout.IntField("", selected3DRadar._RadarCenterObject3D.MatCount, GUILayout.MaxWidth(200), GUILayout.MaxWidth(150), GUILayout.MinWidth(50)), 0, 8000);
                    GUILayout.EndHorizontal();

                    if (Event.current.keyCode == KeyCode.Return)
                    {
                        Array.Resize(ref selected3DRadar._RadarCenterObject3D.MeshMaterials, selected3DRadar._RadarCenterObject3D.MatCount);

                    }


                    GUILayout.BeginVertical();
                    for (int i = 0; i < selected3DRadar._RadarCenterObject3D.MeshMaterials.Count(); i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material " + i);

                        selected3DRadar._RadarCenterObject3D.MeshMaterials[i] = (Material)EditorGUILayout.ObjectField(selected3DRadar._RadarCenterObject3D.MeshMaterials[i], typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));

                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();


                    #endregion
                }


                Separator();

                EditorGUILayout.Space();
                #endregion

                #region Prefab


                HelpMessage("If using prefab as blips; se the prefab here");
                selected3DRadar._RadarCenterObject3D.ShowPrefabBlipSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowPrefabBlipSettings, selected3DRadar._RadarCenterObject3D.Tag + " Prefab");
                if (selected3DRadar._RadarCenterObject3D.ShowPrefabBlipSettings)
                {
                    #region PrefabBLipDesign
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Prefab");
                    selected3DRadar._RadarCenterObject3D.prefab = (Transform)EditorGUILayout.ObjectField("", selected3DRadar._RadarCenterObject3D.prefab, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    #endregion
                }


                Separator();

                EditorGUILayout.Space();
                #endregion

                #region Tracking Line
                HelpMessage("Tracking lines allows you to visualize the y distance of your blips from the radar");



                string trackingLineState = (selected3DRadar._RadarCenterObject3D.UseTrackingLine) ? "Active" : "Inactive";

                GUILayout.BeginHorizontal();
                selected3DRadar._RadarCenterObject3D.ShowTrackingLineSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowTrackingLineSettings, "Tracking Line is " + trackingLineState);
                if (GUILayout.Button((selected3DRadar._RadarCenterObject3D.UseTrackingLine) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(20))) selected3DRadar._RadarCenterObject3D.UseTrackingLine = !selected3DRadar._RadarCenterObject3D.UseTrackingLine;
                GUILayout.EndHorizontal();


                if (selected3DRadar._RadarCenterObject3D.ShowTrackingLineSettings)
                {
                    HelpMessage("Set the Colour and Material here. You can set any material");
                    #region Tracking Line Design

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material");
                    selected3DRadar._RadarCenterObject3D.TrackingLineMaterial = (Material)EditorGUILayout.ObjectField("", selected3DRadar._RadarCenterObject3D.TrackingLineMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Start Colour");
                    selected3DRadar._RadarCenterObject3D.TrackingLineStartColour = EditorGUILayout.ColorField("", selected3DRadar._RadarCenterObject3D.TrackingLineStartColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("End Colour");
                    selected3DRadar._RadarCenterObject3D.TrackingLineEndColour = EditorGUILayout.ColorField("", selected3DRadar._RadarCenterObject3D.TrackingLineEndColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scale");
                    selected3DRadar._RadarCenterObject3D.TrackingLineDimention = EditorGUILayout.FloatField(selected3DRadar._RadarCenterObject3D.TrackingLineDimention, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    #region BaseTracker Design
                    HelpMessage("Base Trackers are sprites which appear that the base of the reacking line and are optional");
                    GUILayout.BeginHorizontal();
                    selected3DRadar._RadarCenterObject3D.ShowBaseTrackerSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowBaseTrackerSettings, "Base Tracker settings");
                    if (GUILayout.Button((selected3DRadar._RadarCenterObject3D.UseBaseTracker) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(20))) selected3DRadar._RadarCenterObject3D.UseBaseTracker = !selected3DRadar._RadarCenterObject3D.UseBaseTracker;
                    GUILayout.EndHorizontal();

                    if (selected3DRadar._RadarCenterObject3D.ShowBaseTrackerSettings)
                    {

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Base Tracker");
                        selected3DRadar._RadarCenterObject3D.BaseTrackerSprite = (Sprite)EditorGUILayout.ObjectField(selected3DRadar._RadarCenterObject3D.BaseTrackerSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material");
                        selected3DRadar._RadarCenterObject3D.BaseTrackerMaterial = (Material)EditorGUILayout.ObjectField("", selected3DRadar._RadarCenterObject3D.BaseTrackerMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Colour");
                        selected3DRadar._RadarCenterObject3D.BaseTrackerColour = EditorGUILayout.ColorField("", selected3DRadar._RadarCenterObject3D.BaseTrackerColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Size");
                        selected3DRadar._RadarCenterObject3D.BaseTrackerSize = EditorGUILayout.FloatField("", selected3DRadar._RadarCenterObject3D.BaseTrackerSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();



                        if (selected3DRadar._RadarCenterObject3D.BaseTrackerSprite)
                            GUILayout.Box(selected3DRadar._RadarCenterObject3D.BaseTrackerSprite.texture, GUILayout.Height(50), GUILayout.Width(50));




                    }
                    #endregion


                    #endregion
                }


                Separator();
                EditorGUILayout.Separator();
                #endregion

                #region Additional Options



                HelpMessage("Displaying additional options for your blip");
                selected3DRadar._RadarCenterObject3D.ShowAdditionalOptions = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowAdditionalOptions, "AdditionalOptions");
                if (selected3DRadar._RadarCenterObject3D.ShowAdditionalOptions)
                {
                    HelpMessage("When eabled all " + selected3DRadar._RadarCenterObject3D.Tag + "blips will not disppear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on its distance from the center object");

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Always Show " + selected3DRadar._RadarCenterObject3D.Tag + " in radar");
                    selected3DRadar._RadarCenterObject3D.AlwaysShowCenterObject = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.AlwaysShowCenterObject, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                }


                Separator();

                EditorGUILayout.Space();

                #endregion

                #region Scale And Rotation Settings




                selected3DRadar._RadarCenterObject3D.ShowGeneralSettings = EditorGUILayout.Foldout(selected3DRadar._RadarCenterObject3D.ShowGeneralSettings, "Rotation and Scale");
                if (selected3DRadar._RadarCenterObject3D.ShowGeneralSettings)
                {
                    #region Blip Scale Settings
                    if (!selected3DRadar._RadarCenterObject3D.CenterObjectCanScaleByDistance)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale");
                        selected3DRadar._RadarCenterObject3D.BlipSize = EditorGUILayout.FloatField(selected3DRadar._RadarCenterObject3D.BlipSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                    }


                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scale By Distance");
                    selected3DRadar._RadarCenterObject3D.CenterObjectCanScaleByDistance = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.CenterObjectCanScaleByDistance, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (selected3DRadar._RadarCenterObject3D.CenterObjectCanScaleByDistance)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Min Scale");
                        selected3DRadar._RadarCenterObject3D.BlipMinSize = EditorGUILayout.FloatField("", selected3DRadar._RadarCenterObject3D.BlipMinSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Max Scale");
                        selected3DRadar._RadarCenterObject3D.BlipMaxSize = EditorGUILayout.FloatField("", selected3DRadar._RadarCenterObject3D.BlipMaxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();



                    }
                    #endregion


                    #region Blip Rotation Settings

                    HelpMessage("Use custom rotation allows yo uto set a static rotation for all of these blip types");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Use Custom Rotation");
                    selected3DRadar._RadarCenterObject3D.UseCustomRotation = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.UseCustomRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();

                    if (selected3DRadar._RadarCenterObject3D.UseCustomRotation && !selected3DRadar._RadarCenterObject3D.IsTrackRotation)
                    {
                        HelpMessage("Set a X, Y and Z rotation for this blip type");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Rotation");
                        GUILayout.Label("X");
                        selected3DRadar._RadarCenterObject3D.CustomXRotation = EditorGUILayout.FloatField(selected3DRadar._RadarCenterObject3D.CustomXRotation);
                        GUILayout.Label("Y");
                        selected3DRadar._RadarCenterObject3D.CustomYRotation = EditorGUILayout.FloatField(selected3DRadar._RadarCenterObject3D.CustomYRotation);
                        GUILayout.Label("Z");
                        selected3DRadar._RadarCenterObject3D.CustomZRotation = EditorGUILayout.FloatField(selected3DRadar._RadarCenterObject3D.CustomZRotation);
                        GUILayout.EndHorizontal();
                    }

                    HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Track Rotation");
                    selected3DRadar._RadarCenterObject3D.IsTrackRotation = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.IsTrackRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                    GUILayout.EndHorizontal();


                    if (selected3DRadar._RadarCenterObject3D.IsTrackRotation && !selected3DRadar._RadarCenterObject3D.UseCustomRotation)
                    {
                        HelpMessage("Freeze rotation through the x,y, or z axis");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Freeze");
                        selected3DRadar._RadarCenterObject3D.lockX = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.lockX, "X");
                        selected3DRadar._RadarCenterObject3D.lockY = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.lockY, "Y");
                        selected3DRadar._RadarCenterObject3D.lockZ = GUILayout.Toggle(selected3DRadar._RadarCenterObject3D.lockZ, "Z");
                        GUILayout.EndHorizontal();
                    }
                    #endregion
                    if (selected3DRadar._RadarCenterObject3D.IsTrackRotation && selected3DRadar._RadarCenterObject3D.UseCustomRotation)
                    {
                        EditorGUILayout.HelpBox("Do not use 'Track Rotation' and 'Custom Rotation at the same time'", MessageType.Warning);
                    }
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
                selected3DRadar._RadarCenterObject3D._CreateBlipAs = (CreateBlipAs)EditorGUILayout.EnumPopup(selected3DRadar._RadarCenterObject3D._CreateBlipAs, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
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
                selected3DRadar._RadarCenterObject3D.Tag = EditorGUILayout.TagField(selected3DRadar._RadarCenterObject3D.Tag, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
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
                selected3DRadar._RadarCenterObject3D.Layer = EditorGUILayout.LayerField(selected3DRadar._RadarCenterObject3D.Layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();




                #endregion

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
           



            GUILayout.Space(15);
            Separator();




            #region Setting and creating All other Blips

            if (selected3DRadar.Blips.Count == 0)
                if (GUILayout.Button("Add a blip"))
                    selected3DRadar.Blips.Add(new RadarBlips3D());

            for (int j = 0; j < selected3DRadar.Blips.Count; j++)
            {
                var _Blip = selected3DRadar.Blips[j];

                if (_Blip == null) return;

                _Blip.State = _Blip.IsActive ? _Blip.Tag + " is Active" : _Blip.Tag + " is Inactive";

                Separator2();

                GUILayout.BeginHorizontal();
                _Blip.ShowBLipSettings = EditorGUILayout.Foldout(_Blip.ShowBLipSettings, _Blip.State);




                if (_Blip.BlipSprite)
                {
                    GUILayout.Box(_Blip.BlipSprite.texture, "Label", GUILayout.Height(18), GUILayout.Width(18));
                }

                if (GUILayout.Button((_Blip.IsActive) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(18), GUILayout.Height(18))) _Blip.IsActive = !_Blip.IsActive;

                GUILayout.Space(5);
                if (GUILayout.Button(new GUIContent(ImageLibrary.CopyIcon, "Copy"), "Label", GUILayout.Width(16), GUILayout.Height(16)))
                {
                    // copiedBlipData3D.DoRemoval = _Blip.DoRemoval;
                    //  copiedBlipData3D.Instanced = _Blip.Instanced;
                    copiedBlipData3D.IsActive = _Blip.IsActive;
                    copiedBlipData3D.ShowBLipSettings = _Blip.ShowBLipSettings;
                    copiedBlipData3D.ShowSpriteBlipSettings = _Blip.ShowSpriteBlipSettings;
                    copiedBlipData3D.ShowMeshBlipSettings = _Blip.ShowMeshBlipSettings;
                    copiedBlipData3D.ShowPrefabBlipSettings = _Blip.ShowPrefabBlipSettings;
                    copiedBlipData3D.IsTrackRotation = _Blip.IsTrackRotation;
                    copiedBlipData3D.BlipCanScleBasedOnDistance = _Blip.BlipCanScleBasedOnDistance;
                    copiedBlipData3D.ShowTrackingLineSettings = _Blip.ShowTrackingLineSettings;
                    copiedBlipData3D.UseTrackingLine = _Blip.UseTrackingLine;
                    copiedBlipData3D.UseBaseTracker = _Blip.UseBaseTracker;
                    copiedBlipData3D.ShowBaseTrackerSettings = _Blip.ShowBaseTrackerSettings;
                    copiedBlipData3D.lockX = _Blip.lockX;
                    copiedBlipData3D.lockY = _Blip.lockY;
                    copiedBlipData3D.lockZ = _Blip.lockZ;
                    copiedBlipData3D.UseLOD = _Blip.UseLOD;
                    copiedBlipData3D.ShowLODSettings = _Blip.ShowLODSettings;
                    copiedBlipData3D.ShowGeneralSettings = _Blip.ShowGeneralSettings;
                    copiedBlipData3D.ShowAdditionalOptions = _Blip.ShowAdditionalOptions;
                    copiedBlipData3D.AlwaysShowBlipsInRadarSpace = _Blip.AlwaysShowBlipsInRadarSpace;
                    copiedBlipData3D.ShowLowMeshSetings = _Blip.ShowLowMeshSetings;
                    copiedBlipData3D.ShowLowMeshSetings = _Blip.ShowLowMeshSetings;
                    copiedBlipData3D.ShowMediumMeshSettings = _Blip.ShowMediumMeshSettings;
                    copiedBlipData3D.ShowHighMeshSettings = _Blip.ShowHighMeshSettings;
                    copiedBlipData3D.ShowOptimizationSettings = _Blip.ShowOptimizationSettings;
                    copiedBlipData3D.SmoothScaleTransition = _Blip.SmoothScaleTransition;
                    copiedBlipData3D.UseCustomRotation = _Blip.UseCustomRotation;
                    copiedBlipData3D.KeepBlipsAboveRadarPlane = _Blip.KeepBlipsAboveRadarPlane;
                    copiedBlipData3D.UseHeightTracking = _Blip.UseHeightTracking;


                    copiedBlipData3D.icon = _Blip.icon;

                    copiedBlipData3D.BaseTracker = _Blip.BaseTracker;

                    copiedBlipData3D.prefab = _Blip.prefab;

                    copiedBlipData3D.State = _Blip.State;
                    copiedBlipData3D.Tag = _Blip.Tag;

                    copiedBlipData3D.SpriteMaterial = _Blip.SpriteMaterial;

                    copiedBlipData3D.mesh = _Blip.mesh;
                    copiedBlipData3D.Low = _Blip.Low;
                    copiedBlipData3D.Medium = _Blip.Medium;
                    copiedBlipData3D.High = _Blip.High;

                    copiedBlipData3D.tlm = _Blip.tlm;
                    copiedBlipData3D.btm = _Blip.btm;


                    copiedBlipData3D.MeshMaterials = new Material[] { };
                    Array.Resize(ref copiedBlipData3D.MeshMaterials, _Blip.MeshMaterials.Length);

                    for (int q = 0; q < _Blip.MeshMaterials.Length; q++)
                    {
                        copiedBlipData3D.MeshMaterials[q] = _Blip.MeshMaterials[q];
                    }

                    copiedBlipData3D.colour = _Blip.colour;
                    copiedBlipData3D.TrackingLineStartColour = _Blip.TrackingLineStartColour;
                    copiedBlipData3D.TrackingLineEndColour = _Blip.TrackingLineEndColour;
                    copiedBlipData3D.BaseTrackerColour = _Blip.BaseTrackerColour;

                    copiedBlipData3D.BlipSize = _Blip.BlipSize;
                    // copiedBlipData3D.DynamicBlipSize = _Blip.DynamicBlipSize;
                    copiedBlipData3D.BlipMinSize = _Blip.BlipMinSize;
                    copiedBlipData3D.BlipMaxSize = _Blip.BlipMaxSize;
                    copiedBlipData3D.TrackingLineDimention = _Blip.TrackingLineDimention;
                    copiedBlipData3D.LowDistance = _Blip.LowDistance;
                    copiedBlipData3D.MediumDistance = _Blip.MediumDistance;
                    copiedBlipData3D.HighDistance = _Blip.HighDistance;
                    copiedBlipData3D.BaseTrackerSize = _Blip.BaseTrackerSize;
                    copiedBlipData3D.CustomXRotation = _Blip.CustomXRotation;
                    copiedBlipData3D.CustomYRotation = _Blip.CustomYRotation;
                    copiedBlipData3D.CustomZRotation = _Blip.CustomZRotation;
                    copiedBlipData3D.NumberOfBLips = _Blip.NumberOfBLips;
                    copiedBlipData3D.count = _Blip.count;
                    copiedBlipData3D.MatCount = _Blip.MatCount;
                    copiedBlipData3D.Layer = _Blip.Layer;
                    copiedBlipData3D._CreateBlipAs = _Blip._CreateBlipAs;
                    copiedBlipData3D.ObjectCount = _Blip.ObjectCount;
                    copiedBlipData3D.OrderInLayer = _Blip.OrderInLayer;
                    copiedBlipData3D.sortingLayer = _Blip.sortingLayer;

                    copiedBlipData3D.optimization = new OptimizationModule();

                    copiedBlipData3D.optimization.poolSize = _Blip.optimization.poolSize;
                    copiedBlipData3D.optimization.SetPoolSizeManually = _Blip.optimization.SetPoolSizeManually;
                    copiedBlipData3D.optimization.objectFindingMethod = _Blip.optimization.objectFindingMethod;
                    copiedBlipData3D.optimization.RemoveBlipsOnTagChange = _Blip.optimization.RemoveBlipsOnTagChange;
                    copiedBlipData3D.optimization.RemoveBlipsOnDisable = _Blip.optimization.RemoveBlipsOnDisable;
                    copiedBlipData3D.optimization.RequireInstanceObjectCheck = _Blip.optimization.RequireInstanceObjectCheck;
                    copiedBlipData3D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = _Blip.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;


                }

                GUILayout.Space(5);

                if (GUILayout.Button(new GUIContent(ImageLibrary.PasteIcon, "Paste"), "Label", GUILayout.Width(15)))
                {
                    // copiedBlipData3D.DoRemoval = copiedBlipData3D.DoRemoval;
                    //  copiedBlipData3D.Instanced = copiedBlipData3D.Instanced;
                    _Blip.IsActive = copiedBlipData3D.IsActive;
                    _Blip.ShowBLipSettings = copiedBlipData3D.ShowBLipSettings;
                    _Blip.ShowSpriteBlipSettings = copiedBlipData3D.ShowSpriteBlipSettings;
                    _Blip.ShowMeshBlipSettings = copiedBlipData3D.ShowMeshBlipSettings;
                    _Blip.ShowPrefabBlipSettings = copiedBlipData3D.ShowPrefabBlipSettings;
                    _Blip.IsTrackRotation = copiedBlipData3D.IsTrackRotation;
                    _Blip.BlipCanScleBasedOnDistance = copiedBlipData3D.BlipCanScleBasedOnDistance;
                    _Blip.ShowTrackingLineSettings = copiedBlipData3D.ShowTrackingLineSettings;
                    _Blip.UseTrackingLine = copiedBlipData3D.UseTrackingLine;
                    _Blip.UseBaseTracker = copiedBlipData3D.UseBaseTracker;
                    _Blip.ShowBaseTrackerSettings = copiedBlipData3D.ShowBaseTrackerSettings;
                    _Blip.lockX = copiedBlipData3D.lockX;
                    _Blip.lockY = copiedBlipData3D.lockY;
                    _Blip.lockZ = copiedBlipData3D.lockZ;
                    _Blip.UseLOD = copiedBlipData3D.UseLOD;
                    _Blip.ShowLODSettings = copiedBlipData3D.ShowLODSettings;
                    _Blip.ShowGeneralSettings = copiedBlipData3D.ShowGeneralSettings;
                    _Blip.ShowAdditionalOptions = copiedBlipData3D.ShowAdditionalOptions;
                    _Blip.AlwaysShowBlipsInRadarSpace = copiedBlipData3D.AlwaysShowBlipsInRadarSpace;
                    _Blip.ShowLowMeshSetings = copiedBlipData3D.ShowLowMeshSetings;
                    _Blip.ShowLowMeshSetings = copiedBlipData3D.ShowLowMeshSetings;
                    _Blip.ShowMediumMeshSettings = copiedBlipData3D.ShowMediumMeshSettings;
                    _Blip.ShowHighMeshSettings = copiedBlipData3D.ShowHighMeshSettings;
                    _Blip.ShowOptimizationSettings = copiedBlipData3D.ShowOptimizationSettings;
                    _Blip.SmoothScaleTransition = copiedBlipData3D.SmoothScaleTransition;
                    _Blip.UseCustomRotation = copiedBlipData3D.UseCustomRotation;
                    _Blip.KeepBlipsAboveRadarPlane = copiedBlipData3D.KeepBlipsAboveRadarPlane;
                    _Blip.UseHeightTracking = copiedBlipData3D.UseHeightTracking;


                    _Blip.icon = copiedBlipData3D.icon;

                    _Blip.BaseTracker = copiedBlipData3D.BaseTracker;

                    _Blip.prefab = copiedBlipData3D.prefab;

                    _Blip.State = copiedBlipData3D.State;
                    _Blip.Tag = copiedBlipData3D.Tag;

                    _Blip.SpriteMaterial = copiedBlipData3D.SpriteMaterial;

                    _Blip.mesh = copiedBlipData3D.mesh;
                    _Blip.Low = copiedBlipData3D.Low;
                    _Blip.Medium = copiedBlipData3D.Medium;
                    _Blip.High = copiedBlipData3D.High;

                    _Blip.tlm = copiedBlipData3D.tlm;
                    _Blip.btm = copiedBlipData3D.btm;


                    _Blip.MeshMaterials = new Material[] { };
                    Array.Resize(ref copiedBlipData3D.MeshMaterials, copiedBlipData3D.MeshMaterials.Length);

                    for (int q = 0; q < copiedBlipData3D.MeshMaterials.Length; q++)
                    {
                        copiedBlipData3D.MeshMaterials[q] = copiedBlipData3D.MeshMaterials[q];
                    }

                    _Blip.colour = copiedBlipData3D.colour;
                    _Blip.TrackingLineStartColour = copiedBlipData3D.TrackingLineStartColour;
                    _Blip.TrackingLineEndColour = copiedBlipData3D.TrackingLineEndColour;
                    _Blip.BaseTrackerColour = copiedBlipData3D.BaseTrackerColour;

                    _Blip.BlipSize = copiedBlipData3D.BlipSize;
                    // copiedBlipData3D.DynamicBlipSize = copiedBlipData3D.DynamicBlipSize;
                    _Blip.BlipMinSize = copiedBlipData3D.BlipMinSize;
                    _Blip.BlipMaxSize = copiedBlipData3D.BlipMaxSize;
                    _Blip.TrackingLineDimention = copiedBlipData3D.TrackingLineDimention;
                    _Blip.LowDistance = copiedBlipData3D.LowDistance;
                    _Blip.MediumDistance = copiedBlipData3D.MediumDistance;
                    _Blip.HighDistance = copiedBlipData3D.HighDistance;
                    _Blip.BaseTrackerSize = copiedBlipData3D.BaseTrackerSize;
                    _Blip.CustomXRotation = copiedBlipData3D.CustomXRotation;
                    _Blip.CustomYRotation = copiedBlipData3D.CustomYRotation;
                    _Blip.CustomZRotation = copiedBlipData3D.CustomZRotation;
                    _Blip.NumberOfBLips = copiedBlipData3D.NumberOfBLips;
                    _Blip.count = copiedBlipData3D.count;
                    _Blip.MatCount = copiedBlipData3D.MatCount;
                    _Blip.Layer = copiedBlipData3D.Layer;
                    _Blip._CreateBlipAs = copiedBlipData3D._CreateBlipAs;
                    _Blip.ObjectCount = copiedBlipData3D.ObjectCount;
                    _Blip.OrderInLayer = copiedBlipData3D.OrderInLayer;
                    _Blip.sortingLayer = copiedBlipData3D.sortingLayer;

                    _Blip.optimization = new OptimizationModule();

                    _Blip.optimization.poolSize = copiedBlipData3D.optimization.poolSize;
                    _Blip.optimization.SetPoolSizeManually = copiedBlipData3D.optimization.SetPoolSizeManually;
                    _Blip.optimization.objectFindingMethod = copiedBlipData3D.optimization.objectFindingMethod;
                    _Blip.optimization.RemoveBlipsOnTagChange = copiedBlipData3D.optimization.RemoveBlipsOnTagChange;
                    _Blip.optimization.RemoveBlipsOnDisable = copiedBlipData3D.optimization.RemoveBlipsOnDisable;
                    _Blip.optimization.RequireInstanceObjectCheck = copiedBlipData3D.optimization.RequireInstanceObjectCheck;
                    _Blip.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = copiedBlipData3D.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects;

                }

            

                GUILayout.EndHorizontal();


                //   GUILayout.BeginHorizontal();
                //  GUILayout.Space(5);
                //  GUILayout.BeginVertical();


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

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material");
                        _Blip.BlipMaterial = (Material)EditorGUILayout.ObjectField(_Blip.BlipMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Colour");
                        _Blip.colour = EditorGUILayout.ColorField("", _Blip.colour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Order In Layer");
                        _Blip.OrderInLayer = EditorGUILayout.IntField(_Blip.OrderInLayer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                        #endregion

                        if (_Blip.BlipSprite)
                        {
                            GUILayout.Box(_Blip.BlipSprite.texture, "Label", GUILayout.Height(50), GUILayout.Width(50));
                        }

                    }


                    Separator();

                    EditorGUILayout.Space();
                    #endregion

                    #region Mesh

                    HelpMessage("If using meshes as blips; set the Mesh and Materials here, Optional LODs and the distances at which each mesh is rendered in the radar");

                    _Blip.ShowMeshBlipSettings = EditorGUILayout.Foldout(_Blip.ShowMeshBlipSettings, _Blip.Tag + " Mesh");
                    if (_Blip.ShowMeshBlipSettings)
                    {
                        #region MeshBlipDesign

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Mesh");
                        _Blip.mesh = (Mesh)EditorGUILayout.ObjectField(_Blip.mesh, typeof(Mesh), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        #region LOD area

                        string statement = (_Blip.UseLOD) ? " is on" : " is off";

                        GUILayout.BeginHorizontal();
                        _Blip.ShowLODSettings = EditorGUILayout.Foldout(_Blip.ShowLODSettings, _Blip.Tag + " LOD" + statement);
                        if (GUILayout.Button((_Blip.UseLOD) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(20))) _Blip.UseLOD = !_Blip.UseLOD;
                        GUILayout.EndHorizontal();


                        if (_Blip.ShowLODSettings)
                        {
                            Separator();
                            EditorGUILayout.Separator();
                            #region LowSettings
                            _Blip.ShowLowMeshSetings = EditorGUILayout.Foldout(_Blip.ShowLowMeshSetings, "Low");
                            if (_Blip.ShowLowMeshSetings)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Mesh");
                                _Blip.Low = (Mesh)EditorGUILayout.ObjectField(_Blip.Low, typeof(Mesh), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Distance");
                                _Blip.LowDistance = EditorGUILayout.FloatField(_Blip.LowDistance, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();
                            }

                            #endregion
                            Separator();
                            EditorGUILayout.Separator();
                            #region MediumDistance
                            _Blip.ShowMediumMeshSettings = EditorGUILayout.Foldout(_Blip.ShowMediumMeshSettings, "Medium");
                            if (_Blip.ShowMediumMeshSettings)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Mesh");
                                _Blip.Medium = (Mesh)EditorGUILayout.ObjectField(_Blip.Medium, typeof(Mesh), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Distance");
                                _Blip.MediumDistance = EditorGUILayout.FloatField(_Blip.MediumDistance, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();
                            }

                            #endregion
                            Separator();
                            EditorGUILayout.Separator();
                            #region HighDistance
                            _Blip.ShowHighMeshSettings = EditorGUILayout.Foldout(_Blip.ShowHighMeshSettings, "High");
                            if (_Blip.ShowHighMeshSettings)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Mesh");
                                _Blip.High = (Mesh)EditorGUILayout.ObjectField(_Blip.High, typeof(Mesh), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Distance");
                                _Blip.HighDistance = EditorGUILayout.FloatField(_Blip.HighDistance, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();
                            }

                            #endregion
                            Separator();
                            EditorGUILayout.Separator();


                        }

                        #endregion


                        #region Define how many materials the mesh uses
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material Count");
                        _Blip.MatCount = Mathf.Clamp(EditorGUILayout.IntField("", _Blip.MatCount, GUILayout.MaxWidth(200), GUILayout.MaxWidth(150), GUILayout.MinWidth(50)), 0, 8000);
                        GUILayout.EndHorizontal();
                        #endregion

                        if (Event.current.keyCode == KeyCode.Return)
                        {
                            Array.Resize(ref _Blip.MeshMaterials, _Blip.MatCount);
                        }

                        GUILayout.BeginVertical();
                        for (int i = 0; i < _Blip.MeshMaterials.Count(); i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material " + i);


                            _Blip.MeshMaterials[i] = (Material)EditorGUILayout.ObjectField(_Blip.MeshMaterials[i], typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));

                            GUILayout.EndHorizontal();

                        }
                        GUILayout.EndVertical();
                        #endregion


                    }

                    Separator();

                    EditorGUILayout.Space();
                    #endregion

                    #region prefab

                    HelpMessage("If using sprites as blips; set the Prefab here");

                    _Blip.ShowPrefabBlipSettings = EditorGUILayout.Foldout(_Blip.ShowPrefabBlipSettings, _Blip.Tag + " Prefab");
                    if (_Blip.ShowPrefabBlipSettings)
                    {
                        #region Prefab BLip Design
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Prefab");
                        _Blip.prefab = (Transform)EditorGUILayout.ObjectField("", _Blip.prefab, typeof(Transform), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndVertical();
                        #endregion
                    }


                    Separator();

                    EditorGUILayout.Separator();
                    #endregion

                    #region Tracking Line
                    HelpMessage("Tracking lines allows you to visualize the y distance of your blips from the radar");



                    string trackingLineState = (selected3DRadar.Blips[j].UseTrackingLine) ? "Active" : "Inactive";

                    GUILayout.BeginHorizontal();
                    selected3DRadar.Blips[j].ShowTrackingLineSettings = EditorGUILayout.Foldout(selected3DRadar.Blips[j].ShowTrackingLineSettings, "Tracking Line is " + trackingLineState);
                    if (GUILayout.Button((_Blip.UseTrackingLine) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(20))) _Blip.UseTrackingLine = !_Blip.UseTrackingLine;

                    GUILayout.EndHorizontal();


                    if (selected3DRadar.Blips[j].ShowTrackingLineSettings)
                    {
                        HelpMessage("Set the Colour and Material here. You can set any material");
                        #region Tracking Line Design


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Material");
                        selected3DRadar.Blips[j].TrackingLineMaterial = (Material)EditorGUILayout.ObjectField("", selected3DRadar.Blips[j].TrackingLineMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Start Colour");
                        selected3DRadar.Blips[j].TrackingLineStartColour = EditorGUILayout.ColorField("", selected3DRadar.Blips[j].TrackingLineStartColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("End Colour");
                        selected3DRadar.Blips[j].TrackingLineEndColour = EditorGUILayout.ColorField("", selected3DRadar.Blips[j].TrackingLineEndColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale");
                        selected3DRadar.Blips[j].TrackingLineDimention = EditorGUILayout.FloatField(selected3DRadar.Blips[j].TrackingLineDimention, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                        #region BaseTracker Design
                        HelpMessage("Base Trackers are sprites which appear that the base of the reacking line and are optional");
                        GUILayout.BeginHorizontal();
                        selected3DRadar.Blips[j].ShowBaseTrackerSettings = EditorGUILayout.Foldout(selected3DRadar.Blips[j].ShowBaseTrackerSettings, "Base Tracker settings");
                        if (GUILayout.Button((_Blip.UseBaseTracker) ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label", GUILayout.Width(20))) _Blip.UseBaseTracker = !_Blip.UseBaseTracker;
                        GUILayout.EndHorizontal();

                        if (selected3DRadar.Blips[j].ShowBaseTrackerSettings)
                        {

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Base Tracker");
                            _Blip.BaseTrackerSprite = (Sprite)EditorGUILayout.ObjectField(_Blip.BaseTrackerSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            selected3DRadar.Blips[j].BaseTrackerMaterial = (Material)EditorGUILayout.ObjectField("", selected3DRadar.Blips[j].BaseTrackerMaterial, typeof(Material), true, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Colour");
                            selected3DRadar.Blips[j].BaseTrackerColour = EditorGUILayout.ColorField("", selected3DRadar.Blips[j].BaseTrackerColour, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Size");
                            selected3DRadar.Blips[j].BaseTrackerSize = EditorGUILayout.FloatField("", selected3DRadar.Blips[j].BaseTrackerSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();



                            if (_Blip.BaseTrackerSprite)
                                GUILayout.Box(_Blip.BaseTrackerSprite.texture, GUILayout.Height(50), GUILayout.Width(50));




                        }
                        #endregion



                        #endregion
                    }

                    Separator();
                    EditorGUILayout.Separator();
                    #endregion

                    #region Additional Options

                    HelpMessage("Displaying additional options for your blip");
                    selected3DRadar.Blips[j].ShowAdditionalOptions = EditorGUILayout.Foldout(selected3DRadar.Blips[j].ShowAdditionalOptions, "Additional Options");
                    if (selected3DRadar.Blips[j].ShowAdditionalOptions)
                    {
                        HelpMessage("When eabled all " + _Blip.Tag + "blips will not disppear when at they pass the bounderies of the radar, but will remain at the edge and will be scaled based on its distance from the center object");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Always Show " + _Blip.Tag + " in radar");
                        selected3DRadar.Blips[j].AlwaysShowBlipsInRadarSpace = GUILayout.Toggle(selected3DRadar.Blips[j].AlwaysShowBlipsInRadarSpace, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (!selected3DRadar.Blips[j].UseHeightTracking && selected3DRadar.Blips[j].KeepBlipsAboveRadarPlane)
                            EditorGUILayout.HelpBox("Use Heigh Tracking is Off, so all blips will already be above /On the Radar plane", MessageType.Warning);

                        HelpMessage("True by default, ensures that blips do not go under the radar");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Always show " + _Blip.Tag + " above the radar plane");
                        selected3DRadar.Blips[j].KeepBlipsAboveRadarPlane = GUILayout.Toggle(selected3DRadar.Blips[j].KeepBlipsAboveRadarPlane, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        HelpMessage("True by default, ensures that blips track objects Y position");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Use height Tracking");
                        selected3DRadar.Blips[j].UseHeightTracking = GUILayout.Toggle(selected3DRadar.Blips[j].UseHeightTracking, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();
                    }




                    Separator();

                    EditorGUILayout.Space();
                    #endregion

                    #region Optimization 



                    HelpMessage("Options for optimization the radar proceses");
                    GUILayout.BeginHorizontal();
                    selected3DRadar.Blips[j].ShowOptimizationSettings = EditorGUILayout.Foldout(selected3DRadar.Blips[j].ShowOptimizationSettings, "Optimization Options");
                    GUILayout.EndHorizontal();
                    if (selected3DRadar.Blips[j].ShowOptimizationSettings)
                    {
                        if (selected3DRadar.Blips[j].optimization.objectFindingMethod == ObjectFindingMethod.Pooling)
                        {
                            HelpMessage("If you are spawning any new objects into the scene then call radar3D.DoInstanceObjectCheck() at instance or at the end of instancing");
                        }


                        if (selected3DRadar.Blips[j].optimization.objectFindingMethod != ObjectFindingMethod.Recursive)
                        {
                            HelpMessage("This requires that you call ' _3DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");

                            HelpMessage(" If you know exactly ow many scene objects this blip should represet then you can set the pool size manually");
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Set Pool Size");
                            selected3DRadar.Blips[j].optimization.SetPoolSizeManually = EditorGUILayout.Toggle(selected3DRadar.Blips[j].optimization.SetPoolSizeManually, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                            if (selected3DRadar.Blips[j].optimization.SetPoolSizeManually)
                            {
                                HelpMessage("The mumber of scene objects that this blip will represent");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Pool Size");
                                selected3DRadar.Blips[j].optimization.poolSize = EditorGUILayout.IntField(selected3DRadar.Blips[j].optimization.poolSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();

                                HelpMessage("If your pool size is too large then the count will be calculated DOWN");
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Recalculate Pool Size On Start");
                                selected3DRadar.Blips[j].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects = EditorGUILayout.Toggle(selected3DRadar.Blips[j].optimization.RecalculatePoolSizeBasedOnFirstFoundObjects, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                                GUILayout.EndHorizontal();
                            }

                        }




                        HelpMessage("This method allows you to use object pooling to store your blips");
                        GUILayout.BeginHorizontal();
                        var info = selected3DRadar.Blips[j].optimization.objectFindingMethod == ObjectFindingMethod.Pooling ? "Use Pooling (Fast)" : (selected3DRadar.Blips[j].optimization.RequireInstanceObjectCheck) ? "Use Recursive Object Finding (Fast)" : "Use Recursive Object Finding (Slower)";
                        GUILayout.Label(info);
                        selected3DRadar.Blips[j].optimization.objectFindingMethod = (ObjectFindingMethod)EditorGUILayout.EnumPopup(selected3DRadar.Blips[j].optimization.objectFindingMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected3DRadar.Blips[j].optimization.objectFindingMethod == ObjectFindingMethod.Recursive)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Require Instance Object Check");
                            if (selected3DRadar.Blips[j].optimization.RequireInstanceObjectCheck)
                                HelpMessage("This requires that you call ' _3DRadar.doInstanceObjectCheck() whenever you want to make the radar search for objects to create blips from. This can also be used to icrease your internal pool size if you need to track more objects'");
                            selected3DRadar.Blips[j].optimization.RequireInstanceObjectCheck = EditorGUILayout.Toggle(selected3DRadar.Blips[j].optimization.RequireInstanceObjectCheck, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }

                        HelpMessage("Allows blips to be removed whenever the object it represent  , changes its tag");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Remove Blip On Tag Change");
                        selected3DRadar.Blips[j].optimization.RemoveBlipsOnTagChange = EditorGUILayout.Toggle(selected3DRadar.Blips[j].optimization.RemoveBlipsOnTagChange, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        HelpMessage("Allows for the blip to be turned off when the object its represents is disabled");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Remove Blip On Disable");
                        selected3DRadar.Blips[j].optimization.RemoveBlipsOnDisable = EditorGUILayout.Toggle(selected3DRadar.Blips[j].optimization.RemoveBlipsOnDisable, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();


                    }



                    Separator();

                    EditorGUILayout.Space();

                    #endregion

                    #region Scale and Rotation Settings


                    _Blip.ShowGeneralSettings = EditorGUILayout.Foldout(_Blip.ShowGeneralSettings, "Rotation and Scale");
                    if (_Blip.ShowGeneralSettings)
                    {
                        HelpMessage("Set the scale of the blip. If 'Scale by distance' is being used then the blps will scale in the radar based on their distance from the center object . the visibility of the scaling varies based on the size of the radar ");

                        #region Blip Scale Settings
                        if (!selected3DRadar.Blips[j].BlipCanScleBasedOnDistance)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            selected3DRadar.Blips[j].BlipSize = EditorGUILayout.FloatField(selected3DRadar.Blips[j].BlipSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Min Scale");
                            selected3DRadar.Blips[j].BlipMinSize = EditorGUILayout.FloatField("", selected3DRadar.Blips[j].BlipMinSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max Scale");
                            selected3DRadar.Blips[j].BlipMaxSize = EditorGUILayout.FloatField("", selected3DRadar.Blips[j].BlipMaxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale by distance");
                        selected3DRadar.Blips[j].BlipCanScleBasedOnDistance = GUILayout.Toggle(selected3DRadar.Blips[j].BlipCanScleBasedOnDistance, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        #endregion

                        #region Rotation Settings

                        HelpMessage("Use custom rotation allows yo uto set a static rotation for all of these blip types");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Use Custom Rotation");
                        selected3DRadar.Blips[j].UseCustomRotation = GUILayout.Toggle(selected3DRadar.Blips[j].UseCustomRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected3DRadar.Blips[j].UseCustomRotation && !selected3DRadar.Blips[j].IsTrackRotation)
                        {
                            HelpMessage("Set a X, Y and Z rotation for this blip type");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Rotation");
                            GUILayout.Label("X");
                            selected3DRadar.Blips[j].CustomXRotation = EditorGUILayout.FloatField(selected3DRadar.Blips[j].CustomXRotation);
                            GUILayout.Label("Y");
                            selected3DRadar.Blips[j].CustomYRotation = EditorGUILayout.FloatField(selected3DRadar.Blips[j].CustomYRotation);
                            GUILayout.Label("Z");
                            selected3DRadar.Blips[j].CustomZRotation = EditorGUILayout.FloatField(selected3DRadar.Blips[j].CustomZRotation);
                            GUILayout.EndHorizontal();
                        }

                        HelpMessage("Track Rotation allows for the blip to rotate and match the rotation of the tracked object");

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Track Rotation");
                        selected3DRadar.Blips[j].IsTrackRotation = GUILayout.Toggle(selected3DRadar.Blips[j].IsTrackRotation, "", GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (selected3DRadar.Blips[j].IsTrackRotation && !selected3DRadar.Blips[j].UseCustomRotation)
                        {
                            HelpMessage("Freeze rotation through the x,y, or z axis");

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Freeze");
                            selected3DRadar.Blips[j].lockX = GUILayout.Toggle(selected3DRadar.Blips[j].lockX, "X");
                            selected3DRadar.Blips[j].lockY = GUILayout.Toggle(selected3DRadar.Blips[j].lockY, "Y");
                            selected3DRadar.Blips[j].lockZ = GUILayout.Toggle(selected3DRadar.Blips[j].lockZ, "Z");
                            GUILayout.EndHorizontal();
                        }
                        #endregion

                        if (selected3DRadar.Blips[j].UseCustomRotation && selected3DRadar.Blips[j].IsTrackRotation)
                        {
                            EditorGUILayout.HelpBox("Do not use 'Track Rotation' and 'Custom Rotation at the same time'", MessageType.Warning);
                        }
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
                    selected3DRadar.Blips.Insert(j + 1, new RadarBlips3D());
                }

                var deleteConditionButtonArea = buttonArea.ToCenterRight(8, 8, -10);
                //  if(i!=0)
                if (ClickEvent.Click(4, deleteConditionButtonArea, ImageLibrary.deleteConditionIcon, "Delete this blip"))
                {
                    selected3DRadar.Blips.RemoveAt(j);
                    return;
                }


                //  GUILayout.EndVertical();
                //   GUILayout.EndHorizontal();
               
                GUILayout.Space(40);
              
            }

            #endregion

            #endregion
            Separator();
        }



        #endregion
        Separator3();

        #region Visualize and HelpMessage
        HelpMessage("Turn on 'Visualize'. This show you your radar settings.");
        selected3DRadar.RadarDesign.Visualize = GUILayout.Toggle(selected3DRadar.RadarDesign.Visualize, "Visualize");
        ShowHelpMessages = GUILayout.Toggle(ShowHelpMessages, "Show Help Messages");
        HelpMessage("These help messages will help you to set up your radar");
        #endregion


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Add UI Target Tracker", " The Ui Target tracker allows you to setup on screen and off screen UI that will hover over and point at tracked gameobjects")))
        {
            if (!selected3DRadar.gameObject.GetComponent<UITargetTracker>())
                selected3DRadar.gameObject.AddComponent<UITargetTracker>();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

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

        radar.GetComponent<_3DRadar>().RadarDesign.renderingCamera = RenderCamera.GetComponent<Camera>();


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


            RenderCamera.GetComponent<Camera>().targetTexture = selected3DRadar.minimapModule.renderTexture;
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
    private _3DRadar selected3DRadar;
    private EditorWindow[] edwin;
    GUISkin skin;
    bool ShowHelpMessages;
    private RadarBlips3D copiedBlipData3D = new RadarBlips3D();
    #endregion

}