using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GogoGaga.UHM
{
    public class ExampleTrigger : MonoBehaviour
    {
        public TriggerType type;

        public Sprite _Image;
        public int Index;

        public Transform tr;

        public WaypointMarkerProperties Marker;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ExamplePlayerController>())
            {
                switch (type)
                {
                    case TriggerType.missions:
                        MissionStart();
                        break;

                    case TriggerType.skill:
                        SkillBarStart();
                    break;

                    case TriggerType.primaryNotif:
                        PrimaryNotifStart();
                        break;

                    case TriggerType.secondaryNotif:
                        SecondaryNotifStart();
                        break;

                    case TriggerType.pickupNotif:
                        PickupNotifStart();
                        break;

                    case TriggerType.inputTooltip:
                        InputTooltipStart();
                        break;

                    case TriggerType.crosshair:
                        ChangeCrossHair();
                        break;

                    case TriggerType.damage:
                        DamageEffectStart();
                        break;

                    case TriggerType.healthbarAdd:
                        HealthBarHeal();
                        break;

                    case TriggerType.waypoint:
                        WaypointStart();
                        break;

                    case TriggerType.healtbar:
                        HealtbarStart();
                        break;

                    case TriggerType.progressTooltip:
                        ProgressTooltipStart();
                        break;
                    
                    case TriggerType.FocusOnGameobject:
                        FocusObjectStart();
                        break;
                    default:
                        break;
                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<ExamplePlayerController>())
            {
                switch (type)
                {
                    case TriggerType.missions:
                        MissionEnd();
                        break;

                    case TriggerType.skill:
                        SkillBarEnd();
                        break;

                    case TriggerType.inputTooltip:
                        InputTooltipStop();
                        break;


                    case TriggerType.damage:
                        DamageEffectEnd();
                        break;

                    case TriggerType.waypoint:
                        WaypointEnd();
                        break;

                    case TriggerType.healtbar:
                        HealthbarEnd();
                        break;

                    case TriggerType.progressTooltip:
                        ProgressTooltipStop();
                        break;

                    case TriggerType.FocusOnGameobject:
                        FocusObjectEnd();
                        break;

                    default:
                        break;
                }
            }
        }

        void MissionStart()
        {
            TaskData[] tasks = new TaskData[] { new TaskData("Press button 1") , new TaskData("Press button 2"),
            new TaskData("Press button 3"), new TaskData("Press button 4"),new TaskData("Press button 5")
            ,new TaskData("Press button 6"),new TaskData("Press button 7")};
            
            UltimateHudManager.Instance.CreateMission("Example Missions", tasks);

          

                
        }

        void MissionEnd()
        {
            UltimateHudManager.Instance.HideMission(true);
        }


        void SkillBarStart()
        {
            UltimateHudManager.Instance.CreateSkillBar(new SkillProgressProperties("Progress", 0, 100,"", _Image),false);
        }

        void SkillBarEnd()
        {
            UltimateHudManager.Instance.RemoveSkillProgress("Progress");
        }

        void PrimaryNotifStart()
        {
            UltimateHudManager.Instance.CreatePrimaryNotification("Danger Zone","Stay Out Of This");
        }
        void SecondaryNotifStart()
        {
            UltimateHudManager.Instance.CreateSecondaryNotification("Notification for some event",
                "Some description",
                "Hint: Press " + 
                
                UltimateHudManager.Instance.GetInputIconCode(KeyCode.W) +  " to move <color=yellow>Forward</color>"

                , 2,_Image);
        }

        void PickupNotifStart()
        {
            UltimateHudManager.Instance.CreatePickupNotification("Pickup",_Image);
        }

        void InputTooltipStart()
        {
            UltimateHudManager.Instance.ShowInputTooltipText("", KeyCode.Space, " Press to Jump");
        }


        void InputTooltipStop()
        {
            UltimateHudManager.Instance.HideInputTooltip();
        }

        void ChangeCrossHair()
        {
            UltimateHudManager.Instance.CreateCrossHair(Index);
        }

        void DamageEffectStart()
        {
            if(Index == 0)
            {
                UltimateHudManager.Instance.ShowDamageEffect(true);
            }
            else
            {
                UltimateHudManager.Instance.ShowHealEffect(true);
            }
        }




        void DamageEffectEnd()
        {
            UltimateHudManager.Instance.HideDamageHealEffect();
        }



        public void HealtbarStart()
        {
            UltimateHudManager.Instance.CreateHealthBar(new HeathData(50, 100, 20, 100));
        }

        public void HealthbarEnd()
        {
            UltimateHudManager.Instance.RemoveHealthBar();
        }


        void HealthBarHeal()
        {
            
            UltimateHudManager.Instance.AddHealth(Index);
            
        }



        
        void WaypointStart()
        {
            UltimateHudManager.Instance.ShowCompass();

            UltimateHudManager.Instance.CreateWaypoint(Marker, tr.position);
        }

        void WaypointEnd()
        {
            UltimateHudManager.Instance.ShowCompass(false);

            UltimateHudManager.Instance.RemoveAllWaypoints();
        }

        void ProgressTooltipStart()
        {
            UltimateHudManager.Instance.ShowProgressTooltip(2, "UNLOCKING");
        }

        void ProgressTooltipStop()
        {
            UltimateHudManager.Instance.HideProgressTooltip();
        }


        void FocusObjectStart()
        {
            UltimateHudManager.Instance.CreateFocusPoint(tr,new FOCUS_PROPERTIES(-1,250));
        }

        void FocusObjectEnd()
        {
            UltimateHudManager.Instance.StopFocusPoint();
        }

        public enum TriggerType
        {
            missions,
            skill,
            primaryNotif,
            secondaryNotif,
            pickupNotif,
            inputTooltip,
            crosshair,
            damage,
            healtbar,
            healthbarAdd,
            waypoint,
            progressTooltip,
            FocusOnGameobject
        }
    }
}
