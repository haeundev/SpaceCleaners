using System;
using System.Collections.Generic;
using System.Linq;
using DaiMangou.ProRadarBuilder.Editor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace DaiMangou.ProRadarBuilder
{
    [AddComponentMenu("Tools/DaiMangou/2D Radar")]

    public class _2DRadar : MonoBehaviour
    {


        public void OnEnable()
        {
            // subscrive to the event
            doInstanceObjectCheck += InstanceObjectCheck;
        }

        public void OnDisable()
        {
            // unsubscribe from the evnet
            doInstanceObjectCheck -= InstanceObjectCheck;
        }

        public void Start()
        {
            InstanceObjectCheck();
            ReadyCheck();
            SetBlipParentObject();

        }

        /// <summary>
        ///     will trigger a search for objects to track
        /// </summary>
        private void InstanceObjectCheck()
        {
            BeginObjectInstanceCheck = true;
        }


        public void Update()
        {

            // set up our  renderingCamera and main Camera
            CameraSetup();
            // Determine the front facing direction of the radar
            Frontis();
            // manage the rotations of the radar itself
            if (!RadarDesign.UseCustomPositioning)
                SetRotations();
            // set the dimentions of the radar 
            SetRadarDimentions();
            // manages the positioning of the radar
            if (!RadarDesign.UseCustomPositioning && !RadarDesign.UseUI)
                SnapAndPositioning();
            //manage the poition, rotation and scale of the center blip
            CenterObjectBlip();
            // manage the position, rotation and scale for all other blips
            CheckAndSetBlips();
            // manages the Static and Realtime minimap systems
            Minimap();
            // manages specifically target objects rotations
            RotateSpecifics();
        }

        internal void ReadyCheck()
        {
            if (transform.parent && !RadarDesign.UseUI)
            {
                Debug.LogWarning("2D Radar des not need to be the child of any object.This may cause unanted effects");
            }
        }

        private void SetBlipParentObject()
        {
            RadarDesign.BlipsParentObject = transform.Find("Blips");

            if (RadarDesign.BlipsParentObject == null)
            {
                Debug.LogWarning("This version of the Radar Builder requires that an object named 'Blips' is made the child of your radar. This is done automatically when you create a new Radar");
                GameObject tempBLipsContiner = new GameObject("Blips");
                tempBLipsContiner.transform.SetParent(transform);
                tempBLipsContiner.transform.localPosition = Vector3.zero;
                RadarDesign.BlipsParentObject = tempBLipsContiner.transform;
                Debug.Log("Radar Builder has created a Temporary 'Blips' container object, Please create a new gameobject named 'Blips' and make it a child of your Radar gameobject");
            }
        }

        /// <summary>
        ///     here we setup main camera, this script will constantly searn for your main camera or camera you wish to find IF it
        ///     is not yet found
        ///     during th abscence of a camera , no errors will be thrown.
        ///     However you will be warned if you do not set a 'Rendering camera' for your radar.
        /// </summary>
        private void CameraSetup()
        {
            #region Setup the camera

            if (!RadarDesign.camera)
                if (!RadarDesign.ManualCameraSetup)
                    if (RadarDesign.UseMainCamera)
                        RadarDesign.camera = Camera.main;
                    else
                        try
                        {
                            RadarDesign.camera = GameObject.FindWithTag(RadarDesign.CameraTag).GetComponent<Camera>();
                        }
                        catch
                        {
                            return;
                        }
            if (RadarDesign.camera == null)
                return;

            #endregion

            #region Setup RenderCamera

            if (!RadarDesign.UseUI)
            {
                if (RadarDesign.renderingCamera)
                {
                    if (!RadarDesign.UseCustomPositioning)
                        RadarDesign.renderingCamera.transform.rotation = RadarDesign.camera.transform.rotation;
                }
                else
                {
                    Debug.LogWarning(
                        "Please specify a rendering camera, Your rendering camera was created when you created this radar ");
                    Debug.Break();
                }
            }

            #endregion
        }

        /// <summary>
        ///     Here we determine which direction we wish for our radars front to be
        ///     For example, If you ser Frontis to be East then all blips moving forward through the Z axis will appear to be
        ///     moving to the east in your radar .
        ///     This makes the function very useful for various games
        /// </summary>
        private void Frontis()
        {
            #region Front is 

            switch (RadarDesign.frontIs)
            {
                case FrontIs.North:
                    RadarDesign.RadarRotationOffset = 0;
                    break;
                case FrontIs.East:
                    RadarDesign.RadarRotationOffset = 270;

                    break;
                case FrontIs.South:
                    RadarDesign.RadarRotationOffset = 180;

                    break;
                case FrontIs.West:
                    RadarDesign.RadarRotationOffset = 90;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RadarDesign.BlipsParentObject.transform.localEulerAngles = new Vector3(RadarDesign.BlipsParentObject.transform.localEulerAngles.x, RadarDesign.BlipsParentObject.transform.localEulerAngles.y, RadarDesign.RadarRotationOffset);
            #endregion
        }

        /// <summary>
        ///     Here we set the rotation of our radar to be proportional to the rotation of the target camera
        /// </summary>
        private void SetRotations()
        {
            #region Set Rotation of radar
            if (RadarDesign.UseUI)
            {
                transform.eulerAngles = new Vector3(
    transform.eulerAngles.x,
    transform.eulerAngles.y,
    RadarDesign.DontRotateMapAndContent ? 0 :
 RadarDesign.camera.transform.eulerAngles.y);
            }
            else
            {
                transform.eulerAngles = new Vector3(
                    RadarDesign.camera.transform.eulerAngles.x,
                    RadarDesign.camera.transform.eulerAngles.y,
                    RadarDesign.DontRotateMapAndContent ? 0 :
                 RadarDesign.camera.transform.eulerAngles.y);
            }
            #endregion
        }

        /// <summary>
        ///     Scale the radar lie any other object or by usign the  RadarDiameter Value when UseLocalScale is true
        /// </summary>
        private void SetRadarDimentions()
        {
            #region Setting Radar Dimentions

            transform.localScale = RadarDesign.UseLocalScale
                ? transform.localScale
                : new Vector3(RadarDesign.RadarDiameter, RadarDesign.RadarDiameter, RadarDesign.RadarDiameter);

            #endregion
        }

        /// <summary>
        ///     Snap position allows for the radar to be positioned in 9 points of the screen
        /// </summary>
        private void SnapAndPositioning()
        {
            RadarDesign.SnappedRect.size = new Vector2(DMMath.REQS(Screen.height, DMMath.Mv),
                DMMath.REQS(Screen.height, DMMath.Mv));

            #region Snap&Manual positioning

            // for every increase or decrease of 0.01 in radar diameter above or below 1 will result in  the amount of times * 0.0024096385

            var scale = transform.localScale.x;

            var autoScalingForChangeInDiameter = Mathf.Abs((1 - scale) / 0.01f) * DMMath.ScalingConstant;
            // Do not change this value

            var posOffSetByScale = RadarDesign.IgnoreDiameterScale
                ? 0
                : scale < 1
                    ? Screen.height * autoScalingForChangeInDiameter
                    : scale > 1 ? -(Screen.height * autoScalingForChangeInDiameter) : 0;


            switch (RadarDesign.radarPositioning)
            {
                case RadarPositioning.Manual:
                    RadarDesign.SnappedRect.position = new Vector2(RadarDesign.RadarRect.xMax,
                        -RadarDesign.RadarRect.yMax + Screen.height);

                    transform.position =
                        RadarDesign.renderingCamera.ScreenToWorldPoint(
                            new Vector3(RadarDesign.SnappedRect.center.x + RadarDesign.xPadding,
                                RadarDesign.SnappedRect.center.y - RadarDesign.SnappedRect.height -
                                RadarDesign.yPadding, RadarDesign2D.ConstantRadarRenderDistance));

                    break;
                case RadarPositioning.Snap:
                    switch (RadarDesign.snapPosition)
                    {
                        case SnapPosition.TopLeft:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToUpperLeft(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, -posOffSetByScale, posOffSetByScale)
                                    .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                            break;
                        case SnapPosition.TopRight:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToUpperRight(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, posOffSetByScale, posOffSetByScale)
                                    .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                            break;
                        case SnapPosition.BottomLeft:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToLowerLeft(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, -posOffSetByScale, -posOffSetByScale)
                                    .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                            break;
                        case SnapPosition.BottomRight:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToLowerRight(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, posOffSetByScale, -posOffSetByScale)
                                    .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                            break;
                        case SnapPosition.Center:
                            RadarDesign.SnappedRect = ThisScreen.ScreenRect.ToCenter(RadarDesign.SnappedRect.width,
                                RadarDesign.SnappedRect.width);
                            break;
                        case SnapPosition.LeftMiddle:
                            RadarDesign.SnappedRect = ThisScreen.ScreenRect.ToCenterLeft(RadarDesign.SnappedRect.width,
                                RadarDesign.SnappedRect.width, -posOffSetByScale);
                            break;
                        case SnapPosition.RightMiddle:
                            RadarDesign.SnappedRect = ThisScreen.ScreenRect.ToCenterRight(
                                RadarDesign.SnappedRect.width, RadarDesign.SnappedRect.width, posOffSetByScale);
                            break;
                        case SnapPosition.BottomMiddle:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToCenterBottom(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, 0, -posOffSetByScale)
                                    .AddRect(0, -Screen.height + RadarDesign.SnappedRect.size.y);
                            break;
                        case SnapPosition.TopMiddle:
                            RadarDesign.SnappedRect =
                                ThisScreen.ScreenRect.ToCenterTop(RadarDesign.SnappedRect.width,
                                        RadarDesign.SnappedRect.width, 0, posOffSetByScale)
                                    .AddRect(0, Screen.height - RadarDesign.SnappedRect.size.y);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    transform.localPosition = RadarDesign.renderingCamera.ScreenToWorldPoint(new Vector3(
                        RadarDesign.SnappedRect.center.x + RadarDesign.xPadding,
                        RadarDesign.SnappedRect.center.y - RadarDesign.yPadding,
                        RadarDesign2D.ConstantRadarRenderDistance));


                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion
        }

        /// <summary>
        ///     Here we create the center blip  by simply checking the blip values which you have set and then instancing a sprite
        ///     to match your settings. it simply remains in the center of your radar
        ///     if you have not set the center object to be 'Acive'  then the radar itself will be the centr object and no blip
        ///     will be shown.
        /// </summary>
        private void CenterObjectBlip()
        {
            #region CenterObject BLip

            var dynamicTrackingBOunds = RadarDesign.UseUI ? RadarDesign.TrackingBounds * 50F : RadarDesign.TrackingBounds;

            var dynamicSceneScale = RadarDesign.UseUI ? RadarDesign.SceneScale / 50F : RadarDesign.SceneScale;

            if (_RadarCenterObject2D.IsActive)
            {

                 
                if (!_RadarCenterObject2D.Instanced)
                {
                    // if we are creating the center object as a sprite

                    #region Sprite

                    if (_RadarCenterObject2D._CreateBlipAs == CreateBlipAs.AsSprite ||
                        _RadarCenterObject2D._CreateBlipAs == CreateBlipAs.AsMesh)
                    {

                        if (!RadarDesign.UseUI)
                        {

                            _RadarCenterObject2D.CenterBlip = new GameObject().transform;
                            _RadarCenterObject2D.CenterBlip.transform.SetParent(RadarDesign.BlipsParentObject);//  removed.parent test to make sure that this does nto break anything
                            _RadarCenterObject2D.CenterBlip.name = _RadarCenterObject2D.Tag;
                            _RadarCenterObject2D.CenterBlip.transform.position = transform.position;
                            _RadarCenterObject2D.CenterBlip.localScale = new Vector3(_RadarCenterObject2D.BlipSize,
                                _RadarCenterObject2D.BlipSize, _RadarCenterObject2D.BlipSize);
                            _RadarCenterObject2D.CenterBlip.gameObject.AddComponent<SpriteRenderer>();
                            var centerObjectSpriteRenderer = _RadarCenterObject2D.CenterBlip.GetComponent<SpriteRenderer>();
                            centerObjectSpriteRenderer.material = _RadarCenterObject2D.BlipMaterial;
                            centerObjectSpriteRenderer.color = _RadarCenterObject2D.colour;
                            centerObjectSpriteRenderer.sprite = _RadarCenterObject2D.BlipSprite;
                            centerObjectSpriteRenderer.sortingOrder = _RadarCenterObject2D.OrderInLayer;
                            centerObjectSpriteRenderer.gameObject.layer = _RadarCenterObject2D.Layer;

                        }
                        else
                        {

                            var blip = new GameObject().AddComponent<Image>();
                            _RadarCenterObject2D.CenterBlip = blip.transform;
                            _RadarCenterObject2D.CenterBlip.transform.SetParent(RadarDesign.BlipsParentObject);//  removed.parent test to make sure that this does nto break anything
                            _RadarCenterObject2D.CenterBlip.name = _RadarCenterObject2D.Tag;
                            _RadarCenterObject2D.CenterBlip.transform.position = transform.position;
                            _RadarCenterObject2D.CenterBlip.localScale = new Vector3(_RadarCenterObject2D.BlipSize,
                            _RadarCenterObject2D.BlipSize, _RadarCenterObject2D.BlipSize);
                            // _RadarCenterObject2D.CenterBlip.gameObject.AddComponent<SpriteRenderer>();
                            var centerObjectSpriteRenderer = _RadarCenterObject2D.CenterBlip.GetComponent<Image>();
                            centerObjectSpriteRenderer.material = _RadarCenterObject2D.BlipMaterial;
                            centerObjectSpriteRenderer.color = _RadarCenterObject2D.colour;
                            centerObjectSpriteRenderer.sprite = _RadarCenterObject2D.BlipSprite;
                            centerObjectSpriteRenderer.gameObject.layer = _RadarCenterObject2D.Layer;


                        }

                        _RadarCenterObject2D.Instanced = true;
                    }

                    #endregion

                    // if we are creating the center object as a prefab

                    #region Prefab

                    if (_RadarCenterObject2D._CreateBlipAs == CreateBlipAs.AsPrefab)
                    {
                        _RadarCenterObject2D.CenterBlip = Instantiate(_RadarCenterObject2D.prefab, transform.position,
                            Quaternion.identity);
                        _RadarCenterObject2D.CenterBlip.transform.SetParent(RadarDesign.BlipsParentObject);
                        _RadarCenterObject2D.CenterBlip.transform.position = transform.position;
                        _RadarCenterObject2D.CenterBlip.name = _RadarCenterObject2D.Tag;
                        _RadarCenterObject2D.CenterBlip.localScale = new Vector3(_RadarCenterObject2D.BlipSize,
                            _RadarCenterObject2D.BlipSize, _RadarCenterObject2D.BlipSize);
                        _RadarCenterObject2D.CenterBlip.GetComponent<Transform>().gameObject.layer =
                            _RadarCenterObject2D.Layer;
                        _RadarCenterObject2D.Instanced = true;
                    }

                    #endregion
                }


                var centerBlipTransform = _RadarCenterObject2D.CenterBlip.transform;


                #region Set Center Object 

                if (!_RadarCenterObject2D.CenterObject ||
                    _RadarCenterObject2D.CenterObject == RadarDesign.camera.transform)
                    try
                    {
                        _RadarCenterObject2D.CenterObject =
                            GameObject.FindGameObjectWithTag(_RadarCenterObject2D.Tag).transform;
                    }
                    catch
                    {
                        // if for some reson the center object is destroyed of disable d then we fal back to using the camera as the center object
                        _RadarCenterObject2D.CenterObject = RadarDesign.camera.transform;
                    }

                #endregion

                #region calculate distance and determine action to take based on the center blip distance 

                // we calculate the distance between the blip and this transform 


                var distance = Vector3.Distance(transform.position + (RadarDesign.Pan / dynamicSceneScale), transform.position);

                if (distance > dynamicTrackingBOunds)
                {
                    // here we check if AlwaysShowCenterObject is true. If it is, we will let the center object remain in the radar 
                    if (_RadarCenterObject2D.AlwaysShowCenterObject)
                    {
                        if (!_RadarCenterObject2D.CenterBlip.gameObject.activeInHierarchy)
                            _RadarCenterObject2D.CenterBlip.gameObject.SetActive(true);
                    }
                    else
                    {
                        if (_RadarCenterObject2D.CenterBlip.gameObject.activeInHierarchy)
                            _RadarCenterObject2D.CenterBlip.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (!_RadarCenterObject2D.CenterBlip.gameObject.activeInHierarchy)
                        _RadarCenterObject2D.CenterBlip.gameObject.SetActive(true);
                }

                #endregion

                #region Scale Tracking


                var workingBlipScale = _RadarCenterObject2D.CenterObjectCanScaleByDistance ? _RadarCenterObject2D.BlipMinSize : _RadarCenterObject2D.BlipSize;


                var dynamicScale =
                    (distance < dynamicTrackingBOunds) ?


                    _RadarCenterObject2D.CenterObjectCanScaleByDistance ?
                               Mathf.Clamp(_RadarCenterObject2D.BlipMaxSize - (_RadarCenterObject2D.BlipMaxSize * (distance / dynamicTrackingBOunds))
                             , _RadarCenterObject2D.BlipMinSize
                             , _RadarCenterObject2D.BlipMaxSize)

                             :
                             _RadarCenterObject2D.BlipSize
                             :
                             _RadarCenterObject2D.AlwaysShowCenterObject ?
                              Mathf.Clamp(workingBlipScale - (workingBlipScale * (distance - dynamicTrackingBOunds))
                                        , workingBlipScale
                                        , workingBlipScale)
                                        :
                                        _RadarCenterObject2D.BlipSize
                             ;



                centerBlipTransform.localScale = new Vector3(dynamicScale, dynamicScale, dynamicScale);

                #endregion

                #region Position tracking

                //to place the center object at the center of the radar at all times and also make it use the pan offset we simply do the following 
               /* var centerObjectOffset = Vector3.zero;
                if (!RadarDesign.RoundRadar)
                {
                    centerObjectOffset = !_RadarCenterObject2D.AlwaysShowCenterObject
                          ? RadarDesign.Pan / dynamicSceneScale
                          : Vector3.ClampMagnitude(RadarDesign.Pan / dynamicSceneScale, dynamicTrackingBOunds);
                }
                else
                {
                    if (RadarDesign.UseUI)
                    {
                        var minX = GetComponent<RectTransform>().sizeDelta.x * transform.parent.GetComponent<CanvasScaler>().scaleFactor;
                        var minY = GetComponent<RectTransform>().sizeDelta.y * transform.parent.GetComponent<CanvasScaler>().scaleFactor;

                        var nPos = new Vector3(Mathf.Clamp(RadarDesign.Pan.x / dynamicSceneScale, 0, minX), Mathf.Clamp(RadarDesign.Pan.y / dynamicSceneScale, 0, minY), Mathf.Clamp(RadarDesign.Pan.y / dynamicSceneScale, 0, minY));
                        centerObjectOffset = !_RadarCenterObject2D.AlwaysShowCenterObject
                              ? RadarDesign.Pan / dynamicSceneScale
                              : nPos;
                    }
                    else
                    {
                        centerObjectOffset = !_RadarCenterObject2D.AlwaysShowCenterObject
      ? RadarDesign.Pan / dynamicSceneScale
      : Vector3.ClampMagnitude(RadarDesign.Pan / dynamicSceneScale, dynamicTrackingBOunds);
                    }
                }
                */
               var  centerObjectOffset = !_RadarCenterObject2D.AlwaysShowCenterObject
? RadarDesign.Pan / dynamicSceneScale
: Vector3.ClampMagnitude(RadarDesign.Pan / dynamicSceneScale, dynamicTrackingBOunds);


                centerBlipTransform.localPosition = new Vector3(centerObjectOffset.x,
                    centerObjectOffset.z, 0);
                // = new Vector3(transform.position.x + CenterObjectOffset.x, transform.position.y + CenterObjectOffset.z, transform.position.z - 0.01f);

                #endregion

                #region Rotation tracking



                /*  if (_RadarCenterObject2D.IsTrackRotation && _RadarCenterObject2D.IsActive)
                  {
                      switch (_RadarCenterObject2D.rotatingMethod)
                      {
                          case RotatingMethod.singleAxis:

                              var axisValue = _RadarCenterObject2D.targetRotationAxis == TargetRotationAxis.useX ? _RadarCenterObject2D.CenterObject.transform.eulerAngles.x : _RadarCenterObject2D.targetRotationAxis == TargetRotationAxis.useY ? _RadarCenterObject2D.CenterObject.transform.eulerAngles.y : _RadarCenterObject2D.CenterObject.transform.eulerAngles.z;

                              centerBlipTransform.localEulerAngles = new Vector3(0, 0,
                       -axisValue - transform.rotation.eulerAngles.z);

                              break;
                          case RotatingMethod.multiAxis:

                              centerBlipTransform.localEulerAngles = new Vector3(
                                  _RadarCenterObject2D.lockX ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.x - transform.rotation.eulerAngles.x,
                                  _RadarCenterObject2D.lockY ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.y - transform.rotation.eulerAngles.y,
                                  _RadarCenterObject2D.lockZ ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.z - transform.rotation.eulerAngles.z);

                              break;

                      }

                  }*/
                if (_RadarCenterObject2D.IsTrackRotation && _RadarCenterObject2D.IsActive)
                {
                    switch (_RadarCenterObject2D.rotatingMethod)
                    {
                        case RotatingMethod.singleAxis:

                            var axisValue = _RadarCenterObject2D.targetRotationAxis == TargetRotationAxis.useX ? _RadarCenterObject2D.CenterObject.transform.eulerAngles.x : _RadarCenterObject2D.targetRotationAxis == TargetRotationAxis.useY ? _RadarCenterObject2D.CenterObject.transform.eulerAngles.y : _RadarCenterObject2D.CenterObject.transform.eulerAngles.z;

                            centerBlipTransform.localEulerAngles = new Vector3(0, 0,
                     -axisValue);

                            break;
                        case RotatingMethod.multiAxis:

                            centerBlipTransform.localEulerAngles = new Vector3(
                                _RadarCenterObject2D.lockX ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.x,
                                _RadarCenterObject2D.lockY ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.y,
                                _RadarCenterObject2D.lockZ ? 0 : -_RadarCenterObject2D.CenterObject.transform.eulerAngles.z);

                            break;

                    }

                }
                else

                    centerBlipTransform.rotation = new Quaternion(0, 0, 0, 0);



                #endregion
            }
            else
            {
                if (_RadarCenterObject2D.CenterBlip != null)
                    if (_RadarCenterObject2D.CenterBlip.gameObject.activeInHierarchy)
                        _RadarCenterObject2D.CenterBlip.gameObject.SetActive(false);

                _RadarCenterObject2D.CenterObject = RadarDesign.camera.transform;
            }

            #endregion
        }

        /// <summary>
        ///     Here we go about instancing all other blips
        /// </summary>
        private void CheckAndSetBlips()
        {
            #region Check and Set Blips

            var dynamicTrackingBOunds = RadarDesign.UseUI ? RadarDesign.TrackingBounds * 50F : RadarDesign.TrackingBounds;
            var dynamicInnerCullingZone = RadarDesign.UseUI ? RadarDesign.InnerCullingZone * 50F : RadarDesign.InnerCullingZone;
            var dynamicSceneScale = RadarDesign.UseUI ? RadarDesign.SceneScale / 50F : RadarDesign.SceneScale;


            // we go through all the blip type you create and then...
            foreach (var item in Blips)
            {
                var trackedObjectCount = item.gos.Count;

                #region Removal of BLips when using Recursive Optimization 

                if (item.optimization.objectFindingMethod == ObjectFindingMethod.Recursive)
                {
                    for (var i = 0; i < trackedObjectCount; i++)
                    {
                        var currentWorkingObject = item.gos[i];


                        if (currentWorkingObject == null)
                        {
                            DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                            item.DoRemoval = true;
                        }

                        if (item.optimization.RemoveBlipsOnTagChange)
                            if (currentWorkingObject != null)
                                if (currentWorkingObject.tag != item.Tag)
                                {

                                    DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                                    item.DoRemoval = true;
                                }

                        if (item.optimization.RemoveBlipsOnDisable)
                            if (currentWorkingObject != null)
                                if (!currentWorkingObject.activeInHierarchy)
                                {

                                    DestroyImmediate(item.RadarObjectToTrack[i].gameObject);
                                    item.DoRemoval = true;
                                }
                    }

                    if (item.DoRemoval)
                    {
                        item.RadarObjectToTrack.RemoveAll(x => x == null);
                    }
                }



                #endregion


                #region Find all objects that we want to track 

                switch (item.optimization.objectFindingMethod)
                {
                    case ObjectFindingMethod.Pooling:
                        if (item.optimization.SetPoolSizeManually)
                        {
                            if (BeginObjectInstanceCheck || trackedObjectCount < item.optimization.poolSize)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!
                                if (item.optimization.RecalculatePoolSizeBasedOnFirstFoundObjects)
                                    if (item.optimization.poolSize > trackedObjectCount)
                                        item.optimization.poolSize = trackedObjectCount;


                            }
                        }
                        else
                        {
                            if (BeginObjectInstanceCheck)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!

                            }
                        }

                        break;
                    case ObjectFindingMethod.Recursive:
                        if (item.optimization.RequireInstanceObjectCheck)
                        {

                            if (BeginObjectInstanceCheck || item.DoRemoval)
                            {
                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!

                            }
                        }
                        else
                        {

                            item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList(); //DO AT END OF FRAME !!
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                #endregion

                #region Ensure that our tracking line count and TrackedObjects= count are always correct ONLY FOR RECURSIVE OPTIMIZATION METHOD

                if (item.DoRemoval)
                    item.DoRemoval = false;

                trackedObjectCount = item.gos.Count;

                #endregion

                // if the blip type is set to active the following will execute
                if (item.IsActive)
                {
                    #region Instancing blips

                    // we will begin populating the list which will hold the blips by checking if the amount of things in the blip list is equal to the amount of theing we want to track
                    for (var a = item.RadarObjectToTrack.Count; a < trackedObjectCount; a++)
                    {
                        //  ListExt.Resize(item.RadarObjectToTrack, trackedObjectCount);

                        #region Sprites

                        if (item._CreateBlipAs == CreateBlipAs.AsSprite || item._CreateBlipAs == CreateBlipAs.AsMesh)
                        {


                            //here we check if we need to add a pivot gameobject to the blip 

                            if (!RadarDesign.UseUI)
                            {
                                var blip = new GameObject();
                                blip.transform.SetParent(RadarDesign.BlipsParentObject);
                                blip.transform.position = transform.position;
                                blip.name = item.Tag;
                                blip.AddComponent<SpriteRenderer>();
                                var blipSpriteRenderer = blip.GetComponent<SpriteRenderer>();
                                blipSpriteRenderer.material = item.BlipMaterial;
                                blipSpriteRenderer.color = item.colour;
                                blipSpriteRenderer.sprite = item.BlipSprite;
                                blipSpriteRenderer.sortingOrder = item.OrderInLayer;
                                blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                                blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                                item.RadarObjectToTrack.Add(blip.transform);
                            }
                            else
                            {
                                var blip = new GameObject().AddComponent<Image>();
                                blip.transform.SetParent(RadarDesign.BlipsParentObject);
                                blip.transform.position = transform.position;
                                blip.gameObject.name = item.Tag;
                                var blipSpriteRenderer = blip.GetComponent<Image>();
                                blipSpriteRenderer.material = item.BlipMaterial;
                                blipSpriteRenderer.color = item.colour;
                                blipSpriteRenderer.sprite = item.BlipSprite;
                                blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                                blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                                item.RadarObjectToTrack.Add(blip.transform);
                            }

                        }

                        #endregion

                        #region Prefab

                        if (item._CreateBlipAs != CreateBlipAs.AsPrefab) continue;
                        {
                            var blip = Instantiate(item.prefab, transform.position, Quaternion.identity);
                            blip.transform.SetParent(RadarDesign.BlipsParentObject);
                            blip.transform.localScale = new Vector3(item.BlipSize, item.BlipSize, item.BlipSize);
                            blip.transform.position = transform.position;
                            blip.name = item.Tag;
                            blip.GetComponent<Transform>().gameObject.layer = item.Layer;
                            item.RadarObjectToTrack.Add(blip.transform);
                        }

                        #endregion
                    }

                    #endregion
                    if (item.RadarObjectToTrack.Count < trackedObjectCount)
                        return;
                    // we then sort through all the object in the gos list
                    for (var i = 0; i < trackedObjectCount; i++)
                    {
                        var currentWorkingRadarObjectToTrack = item.RadarObjectToTrack[i];
                        var currentWorkingRadarObjectToTrackGameObject = currentWorkingRadarObjectToTrack.gameObject;
                        var isRadarObjectToTrackActiveInHeirarchy =
                            currentWorkingRadarObjectToTrackGameObject.activeInHierarchy;

                        var currentWorkingObject = item.gos[i];

                        #region Removal of blips when using Pooling optimization mode

                        if (item.optimization.objectFindingMethod == ObjectFindingMethod.Pooling)
                        {
                            // if you ae using pooling then you should not be destroying your objets anyway. if you do. this annoying code will run
                            if (currentWorkingObject == null)
                            {
                                // remove and add back to the list

                                //  blips to the front of their respective lists This is a fast process

                                item.RadarObjectToTrack.Remove(currentWorkingRadarObjectToTrack);
                                item.RadarObjectToTrack.Add(currentWorkingRadarObjectToTrack);
                                foreach (var b in item.RadarObjectToTrack)
                                    b.gameObject.SetActive(false);

                                item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList();
                                //DO AT END OF FRAME !! // dont let it come to this :( EVEN THOUGH THIS IS REALLY FASE :P
                                // in case you destroy objects that should be pooled we reset the pool size.
                                item.optimization.poolSize = item.gos.Count;




                                return;
                            }


                            if (item.optimization.RemoveBlipsOnTagChange)
                                if (currentWorkingObject.tag != item.Tag)
                                {
                                    // remove and add back to the list

                                    // blips to the front of their respective lists This is a fast process
                                    item.RadarObjectToTrack.Remove(currentWorkingRadarObjectToTrack);
                                    item.RadarObjectToTrack.Add(currentWorkingRadarObjectToTrack);
                                    foreach (var b in item.RadarObjectToTrack)
                                        b.gameObject.SetActive(false);

                                    item.gos = GameObject.FindGameObjectsWithTag(item.Tag).ToList();
                                    //DO AT END OF FRAME !! // dont let it come to this :( EVEN THOUGH THIS IS REALLY FASE :P
                                    // in case you destroy objects that should be pooled we reset the pool size.
                                    item.optimization.poolSize = item.gos.Count;



                                    return;
                                }

                            if (item.optimization.RemoveBlipsOnDisable)
                                if (!currentWorkingObject.activeInHierarchy)
                                    currentWorkingRadarObjectToTrackGameObject.SetActive(false);
                        }

                        #endregion

                        var currentWorkingObjectTransform = currentWorkingObject.transform;
                        var trackedObjectsPoition = currentWorkingObjectTransform.position;
                        var currentWorkingRadarObjectToTrackTransform = currentWorkingRadarObjectToTrack.transform;
                        var centerObjectsPosition = _RadarCenterObject2D.CenterObject.transform.position;

                        #region CheckAnd Set Distance

                        // the distance each blip is from the center of the radar is calculated now that we have access to everything we want to track

                      /*  var distance = 0.0f ;
                        var xDistance = 0.0f;
                        var yDistance = 0.0f;
                        var zDistance = 0.0f;

                        if (RadarDesign.radarStyle == RadarStyle.Round)
                            distance = Vector3.Distance(trackedObjectsPoition,
                                    centerObjectsPosition - RadarDesign.Pan) /
                                  dynamicSceneScale;
                        else
                        {
                            xDistance = (trackedObjectsPoition.x - (centerObjectsPosition.x - RadarDesign.Pan.x)) / dynamicSceneScale;
                            yDistance = (trackedObjectsPoition.y - (centerObjectsPosition.y - RadarDesign.Pan.y)) / dynamicSceneScale;
                            zDistance = (trackedObjectsPoition.z - (centerObjectsPosition.z - RadarDesign.Pan.z)) / dynamicSceneScale;

                            distance = Mathf.Max(new float[] { xDistance, yDistance, zDistance });

                            distance = distance < 0 ? distance * -1 : distance;
                        }*/

                       // Debug.Log(distance);

                        var distance = Vector3.Distance(trackedObjectsPoition,
                                    centerObjectsPosition - RadarDesign.Pan) /
                                  dynamicSceneScale;
                        #endregion


                        //  bool blipIsInsideDisplayZone = RadarDesign.radarStyle == RadarStyle.Round ? distance < dynamicTrackingBOunds && distance >= dynamicInnerCullingZone : (xDistance < dynamicTrackingBOunds && xDistance >= dynamicInnerCullingZone && yDistance < dynamicTrackingBOunds && yDistance >= dynamicInnerCullingZone & zDistance < dynamicTrackingBOunds && zDistance >= dynamicInnerCullingZone);
                        if (distance < dynamicTrackingBOunds && distance >= dynamicInnerCullingZone)
                        {

                            #region Position Tracking
                   

                                var trackedObjectPos = (trackedObjectsPoition -
                                                        (centerObjectsPosition -
                                                         RadarDesign.Pan)) / dynamicSceneScale;
                                currentWorkingRadarObjectToTrackTransform.localPosition = new Vector3(trackedObjectPos.x,
                                    (RadarDesign.TrackYPosition) ? trackedObjectPos.y : trackedObjectPos.z, -0.01f);

                         
                            #endregion


                            #region Rotation Tracking

                            if (item.IsTrackRotation)
                            {
                                switch (item.rotatingMethod)
                                {
                                    case RotatingMethod.singleAxis:

                                        var axisValue = item.targetRotationAxis == TargetRotationAxis.useX ? currentWorkingObjectTransform.eulerAngles.x : item.targetRotationAxis == TargetRotationAxis.useY ? currentWorkingObjectTransform.eulerAngles.y : currentWorkingObjectTransform.eulerAngles.z;

                                        currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(0, 0,
                                 -(item.InvertRotation ? -axisValue : axisValue));

                                        break;
                                    case RotatingMethod.multiAxis:

                                        currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(
                                            item.lockX ? 0 : -(item.InvertRotation ? -currentWorkingObjectTransform.eulerAngles.x : currentWorkingObjectTransform.eulerAngles.x),
                                            item.lockY ? 0 : -(item.InvertRotation ? -currentWorkingObjectTransform.eulerAngles.y : currentWorkingObjectTransform.eulerAngles.y),
                                            item.lockZ ? 0 : -(item.InvertRotation ? -currentWorkingObjectTransform.eulerAngles.z : currentWorkingObjectTransform.eulerAngles.z));

                                        break;

                                }

                            }
                            else
                            {

                                if (item.IgnoreRadarRotation)
                                {
                                    currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(0, 0, RadarDesign.TrackYPosition ? -_RadarCenterObject2D.CenterObject.eulerAngles.z : -_RadarCenterObject2D.CenterObject.eulerAngles.y);

                                }
                                else
                                    currentWorkingRadarObjectToTrackTransform.rotation = new Quaternion(0, 0, 0, 0);
                            }

                            #endregion

                            #region Scale Trcking
                            var newScalingDistance = item.IgnoreYDistanceScaling && !RadarDesign.TrackYPosition ? Vector3.Distance(new Vector3(trackedObjectsPoition.x, centerObjectsPosition.y, trackedObjectsPoition.z),
                                    centerObjectsPosition - RadarDesign.Pan) /
                                  dynamicSceneScale : distance;

                            var currentBlipScale = item.BlipCanScleBasedOnDistance ? Mathf.Clamp(item.BlipMaxSize - (item.BlipMaxSize * (newScalingDistance / dynamicTrackingBOunds)),
                                    item.BlipMinSize,
                                    item.BlipMaxSize) : item.BlipSize;
                            currentWorkingRadarObjectToTrackTransform.localScale = new Vector3(currentBlipScale, currentBlipScale, currentBlipScale);

                            #endregion


                            #region here we ensure that any blip that is not active must be made active

                            if (!isRadarObjectToTrackActiveInHeirarchy && currentWorkingObject.activeInHierarchy)
                                currentWorkingRadarObjectToTrackGameObject.SetActive(true);

                            #endregion
                        }

                        #region here we must disable all blips that enter the inner cullin zone or exceed the tracking bounds
                        //if (RadarDesign.UseUI && RadarDesign.RectangleStyle)
                       // {

                      //  }
                       // else
                       // {
                            if (distance < dynamicInnerCullingZone || distance > dynamicTrackingBOunds && !item.AlwaysShowBlipsInRadarSpace)
                                if (isRadarObjectToTrackActiveInHeirarchy)
                                {

                                    currentWorkingRadarObjectToTrackGameObject.SetActive(false);
                                }
                      //  }

                        #endregion

                        #region Always show blips in radar space

            

                        if (!item.AlwaysShowBlipsInRadarSpace || !(distance >= dynamicTrackingBOunds)) continue;
                        {
                            #region Position Tracking

                            if (RadarDesign.radarStyle == RadarStyle.Round)
                            {
                                var trackedObjectPos = (trackedObjectsPoition -
                                                    (centerObjectsPosition -
                                                     RadarDesign.Pan)) / dynamicSceneScale;
                                var offset = new Vector3(trackedObjectPos.x, trackedObjectPos.z, -0.01f);
                                currentWorkingRadarObjectToTrackTransform.localPosition = Vector3.ClampMagnitude(offset,
                                    dynamicTrackingBOunds);
                            }
                            else
                            {
                       
                            }
                            #endregion

                            #region Rotation Tracking
                            if (item.IsTrackRotation)
                            {
                                switch (item.rotatingMethod)
                                {
                                    case RotatingMethod.singleAxis:

                                        var axisValue = item.targetRotationAxis == TargetRotationAxis.useX ? currentWorkingObjectTransform.eulerAngles.x : item.targetRotationAxis == TargetRotationAxis.useY ? currentWorkingObjectTransform.eulerAngles.y : currentWorkingObjectTransform.eulerAngles.z;

                                        currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(0, 0,
                                 -axisValue);

                                        break;
                                    case RotatingMethod.multiAxis:

                                        currentWorkingRadarObjectToTrackTransform.localEulerAngles = new Vector3(
                                            item.lockX ? 0 : -currentWorkingObjectTransform.eulerAngles.x,
                                            item.lockY ? 0 : -currentWorkingObjectTransform.eulerAngles.y,
                                            item.lockZ ? 0 : -currentWorkingObjectTransform.eulerAngles.z);

                                        break;

                                }

                            }
                            else
                                currentWorkingRadarObjectToTrackTransform.rotation = new Quaternion(0, 0, 0, 0);


                            #endregion

                            #region Re-Scale Tracking

                            if (distance > dynamicTrackingBOunds)
                            {

                                var workingBlipScale = item.BlipCanScleBasedOnDistance ? item.BlipMinSize : item.BlipSize;

                                var dynamicScale =
                                               Mathf.Clamp(workingBlipScale - (workingBlipScale * (distance - dynamicTrackingBOunds))
                                             , workingBlipScale
                                             , workingBlipScale);

                                currentWorkingRadarObjectToTrackTransform.localScale = new Vector3(dynamicScale,
                                    dynamicScale,
                                    dynamicScale);
                            }

                            #endregion

                            // here we ensure that any blip outside of the tracking bounds must be enabled
                            if (!isRadarObjectToTrackActiveInHeirarchy && currentWorkingObject.activeInHierarchy)
                                currentWorkingRadarObjectToTrackGameObject.SetActive(true);
                        }

                        #endregion
                    }
                }
                else
                {
                    #region turn off all blips if blip type is set to be inactive

                    if (item.RadarObjectToTrack.Count != 0)
                    {

                      //  var difCount = trackedObjectCount - item.RadarObjectToTrack.Count;

                        for (var i = 0; i < item.RadarObjectToTrack.Count; i++) // trackedObjectCount to 
                        {
                            item.RadarObjectToTrack[i].gameObject.SetActive(false);
                        }
                    }

                    #endregion
                }

                if (item.RadarObjectToTrack.Count > trackedObjectCount)
                {
                    var difCount = item.RadarObjectToTrack.Count - trackedObjectCount;

                    for (int x = 0; x < difCount; x++)
                    {
                        Destroy(item.RadarObjectToTrack[x].gameObject);
                        item.RadarObjectToTrack[x] = null;

                    }
                    item.RadarObjectToTrack.RemoveAll(x => x == null);
                }
            }

            #endregion

            if (BeginObjectInstanceCheck) BeginObjectInstanceCheck = false;
        }

        /// <summary>
        ///     here we set up and reposition the minimap
        /// </summary>
        private void Minimap()
        {
            if (!RadarDesign._2DSystemsWithMinimapFunction) return;

            var dynamicSceneScale = RadarDesign.UseUI ? RadarDesign.SceneScale / 50F : RadarDesign.SceneScale;

            #region creationg the minimap

            if (!minimapModule.generated)
            {
                var miniMap = new GameObject("MiniMap");
                miniMap.transform.SetParent(transform);
                miniMap.transform.localScale = transform.localScale;
                if (RadarDesign.UseUI)
                    miniMap.transform.SetSiblingIndex(0);
                miniMap.transform.localPosition = Vector3.zero;
                miniMap.transform.localScale = Vector3.one;
                var mapPivot = new GameObject("map Pivot");
                mapPivot.transform.SetParent(miniMap.transform);
                mapPivot.transform.localScale = Vector3.one;
                minimapModule.MapPivot = mapPivot;
                mapPivot.transform.localPosition = Vector3.zero;
                var map = new GameObject("map");
                map.transform.SetParent(mapPivot.transform);
                map.transform.localPosition = Vector3.zero;
                map.transform.localScale = Vector3.one;
                if (!RadarDesign.UseUI)
                {
                    var mask = new GameObject("Minimap Mask");
                    mask.transform.SetParent(miniMap.transform);
                    minimapModule.Mask = mask;
                    mask.AddComponent<SpriteRenderer>();
                    mask.GetComponent<SpriteRenderer>().sprite = (minimapModule.UseCustomMapMaskShape && minimapModule.CustomMapMaskShape != null) ? minimapModule.CustomMapMaskShape : minimapModule.MaskSprite();
                    mask.GetComponent<SpriteRenderer>().sortingOrder = minimapModule.OrderInLayer;
                    mask.GetComponent<SpriteRenderer>().sharedMaterial = minimapModule.MaskMaterial;
                    mask.transform.localScale = Vector3.one;
                    mask.transform.localPosition = new Vector3(0, 0, 0.01f);
                    mask.layer = minimapModule.layer;
                }
                minimapModule.Map = map;

                if (minimapModule.mapType == MapType.Realtime)
                {
                    if (!RadarDesign.UseUI)
                    {
                        map.AddComponent<MeshFilter>().sharedMesh = minimapModule.ProceduralMapQuad();
                        // create a new masked material on the fly so that we dont overwrite any preexisting matrials in the project
                        // and use it as the map material
                        var mapRenderTextureMaterial = new Material(minimapModule.MapMaterial.shader)
                        {
                            mainTexture = minimapModule.renderTexture
                        };
                        // use the render texture as the main texture
                        // Add a mesh renderer and set its shared materia lto be the MapRenderTextureMaterial
                        var mapRenderer = map.AddComponent<MeshRenderer>();
                        mapRenderer.sharedMaterial = mapRenderTextureMaterial;
                        mapRenderer.receiveShadows = false;
                        mapRenderer.lightProbeUsage = LightProbeUsage.Off;
                        mapRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
                        mapRenderer.sortingOrder = minimapModule.OrderInLayer;
                        // Map.AddComponent<RT2S>();
                        //  Map.GetComponent<RT2S>().renderTexture = minimapModule.renderTexture;
                    }
                    else
                    {
                        map.AddComponent<CanvasRenderer>();
                        map.AddComponent<RawImage>().texture = minimapModule.renderTexture;
                    }
                }
                else
                {
                    if (!RadarDesign.UseUI)
                    {
                        map.AddComponent<SpriteRenderer>();
                        map.GetComponent<SpriteRenderer>().sprite = minimapModule.MapTexture;
                        map.GetComponent<SpriteRenderer>().sharedMaterial = minimapModule.MapMaterial;
                        map.GetComponent<SpriteRenderer>().sortingOrder = minimapModule.OrderInLayer;
                    }
                    else
                    {
                        map.AddComponent<CanvasRenderer>();
                        map.AddComponent<Image>().sprite = minimapModule.MapTexture;
                    }
                }
                map.layer = minimapModule.layer;
                minimapModule.SavedSceneScale = dynamicSceneScale;
                minimapModule.SavedMapScale = minimapModule.MapScale;
                if (!minimapModule.calibrate)
                    minimapModule.Scalingfactor = minimapModule.SavedMapScale / minimapModule.SavedSceneScale;
                minimapModule.generated = true;
                miniMap.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            #endregion

            #region Map Positioning

            if (minimapModule.mapType == MapType.Static)
            {

                if (minimapModule.Refreshedmap)
                {
                    minimapModule.StartPosition = _RadarCenterObject2D.CenterObject.transform.position;
                    minimapModule.Refreshedmap = false;
                }

                var trackedObjectPos = -((_RadarCenterObject2D.CenterObject.transform.position - minimapModule.StartPosition) - RadarDesign.Pan) /
                                       dynamicSceneScale;

                if (minimapModule.MapCenterReference == null)
                {
                    Debug.Log("You are now required to assign a map center reference gameObject, Please open toi Radar Builder editor , go to the Designs tab and assign a map center reference under minmap settings ");
                    Debug.Break();
                    return;
                }
                var distancefromMapCenter = new Vector3(
                    minimapModule.StartPosition.x - minimapModule.MapCenterReference.position.x,
                    minimapModule.StartPosition.y - minimapModule.MapCenterReference.position.y,
                    minimapModule.StartPosition.z - minimapModule.MapCenterReference.position.z) / dynamicSceneScale;


                minimapModule.Map.transform.localPosition = new Vector3(trackedObjectPos.x - distancefromMapCenter.x, RadarDesign.TrackYPosition ? trackedObjectPos.y - distancefromMapCenter.y : trackedObjectPos.z - distancefromMapCenter.z, -0.01f);

            }
            else
            {
                var trackedObjectPos = _RadarCenterObject2D.CenterObject.transform.position - RadarDesign.Pan;
                minimapModule.RealtimeMinimapCamera.transform.position = new Vector3(
                    trackedObjectPos.x,
                    trackedObjectPos.y + (RadarDesign.TrackYPosition ? 0 : minimapModule.CameraHeight),
                    trackedObjectPos.z + (RadarDesign.TrackYPosition ? minimapModule.CameraHeight : 0));


            }

            #endregion

            #region Map Scale

            if (minimapModule.calibrate) return;
            if (minimapModule.mapType == MapType.Static)
            {
                var scaleOffset = dynamicSceneScale - minimapModule.SavedSceneScale;

                var scale = minimapModule.SavedMapScale -
                            scaleOffset * minimapModule.Scalingfactor /
                            (dynamicSceneScale / minimapModule.SavedSceneScale);
                minimapModule.Map.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                if (!RadarDesign.UseUI)
                {
                    minimapModule.RealtimeMinimapCamera.orthographicSize = dynamicSceneScale;
                    //  minimapModule.Map.transform.localScale = new Vector3(1, 1, 1); Debug.Log("setting");
                }
                else
                {
                    minimapModule.RealtimeMinimapCamera.orthographicSize = dynamicSceneScale * 100;
                    minimapModule.Map.transform.localScale = new Vector3(2, 2, 2);
                }
            }

            #endregion


            #region Map rotation
            minimapModule.MapPivot.transform.localEulerAngles = new Vector3(0, 0, RadarDesign.RadarRotationOffset);
            #endregion
        }

        /// <summary>
        ///  This allows you to set a new static map texture from a custom script
        /// </summary>
        /// <param name="sprite"></param>
        public void SwapStaticMinimapTexture(Sprite sprite)
        {
            minimapModule.Map.GetComponent<SpriteRenderer>().sprite = sprite;
            minimapModule.Refreshedmap = true;
        }

        /// <summary>
        ///     Here we determine what 'Design layers' are to be rotates and what object rotation they will match proportionally |
        ///     inversely and with a damping
        /// </summary>
        private void RotateSpecifics()
        {
            #region Rotating specific design layers

            foreach (var rotationTarget in RadarDesign.RotationTargets)
            {
                if (rotationTarget.UseRotationTarget)
                {
                    if (!rotationTarget.Target)
                        switch (rotationTarget.ObjectToTrack)
                        {
                            case TargetObject.FindObject:
                                try
                                {
                                    if (rotationTarget.FindingName.Length > 0)
                                        rotationTarget.Target = GameObject.Find(rotationTarget.FindingName);
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.ObjectWithTag:
                                try
                                {
                                    if (rotationTarget.tag.Length > 0)
                                        rotationTarget.Target = GameObject.FindWithTag(rotationTarget.tag);
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.InstancedBlip:
                                try
                                {
                                    if (rotationTarget.InstancedObjectToTrackBlipName.Length > 0)
                                        rotationTarget.Target =
                                            RadarDesign.BlipsParentObject.transform.Find(rotationTarget.InstancedObjectToTrackBlipName).gameObject;
                                }
                                catch
                                {
                                    // ignored
                                }
                                break;
                            case TargetObject.ThisObject:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    switch (rotationTarget.target)
                    {
                        case TargetBlip.InstancedBlip:
                            try
                            {
                                if (rotationTarget.InstancedTargetBlipname.Length > 0)
                                    rotationTarget.TargetedObject =
                                       RadarDesign.BlipsParentObject.transform.Find(rotationTarget.InstancedTargetBlipname).gameObject;
                            }
                            catch
                            {
                                // ignored
                            }
                            break;
                        case TargetBlip.ThisObject:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (rotationTarget.TargetedObject && rotationTarget.Target)
                        rotationTarget.TargetedObject.transform.localEulerAngles = (new Vector3(
                   0,
                   0,
                   rotationTarget.UseY
                        ? (rotationTarget.rotations == Rotations.Proportional
                            ? -rotationTarget.Target.transform.eulerAngles.y
                            : rotationTarget.Target.transform.eulerAngles.y)
                        : (rotationTarget.rotations == Rotations.Proportional
                            ? -rotationTarget.Target.transform.eulerAngles.z
                            : rotationTarget.Target.transform.eulerAngles.z)
                    ) / 100) * (100 - rotationTarget.RotationDamping);
                }
            }

            #endregion
        }

        #region variables (View source code listed aove for reference to these 4 classes, if you wish to replave these variables you will have to rename the classes to prevent clashing )

        /// <summary>
        /// The object being displayed at the center of your radar
        /// </summary>
        [HideInInspector]
        public RadarCenterObject2D _RadarCenterObject2D;

        /// <summary>
        /// all other radar blips 
        /// </summary>
        [HideInInspector]
        public List<RadarBlips2D> Blips = new List<RadarBlips2D>();

        /// <summary>
        /// Radar Design area 
        /// </summary>
        [HideInInspector]
        public RadarDesign2D RadarDesign = new RadarDesign2D();

        /// <summary>
        /// Minimap modeule
        /// </summary>
        [HideInInspector]
        public MiniMapModule minimapModule = new MiniMapModule();

        [HideInInspector]
        public delegate void DoInstanceObjectCheck();

        /// <summary>
        ///     trigger search for new objes for the radar to track as blips
        /// </summary>
        public static DoInstanceObjectCheck doInstanceObjectCheck;


        /// <summary>
        ///     when set to true, will trigger a search for objects to track, once completed , it will reset it false
        /// </summary>
        public bool BeginObjectInstanceCheck = true;


        #endregion


    }


}