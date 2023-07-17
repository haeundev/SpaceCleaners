using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GogoGaga.UHM
{
    public class UltimateHudManager : MonoBehaviour
    {
        public static UltimateHudManager Instance { get; private set; }

        [Header("Panel Refrences")]
        public MissionPanelManager missionPanelManager;

        public SkillPanelManager skillPanelManager;

        public PrimaryNotificationPanel primaryNotificationPanel;

        public SecondaryNotificationManager secondaryNotificationManager;


        public InputTooltipManager inputTooltipManager;

        public PickupNotificationManager pickupNotificationManager;

        public HealthBarManager healthBarManager;

        public CrossHairManager crossHairManager;

        public WaypointPanelManager waypointPanelManager;

        public CompassPanelManager compassPanelManager;

        public ScreenEffectPanelManager screenEffectPanel;

        public CanvasGroup fadeToColorPanel;

        public ProgressToolTipManager progressToolTipManager;

        public FocusOnObjectManager focusOnObjectManager;

        [Header("Others")]
        public InputIconManager inputIconManager;
        
        [Header("Defaults")]
        [SerializeField] bool _defaultState_compassPanel = false;

        public Camera mainCam { get;  set; }
        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion


            if (missionPanelManager)
                missionPanelManager.gameObject.SetActive(false);

            if (skillPanelManager)
                skillPanelManager.gameObject.SetActive(false);

            if (primaryNotificationPanel)
                primaryNotificationPanel.gameObject.SetActive(false);

            if (secondaryNotificationManager)
                secondaryNotificationManager.gameObject.SetActive(false);

            if (inputIconManager)
                inputIconManager.gameObject.SetActive(false);

            if (inputTooltipManager)
                inputTooltipManager.gameObject.SetActive(false);

            if (pickupNotificationManager)
                pickupNotificationManager.gameObject.SetActive(false);

            if (healthBarManager)
                healthBarManager.gameObject.SetActive(false);

            if (crossHairManager)
                crossHairManager.gameObject.SetActive(false);

            if (screenEffectPanel)
                screenEffectPanel.gameObject.SetActive(false);

            if (compassPanelManager)
                compassPanelManager.gameObject.SetActive(_defaultState_compassPanel);

            if(progressToolTipManager)
                progressToolTipManager.gameObject.SetActive(false);

            if(focusOnObjectManager != null)
                focusOnObjectManager.gameObject.SetActive(false);

            mainCam = Camera.main;
        }

        void Start()
        {
            
        }







        #region Missions



        public void CreateMission(string missonTitle)
        {
            CreateMission(missonTitle, new TaskData[0]);
        }

        public void CreateMission(string missonTitle, TaskData[] taskDatas)
        {
            if (missionPanelManager == null)
                return;

            if (!missionPanelManager.gameObject.activeInHierarchy)
                missionPanelManager.gameObject.SetActive(true);

            missionPanelManager.CreateMission(missonTitle, taskDatas);
        }

        public void CreateMission(string missonTitle, string[] tasks)
        {
            if (missionPanelManager == null)
                return;

            TaskData[] taskDatas = new TaskData[tasks.Length];

            for (int i = 0; i < taskDatas.Length; i++)
            {
                taskDatas[i] = new TaskData(tasks[i]);
            }

            if (!missionPanelManager.gameObject.activeInHierarchy)
                missionPanelManager.gameObject.SetActive(true);

            missionPanelManager.CreateMission(missonTitle, taskDatas);
        }


        public void AddMissionTask(string taskText)
        {
            AddMissionTask(new TaskData(taskText, TaskProgressType.pending));
        }

        public void AddMissionTask(string[] taskText)
        {
            TaskData[] ts = new TaskData[taskText.Length];

            for (int i = 0; i < taskText.Length; i++)
            {
                ts[i] = new TaskData(taskText[i], TaskProgressType.pending);
            }

            AddMissionTask(ts);
        }

        public void AddMissionTask(TaskData taskData)
        {
            if (missionPanelManager == null)
                return;

            missionPanelManager.AddTask(taskData);
        }
        public void AddMissionTask(TaskData[] taskData)
        {
            if (missionPanelManager == null)
                return;

            missionPanelManager.AddTask(taskData);
        }




        public void RemoveMissionTask(int Index)
        {
            if (missionPanelManager == null)
                return;

            missionPanelManager.RemoveTask(Index);
        }


        public void ChangeTaskStatus(TaskProgressType NewType,
                                        int Index = 0)
        {
            if (missionPanelManager == null)
                return;

            missionPanelManager.ChangeTaskStatus(NewType, Index);
        }


        public void ShowMissions()
        {
            if (missionPanelManager == null)
                return;
            missionPanelManager.gameObject.SetActive(true);
            missionPanelManager.Show();
        }

        public void HideMission(bool clearMissionData = false)
        {

            if (missionPanelManager == null)
                return;


            if (!missionPanelManager.gameObject.activeInHierarchy)
                return;

            missionPanelManager.show = false;

            if (clearMissionData)
                missionPanelManager.RemoveMission();
            else
                missionPanelManager.Hide();

        }



        #endregion






        #region Skills

        

        public void CreateSkillBar(SkillProgressProperties newSkillData, bool ShowAfterCreating = false)
        {
            if (skillPanelManager == null)
                return;

            if (ShowAfterCreating)
                skillPanelManager.gameObject.SetActive(true);

            skillPanelManager.CreateSkillNotification(newSkillData, ShowAfterCreating);
        }

        public void AddValueSkillProgress(string title, int add)
        {
            ShowSkillProgress(title, (int)GetSkillProgressValue(title) + add);
        }

        public void ShowSkillProgress(string title, int newVal = -1)
        {
            if (skillPanelManager == null)
                return;

            if (!skillPanelManager.gameObject.activeInHierarchy)
                skillPanelManager.gameObject.SetActive(true);

            skillPanelManager.ShowSkillNotification(title, newVal);
        }

        public void UpdateSkillProgressData(SkillProgressProperties newData,
                                            bool ShowAfterUpdating = true)
        {
            if (skillPanelManager == null)
                return;

            skillPanelManager.UpdateSkillProgressData(newData, ShowAfterUpdating);
        }


        public float GetSkillProgressValue(string title)
        {
            if(skillPanelManager == null)
                return 0;

            return skillPanelManager.GetValue(title);
        }


        public void RemoveSkillProgress(string Title)
        {
            if (skillPanelManager == null)
                return;

            skillPanelManager.RemoveSkillProgress(Title);
        }

        public void RemoveAllSkillProgress()
        {
            if (skillPanelManager == null)
                return;

            skillPanelManager.RemoveAllSkillProgress();
        }

        #endregion






        #region Primary Notification


        public void CreatePrimaryNotification(string topTitleText, string bottomTitleText)
        {
            CreatePrimaryNotification(topTitleText, bottomTitleText, 2, 0, Color.white, Color.white);
        }

        public void CreatePrimaryNotification(
                    string topTitleText, 
                    string bottomTitleText, 
                    float StayTime)
        {
            CreatePrimaryNotification(topTitleText, bottomTitleText, StayTime, 0, Color.white, Color.white);
        }

        public void CreatePrimaryNotification(
                    string topTitleText,
                    string bottomTitleText,
                    float StayTime,
                    float initialDelay)
        {
            CreatePrimaryNotification(topTitleText, bottomTitleText, StayTime, initialDelay, Color.white, Color.white);
        }

        public void CreatePrimaryNotification(
                    string topTitleText, string bottomTitleText,
                    float StayTime, float initialDelay,
                    Color topTextColor, Color bottomTextColor)
        {
            if (primaryNotificationPanel == null)
                return;

            if (ShowPrimaryNotifCorotuine != null)
                StopCoroutine(ShowPrimaryNotifCorotuine);

            ShowPrimaryNotifCorotuine = StartCoroutine(ShowingPrimaryNotif(
                topTitleText,bottomTitleText,StayTime,
                initialDelay,topTextColor,bottomTextColor));

        }

        Coroutine ShowPrimaryNotifCorotuine;
        IEnumerator ShowingPrimaryNotif(string topTitleText, string bottomTitleText, float StayTime, float initialDelay, Color topTextColor, Color bottomTextColor)
        {
            primaryNotificationPanel.gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();

            yield return new WaitForSeconds(initialDelay);

            primaryNotificationPanel.gameObject.SetActive(true);
            primaryNotificationPanel.CreatePrimaryNotification(topTitleText, bottomTitleText, topTextColor, bottomTextColor);
            primaryNotificationPanel.Show();

            yield return new WaitForSeconds(StayTime);
            primaryNotificationPanel.Hide();
            yield return new WaitForSeconds(2f); 
            primaryNotificationPanel.gameObject.SetActive(false);
        }
        #endregion






        #region Secondary Notification

        public void CreateSecondaryNotification( string topText, 
                    string bottomtext, float AutoHideDelay)
        {
            CreateSecondaryNotification(topText, bottomtext, "", AutoHideDelay, null, Color.yellow, Color.white);
        }


        public void CreateSecondaryNotification(string topText,
                    string bottomtext, string tooltip = "", Sprite icon = null)
        {
            CreateSecondaryNotification(topText, bottomtext, tooltip, 0, icon, Color.yellow, Color.white);
        }


        public void CreateSecondaryNotification(string topText, string bottomtext,
                    string tooltip, float AutoHideDelay, Sprite icon = null)
        {
            CreateSecondaryNotification(topText, bottomtext, tooltip, AutoHideDelay, icon, Color.yellow, Color.white);
        }


        public void CreateSecondaryNotification( string topText,
                    string bottomtext, string toolTip, float AutoHidedelay,
                    Sprite icon, Color topTextColor, Color bottomTextColor)
        {
            if (secondaryNotificationManager == null)
                return;

            secondaryNotificationManager.gameObject.SetActive(true);

            secondaryNotificationManager.Create(topText, bottomtext, toolTip, AutoHidedelay, icon, topTextColor, bottomTextColor);
        }


        public void HideSecondaryNotification(float delay = 0)
        {
            if (secondaryNotificationManager == null)
                return;

            if (!secondaryNotificationManager.gameObject.activeInHierarchy)
                return;

            secondaryNotificationManager.Hide(delay);
        }



        #endregion






        #region Input tooltip

        public void ShowInputTooltipText(string preText, KeyCode key,
                        string postText , float AutoHideAfter = -1)
        {
            ShowInputTooltipText(preText + GetInputIconCode(key) + postText , AutoHideAfter);
        }

        public void ShowInputTooltipText(string text ,
                                float AutoHideAfter = -1)
        {
            if (inputTooltipManager == null)
                return;

            inputTooltipManager.gameObject.SetActive(true);

            inputTooltipManager.Set(text);

            if(AutoHideAfter > 0)
            {
                inputTooltipManager.Hide(AutoHideAfter);
            }
        }


        public void HideInputTooltip(float delay = 0)
        {
            if (inputTooltipManager == null)
                return;


            if (inputTooltipManager.gameObject.activeInHierarchy)
                inputTooltipManager.Hide(delay);
        }


        #endregion






        #region Input Functions

        public string GetInputIconCode(KeyCode keycode)
        {
            if (inputIconManager == null)
                return "";

            return inputIconManager.GetIconCode(keycode);
        }




        #endregion






        #region Pickup notification

        public void CreatePickupNotification(string text,
                    float delay = 0, float stayTime = 4)
        {
            CreatePickupNotification(text, null, delay,stayTime);
        }


        public void CreatePickupNotification(string text,
                            Sprite icon = null, float delay = 0, float stayTime = 4)
        {
            if (pickupNotificationManager == null)
                return;

            pickupNotificationManager.gameObject.SetActive(true);

            pickupNotificationManager.Create(text, icon, delay);
        }

        #endregion






        #region Damage and Heal Effect

        public void ShowHealEffect(bool loop = false)
        {
            ShowHealEffect(0, loop);
        }

        public void ShowHealEffect(float startingDelay, bool loop)
        {
            if (loop)
            {
                ShowHealEffect(startingDelay, -1);
            }
            else
            {
                ShowHealEffect(startingDelay, 1f);
            }
        }

        public void ShowHealEffect(float startingDelay, float displayTime = 0.2f)
        {
            if (screenEffectPanel == null)
                return;

            screenEffectPanel.gameObject.SetActive(true);

            screenEffectPanel.ShowHealEffect(startingDelay, displayTime);
        }


        public void ShowDamageEffect(bool loop = false)
        {
            ShowDamageEffect(0, loop);
        }

        public void ShowDamageEffect(float startingDelay, bool loop)
        {
            if (loop)
            {
                ShowDamageEffect(startingDelay, -1);
            }
            else
            {
                ShowDamageEffect(startingDelay, 0.2f);
            }
        }

        public void ShowDamageEffect(float startingDelay, float displayTime = 0.2f)
        {
            if (screenEffectPanel == null)
                return;

            screenEffectPanel.gameObject.SetActive(true);

            screenEffectPanel.ShowDamageEffect(startingDelay, displayTime);
        }

        public void HideDamageHealEffect()
        {
            if (screenEffectPanel == null)
                return;

            screenEffectPanel.HideEffect();
        }



        #endregion






        #region Health Bar



        public void CreateHealthBar(HeathData heathData)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.gameObject.SetActive(true);

            healthBarManager.Set(heathData);
        }

        public void AddHealth(float add)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.AddHealth(add);
        }

        public void SetHealth(float newHeatlh)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.SetHealth(newHeatlh);
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.SetMaxHeatlh(newMaxHealth);
        }

        public void SetExtraMaxHealth(float newExtraMaxHealth)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.SetExtraMaxHeatlh(newExtraMaxHealth);
        }

        public void AddExtraHealth(float add)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.AddExtraHealth(add);
        }

        public void SetExtraHealth(float newExtraHeatlh)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.SetExtraHealth(newExtraHeatlh);
        }

        public void SetHealthbarColor(Color newColor)
        {
            SetHealthbarColor(newColor , Color.blue);
        }

        public void SetExtraHealthbarColor(Color newColor)
        {
            SetHealthbarColor(Color.green, newColor);
        }

        public void SetHealthbarColor(Color upperBarColor,Color lowerBarColor)
        {
            if (healthBarManager == null)
                return;

            healthBarManager.SetColor(upperBarColor, lowerBarColor) ;
        }

        


        public void HideHealthBar()
        {
            if (healthBarManager == null)
                return;

            healthBarManager.Hide();
        }

        public void RemoveHealthBar()
        {
            if (healthBarManager == null)
                return;

            healthBarManager.Remove();
        }


        #endregion







        #region Cross hair


        public void CreateCrossHair(int CrossHairTypeFromAvaliable)
        {
            if (crossHairManager == null)
                return;

            crossHairManager.gameObject.SetActive(true);

            crossHairManager.Create(CrossHairTypeFromAvaliable);
        }

        public void CreateCrossHair(CrosshairGraphics crossHairPrefab)
        {
            if (crossHairManager == null)
                return;

            crossHairManager.gameObject.SetActive(true);

            crossHairManager.Create(crossHairPrefab);
        }

        public void SetCrossHairColor(Color newColor)
        {
            if (crossHairManager == null)
                return;


            crossHairManager.SetColor(newColor);
        }

        public void SetCrossHairHitColor(Color newColor)
        {
            if (crossHairManager == null)
                return;


            crossHairManager.SetHitColor(newColor);
        }

        public void SetCrossHairColors(Color newColor,Color newHitColor)
        {
            if (crossHairManager == null)
                return;


            crossHairManager.SetColor(newColor);
            crossHairManager.SetHitColor(newHitColor);
        }


        public void ShowCrossHair()
        {
            if (crossHairManager == null)
                return;


            crossHairManager.Show();
        }


        public void ShootEffectCrossHair()
        {
            if (crossHairManager == null)
                return;

            crossHairManager.ShootEffect();
        }

        public void HighlightEffectCrossHair()
        {
            if (crossHairManager == null)
                return;

            crossHairManager.HighlightEffect();
        }

        public void IdleEffectCrossHair()
        {
            if (crossHairManager == null)
                return;

            crossHairManager.Idle();
        }

        public void HitEfectCrossHair()
        {
            if (crossHairManager == null)
                return;

            crossHairManager.HitEffect();
        }
        #endregion







        #region Waypoint

        public WaypointWorldMarker CreateWaypoint(Vector3 position)
        {
            WaypointMarkerProperties p = new WaypointMarkerProperties();

            return CreateWaypoint(p, position);

        }

        public WaypointWorldMarker CreateWaypoint(WaypointWorldMarker marker, Vector3 position)
        { 
            return CreateWaypoint(marker.WaypointProperties, position);
        }

        public WaypointWorldMarker CreateWaypoint(WaypointMarkerProperties properties, Vector3 position)
        {
            if (waypointPanelManager == null)
                return null;

            WaypointWorldMarker w = waypointPanelManager.CreateWaypoint(properties, position);

            w.waypointPanelManager = waypointPanelManager;
            w.compassPanelManager = compassPanelManager;

            return w;
        }

        public void RemoveAllWaypoints()
        {
            if (waypointPanelManager == null)
                return;

            waypointPanelManager.RemoveAll();
        }

        public void ShowCompass(bool show = true)
        {
            if (compassPanelManager == null)
                return;

            compassPanelManager.gameObject.SetActive(show);
        }



        #endregion








        #region Fade out to Color


        public void MoveFadeToBottom()
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.transform.SetAsFirstSibling();
        }

        public void MoveFadeToTop()
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.transform.SetAsLastSibling();
        }

        public void ToggleFade(float FadeDuration = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.GetComponent<Image>().color = Color.black;

            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(!lastFadeType, FadeDuration));
        }


        public void ToggleFade(Color color,float time = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.GetComponent<Image>().color = color;

            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(!lastFadeType, time));
        }


        public void FadeInToBlack(float time = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.GetComponent<Image>().color = Color.black;

            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(true, time));
        }


        public void FadeInToWhite(float time = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.GetComponent<Image>().color = Color.white;

            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(true, time));
        }


        public void FadeInToColor(Color fadeColor,float time = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;

            fadeToColorPanel.GetComponent<Image>().color = fadeColor;

            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(true, time));
        }


        public void FadeOut(float time = 0.3f)
        {
            if (fadeToColorPanel == null)
                return;


            if (fadeToColorCorortuine != null)
                StopCoroutine(fadeToColorCorortuine);

            fadeToColorCorortuine = StartCoroutine(FadingToColor(false, time));
        }


        Coroutine fadeToColorCorortuine;
        bool lastFadeType;
        IEnumerator FadingToColor(bool fadeIn , float fadeTime)
        {
            lastFadeType = fadeIn;
            float timer = 0;
            while(timer <  fadeTime)
            {
                yield return new WaitForEndOfFrame();
                timer += Time.deltaTime;

                if (fadeIn)
                    fadeToColorPanel.alpha = (timer / fadeTime);
                else
                    fadeToColorPanel.alpha = 1 - (timer / fadeTime);
            }
        }


        #endregion






        #region Progress ToolTip

        public void ShowProgressTooltip(float progressTime , string titleText,
                                         bool autoHideOnComplete = true)
        {
            if (progressToolTipManager == null)
                return;

            progressToolTipManager.gameObject.SetActive(true);

            progressToolTipManager.Show(progressTime, titleText,autoHideOnComplete);
        }

        public void HideProgressTooltip()
        {
            if (progressToolTipManager == null)
                return;

            if (progressToolTipManager.gameObject.activeInHierarchy)
                progressToolTipManager.Hide();
        }

        #endregion




        #region Focus On Gameobject

        public void CreateFocusPoint(Transform target)
        {
            FOCUS_PROPERTIES properties = new FOCUS_PROPERTIES();
            CreateFocusPoint(target, properties);
        }


        /// <param name="size">Size in screen space for the circle. Limit is 100-250</param>
        /// <param name="ShowTime">After that time effect will be disabled. Use -1 if you don't want to use this property</param>
        public void CreateFocusPoint(Transform target, FOCUS_TYPE mode,
                    float size = 150,float ShowTime = -1)
        {
            FOCUS_PROPERTIES properties = new FOCUS_PROPERTIES(ShowTime, size, mode);
            CreateFocusPoint(target, properties);
        }

        public void CreateFocusPoint(Vector3 targetPos, FOCUS_TYPE mode,
                    float size = 150, float ShowTime = -1)
        {
            FOCUS_PROPERTIES properties = new FOCUS_PROPERTIES(ShowTime, size, mode);
            CreateFocusPoint(targetPos, properties);
        }

        public void CreateFocusPoint( Transform target, FOCUS_PROPERTIES properties)
        { 
            if (focusOnObjectManager == null)
                return;

            focusOnObjectManager.gameObject.SetActive(true);
            focusOnObjectManager.Create(target,properties);
        }

        public void CreateFocusPoint(Vector3 targetPos, FOCUS_PROPERTIES properties)
        {
            if (focusOnObjectManager == null)
                return;

            focusOnObjectManager.gameObject.SetActive(true);
            focusOnObjectManager.Create(targetPos, properties);
        }
        public void StopFocusPoint()
        {
            if (focusOnObjectManager == null)
                return;

            focusOnObjectManager.Stop();
        }

        #endregion




        #region Others


        public static float RemapValue(float from, float fromMin, float fromMax, float toMin, float toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = fromAbs / fromMaxAbs;

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        public static float GetAngle(Vector3 a1, Vector3 a2, Vector3 n)
        {
            float angle = Vector3.Angle(a1, a2);
            float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a1, a2)));
            return angle * sign;
        }


        public static bool CheckIfMissingOrNull(UnityEngine.Object file)
        {
            try
            {
                var blarf = file.name;
            }
            catch (MissingReferenceException) // General Object like GameObject/Sprite etc
            {
                // Debug.LogError("The provided reference is missing!");
                return true;
            }
            catch (UnassignedReferenceException) // Specific for unassigned fields
            {
                //Debug.LogWarning("The provided reference is null!");
                return true;
            }
            catch (System.NullReferenceException) // Any other null reference like for local variables
            {
                //Debug.LogWarning("The provided reference is null!");
                return true;
            }

            return false;
        }



        public void SetMainCamera(Camera cam)
        {
            mainCam = cam;
        }

        #endregion




    }
}
