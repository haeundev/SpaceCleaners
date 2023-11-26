using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// 
/// </summary>

namespace DaiMangou.ProRadarBuilder
{
    [Serializable]
    public enum OptimizationMethod
    {
        /// <summary>
        ///     ensures that the daa for the target get set once
        /// </summary>
        Single,

        /// <summary>
        ///     ensures that the data for the target gets set every frame
        /// </summary>
        Constant
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class UIData
    {
        public TextMeshProUGUI distanceText;
        public Image image;
        public Image OffScreenIcon;
        public bool lockedOn;
        public TextMeshProUGUI text;
        public GameObject OnScreenObject;
        public GameObject OffScreenObject;
        public GameObject TrackedObject;
        public GameObject UIParent;
    
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class TargetTracker
    {
        public List<UIData> uIData = new List<UIData>();
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class CustomUITargetData
    {
        public bool isActive;
        public OptimizationMethod optimizationMethod = OptimizationMethod.Single;

        public bool showFoldout, showTextSettingsFoldout, showImageSettingsFoldout, showDistanceTextSettingsFoldout, showOffScreenIndicatorSettingsFoldout;

        #region scale settings

        public float scale = 0.5f;
        public bool scaleByDistance;
        public float minSize = 0.1f, maxSize = 0.5f;
        public bool showDistance;
        public bool showOffScreenIndicator;

        #endregion

        #region TextData
        public TMP_FontAsset NameFont;
       /* public TMP_FontAsset NameFont
        {
            get
            {
             //   f = Resources.Load<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset") as TMP_FontAsset;
               // if (f == null)
                    f = TMP_FontAsset.CreateFontAsset(Resources.GetBuiltinResource<Font>("Arial.ttf"));
                
                return f;
            }
            set
            {

                f = value;
            }
        }

        public TMP_FontAsset f;*/

        public int fontSize = 25;
        public Color textColor = new Color32(216, 216, 216, 190);
        public Material textMaterial;

        #endregion

        #region distanceText
        public TMP_FontAsset DistanceTextFont;
      /*  public TMP_FontAsset DistanceTextFont
        {
            get
            {

                if (df == null)
                    df = TMP_FontAsset.CreateFontAsset(Resources.GetBuiltinResource<Font>("Arial.ttf"));
                return df;

            }
            set
            {

                df = value;
            }
        }

        public TMP_FontAsset df;*/

        public int distanceFontSize = 25;
        public Color distanceTextColor = new Color32(216, 216, 216, 190);
        public Material distanceTextMaterial;

        #endregion

        #region ImageData

        public Sprite targetSprite
        {
            get
            {

                if (s == null)
                    s = Resources.Load<Sprite>("DefaultUI/DefaultTargetSprite");
                return s;
            }
            set
            {


                s = value;
            }
        }

        public Sprite s;
        public Color imageColor = new Color32(216, 216, 216, 190);

        public Material imageMaterial
        {
            get
            {
                if (m == null)
                    m = new Material(Shader.Find("Sprites/Default"));

                return m;
            }
            set
            {
                m = value;
            }
        }

        public Material m;

        #endregion

        #region OffScreenImageData

        public Sprite offScreenImageSprite
        {
            get
            {

                if (offScreensprite == null)
                    offScreensprite = Resources.Load<Sprite>("DefaultUI/DefaultOffScreenIcon");
                return offScreensprite;
            }
            set
            {


                offScreensprite = value;
            }
        }

        public Sprite offScreensprite;
        public Color offScreenImageColor = new Color32(216, 216, 216, 190);

        public Material OffScreenImageMaterial
        {
            get
            {
                if (offScreenMaterial == null)
                    offScreenMaterial = new Material(Shader.Find("Sprites/Default"));

                return offScreenMaterial;
            }
            set
            {
                offScreenMaterial = value;
            }
        }

        public Material offScreenMaterial;

        public float OffScreenImagePadding;

        public float OffScreenIconScale = 0.5f;



        #endregion
        public LayerMask layer;

        public TextAnchor textAnchor = TextAnchor.MiddleRight;
    }


    /// <summary>
    /// </summary>
    public class UITargetTracker : MonoBehaviour
    {
        /// <summary>
        /// </summary>
        private void Reset()
        {
            if (!TargetTrackerParentObject)
            {
                if (!GameObject.Find("Target Trackers Canvas"))
                {
                    TargetTrackerParentObject = new GameObject("Target Trackers Canvas");

                    //    TargetTrackers.transform.SetParent(transform);
                    TargetTrackerParentObject.transform.gameObject.AddComponent<Canvas>();
                    canvas = TargetTrackerParentObject.GetComponent<Canvas>();

                    TargetTrackerParentObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

                    TargetTrackerParentObject.transform.gameObject.AddComponent<CanvasScaler>();
                    var canvasScaler = TargetTrackerParentObject.GetComponent<CanvasScaler>();
                    canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

                    TargetTrackerParentObject.transform.gameObject.AddComponent<GraphicRaycaster>();
                    TargetTrackerParentObject.transform.localPosition = Vector3.zero;
                }
                else
                {
                    TargetTrackerParentObject = GameObject.Find("Target Trackers Canvas");
                    canvas = TargetTrackerParentObject.GetComponent<Canvas>();
                }
            }


            if (GetComponent<_2DRadar>())
            {
                _2dRadar = GetComponent<_2DRadar>();

                if (_2dRadar.RadarDesign.camera != null)
                {
                    radarcamera = _2dRadar.RadarDesign.camera;
                }
                else
                {
                    var cam = GameObject.FindGameObjectWithTag(_2dRadar.RadarDesign.CameraTag).GetComponent<Camera>();
                    if (cam != null)
                        radarcamera = cam;
                    else
                        radarcamera = Camera.main;
                }
            }

            if (GetComponent<_3DRadar>())
            {
                _3dRadar = GetComponent<_3DRadar>();

                if (_3dRadar.RadarDesign.camera != null)
                {
                    radarcamera = _3dRadar.RadarDesign.camera;
                }
                else
                {
                    var cam = GameObject.FindGameObjectWithTag(_3dRadar.RadarDesign.CameraTag).GetComponent<Camera>();
                    if (cam != null)
                        radarcamera = cam;
                    else
                        radarcamera = Camera.main;
                }
            }


            gameObject.AddComponent<LockonManager>();
        }

        private void OnDisable()
        {
            if (TargetTrackerParentObject)
                TargetTrackerParentObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (TargetTrackerParentObject)
                TargetTrackerParentObject.SetActive(true);
        }

        public void Awake()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        public void Update()
        {
            if (_2dRadar)
                TrackWith2DRadar();

            if (_3dRadar)
                TrackWith3DRadar();
        }

        #region 2D

        public void TrackWith2DRadar()
        {
            #region ensure that the targettrackers dataset count is the same as the number of blip types

            if (_2dRadar.Blips.Count != customUITargetDataset.Count) customUITargetDataset.Resize(_2dRadar.Blips.Count);

            #endregion


            var blipsCount = _2dRadar.Blips.Count;


            #region ensure that the list of target trackers is the same count as the blip types

            if (trackers.Count != blipsCount) trackers.Resize(blipsCount);

            #endregion

            for (var b = 0; b < blipsCount; b++)
            {
                if (trackers[b] == null)
                    trackers[b] = new TargetTracker();

                var trackedObjectCount = _2dRadar.Blips[b].gos.Count;

                if (trackedObjectCount > trackers[b].uIData.Count)
                    trackers[b].uIData.Resize(trackedObjectCount);


                for (var o = 0; o < trackedObjectCount; o++)
                {
                    var theTrackedGameObject = _2dRadar.Blips[b].gos[o];


                    #region generate the Target UI parent and the text and sprite child objects 

                    if (trackers[b].uIData[o] == null)
                    {
                        trackers[b].uIData[o] = new UIData();


                        var targetUI = new GameObject(theTrackedGameObject.name + " Target UI" + "(" +
                                                      theTrackedGameObject.tag + ")");
                        var onScreenUI = new GameObject("On screen UI");
                        var targetIcon = new GameObject("target Icon");
                        var textPanel = new GameObject("text panel");
                        var nameText = new GameObject("name Text");
                        var distanceText = new GameObject("distance Text");
                        var offScreenUI = new GameObject("Off Screen UI");




                        targetUI.transform.SetParent(TargetTrackerParentObject.transform);
                        onScreenUI.transform.SetParent(targetUI.transform);
                        targetIcon.transform.SetParent(onScreenUI.transform);
                        textPanel.transform.SetParent(onScreenUI.transform);
                        nameText.transform.SetParent(textPanel.transform);
                        distanceText.transform.SetParent(textPanel.transform);
                        offScreenUI.transform.SetParent(targetUI.transform);

                        onScreenUI.transform.localPosition = Vector3.zero;
                        targetUI.transform.localPosition = Vector3.zero;
                        nameText.transform.localPosition = Vector3.zero;
                        distanceText.transform.localPosition = Vector3.zero;
                        targetIcon.transform.localPosition = Vector3.zero;
                        offScreenUI.transform.localPosition = Vector3.zero;
                        // put everything on the UI layer which has a value of 5
                        targetUI.layer = 5;
                        onScreenUI.layer = 5;
                        targetIcon.layer = 5;
                        offScreenUI.layer = 5;
                        textPanel.layer = 5;
                        nameText.layer = 5;
                        distanceText.layer = 5;

                        targetUI.AddComponent<RectTransform>();
                        nameText.AddComponent<TextMeshProUGUI>();
                        distanceText.AddComponent<TextMeshProUGUI>();
                        targetIcon.AddComponent<Image>();
                        offScreenUI.AddComponent<Image>();
                        onScreenUI.AddComponent<RectTransform>();

                        textPanel.AddComponent<VerticalLayoutGroup>();
                        var verticalLayoutGroup = textPanel.GetComponent<VerticalLayoutGroup>();

                        verticalLayoutGroup.childControlHeight = false;
                        verticalLayoutGroup.childControlWidth = false;
                        verticalLayoutGroup.spacing = -90;
                        verticalLayoutGroup.childAlignment = customUITargetDataset[b].textAnchor;

                        trackers[b].uIData[o].text = nameText.GetComponent<TextMeshProUGUI>();
                        trackers[b].uIData[o].text.overflowMode = TextOverflowModes.Overflow;
                        trackers[b].uIData[o].distanceText = distanceText.GetComponent<TextMeshProUGUI>();
                        trackers[b].uIData[o].distanceText.overflowMode = TextOverflowModes.Overflow;

                        trackers[b].uIData[o].image = targetIcon.GetComponent<Image>();
                        trackers[b].uIData[o].OffScreenIcon = offScreenUI.GetComponent<Image>();

                        var trackerImageData = targetIcon.GetComponent<RectTransform>();
                        trackerImageData.sizeDelta = new Vector2(50, 50);

                        var offScreenImageData = offScreenUI.GetComponent<RectTransform>();
                        offScreenImageData.sizeDelta = new Vector2(50, 50); 


                        var nameTextData = nameText.GetComponent<RectTransform>();
                        nameTextData.sizeDelta = new Vector2(163.3f, 25);
                        trackers[b].uIData[o].text.alignment = TextAlignmentOptions.Center;

                        var distanceTextData = distanceText.GetComponent<RectTransform>();
                        distanceTextData.sizeDelta = new Vector2(163.3f, 25);
                        trackers[b].uIData[o].distanceText.alignment = TextAlignmentOptions.Center;

                        trackers[b].uIData[o].TrackedObject = theTrackedGameObject;

                        var paneldata = onScreenUI.GetComponent<RectTransform>();
                        paneldata.anchorMin = new Vector2(0.5f, 0.5f);
                        paneldata.anchorMax = new Vector2(0.5f, 0.5f);
                        paneldata.pivot = new Vector2(0.5f, 0.5f);

                        paneldata.sizeDelta = new Vector2(50, 50);

                        var textPanelData = textPanel.GetComponent<RectTransform>();
                        textPanelData.sizeDelta = new Vector2(400, 150);
                        textPanelData.pivot = new Vector2(0.5f, 0.5f);

                        trackers[b].uIData[o].UIParent = targetUI;
                        trackers[b].uIData[o].OnScreenObject = onScreenUI;
                        trackers[b].uIData[o].OffScreenObject = offScreenUI;
                    }

                    #endregion

                    // update theTrackedGameObject
                    theTrackedGameObject = trackers[b].uIData[o].TrackedObject;

                    #region update and set the UI text and icon data in realtime

                    switch (customUITargetDataset[b].optimizationMethod)
                    {
                        case OptimizationMethod.Single:
                            if (trackers[b].uIData[o].image.sprite == null)
                            {
                                //  Debug.Log("null");
                                //Debug.Log(customUITargetDataset[b].targetSprite);

                                trackers[b].uIData[o].image.sprite = customUITargetDataset[b].targetSprite;
                                trackers[b].uIData[o].image.color = customUITargetDataset[b].imageColor;
                                trackers[b].uIData[o].image.material = customUITargetDataset[b].imageMaterial;


                                trackers[b].uIData[o].OffScreenIcon.sprite = customUITargetDataset[b].offScreenImageSprite;
                                trackers[b].uIData[o].OffScreenIcon.color = customUITargetDataset[b].offScreenImageColor;
                                trackers[b].uIData[o].OffScreenIcon.material = customUITargetDataset[b].offScreenMaterial;
                               // trackers[b].uIData[o].OffScreeinIconPadding = customUITargetDataset[b].OffScreenImagePadding;

                                trackers[b].uIData[o].text.text = theTrackedGameObject.name;
                              //  trackers[b].uIData[o].text.font = customUITargetDataset[b].NameFont;
                                trackers[b].uIData[o].text.fontSize = customUITargetDataset[b].fontSize;
                                trackers[b].uIData[o].text.color = customUITargetDataset[b].textColor;
                                trackers[b].uIData[o].text.material = customUITargetDataset[b].textMaterial;

                              //  trackers[b].uIData[o].distanceText.font = customUITargetDataset[b].DistanceTextFont;
                                trackers[b].uIData[o].distanceText.fontSize = customUITargetDataset[b].distanceFontSize;
                                trackers[b].uIData[o].distanceText.color = customUITargetDataset[b].distanceTextColor;
                                trackers[b].uIData[o].distanceText.material =
                                    customUITargetDataset[b].distanceTextMaterial;


                            }

                            break;
                        case OptimizationMethod.Constant:

                            trackers[b].uIData[o].image.sprite = customUITargetDataset[b].targetSprite;
                            trackers[b].uIData[o].image.color = customUITargetDataset[b].imageColor;
                            trackers[b].uIData[o].image.material = customUITargetDataset[b].imageMaterial;

                            trackers[b].uIData[o].OffScreenIcon.sprite = customUITargetDataset[b].offScreenImageSprite;
                            trackers[b].uIData[o].OffScreenIcon.color = customUITargetDataset[b].offScreenImageColor;
                            trackers[b].uIData[o].OffScreenIcon.material = customUITargetDataset[b].offScreenMaterial;
                            //trackers[b].uIData[o].OffScreeinIconPadding = customUITargetDataset[b].OffScreenImagePadding;

                            trackers[b].uIData[o].text.text = theTrackedGameObject.name;
                          //  trackers[b].uIData[o].text.font = customUITargetDataset[b].NameFont;
                            trackers[b].uIData[o].text.fontSize = customUITargetDataset[b].fontSize;
                            trackers[b].uIData[o].text.color = customUITargetDataset[b].textColor;
                            trackers[b].uIData[o].text.material = customUITargetDataset[b].textMaterial;

                           // trackers[b].uIData[o].distanceText.font = customUITargetDataset[b].DistanceTextFont;
                            trackers[b].uIData[o].distanceText.fontSize = customUITargetDataset[b].distanceFontSize;
                            trackers[b].uIData[o].distanceText.color = customUITargetDataset[b].distanceTextColor;
                            trackers[b].uIData[o].distanceText.material = customUITargetDataset[b].distanceTextMaterial;
                            break;
                    }

                    #endregion









                }

                #region delete and cleanup ui 
                for (var o = 0; o < trackers[b].uIData.Count; o++)
                {

                    var onScreenUIGameobject = trackers[b].uIData[o].OnScreenObject;
                    var offScreenGameobject = trackers[b].uIData[o].OffScreenObject;
                    var theTrackedGameObject = trackers[b].uIData[o].TrackedObject;
                    var mainUI = trackers[b].uIData[o].UIParent;

                    #region place UI Over object if the blip is active and the target tracker is set to lockon

                    #region If we are using lockon or not
                    if (theTrackedGameObject != null && onScreenUIGameobject != null)
                    {
                        if (useLockon)
                        {
                            if (trackers[b].uIData[o].lockedOn)
                            {
                                DrawUI2D(theTrackedGameObject, mainUI, onScreenUIGameobject, offScreenGameobject, b, o);
                            }
                            else
                            {
                                if (onScreenUIGameobject.activeInHierarchy) onScreenUIGameobject.SetActive(false);
                            }
                        }

                        else
                        {
                            DrawUI2D(theTrackedGameObject, mainUI, onScreenUIGameobject, offScreenGameobject, b, o);
                        }
                    }
                    #endregion

                    #endregion


                    if (trackers[b].uIData[o].TrackedObject == null)
                    {

                        Destroy(trackers[b].uIData[o].UIParent);
                        trackers[b].uIData.RemoveAt(o);
                    }







                }

                #endregion
            }
        }

        #endregion

        #region 3D

        public void TrackWith3DRadar()
        {
            #region ensure that the targettrackers dataset count is the same as the number of blip types

            if (_3dRadar.Blips.Count != customUITargetDataset.Count) customUITargetDataset.Resize(_3dRadar.Blips.Count);

            #endregion


            var blipsCount = _3dRadar.Blips.Count;


            #region ensure that the list of target trackers is the same count as the blip types

            if (trackers.Count != blipsCount) trackers.Resize(blipsCount);

            #endregion

            for (var b = 0; b < blipsCount; b++)
            {
                if (trackers[b] == null)
                    trackers[b] = new TargetTracker();

                var trackedObjectCount = _3dRadar.Blips[b].gos.Count;

                if (trackedObjectCount > trackers[b].uIData.Count)
                    trackers[b].uIData.Resize(trackedObjectCount);


                for (var o = 0; o < trackedObjectCount; o++)
                {
                    var theTrackedGameObject = _3dRadar.Blips[b].gos[o];


                    #region generate the Target UI parent and the text and sprite child objects 

                    if (trackers[b].uIData[o] == null)
                    {
                        trackers[b].uIData[o] = new UIData();


                        var targetUI = new GameObject(theTrackedGameObject.name + " Target UI" + "(" +
                                                      theTrackedGameObject.tag + ")");
                        var onScreenUI = new GameObject("On screen UI");
                        var targetIcon = new GameObject("target Icon");
                        var textPanel = new GameObject("text panel");
                        var nameText = new GameObject("name Text");
                        var distanceText = new GameObject("distance Text");
                        var offScreenUI = new GameObject("Off Screen UI");

                        targetUI.transform.SetParent(TargetTrackerParentObject.transform);
                        onScreenUI.transform.SetParent(targetUI.transform);
                        targetIcon.transform.SetParent(onScreenUI.transform);
                        textPanel.transform.SetParent(onScreenUI.transform);
                        nameText.transform.SetParent(textPanel.transform);
                        distanceText.transform.SetParent(textPanel.transform);
                        offScreenUI.transform.SetParent(targetUI.transform);

                        onScreenUI.transform.localPosition = Vector3.zero;
                        targetUI.transform.localPosition = Vector3.zero;
                        nameText.transform.localPosition = Vector3.zero;
                        distanceText.transform.localPosition = Vector3.zero;
                        targetIcon.transform.localPosition = Vector3.zero;
                        offScreenUI.transform.localPosition = Vector3.zero;

                        // put everything on the UI layer which has a value of 5
                        targetUI.layer = 5;
                        onScreenUI.layer = 5;
                        targetIcon.layer = 5;
                        offScreenUI.layer = 5;
                        textPanel.layer = 5;
                        nameText.layer = 5;
                        distanceText.layer = 5;

                        targetUI.AddComponent<RectTransform>();
                        nameText.AddComponent<TextMeshProUGUI>();
                        distanceText.AddComponent<TextMeshProUGUI>();
                        targetIcon.AddComponent<Image>();
                        offScreenUI.AddComponent<Image>();
                        onScreenUI.AddComponent<RectTransform>();

                        textPanel.AddComponent<VerticalLayoutGroup>();
                        var verticalLayoutGroup = textPanel.GetComponent<VerticalLayoutGroup>();

                        verticalLayoutGroup.childControlHeight = false;
                        verticalLayoutGroup.childControlWidth = false;
                        verticalLayoutGroup.spacing = -90;
                        verticalLayoutGroup.childAlignment = customUITargetDataset[b].textAnchor;

                        trackers[b].uIData[o].text = nameText.GetComponent<TextMeshProUGUI>();
                        trackers[b].uIData[o].text.overflowMode = TextOverflowModes.Overflow;
                        trackers[b].uIData[o].distanceText = distanceText.GetComponent<TextMeshProUGUI>();
                        trackers[b].uIData[o].distanceText.overflowMode = TextOverflowModes.Overflow;

                        trackers[b].uIData[o].image = targetIcon.GetComponent<Image>();
                        trackers[b].uIData[o].OffScreenIcon = offScreenUI.GetComponent<Image>();

                        var trackerImageData = targetIcon.GetComponent<RectTransform>();
                        trackerImageData.sizeDelta = new Vector2(50, 50);

                        var offScreenImageData = offScreenUI.GetComponent<RectTransform>();
                        offScreenImageData.sizeDelta = new Vector2(50, 50);

                        var nameTextData = nameText.GetComponent<RectTransform>();
                        nameTextData.sizeDelta = new Vector2(163.3f, 25);
                        trackers[b].uIData[o].text.alignment = TextAlignmentOptions.Center;

                        var distanceTextData = distanceText.GetComponent<RectTransform>();
                        distanceTextData.sizeDelta = new Vector2(163.3f, 25);
                        trackers[b].uIData[o].distanceText.alignment = TextAlignmentOptions.Center;

                        trackers[b].uIData[o].TrackedObject = theTrackedGameObject;

                        var paneldata = onScreenUI.GetComponent<RectTransform>();
                        paneldata.anchorMin = new Vector2(0.5f, 0.5f);
                        paneldata.anchorMax = new Vector2(0.5f, 0.5f);
                        paneldata.pivot = new Vector2(0.5f, 0.5f);

                        paneldata.sizeDelta = new Vector2(50, 50);

                        var textPanelData = textPanel.GetComponent<RectTransform>();
                        textPanelData.sizeDelta = new Vector2(400, 150);
                        textPanelData.pivot = new Vector2(0.5f, 0.5f);


                        trackers[b].uIData[o].UIParent = targetUI;
                        trackers[b].uIData[o].OnScreenObject = onScreenUI;
                        trackers[b].uIData[o].OffScreenObject = offScreenUI;
                    }

                    #endregion

                    // update theTrackedGameObject
                    theTrackedGameObject = trackers[b].uIData[o].TrackedObject;

                    #region update and set the UI text and icon data in realtime

                    switch (customUITargetDataset[b].optimizationMethod)
                    {
                        case OptimizationMethod.Single:
                            if (trackers[b].uIData[o].image.sprite == null)
                            {
                                //  Debug.Log("null");
                                //Debug.Log(customUITargetDataset[b].targetSprite);

                                trackers[b].uIData[o].image.sprite = customUITargetDataset[b].targetSprite;
                                trackers[b].uIData[o].image.color = customUITargetDataset[b].imageColor;
                                trackers[b].uIData[o].image.material = customUITargetDataset[b].imageMaterial;

                                trackers[b].uIData[o].OffScreenIcon.sprite = customUITargetDataset[b].offScreenImageSprite;
                                trackers[b].uIData[o].OffScreenIcon.color = customUITargetDataset[b].offScreenImageColor;
                                trackers[b].uIData[o].OffScreenIcon.material = customUITargetDataset[b].offScreenMaterial;


                                trackers[b].uIData[o].text.text = theTrackedGameObject.name;
                               // trackers[b].uIData[o].text.font = customUITargetDataset[b].NameFont;
                                trackers[b].uIData[o].text.fontSize = customUITargetDataset[b].fontSize;
                                trackers[b].uIData[o].text.color = customUITargetDataset[b].textColor;
                                trackers[b].uIData[o].text.material = customUITargetDataset[b].textMaterial;

                               // trackers[b].uIData[o].distanceText.font = customUITargetDataset[b].DistanceTextFont;
                                trackers[b].uIData[o].distanceText.fontSize = customUITargetDataset[b].distanceFontSize;
                                trackers[b].uIData[o].distanceText.color = customUITargetDataset[b].distanceTextColor;
                                trackers[b].uIData[o].distanceText.material =
                                    customUITargetDataset[b].distanceTextMaterial;


                            }

                            break;
                        case OptimizationMethod.Constant:

                            trackers[b].uIData[o].image.sprite = customUITargetDataset[b].targetSprite;
                            trackers[b].uIData[o].image.color = customUITargetDataset[b].imageColor;
                            trackers[b].uIData[o].image.material = customUITargetDataset[b].imageMaterial;

                            trackers[b].uIData[o].OffScreenIcon.sprite = customUITargetDataset[b].offScreenImageSprite;
                            trackers[b].uIData[o].OffScreenIcon.color = customUITargetDataset[b].offScreenImageColor;
                            trackers[b].uIData[o].OffScreenIcon.material = customUITargetDataset[b].offScreenMaterial;


                            trackers[b].uIData[o].text.text = theTrackedGameObject.name;
                          //  trackers[b].uIData[o].text.font = customUITargetDataset[b].NameFont;
                            trackers[b].uIData[o].text.fontSize = customUITargetDataset[b].fontSize;
                            trackers[b].uIData[o].text.color = customUITargetDataset[b].textColor;
                            trackers[b].uIData[o].text.material = customUITargetDataset[b].textMaterial;

                            //trackers[b].uIData[o].distanceText.font = customUITargetDataset[b].DistanceTextFont;
                            trackers[b].uIData[o].distanceText.fontSize = customUITargetDataset[b].distanceFontSize;
                            trackers[b].uIData[o].distanceText.color = customUITargetDataset[b].distanceTextColor;
                            trackers[b].uIData[o].distanceText.material = customUITargetDataset[b].distanceTextMaterial;
                            break;
                    }

                    #endregion









                }

                #region delete and cleanup ui 
                for (var o = 0; o < trackers[b].uIData.Count; o++)
                {
                    var onScreenUIGameobject = trackers[b].uIData[o].OnScreenObject;
                    var offScreenGameobject = trackers[b].uIData[o].OffScreenObject;
                    var mainUI = trackers[b].uIData[o].UIParent;
                    var theTrackedGameObject = trackers[b].uIData[o].TrackedObject;

                    #region place UI Over object if the blip is active and the target tracker is set to lockon

                    #region If we are using lockon or not
                    if (theTrackedGameObject != null && onScreenUIGameobject != null)
                    {
                        if (useLockon)
                        {
                            if (trackers[b].uIData[o].lockedOn)
                            {
                                DrawUI3D(theTrackedGameObject, mainUI, onScreenUIGameobject, offScreenGameobject, b, o);
                            }
                            else
                            {
                                if (onScreenUIGameobject.activeInHierarchy) onScreenUIGameobject.SetActive(false);
                            }
                        }

                        else
                        {
                            DrawUI3D(theTrackedGameObject, mainUI, onScreenUIGameobject, offScreenGameobject, b, o);
                        }
                    }
                    #endregion

                    #endregion


                    if (trackers[b].uIData[o].TrackedObject == null)
                    {

                        Destroy(trackers[b].uIData[o].UIParent);
                        trackers[b].uIData.RemoveAt(o);
                    }







                }

                #endregion
            }
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="theTrackedGameObject"></param>
        /// <param name="onScreenUI"></param>
        /// <param name="blipPos"></param>
        /// <param name="trackedObjectPos"></param>
        private void DrawUI2D(GameObject theTrackedGameObject, GameObject mainUI, GameObject onScreenUI, GameObject offScreenUI, int blipPos,
            int trackedObjectPos)
        {
            if (_2dRadar.Blips[blipPos].IsActive && customUITargetDataset[blipPos].isActive)
            {
                //  var screenPoint = radarcamera.WorldToViewportPoint(theTrackedGameObject.transform.position);
                // var onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 &&  screenPoint.y < 1;

                var screenPoint = radarcamera.WorldToScreenPoint(theTrackedGameObject.transform.position);

                var onScreen = (screenPoint.z >= 0f & screenPoint.x <= canvasRect.rect.width * canvasRect.localScale.x
                         & screenPoint.y <= canvasRect.rect.height * canvasRect.localScale.x & screenPoint.x >= 0f & screenPoint.y >= 0f);




                var distance = Vector3.Distance(theTrackedGameObject.transform.position,
                    radarcamera.transform.position);

  var sceneScaledDistance =
                        useSceneScale ? _2dRadar.RadarDesign.SceneScale : radarcamera.farClipPlane;
                // check if line of sight to tracked object is NOT blocked by a collider
                if (!Physics.Linecast(theTrackedGameObject.transform.position, radarcamera.transform.position, ~(1 << customUITargetDataset[blipPos].layer.value)))

                {
                  
                    if (distance > 0 && distance < sceneScaledDistance && onScreen)
                    {
                        #region here we have to do the assignment of distance to the distance text

                        var theDistanceText = trackers[blipPos].uIData[trackedObjectPos].distanceText;
                        if (customUITargetDataset[blipPos].showDistance)
                        {
                            if (!theDistanceText.gameObject.activeInHierarchy)
                                theDistanceText.gameObject.SetActive(true);
                            theDistanceText.text = distance.ToString("0.000");
                        }
                        else
                        {
                            theDistanceText.gameObject.SetActive(false);
                        }

                        #endregion


                        if ((_2dRadar.Blips[blipPos].optimization.RemoveBlipsOnDisable && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                            ||
                            (_2dRadar.Blips[blipPos].optimization.RemoveBlipsOnTagChange && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.CompareTag(_2dRadar.Blips[blipPos].Tag))
                            || !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                        {
                            onScreenUI.SetActive(false);
                        }
                        else
                            onScreenUI.SetActive(true);





                        var UIInfoScreenPos = radarcamera.WorldToScreenPoint(theTrackedGameObject.transform.position);
                        trackers[blipPos].uIData[trackedObjectPos].UIParent.transform.position = new Vector3(UIInfoScreenPos.x, UIInfoScreenPos.y) ;


                        if (customUITargetDataset[blipPos].scaleByDistance)
                        {
                            var distanceNadedScale =
                                Mathf.Clamp(customUITargetDataset[blipPos].maxSize - distance / 100,
                                    customUITargetDataset[blipPos].minSize, customUITargetDataset[blipPos].maxSize);
                            onScreenUI.transform.localScale = new Vector3(distanceNadedScale,
                                distanceNadedScale, distanceNadedScale);
                        }
                        else
                        {
                            onScreenUI.transform.localScale = new Vector3(
                                customUITargetDataset[blipPos].scale, customUITargetDataset[blipPos].scale,
                                customUITargetDataset[blipPos].scale);
                        }
                    }
                    else
                        onScreenUI.SetActive(false);
                }
                else
                    onScreenUI.SetActive(false);

                // rotate and position off screen ui

                if (distance > 0 && distance < sceneScaledDistance && !onScreen && customUITargetDataset[blipPos].showOffScreenIndicator)
                {
                    // if (!offScreenUI.activeInHierarchy)
                    //  offScreenUI.SetActive(true);
                    if ((_2dRadar.Blips[blipPos].optimization.RemoveBlipsOnDisable && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                         ||
                         (_2dRadar.Blips[blipPos].optimization.RemoveBlipsOnTagChange && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.CompareTag(_2dRadar.Blips[blipPos].Tag))
                         || !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                    {
                        offScreenUI.SetActive(false);
                    }
                    else
                        offScreenUI.SetActive(true);

                    offScreenUI.transform.localScale = new Vector3(customUITargetDataset[blipPos].OffScreenIconScale, customUITargetDataset[blipPos].OffScreenIconScale, customUITargetDataset[blipPos].OffScreenIconScale);

                    // calculation sets the value depending on if the target is in front or behind.
                    var dynamicScreenPoint = screenPoint.z >= 0 ? screenPoint : screenPoint *= -1;
                    dynamicScreenPoint.z = 0;

                    #region 1

                    Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f , 0f) * canvasRect.localScale.x;
                    dynamicScreenPoint -= canvasCenter;

                    float divX = (canvasRect.rect.width / 2f  ) / Mathf.Abs(dynamicScreenPoint.x);
                    float divY = (canvasRect.rect.height / 2f ) / Mathf.Abs(dynamicScreenPoint.y);

                    if (divX < divY)
                    {
                        float angle = Vector3.SignedAngle(Vector3.right, dynamicScreenPoint, Vector3.forward);
                        dynamicScreenPoint.x = Mathf.Sign(dynamicScreenPoint.x) * (canvasRect.rect.width /2f - customUITargetDataset[blipPos].OffScreenImagePadding) * canvasRect.localScale.x;
                        dynamicScreenPoint.y = Mathf.Tan(Mathf.Deg2Rad * angle) * dynamicScreenPoint.x;
                    }

                    else
                    {
                        float angle = Vector3.SignedAngle(Vector3.up, dynamicScreenPoint, Vector3.forward);

                        dynamicScreenPoint.y = Mathf.Sign(dynamicScreenPoint.y) * (canvasRect.rect.height / 2f - customUITargetDataset[blipPos].OffScreenImagePadding) * canvasRect.localScale.y;
                        dynamicScreenPoint.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * dynamicScreenPoint.y;
                    }

                    dynamicScreenPoint += canvasCenter;
                    #endregion



                    float rotangle = Vector3.SignedAngle(Vector3.up, dynamicScreenPoint - canvasCenter, Vector3.forward);

            
                     trackers[blipPos].uIData[trackedObjectPos].OffScreenIcon.rectTransform.rotation = Quaternion.Euler( new Vector3(0f, 0f, rotangle));


                    trackers[blipPos].uIData[trackedObjectPos].OffScreenIcon.rectTransform.position = dynamicScreenPoint;

                }
                else
                {
                    if (offScreenUI.activeInHierarchy)
                        offScreenUI.SetActive(false);
                }


            }
            else
            {
                onScreenUI.SetActive(false);

                if (offScreenUI.activeInHierarchy)
                    offScreenUI.SetActive(false);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="theTrackedGameObject"></param>
        /// <param name="onScreenUI"></param>
        /// <param name="blipPos"></param>
        /// <param name="trackedObjectPos"></param>
        private void DrawUI3D(GameObject theTrackedGameObject, GameObject mainUI, GameObject onScreenUI, GameObject offScreenUI, int blipPos,
            int trackedObjectPos)
        {
            if (_3dRadar.Blips[blipPos].IsActive && customUITargetDataset[blipPos].isActive)
            {
                //  var screenPoint = radarcamera.WorldToViewportPoint(theTrackedGameObject.transform.position);
                // var onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 &&  screenPoint.y < 1;

                var screenPoint = radarcamera.WorldToScreenPoint(theTrackedGameObject.transform.position);

                var onScreen = (screenPoint.z >= 0f & screenPoint.x <= canvasRect.rect.width * canvasRect.localScale.x
                         & screenPoint.y <= canvasRect.rect.height * canvasRect.localScale.x & screenPoint.x >= 0f & screenPoint.y >= 0f);




                var distance = Vector3.Distance(theTrackedGameObject.transform.position,
                    radarcamera.transform.position);

                var sceneScaledDistance =
                                      useSceneScale ? _3dRadar.RadarDesign.SceneScale : radarcamera.farClipPlane;

                // check if line of sight to tracked object is NOT blocked by a collider
                if (!Physics.Linecast(theTrackedGameObject.transform.position, radarcamera.transform.position, ~(1 << customUITargetDataset[blipPos].layer.value)))

                {

                    if (distance > 0 && distance < sceneScaledDistance && onScreen)
                    {
                        #region here we have to do the assignment of distance to the distance text

                        var theDistanceText = trackers[blipPos].uIData[trackedObjectPos].distanceText;
                        if (customUITargetDataset[blipPos].showDistance)
                        {
                            if (!theDistanceText.gameObject.activeInHierarchy)
                                theDistanceText.gameObject.SetActive(true);
                            theDistanceText.text = distance.ToString("0.000");
                        }
                        else
                        {
                            theDistanceText.gameObject.SetActive(false);
                        }

                        #endregion


                        if ((_3dRadar.Blips[blipPos].optimization.RemoveBlipsOnDisable && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                            ||
                            (_3dRadar.Blips[blipPos].optimization.RemoveBlipsOnTagChange && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.CompareTag(_3dRadar.Blips[blipPos].Tag))
                            || !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                        {
                            onScreenUI.SetActive(false);
                        }
                        else
                            onScreenUI.SetActive(true);





                        var UIInfoScreenPos = radarcamera.WorldToScreenPoint(theTrackedGameObject.transform.position);
                        trackers[blipPos].uIData[trackedObjectPos].UIParent.transform.position = new Vector3(UIInfoScreenPos.x, UIInfoScreenPos.y) ;


                        if (customUITargetDataset[blipPos].scaleByDistance)
                        {
                            var distanceNadedScale =
                                Mathf.Clamp(customUITargetDataset[blipPos].maxSize - distance / 100,
                                    customUITargetDataset[blipPos].minSize, customUITargetDataset[blipPos].maxSize);
                            onScreenUI.transform.localScale = new Vector3(distanceNadedScale,
                                distanceNadedScale, distanceNadedScale);
                        }
                        else
                        {
                            onScreenUI.transform.localScale = new Vector3(
                                customUITargetDataset[blipPos].scale, customUITargetDataset[blipPos].scale,
                                customUITargetDataset[blipPos].scale);
                        }
                    }
                    else
                        onScreenUI.SetActive(false);
                }
                else
                    onScreenUI.SetActive(false);

                // rotate and position off screen ui

                if (distance > 0 && distance < sceneScaledDistance && !onScreen && customUITargetDataset[blipPos].showOffScreenIndicator)
                {
                    //if (!offScreenUI.activeInHierarchy)
                    //   offScreenUI.SetActive(true);
                    if ((_3dRadar.Blips[blipPos].optimization.RemoveBlipsOnDisable && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
   ||
   (_3dRadar.Blips[blipPos].optimization.RemoveBlipsOnTagChange && !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.CompareTag(_3dRadar.Blips[blipPos].Tag))
   || !trackers[blipPos].uIData[trackedObjectPos].TrackedObject.activeInHierarchy)
                    {
                        offScreenUI.SetActive(false);
                    }
                    else
                        offScreenUI.SetActive(true);

                    offScreenUI.transform.localScale = new Vector3(customUITargetDataset[blipPos].OffScreenIconScale, customUITargetDataset[blipPos].OffScreenIconScale, customUITargetDataset[blipPos].OffScreenIconScale);

                    // calculation sets the value depending on if the target is in front or behind.
                    var dynamicScreenPoint = screenPoint.z >= 0 ? screenPoint : screenPoint *= -1;
                    dynamicScreenPoint.z = 0;

                    #region 1

                    Vector3 canvasCenter = new Vector3(canvasRect.rect.width / 2f, canvasRect.rect.height / 2f, 0f) * canvasRect.localScale.x;
                    dynamicScreenPoint -= canvasCenter;

                    float divX = (canvasRect.rect.width / 2f) / Mathf.Abs(dynamicScreenPoint.x);
                    float divY = (canvasRect.rect.height / 2f) / Mathf.Abs(dynamicScreenPoint.y);

                    if (divX < divY)
                    {
                        float angle = Vector3.SignedAngle(Vector3.right, dynamicScreenPoint, Vector3.forward);
                        dynamicScreenPoint.x = Mathf.Sign(dynamicScreenPoint.x) * (canvasRect.rect.width / 2f - customUITargetDataset[blipPos].OffScreenImagePadding) * canvasRect.localScale.x;
                        dynamicScreenPoint.y = Mathf.Tan(Mathf.Deg2Rad * angle) * dynamicScreenPoint.x;
                    }

                    else
                    {
                        float angle = Vector3.SignedAngle(Vector3.up, dynamicScreenPoint, Vector3.forward);

                        dynamicScreenPoint.y = Mathf.Sign(dynamicScreenPoint.y) * (canvasRect.rect.height / 2f - customUITargetDataset[blipPos].OffScreenImagePadding) * canvasRect.localScale.y;
                        dynamicScreenPoint.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * dynamicScreenPoint.y;
                    }

                    dynamicScreenPoint += canvasCenter;
                    #endregion



                    float rotangle = Vector3.SignedAngle(Vector3.up, dynamicScreenPoint - canvasCenter, Vector3.forward);


                    trackers[blipPos].uIData[trackedObjectPos].OffScreenIcon.rectTransform.rotation = Quaternion.Euler(new Vector3(0f, 0f, rotangle));


                    trackers[blipPos].uIData[trackedObjectPos].OffScreenIcon.rectTransform.position = dynamicScreenPoint;

                }
                else
                {
                    if (offScreenUI.activeInHierarchy)
                        offScreenUI.SetActive(false);
                }


            }
            else
            {
                onScreenUI.SetActive(false);

                if (offScreenUI.activeInHierarchy)
                    offScreenUI.SetActive(false);
            }
        }


        /// <summary>
        /// </summary>
        private void OnDestroy()
        {
            DestroyImmediate(TargetTrackerParentObject);
        }



        #region variables

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public Canvas canvas;

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public _3DRadar _3dRadar;

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public _2DRadar _2dRadar;

        /// <summary>
        /// </summary>
        [HideInInspector]
        [SerializeField]
        public List<CustomUITargetData> customUITargetDataset = new List<CustomUITargetData>();

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public bool useSceneScale;

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public Camera radarcamera;

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public GameObject TargetTrackerParentObject;

        /// <summary>
        /// </summary>
        [HideInInspector]
        [SerializeField] public List<TargetTracker> trackers = new List<TargetTracker>();

        /// <summary>
        /// </summary>
        [HideInInspector] [SerializeField] public bool useLockon;

        private RectTransform canvasRect;

        #endregion
    }
}