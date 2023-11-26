using System;
using DaiMangou.ProRadarBuilder;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DaiMangou.ProRadarBuilder.Editor;
using DaiMangou.ProRadarBuilderEditor;

namespace DaiMangou.ProRadarBuilderEditor
{
    [CustomEditor(typeof(UITargetTracker))]
    [CanEditMultipleObjects]
    public class UITargetTrackerEditor : Editor
    {

        public void OnEnable()
        {
            selectedTargetTracker = (UITargetTracker)target;
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



            /*   if (GUI.Button( ScreenRect.ToCenter(100, 20), "Add to selected"))
               {
                   _Selection().AddComponent<UITargetTracker>();
                   selectedTargetTracker = _Selection().GetComponent<UITargetTracker>();

                   if (selectedTargetTracker._2dRadar)
                       if (selectedTargetTracker._2dRadar.Blips.Count != selectedTargetTracker.customUITargetDataset.Count)
                           selectedTargetTracker.customUITargetDataset.Resize(selectedTargetTracker._2dRadar.Blips.Count);

                   if (selectedTargetTracker._3dRadar)
                       if (selectedTargetTracker._3dRadar.Blips.Count != selectedTargetTracker.customUITargetDataset.Count)
                           selectedTargetTracker.customUITargetDataset.Resize(selectedTargetTracker._3dRadar.Blips.Count);

                   return;
               }*/




            #region Header Bar



            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Use Scene Scale: " + (selectedTargetTracker.useSceneScale ? "On" : "Off"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                selectedTargetTracker.useSceneScale = !selectedTargetTracker.useSceneScale;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Use Lockon: " + (selectedTargetTracker.useLockon ? "On" : "Off"), EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                selectedTargetTracker.useLockon = !selectedTargetTracker.useLockon;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (!selectedTargetTracker.canvas)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix missing canvas", EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                {
                    var foundObj = GameObject.Find("Target Trackers Canvas");
                    if (foundObj != null)
                    {
                        selectedTargetTracker.canvas = foundObj.GetComponent<Canvas>();
                        selectedTargetTracker.TargetTrackerParentObject = foundObj;
                    }
                    else
                    {
                        var newTargetTrackerParent = new GameObject("Target Trackers Canvas");
                        newTargetTrackerParent.AddComponent<Canvas>();

                        newTargetTrackerParent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

                        newTargetTrackerParent.AddComponent<CanvasScaler>();
                        var canvasScaler = newTargetTrackerParent.GetComponent<CanvasScaler>();
                        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

                        newTargetTrackerParent.AddComponent<GraphicRaycaster>();
                        newTargetTrackerParent.transform.localPosition = Vector3.zero;

                        selectedTargetTracker.canvas = newTargetTrackerParent.GetComponent<Canvas>();
                        selectedTargetTracker.TargetTrackerParentObject = newTargetTrackerParent;
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(5);


            if (!selectedTargetTracker.radarcamera)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Fix missing camera", EditorStyles.toolbarButton, GUILayout.MaxWidth(150), GUILayout.MinWidth(50)))
                {


                    if (selectedTargetTracker._3dRadar.RadarDesign.camera != null)
                    {
                        selectedTargetTracker.radarcamera = selectedTargetTracker._3dRadar.RadarDesign.camera;
                    }
                    else
                    {
                        try
                        {
                            var cam = GameObject.FindGameObjectWithTag(selectedTargetTracker._3dRadar.RadarDesign.CameraTag)
                                .GetComponent<Camera>();
                            if (cam != null)
                                selectedTargetTracker.radarcamera = cam;
                            else
                                selectedTargetTracker.radarcamera = Camera.main;
                        }
                        catch
                        {
                            Debug.Log("No camera tagged with " + selectedTargetTracker._3dRadar.RadarDesign.CameraTag + " was found in the scene");
                        }
                    }

                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            #endregion






            #region 2D

            if (selectedTargetTracker._2dRadar)
            {
                if (selectedTargetTracker._2dRadar.Blips.Count != selectedTargetTracker.customUITargetDataset.Count)
                    selectedTargetTracker.customUITargetDataset.Resize(selectedTargetTracker._2dRadar.Blips.Count);

                if (selectedTargetTracker._2dRadar == null)
                {
                    if (selectedTargetTracker.GetComponent<_2DRadar>())
                        selectedTargetTracker._2dRadar = selectedTargetTracker.GetComponent<_2DRadar>();
                    else
                        return;
                }



                for (var i = 0; i < selectedTargetTracker.customUITargetDataset.Count; i++)
                {
                    if (selectedTargetTracker.customUITargetDataset[i] == null)
                        selectedTargetTracker.customUITargetDataset[i] = new CustomUITargetData();

                    var targetedObject = selectedTargetTracker.customUITargetDataset[i];


                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();


                    targetedObject.showFoldout = EditorGUILayout.Foldout(targetedObject.showFoldout,
                        selectedTargetTracker._2dRadar.Blips[i].Tag, true);

                    if (selectedTargetTracker.customUITargetDataset[i].targetSprite)
                        GUILayout.Box(selectedTargetTracker.customUITargetDataset[i].targetSprite.texture, "Label",
                            GUILayout.Height(20));

                    if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].isActive ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                        GUILayout.Width(20)))
                        selectedTargetTracker.customUITargetDataset[i].isActive =
                            !selectedTargetTracker.customUITargetDataset[i].isActive;

                    GUILayout.EndHorizontal();


                    //  GUI.DrawTexture(GUILayoutUtility.GetLastRect().ToLowerLeft(0,1),textu)
                    if (targetedObject.showFoldout)
                    {
                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale By Distance");
                        targetedObject.scaleByDistance = GUILayout.Toggle(targetedObject.scaleByDistance, GUIContent.none, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (targetedObject.scaleByDistance)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Min");
                            targetedObject.minSize =
                                EditorGUILayout.FloatField(targetedObject.minSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max");
                            targetedObject.maxSize =
                                EditorGUILayout.FloatField(targetedObject.maxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            targetedObject.scale =
                                EditorGUILayout.FloatField(targetedObject.scale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }




                        Separator();
                        EditorGUILayout.Space();



                        targetedObject.showTextSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showTextSettingsFoldout, selectedTargetTracker._2dRadar.Blips[i].Tag + " Text");


                        if (targetedObject.showTextSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font");
                            targetedObject.NameFont = (TMP_FontAsset)EditorGUILayout.ObjectField(targetedObject.NameFont, typeof(TMP_FontAsset),
                                false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font Size");
                            targetedObject.fontSize =
                                (int)EditorGUILayout.FloatField(targetedObject.fontSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("text Color");
                            targetedObject.textColor =
                                EditorGUILayout.ColorField(targetedObject.textColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.textMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.textMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }


                        Separator();
                        EditorGUILayout.Space();




                        targetedObject.showImageSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showImageSettingsFoldout,
                            selectedTargetTracker._2dRadar.Blips[i].Tag + " Sprite");


                        if (targetedObject.showImageSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Target Sprite");
                            targetedObject.targetSprite = (Sprite)EditorGUILayout.ObjectField(
                                targetedObject.targetSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Sprite Color");
                            targetedObject.imageColor =
                                EditorGUILayout.ColorField(targetedObject.imageColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.imageMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.imageMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }



                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        targetedObject.showOffScreenIndicatorSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showOffScreenIndicatorSettingsFoldout,
                            "Off Screen Sprite");
                        if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                 GUILayout.Width(20)))
                            selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator =
                                !selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator;
                        GUILayout.EndHorizontal();


                        if (targetedObject.showOffScreenIndicatorSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Off Screen Sprite");
                            targetedObject.offScreenImageSprite = (Sprite)EditorGUILayout.ObjectField(
                                targetedObject.offScreenImageSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Sprite Color");
                            targetedObject.offScreenImageColor =
                                EditorGUILayout.ColorField(targetedObject.offScreenImageColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.OffScreenImageMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.OffScreenImageMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            targetedObject.OffScreenIconScale = EditorGUILayout.DelayedFloatField(
                                targetedObject.OffScreenIconScale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();


                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Padding");
                            targetedObject.OffScreenImagePadding = EditorGUILayout.DelayedFloatField(
                                targetedObject.OffScreenImagePadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                          //  if (targetedObject.offScreenImageSprite)
                          //  {
                            //    GUILayout.Box(targetedObject.offScreenImageSprite.texture, "Label", GUILayout.Height(50), GUILayout.Width(50));
                          //  }
                        }




                        Separator();
                        EditorGUILayout.Space();




                        GUILayout.BeginHorizontal();
                        targetedObject.showDistanceTextSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showDistanceTextSettingsFoldout,
                            selectedTargetTracker._2dRadar.Blips[i].Tag + " Distance Text");
                        if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].showDistance ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                            GUILayout.Width(20)))
                            selectedTargetTracker.customUITargetDataset[i].showDistance =
                                !selectedTargetTracker.customUITargetDataset[i].showDistance;
                        GUILayout.EndHorizontal();

                        if (targetedObject.showDistanceTextSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font");
                            targetedObject.DistanceTextFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
                                targetedObject.DistanceTextFont, typeof(TMP_FontAsset), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font Size");
                            targetedObject.distanceFontSize =
                                (int)EditorGUILayout.FloatField(targetedObject.distanceFontSize,
                                     GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("text Color");
                            targetedObject.distanceTextColor =
                                EditorGUILayout.ColorField(targetedObject.distanceTextColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.distanceTextMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.distanceTextMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();


                        }



                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Ignore This Layer"); targetedObject.layer = EditorGUILayout.LayerField(targetedObject.layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Optimization Method");
                        targetedObject.optimizationMethod =
                            (OptimizationMethod)EditorGUILayout.EnumPopup(targetedObject.optimizationMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();




                    }


                    EditorGUILayout.Space();
                    Separator2();
                }

            }

            #endregion

            #region 3D

            if (selectedTargetTracker._3dRadar)
            {
                if (selectedTargetTracker._3dRadar.Blips.Count != selectedTargetTracker.customUITargetDataset.Count)
                    selectedTargetTracker.customUITargetDataset.Resize(selectedTargetTracker._3dRadar.Blips.Count);

                if (selectedTargetTracker._3dRadar == null)
                {
                    if (selectedTargetTracker.GetComponent<_3DRadar>())
                        selectedTargetTracker._3dRadar = selectedTargetTracker.GetComponent<_3DRadar>();
                    else
                        return;
                }



                for (var i = 0; i < selectedTargetTracker.customUITargetDataset.Count; i++)
                {
                    if (selectedTargetTracker.customUITargetDataset[i] == null)
                        selectedTargetTracker.customUITargetDataset[i] = new CustomUITargetData();

                    var targetedObject = selectedTargetTracker.customUITargetDataset[i];


                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();


                    targetedObject.showFoldout = EditorGUILayout.Foldout(targetedObject.showFoldout,
                        selectedTargetTracker._3dRadar.Blips[i].Tag, true);

                    if (selectedTargetTracker.customUITargetDataset[i].targetSprite)
                        GUILayout.Box(selectedTargetTracker.customUITargetDataset[i].targetSprite.texture, "Label",
                            GUILayout.Height(20));

                    if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].isActive ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                        GUILayout.Width(20)))
                        selectedTargetTracker.customUITargetDataset[i].isActive =
                            !selectedTargetTracker.customUITargetDataset[i].isActive;

                    GUILayout.EndHorizontal();


                    //  GUI.DrawTexture(GUILayoutUtility.GetLastRect().ToLowerLeft(0,1),textu)
                    if (targetedObject.showFoldout)
                    {
                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Scale By Distance");
                        targetedObject.scaleByDistance = GUILayout.Toggle(targetedObject.scaleByDistance, GUIContent.none, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        if (targetedObject.scaleByDistance)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Min");
                            targetedObject.minSize =
                                EditorGUILayout.FloatField(targetedObject.minSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Max");
                            targetedObject.maxSize =
                                EditorGUILayout.FloatField(targetedObject.maxSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            targetedObject.scale =
                                EditorGUILayout.FloatField(targetedObject.scale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }




                        Separator();
                        EditorGUILayout.Space();



                        targetedObject.showTextSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showTextSettingsFoldout, selectedTargetTracker._3dRadar.Blips[i].Tag + " Text");


                        if (targetedObject.showTextSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font");
                            targetedObject.NameFont = (TMP_FontAsset)EditorGUILayout.ObjectField(targetedObject.NameFont, typeof(TMP_FontAsset),
                                false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font Size");
                            targetedObject.fontSize =
                                (int)EditorGUILayout.FloatField(targetedObject.fontSize, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("text Color");
                            targetedObject.textColor =
                                EditorGUILayout.ColorField(targetedObject.textColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.textMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.textMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }


                        Separator();
                        EditorGUILayout.Space();




                        targetedObject.showImageSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showImageSettingsFoldout,
                            selectedTargetTracker._3dRadar.Blips[i].Tag + " Sprite");


                        if (targetedObject.showImageSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Target Sprite");
                            targetedObject.targetSprite = (Sprite)EditorGUILayout.ObjectField(
                                targetedObject.targetSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Sprite Color");
                            targetedObject.imageColor =
                                EditorGUILayout.ColorField(targetedObject.imageColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.imageMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.imageMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }



                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        targetedObject.showOffScreenIndicatorSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showOffScreenIndicatorSettingsFoldout,
                            "Off Screen Sprite");
                        if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                 GUILayout.Width(20)))
                            selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator =
                                !selectedTargetTracker.customUITargetDataset[i].showOffScreenIndicator;
                        GUILayout.EndHorizontal();


                        if (targetedObject.showOffScreenIndicatorSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Off Screen Sprite");
                            targetedObject.offScreenImageSprite = (Sprite)EditorGUILayout.ObjectField(
                                targetedObject.offScreenImageSprite, typeof(Sprite), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Sprite Color");
                            targetedObject.offScreenImageColor =
                                EditorGUILayout.ColorField(targetedObject.offScreenImageColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.OffScreenImageMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.OffScreenImageMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Scale");
                            targetedObject.OffScreenIconScale = EditorGUILayout.DelayedFloatField(
                                targetedObject.OffScreenIconScale, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();


                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Padding");
                            targetedObject.OffScreenImagePadding = EditorGUILayout.DelayedFloatField(
                                targetedObject.OffScreenImagePadding, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();
                        }




                        Separator();
                        EditorGUILayout.Space();




                        GUILayout.BeginHorizontal();
                        targetedObject.showDistanceTextSettingsFoldout = EditorGUILayout.Foldout(
                            targetedObject.showDistanceTextSettingsFoldout,
                            selectedTargetTracker._3dRadar.Blips[i].Tag + " Distance Text");
                        if (GUILayout.Button(selectedTargetTracker.customUITargetDataset[i].showDistance ? ImageLibrary.PowerOnpro : ImageLibrary.PowerOffpro, "Label",
                            GUILayout.Width(20)))
                            selectedTargetTracker.customUITargetDataset[i].showDistance =
                                !selectedTargetTracker.customUITargetDataset[i].showDistance;
                        GUILayout.EndHorizontal();

                        if (targetedObject.showDistanceTextSettingsFoldout)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font");
                            targetedObject.DistanceTextFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
                                targetedObject.DistanceTextFont, typeof(TMP_FontAsset), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Font Size");
                            targetedObject.distanceFontSize =
                                (int)EditorGUILayout.FloatField(targetedObject.distanceFontSize,
                                     GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("text Color");
                            targetedObject.distanceTextColor =
                                EditorGUILayout.ColorField(targetedObject.distanceTextColor, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Material");
                            targetedObject.distanceTextMaterial = (Material)EditorGUILayout.ObjectField(
                                targetedObject.distanceTextMaterial, typeof(Material), false, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                            GUILayout.EndHorizontal();


                        }



                        Separator();
                        EditorGUILayout.Space();



                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Ignore This Layer"); targetedObject.layer = EditorGUILayout.LayerField(targetedObject.layer, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Optimization Method");
                        targetedObject.optimizationMethod =
                            (OptimizationMethod)EditorGUILayout.EnumPopup(targetedObject.optimizationMethod, GUILayout.MaxWidth(150), GUILayout.MinWidth(50));
                        GUILayout.EndHorizontal();




                    }


                    EditorGUILayout.Space();
                    Separator2();
                }

            }

            #endregion

            Repaint();
        }

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


        #region veriables
        private UITargetTracker selectedTargetTracker;
        private Rect ScreenRect = new Rect(0, 0, 0, 0);
        private EditorWindow[] edwin;
        GUISkin skin;
        #endregion
    }
}