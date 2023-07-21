using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InfiniteWorldsAndMegastructures : MonoBehaviour
{

    //Inspector variables

    [Header("PRIMARY")]
    [Space]
    [Tooltip("Type the Player's tag here.  It must be a unique tag.  You can leave this as the default 'Player' if your player character's tag is set to 'Player'")]
    public string PlayerTag = "Player";
    [Tooltip("This allows you to define additional tags that will move throughout the infinitely wrappable area.  The tags must NOT share a tag with the objects defined below, or with the player's tag.")]
    public string[] AdditionalTags;
    [Tooltip("Tick this if you want the environment to be infinite.  This will cause the player to wrap back around before leaving bounds.  This is usually desirable.  However, you may want to disable this after your player is intended to leave the infinite area.")]
    public bool InfiniteWrapping = true;
    [Tooltip("This is the size of the world.  Enter a value between 200 and 500.  This value also effects how far the player can see.")]
    [Range(200, 800)]
    public int WorldSize = 800;
    [Tooltip("This will further increase the world size and view distance.  However, it can be costly if your models are not low poly")]
    [Range(0, 1700)]
    public int WorldAdditionalSize = 0;
    [Tooltip("Tick this to enable the Additional World Size.  If set too high, may cause lag depending on which game objects you are using.")]
    public bool UseAdditionalSize;
    [Tooltip("This is the distance, in meters, that will form an empty space between the duplicated objects.")]
    public Vector3 DistanceBetweenObjects;    
    [Tooltip("This object must be an object that is in the scene.  This object will also be included in the wrapping. This slot allows you to assign the object that will serve as the location for the player to start on.  It will be near the center of the wrappable area.  Do not parent the player to this object.")]
    public GameObject StartingEnvironmentToWrap;
    [Tooltip("Tick this if you want the starting environment object to be duplicated in the wrapping process.  If you leave this unticked, there will only be 1 of this environment piece.  IF THIS IS TICKED, YOU MUST INCLUDE AT LEAST 1 OBJECT IN THE OTHER ENVIRONMENT SLOT")]
    public bool WrapStartingEnvironment;
    [Tooltip("This slot allows you to assign any objects that you want to be included in the wrapping.  This will effect the size used to calculate wrapping.  The combined total size of objects will be used for wrapping.")]
    public Object[] OtherEnvironmentToWrap;
    [Header("──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────")]
    [Header("RANDOMIZATION")]
    [Space]
    [Tooltip("This will randomize the position of objects, creating a staggered effect.  If there is no distance between objects, this may cause the objects to overlap.")]
    [Range(0, 1)]
    public float RandomPositionStrength = 0;
    [Tooltip("This will randomize the scale of objects.")]
    [Range(0, 1)]
    public float RandomScaleStrength = 0;
    [Tooltip("This will randomize the rotation of objects, creating a staggered effect.  This will effect the space between objects.")]
    [Range(0, 1)]
    public float XRandomRotationStrength = 0;
    [Tooltip("This will randomize the rotation of objects, creating a staggered effect.  This will effect the space between objects.")]
    [Range(0, 1)]
    public float YRandomRotationStrength = 0;
    [Tooltip("This will randomize the rotation of objects, creating a staggered effect.  This will effect the space between objects.")]
    [Range(0, 1)]
    public float ZRandomRotationStrength = 0;
    [Header("──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────")]
    [Header("CAMERA EFFECTS")]
    [Space]
    [Tooltip("This is desirable.  Fog helps to ease in the distant objects.  You will almost always want this enabled.")]
    public bool UseFog = true;
    [Tooltip("This allows you to select the color of the ambient fog.  Fog helps to visually smooth out the world wrapping process.")]
    public Color FogColor = Color.white;
    [Tooltip("This will create a visible outline along the edges of geometry in your scene.  This is an optional effect.")]
    public bool EdgeOutline;
    [Header("──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────")]
    [Header("OPTIMIZATION")]
    [Space]
    [Tooltip("This will enable shadows on the objects.  If your scene is too large or too complex, it could cause some lag.")]
    public bool UseShadows;
    [Tooltip("Tick this to enable GPU instancing on the materials.  This is desirable, because it will drastically increase framerate (FPS)")]
    public bool EnableGPU_Instancing = true;


    //To be added in an update
    //public bool bTestRestart; 

    //Script variables
    [HideInInspector]
    public string sPlayerTag = "Player";
    [HideInInspector]
    public int iWorldSize;
    [HideInInspector]
    public int iAdditionalWorldSize;
    [HideInInspector]
    public bool bUseAdditionalSize;
    [HideInInspector]
    public Object[] oObjects;
    [HideInInspector]
    public GameObject oStartingObject;
    [HideInInspector]
    public Vector3 vDistanceBetweenObjects;
    [HideInInspector]
    public int[] iObjectFrequency;
    [HideInInspector]
    public bool bWrapX;
    [HideInInspector]
    public bool bWrapY;
    [HideInInspector]
    public bool bWrapZ;
    [HideInInspector]
    public bool bWrapPlayer;
    [HideInInspector]
    public Color colorFogColor;
    [HideInInspector]
    public Color colorStoredFogColor;
    [HideInInspector]
    public Material matSkyMat;
    [HideInInspector]
    public float fStaggerPositions;
    [HideInInspector]
    public float fStaggerRotationX;
    [HideInInspector]
    public float fStaggerRotationY;
    [HideInInspector]
    public float fStaggerRotationZ;
    [HideInInspector]
    public float fRandomScale;
    [HideInInspector]
    public bool bGPUInstancing;
    [HideInInspector]
    public bool bUseFog;
    [HideInInspector]
    public bool bEdgeOutline;

    //The bounds of the newly created duplicate object
    [HideInInspector]
    public Bounds boundsNewDuplicateEncapsulated;

    //Variables for logic
    [HideInInspector]
    public List<GameObject> listAllObjects = new List<GameObject>();
    [HideInInspector]
    public int iObjectsCount;
    [HideInInspector]
    public float fCameraFarClip; //The Main Camera's far clip
    [HideInInspector]
    public float fCombinedMeshBounds; //The bounds of the object to wrap;
    [HideInInspector]
    public List<Bounds> listBounds = new List<Bounds>(); //The list of all bounds to consider    
    [HideInInspector]
    public Bounds boundsEncapsulatedBounds;
    [HideInInspector]
    public Vector3 vBoxSize;
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var matrix = transform.localToWorldMatrix;
        float positionY = 0;
        Handles.color = Color.green;
        // Handles.matrix = matrix;
        Handles.DrawWireCube(transform.position, gizmoSize);
        // for (var i = 0; i < heightRaysCount; i++)
        // {
        //     var percentage = (float)i / (float)heightRaysCount;
        //     float value = stepHeightCurve.Evaluate(percentage);
        //     positionY += value * maxStepHeight;
        //     Handles.matrix = matrix;
        //     var position = new Vector3(0, positionY, 0);
        //     Handles.DrawWireCube(transform.position, vBoxSize);
        // }
    }

    [SerializeField] private Vector3 gizmoSize = Vector3.one;
#endif
    [HideInInspector]
    public int iBoxSizeX;
    [HideInInspector]
    public int iBoxSizeY;
    [HideInInspector]
    public int iBoxSizeZ;
    [HideInInspector]
    public List<GameObject> listQuadrantObjects = new List<GameObject>();
    [HideInInspector]
    public int iLateralDupeCountInQuadrantX;
    [HideInInspector]
    public int iLateralDupeCountInQuadrantY;
    [HideInInspector]
    public int iLateralDupeCountInQuadrantZ;
    [HideInInspector]
    public GameObject oGlobalParent;
    [HideInInspector]
    public bool bFirstRun = true;

    //For logic and player wrapping
    [HideInInspector]
    public float fLateralQuadrantSizeX;
    [HideInInspector]
    public float fLateralQuadrantSizeY;
    [HideInInspector]
    public float fLateralQuadrantSizeZ;

    //Variables for player wrapping
    [HideInInspector]
    public GameObject oPlayer;
    [HideInInspector]
    public Vector3 vCenterOfDupeParent;

    //Bool for if we are in play mode or editor
    [HideInInspector]
    public bool bInPlayMode;

    //Bounds for starting bounds
    [HideInInspector]
    public Bounds boundsStartingBounds;

    //Camera
    [HideInInspector]
    public bool bCameraIsChildOfPlayer;
    [HideInInspector]
    public GameObject oCameraRoot;

    [HideInInspector]
    public bool bCameraIsChildOfOther;

    //Additional tags
    [HideInInspector]
    public string[] sOtherTags;

    //Max distance for calculating wrap player
    [HideInInspector]
    public float fMaxDistanceX;
    [HideInInspector]
    public float fMaxDistanceY;
    [HideInInspector]
    public float fMaxDistanceZ;

    //Wrap starting enviro?
    [HideInInspector]
    public bool bWrapStartingEnviro;

    //Use Shadows
    private bool bUseShadows;

    private GameObject oCamera;
    private Camera cCamera;

    private bool bGenerationComplete;

    //Image Effects
    private UnityStandardAssets.ImageEffects.EdgeDetection cEdgDet;
    private UnityStandardAssets.ImageEffects.GlobalFogWrap cFog;

    Bounds boundsEncapsulatedIndividual;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            bInPlayMode = true;
        }
        else { bInPlayMode = false; }
    }

    private void Start()
    {        
        if ((GameObject.FindGameObjectWithTag("MainCamera") != null) && (!bGenerationComplete))
        {
            oCamera = GameObject.FindGameObjectWithTag("MainCamera");
            cCamera = oCamera.GetComponent<Camera>();
            doGeneration();
            vCenterOfDupeParent = oGlobalParent.transform.TransformPoint(Vector3.zero);
            bGenerationComplete = true;
        }
        else if ((FindObjectsOfType<Camera>().Length < 2) && (!bGenerationComplete))
        {
            oCamera = FindObjectOfType<Camera>().gameObject;
            cCamera = oCamera.GetComponent<Camera>();
            doGeneration();
            vCenterOfDupeParent = oGlobalParent.transform.TransformPoint(Vector3.zero);
            bGenerationComplete = true;
        }

        //Max distance for calculating player wrap and other tag wrap
        fMaxDistanceX = ((iLateralDupeCountInQuadrantX * vBoxSize.x) / 2) + 5;
        fMaxDistanceY = ((iLateralDupeCountInQuadrantY * vBoxSize.y) / 2) + 5;
        fMaxDistanceZ = ((iLateralDupeCountInQuadrantZ * vBoxSize.z) / 2) + 5;
    }

    private void Update()
    {
        if (!bGenerationComplete)
        {
            if (Camera.current != null)
            {
                oCamera = Camera.current.gameObject;
                cCamera = Camera.current;
            }
            doGeneration();
            vCenterOfDupeParent = oGlobalParent.transform.TransformPoint(Vector3.zero);
            bGenerationComplete = true;
        }

        bUseShadows = UseShadows;

        if (!bUseShadows)
        {
            if (listAllObjects.Count > 0)
            {
                foreach (var obj in listAllObjects)
                {
                    if (obj.GetComponent<MeshRenderer>() != null)
                    {
                        obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        obj.GetComponent<MeshRenderer>().receiveShadows = false;
                    }
                    if (obj.GetComponentsInChildren<MeshRenderer>().Length > 0)
                    {
                        foreach (var meshrenderer in obj.GetComponentsInChildren<MeshRenderer>())
                        {
                            meshrenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                            meshrenderer.receiveShadows = false;
                        }
                    }
                }
            }
        }

        if (bInPlayMode)
        {            
            //If the camera clip plane changes at runtime, reset it to the stored value
            if (cCamera.farClipPlane != fCameraFarClip)
            {
                cCamera.farClipPlane = fCameraFarClip;
            }

            //If the fog and sky color is changed at runtime, update it at runtime
            if (colorFogColor != FogColor)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = colorFogColor;
                RenderSettings.fog = false;
                colorFogColor = FogColor;
            }

            if (bWrapPlayer)
            {
                doPlayerWrap();
            }
        }
    }



    private void doGeneration()
    {
        if ((OtherEnvironmentToWrap.Length > 0) || (StartingEnvironmentToWrap != null))
        {
            //Set private variables to equal public variables
            bUseShadows = UseShadows;
            sPlayerTag = PlayerTag;
            oObjects = OtherEnvironmentToWrap;
            oStartingObject = StartingEnvironmentToWrap;
            iWorldSize = WorldSize;
            iAdditionalWorldSize = WorldAdditionalSize;
            vDistanceBetweenObjects = DistanceBetweenObjects;
            bWrapPlayer = InfiniteWrapping;
            colorFogColor = FogColor;
            fStaggerPositions = RandomPositionStrength;
            fStaggerRotationX = XRandomRotationStrength;
            fStaggerRotationY = YRandomRotationStrength;
            fStaggerRotationZ = ZRandomRotationStrength;
            fRandomScale = RandomScaleStrength;
            bGPUInstancing = EnableGPU_Instancing;
            bUseFog = UseFog;
            bEdgeOutline = EdgeOutline;
            bUseAdditionalSize = UseAdditionalSize;
            sOtherTags = AdditionalTags;
            bWrapStartingEnviro = WrapStartingEnvironment;
            //Set camera far clip plane to match world size
            if (!bUseAdditionalSize) { fCameraFarClip = iWorldSize; }
            else { fCameraFarClip = iWorldSize + iAdditionalWorldSize; }

            cCamera.farClipPlane = fCameraFarClip;


            if (GameObject.FindGameObjectWithTag(sPlayerTag) != null)
            {
                oPlayer = GameObject.FindGameObjectWithTag(sPlayerTag);
            }

            //Find out if camera is a child of the player
            if (oPlayer != null)
            {
                if (oPlayer.transform.GetComponentInChildren<Camera>() != null)
                {
                    bCameraIsChildOfPlayer = true;
                }
                else
                {
                    if (cCamera.transform.root.gameObject != cCamera.transform.gameObject)
                    {
                        oCameraRoot = cCamera.transform.root.gameObject;
                    }
                }
            }



            if (bUseAdditionalSize) { iWorldSize = WorldSize + iAdditionalWorldSize; }
            else { iWorldSize = WorldSize; }

            //Edge detect depth normals
            if (bEdgeOutline)
            {
                if (cCamera.GetComponent<UnityStandardAssets.ImageEffects.EdgeDetection>() != null)
                {
                    cEdgDet = cCamera.gameObject.GetComponent<UnityStandardAssets.ImageEffects.EdgeDetection>();
                }
                else if (cCamera.GetComponent<UnityStandardAssets.ImageEffects.EdgeDetection>() == null)
                {
                    cEdgDet = cCamera.gameObject.AddComponent<UnityStandardAssets.ImageEffects.EdgeDetection>();
                }
                cEdgDet.mode = UnityStandardAssets.ImageEffects.EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals;
                cEdgDet.sensitivityDepth = 1;
                cEdgDet.sensitivityNormals = 1;
                cEdgDet.sampleDist = .5f;
                cEdgDet.enabled = false;
                cEdgDet.enabled = true;                
            }

            //If fog script is not on camera, add to camera, etc fog
            if (bUseFog)
            {
                if (cCamera.GetComponent<UnityStandardAssets.ImageEffects.GlobalFogWrap>() != null)
                {
                    cFog = cCamera.GetComponent<UnityStandardAssets.ImageEffects.GlobalFogWrap>();
                }
                else if (cCamera.GetComponent<UnityStandardAssets.ImageEffects.GlobalFogWrap>() == null)
                {
                    cFog = cCamera.gameObject.AddComponent<UnityStandardAssets.ImageEffects.GlobalFogWrap>();
                }
                cFog.heightFog = false;
                cFog.excludeFarPixels = false;
                cFog.useRadialDistance = true;                
                RenderSettings.fog = true;
                RenderSettings.fogColor = colorFogColor;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogEndDistance = fCameraFarClip;
                RenderSettings.fogStartDistance = 10;
                RenderSettings.fog = false;
            }

            //Add each object to the list of objects            
            if (oStartingObject != null)
            {
                listAllObjects.Add(oStartingObject);
            }

            if (oObjects.Length > 0)
            {
                foreach (GameObject obj in oObjects)
                {
                    if (obj != null)
                    {
                        obj.transform.eulerAngles = Vector3.zero;
                        listAllObjects.Add(obj);
                    }
                }
            }

            //Get the starting location                
            var bGotStartingBounds = false;
            if (StartingEnvironmentToWrap != null)
            {
                if (StartingEnvironmentToWrap.GetComponent<MeshRenderer>() != null)
                {                    
                    boundsStartingBounds = StartingEnvironmentToWrap.GetComponent<MeshRenderer>().bounds;
                    bGotStartingBounds = true;
                }
                if (StartingEnvironmentToWrap.GetComponentsInChildren<MeshRenderer>().Length > 0)
                {
                    var StartingMeshes = StartingEnvironmentToWrap.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in StartingMeshes)
                    {
                        if (!bGotStartingBounds)
                        {
                            boundsStartingBounds = mesh.bounds;
                            bGotStartingBounds = true;
                        }
                        else
                        {
                            boundsStartingBounds.Encapsulate(mesh.bounds);
                            Debug.Log(boundsStartingBounds + " Starting bounds");
                        }
                    }
                }
            }

            //Get a count of all objects
            iObjectsCount = listAllObjects.Count;

            //Add each mesh renderer bounds to the list            
            if (listAllObjects.Count > 0)
            {
                foreach (var obj in listAllObjects)
                {                    
                    var bGotFirst = false;

                    if (obj != null)
                    {
                        if (obj.GetComponent<MeshRenderer>() != null)
                        {
                            if (bGPUInstancing)
                            {
                                foreach (var mat in obj.GetComponent<MeshRenderer>().sharedMaterials)
                                {
                                    mat.enableInstancing = true;
                                }
                            }                            
                            if (!bGotFirst)
                            {
                                boundsEncapsulatedIndividual = obj.GetComponent<MeshRenderer>().bounds;
                                bGotFirst = true;
                            }                                
                        }

                        if (obj.GetComponentsInChildren<MeshRenderer>().Length > 0)
                        {
                            var cMeshes = obj.GetComponentsInChildren<MeshRenderer>();
                            foreach (var meshrenderer in cMeshes)
                            {
                                if (bGPUInstancing)
                                {
                                    foreach (var mat in meshrenderer.sharedMaterials)
                                    {
                                        mat.enableInstancing = true;
                                    }
                                }
                                if (!bGotFirst)
                                {
                                    boundsEncapsulatedIndividual = meshrenderer.bounds;
                                    bGotFirst = true;
                                }
                                else
                                {
                                    boundsEncapsulatedIndividual.Encapsulate(meshrenderer.bounds);
                                }                                
                            }
                        }
                        if (bGotFirst)
                        {
                            listBounds.Add(boundsEncapsulatedIndividual);
                        }
                    }
                }
            }

            //Get the greatest bounds for each axis
            if (listBounds.Count > 0)
            {
                var bBoxSizeStarted = false;
                foreach (var bounds in listBounds)
                {
                    if (!bBoxSizeStarted)
                    {
                        vBoxSize = bounds.size;
                        bBoxSizeStarted = true;
                    }
                    else
                    {
                        if (bounds.size.x > vBoxSize.x) { vBoxSize.x = bounds.size.x; }
                        if (bounds.size.y > vBoxSize.y) { vBoxSize.y = bounds.size.y; }
                        if (bounds.size.z > vBoxSize.z) { vBoxSize.z = bounds.size.z; }
                    }
                }
                Debug.Log(vBoxSize + "box size");

                //If the mesh bounds are too small, increase the buffer, otherwise it will instantiate too many
                var fBufferBasedOnCamClip = fCameraFarClip * .025f;

                if (vBoxSize.x + vDistanceBetweenObjects.x < fBufferBasedOnCamClip)
                { vDistanceBetweenObjects.x = fBufferBasedOnCamClip - vBoxSize.x; vDistanceBetweenObjects.x = fBufferBasedOnCamClip - vBoxSize.x; }
                if (vBoxSize.y + vDistanceBetweenObjects.y < fBufferBasedOnCamClip)
                { vDistanceBetweenObjects.y = fBufferBasedOnCamClip - vBoxSize.y; vDistanceBetweenObjects.y = fBufferBasedOnCamClip - vBoxSize.y; }
                if (vBoxSize.z + vDistanceBetweenObjects.z < fBufferBasedOnCamClip)
                { vDistanceBetweenObjects.z = fBufferBasedOnCamClip - vBoxSize.z; vDistanceBetweenObjects.z = fBufferBasedOnCamClip - vBoxSize.z; }


                //Add the distance between objects to the box size
                vBoxSize.x += vDistanceBetweenObjects.x;
                vBoxSize.y += vDistanceBetweenObjects.y;
                vBoxSize.z += vDistanceBetweenObjects.z;
                
                //The starting point for object duping
                var vStartPoint = boundsStartingBounds.center;

                //Calculate how many duplicates should be created.  This is relative to the camera's clip so that the edge of the grid is never seen.
                var fMaxDupeSizeLateralX = ((fCameraFarClip * 1.5f / vBoxSize.x) + 1) * vBoxSize.x;
                var fMaxDupeSizeLateralY = ((fCameraFarClip * 1.5f / vBoxSize.y) + 1) * vBoxSize.y;
                var fMaxDupeSizeLateralZ = ((fCameraFarClip * 1.5f / vBoxSize.z) + 1) * vBoxSize.z;
                

                //Calculate half the size of the maximum lateral size, for easier parenting at 0,0,0
                var fHalfDupeSizeLateralX = fMaxDupeSizeLateralX / 2;
                var fHalfDupeSizeLateralY = fMaxDupeSizeLateralY / 2;
                var fHalfDupeSizeLateralZ = fMaxDupeSizeLateralZ / 2;

                var bCountFinished1 = false;
                var bCountFinished2 = false;

                
                
                //Create the grid, account for starting point
                for (var x = vStartPoint.x; x < (fMaxDupeSizeLateralX + vStartPoint.x); x += vBoxSize.x)
                {
                    iLateralDupeCountInQuadrantX += 1;
                    for (var y = vStartPoint.y; y < (fMaxDupeSizeLateralY + vStartPoint.y); y += vBoxSize.y)
                    {
                        if (!bCountFinished1) { iLateralDupeCountInQuadrantY += 1; }
                        for (var z = vStartPoint.z; z < (fMaxDupeSizeLateralZ + vStartPoint.z); z += vBoxSize.z)
                        {                           
                            if (!bCountFinished2) { iLateralDupeCountInQuadrantZ += 1; }
                            doInstantiateObject(x, y, z);
                        }
                        bCountFinished2 = true;
                    }
                    bCountFinished1 = true;
                }

                //Get the total size of a grid quadrant
                fLateralQuadrantSizeX = vBoxSize.x * iLateralDupeCountInQuadrantX;
                fLateralQuadrantSizeY = vBoxSize.y * iLateralDupeCountInQuadrantY;
                fLateralQuadrantSizeZ = vBoxSize.z * iLateralDupeCountInQuadrantZ;
                var fHalfQuadrantSizeX = fLateralQuadrantSizeX * .5f;
                var fHalfQuadrantSizeY = fLateralQuadrantSizeY * .5f;
                var fHalfQuadrantSizeZ = fLateralQuadrantSizeZ * .5f;


                //Parent all objects in the first quadrant to an empty object
                var oQuadrantParent0 = new GameObject("Quadrant Parent 0");
                var vHalfBoxSize = vBoxSize * .5f;                
                oQuadrantParent0.transform.position = new Vector3(vStartPoint.x + fHalfQuadrantSizeX - vHalfBoxSize.x, vStartPoint.y + fHalfQuadrantSizeY - vHalfBoxSize.y, vStartPoint.z + fHalfQuadrantSizeZ - vHalfBoxSize.z);
                foreach (var quadrantobject in listQuadrantObjects)
                {
                    quadrantobject.transform.SetParent(oQuadrantParent0.transform);
                }

                //Duplicate quadrants and position them in the grid, using offset from starting object
                var vOSet = oQuadrantParent0.transform.position;       //U1     
                var oQuadrantParent1 = Instantiate(oQuadrantParent0, vOSet - new Vector3(fLateralQuadrantSizeX, 0, fLateralQuadrantSizeZ), Quaternion.identity);//(-1, 0, -1) U3
                var oQuadrantParent2 = Instantiate(oQuadrantParent0, vOSet - new Vector3(fLateralQuadrantSizeX, 0, 0), Quaternion.identity);//(-1, 0, 0) U4
                var oQuadrantParent3 = Instantiate(oQuadrantParent0, vOSet - new Vector3(fLateralQuadrantSizeX, fLateralQuadrantSizeY, 0), Quaternion.identity);//(-1, -1, 0)  D8

                var oQuadrantParent4 = Instantiate(oQuadrantParent0, vOSet - new Vector3(0, fLateralQuadrantSizeY, fLateralQuadrantSizeZ), Quaternion.identity); //(0, -1, -1) D6 
                var oQuadrantParent5 = Instantiate(oQuadrantParent0, vOSet - new Vector3(0, 0, fLateralQuadrantSizeZ), Quaternion.identity);//(0, 0, -1) U2
                var oQuadrantParent6 = Instantiate(oQuadrantParent0, vOSet - new Vector3(fLateralQuadrantSizeX, fLateralQuadrantSizeY, fLateralQuadrantSizeZ), Quaternion.identity);//(1, 1, 1) 
                var oQuadrantParent7 = Instantiate(oQuadrantParent0, vOSet - new Vector3(0, fLateralQuadrantSizeY, 0), Quaternion.identity);//(0, -1, 0)  D5         

                //Create a parent for all quadrants
                oGlobalParent = new GameObject("Wrap Parent");
                oGlobalParent.transform.position = vOSet - new Vector3(fHalfQuadrantSizeX, fHalfQuadrantSizeY, fHalfQuadrantSizeZ);

                //Set all quadrants as child of the parent
                oQuadrantParent0.transform.parent = oGlobalParent.transform;
                oQuadrantParent1.transform.parent = oGlobalParent.transform;
                oQuadrantParent2.transform.parent = oGlobalParent.transform;
                oQuadrantParent3.transform.parent = oGlobalParent.transform;
                oQuadrantParent4.transform.parent = oGlobalParent.transform;
                oQuadrantParent5.transform.parent = oGlobalParent.transform;
                oQuadrantParent6.transform.parent = oGlobalParent.transform;
                oQuadrantParent7.transform.parent = oGlobalParent.transform;

                //If the originals are in the scene, disable the originals                
                foreach (GameObject obj in oObjects)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }


    private void doInstantiateObject(float x, float y, float z)
    {
        //Instantate one object at random from the list, unless it's the firt one
        if ((!bFirstRun) && (oStartingObject != null))
        {            
            int iIndexStart;
            if (StartingEnvironmentToWrap != null)
            {
                if (bWrapStartingEnviro)
                {
                    iIndexStart = 0;
                }
                else
                {
                    iIndexStart = 1;
                }
            }
            else
            {
                iIndexStart = 0;
            }
            var iRandIndex = Random.Range(iIndexStart, listAllObjects.Count);
            var oNewDuplicate = Instantiate(listAllObjects[iRandIndex]);

            //Get the bounds of this object, to get the center point, for offset.  Just in case the object is parented with offset.                            
            var bGotNewDuplicateFirstBounds = false;
            if (oNewDuplicate.GetComponent<MeshRenderer>() != null)
            { boundsNewDuplicateEncapsulated = oNewDuplicate.GetComponent<MeshRenderer>().bounds; bGotNewDuplicateFirstBounds = true; }
            if (oNewDuplicate.GetComponentsInChildren<MeshRenderer>().Length > 0)
            {
                foreach (var renderer in oNewDuplicate.GetComponentsInChildren<MeshRenderer>())
                {
                    if (bGotNewDuplicateFirstBounds)
                    {
                        boundsNewDuplicateEncapsulated.Encapsulate(renderer.bounds);
                    }
                    else
                    {
                        boundsNewDuplicateEncapsulated = renderer.bounds;
                        bGotNewDuplicateFirstBounds = true;
                    }
                }
            }

            //Create a parent for the new dupe, center it on the new dupe's mesh bounds
            var oNewDuplicateParent = new GameObject("Dupe");
            oNewDuplicateParent.transform.position = boundsNewDuplicateEncapsulated.center;
            oNewDuplicate.transform.parent = oNewDuplicateParent.transform;

            //Add some randomness to the position                            
            var fRandPosX = Random.Range(fStaggerPositions * vBoxSize.x * .5f * -1, fStaggerPositions * vBoxSize.x * .5f);
            var fRandPosY = Random.Range(fStaggerPositions * vBoxSize.y * .5f * -1, fStaggerPositions * vBoxSize.y * .5f);
            var fRandPosZ = Random.Range(fStaggerPositions * vBoxSize.z * .5f * -1, fStaggerPositions * vBoxSize.z * .5f);
            oNewDuplicateParent.transform.position = new Vector3(x + fRandPosX, y + fRandPosY, z + fRandPosZ);

            //Add some randomness to the rotation
            var fRandRotX = Random.Range(-fStaggerRotationX, fStaggerRotationX);
            var fRandRotY = Random.Range(-fStaggerRotationY, fStaggerRotationY);
            var fRandRotZ = Random.Range(-fStaggerRotationZ, fStaggerRotationZ);
            oNewDuplicateParent.transform.eulerAngles = new Vector3(360 * fRandRotX, 360 * fRandRotY, 360 * fRandRotZ);

            //Add some randomness to the scale            
            var vCurrentScale = oNewDuplicateParent.transform.localScale;
            var fRandScaleX = Random.Range(1 - fRandomScale * .8f, vCurrentScale.x);
            var fRandScaleY = Random.Range(1 - fRandomScale * .8f, vCurrentScale.y);
            var fRandScaleZ = Random.Range(1 - fRandomScale * .8f, vCurrentScale.z);
            oNewDuplicateParent.transform.localScale = new Vector3(fRandScaleX, fRandScaleY, fRandScaleZ);

            //Add the newly instantiated object to the list of all objects in this quadrant
            listQuadrantObjects.Add(oNewDuplicateParent);
        }
        //If it's the first object, skip instantiation, and instead just add the starting object to the list of objects
        else if ((bFirstRun) && (oStartingObject != null))
        {
            var oNewDuplicateParent = new GameObject("Dupe");
            oNewDuplicateParent.transform.position = boundsStartingBounds.center;
            oStartingObject.transform.parent = oNewDuplicateParent.transform;
            listQuadrantObjects.Add(oNewDuplicateParent);
            bFirstRun = false;
        }
    }

    //=================================================================
    //           Player wrapping and Other Tag Wrapping
    //=================================================================
    private bool bOk;

    private void doPlayerWrap()
    {
        if (oPlayer != null)
        {
            var vPlayerPos = oGlobalParent.transform.InverseTransformPoint(oPlayer.transform.position);
            var vCameraPos = oGlobalParent.transform.InverseTransformPoint(cCamera.transform.position);

            var vCameraPosGlobal = cCamera.transform.position;
            var vCameraPosLocalInPlayer = oPlayer.transform.InverseTransformPoint(vCameraPosGlobal);
            var vCameraRootPosGlobal = Vector3.zero;
            var vCameraRootPosLocalInPlayer = Vector3.zero;

            if (bCameraIsChildOfOther)
            {
                vCameraRootPosGlobal = oCameraRoot.transform.position;
                vCameraRootPosLocalInPlayer = oPlayer.transform.InverseTransformPoint(vCameraRootPosGlobal);
            }


            //Check if should wrap player by distance, then do wrap player
            if ((Mathf.Abs(vPlayerPos.x) > fMaxDistanceX) && (Mathf.Abs(vCameraPos.x) > fMaxDistanceX))
            {
                var fPlayerPosAbsX = Mathf.Abs(vPlayerPos.x);
                var fMultiplier = vPlayerPos.x / fPlayerPosAbsX;
                fMultiplier *= -1;
                var fPlayerNewPosX = (iLateralDupeCountInQuadrantX * vBoxSize.x) - fPlayerPosAbsX;//
                oPlayer.transform.position = oGlobalParent.transform.TransformPoint(new Vector3((fPlayerNewPosX * fMultiplier), vPlayerPos.y, vPlayerPos.z));
                if (bCameraIsChildOfOther)
                {
                    oCameraRoot.transform.position = oPlayer.transform.TransformPoint(vCameraRootPosLocalInPlayer);
                }
                cCamera.transform.position = oPlayer.transform.TransformPoint(vCameraPosLocalInPlayer);
                Debug.Log($"Wrap axis X");
            }
            else if ((Mathf.Abs(vPlayerPos.y) > fMaxDistanceY) && (Mathf.Abs(vCameraPos.y) > fMaxDistanceY))
            {
                var fPlayerPosAbsY = Mathf.Abs(vPlayerPos.y);
                var fMultiplier = vPlayerPos.y / fPlayerPosAbsY;
                fMultiplier *= -1;
                var fPlayerNewPosY = (iLateralDupeCountInQuadrantY * vBoxSize.y) - fPlayerPosAbsY;//
                oPlayer.transform.position = oGlobalParent.transform.TransformPoint(new Vector3(vPlayerPos.x, (fPlayerNewPosY * fMultiplier), vPlayerPos.z));
                if (bCameraIsChildOfOther)
                {
                    oCameraRoot.transform.position = oPlayer.transform.TransformPoint(vCameraRootPosLocalInPlayer);
                    cCamera.transform.position = oPlayer.transform.TransformPoint(vCameraPosLocalInPlayer);
                }
                Debug.Log($"Wrap axis Y");
            }
            else if ((Mathf.Abs(vPlayerPos.z) > fMaxDistanceZ) && (Mathf.Abs(vCameraPos.z) > fMaxDistanceZ))
            {
                var fPlayerPosAbsZ = Mathf.Abs(vPlayerPos.z);
                var fMultiplier = vPlayerPos.z / fPlayerPosAbsZ;
                fMultiplier *= -1;            
                var fPlayerNewPosZ = (iLateralDupeCountInQuadrantZ * vBoxSize.z) - fPlayerPosAbsZ;//
                oPlayer.transform.position = oGlobalParent.transform.TransformPoint(new Vector3(vPlayerPos.x, vPlayerPos.y, (fPlayerNewPosZ * fMultiplier)));
                if (bCameraIsChildOfOther)
                {
                    oCameraRoot.transform.position = oPlayer.transform.TransformPoint(vCameraRootPosLocalInPlayer);
                    cCamera.transform.position = oPlayer.transform.TransformPoint(vCameraPosLocalInPlayer);
                }
                Debug.Log($"Wrap axis Z");
            }

            //Wrap other objects ---------------------------------------
            if (sOtherTags.Length > 0)
            {
                foreach (var tag in sOtherTags)
                {
                    foreach (var othertag in GameObject.FindGameObjectsWithTag("tag"))
                    {
                        if (othertag != null)
                        {
                            var vOtherTagPos = oGlobalParent.transform.InverseTransformPoint(othertag.transform.position);

                            if (Mathf.Abs(vOtherTagPos.x) > fMaxDistanceX)
                            {
                                var fOtherTagAbsX = Mathf.Abs(vOtherTagPos.x);
                                var fMultiplier = vOtherTagPos.x / fOtherTagAbsX;
                                fMultiplier *= -1;
                                var fOtherTagNewPosX = (iLateralDupeCountInQuadrantX * vBoxSize.x) - fOtherTagAbsX;
                                othertag.transform.position = oGlobalParent.transform.TransformPoint(new Vector3((fOtherTagNewPosX * fMultiplier), vOtherTagPos.y, vOtherTagPos.z));
                            }
                            else if (Mathf.Abs(vOtherTagPos.y) > fMaxDistanceY)
                            {
                                var fOtherTagAbsY = Mathf.Abs(vOtherTagPos.y);
                                var fMultiplier = vOtherTagPos.y / fOtherTagAbsY;
                                fMultiplier *= -1;
                                var fOtherTagNewPosY = (iLateralDupeCountInQuadrantY * vBoxSize.y) - fOtherTagAbsY;
                                othertag.transform.position = oGlobalParent.transform.TransformPoint(new Vector3(vOtherTagPos.x, (fOtherTagNewPosY * fMultiplier), vOtherTagPos.z));
                            }
                            else if (Mathf.Abs(vOtherTagPos.z) > fMaxDistanceZ)
                            {
                                var fOtherTagAbsZ = Mathf.Abs(vOtherTagPos.z);
                                var fMultiplier = vOtherTagPos.z / fOtherTagAbsZ;
                                fMultiplier *= -1;
                                var fOtherTagNewPosZ = (iLateralDupeCountInQuadrantZ * vBoxSize.z) - fOtherTagAbsZ;
                                othertag.transform.position = oGlobalParent.transform.TransformPoint(new Vector3(vOtherTagPos.x, vOtherTagPos.y, (fOtherTagNewPosZ * fMultiplier)));
                            }
                        }
                    }
                }
            }
        }
    }
}

        