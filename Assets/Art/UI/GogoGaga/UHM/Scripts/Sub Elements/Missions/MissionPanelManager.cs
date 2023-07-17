using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace GogoGaga.UHM
{
    public class MissionPanelManager : MonoBehaviour
    {
       [Range(1,6)] public int ActiveTaskLimit = 4;
        public MissionTask taskPrefab;

        public Transform tasksParent;

        public TextMeshProUGUI titleText;
        string LastMissionName;
        List<TaskData> tasks = new List<TaskData>();
        List<MissionTask> missionTasks = new List<MissionTask>();

        AnimationController animCont;

        public bool show { get; set; }

        private void Awake()
        {
            animCont = GetComponent<AnimationController>();

            foreach (Transform t in tasksParent)
            {
                Destroy(t.gameObject);
            }
        }



        public void CreateMission(string MissionTitle, TaskData[] taskDatas)
        {

            if (string.IsNullOrEmpty(MissionTitle))
            {
                Debug.LogError("Mission text is empty no mission is created!.");
                return;
            }

            if (hidingMissionCorotuine != null)
                StopCoroutine(hidingMissionCorotuine);

            //Enable mission panel
            if (animCont != null)
                animCont.Play("Show");


            LastMissionName = MissionTitle;

            if (titleText != null)
                titleText.text = MissionTitle;
            
            foreach (Transform t in tasksParent)
            {
                Destroy(t.gameObject);
            }

            missionTasks.Clear();

            if (taskDatas != null)
                AddTask(taskDatas);
        }

        public void RemoveMission()
        {
            tasks.Clear();
            LastMissionName = "";
            Hide();
        }

        public void Hide()
        {
          
            hidingMissionCorotuine = StartCoroutine(HideMission());
        }

        Coroutine hidingMissionCorotuine;
        IEnumerator HideMission()
        {
            show = false;

            for (int i = missionTasks.Count - 1; i >= 0; i--)
            {
                if (missionTasks[i] != null)
                    missionTasks[i].HideWithDelay(0);
            
                yield return new WaitForSeconds(0.2f);
            }
            
            
            animCont.Play("Hide");

            yield return new WaitForSeconds(1);
            gameObject.SetActive(show);
        }


        public void Show()
        {
            if (string.IsNullOrEmpty(LastMissionName))
            {
                Debug.LogError("No mission created! First create a mission");
                return;
            }
            show = true;
            animCont.Play("Show");

            for (int i = 0; i < missionTasks.Count; i++)
            {
                missionTasks[i].ShowWithDelay(1 + (i * 0.2f));
            }
        }

        public void AddTask(TaskData[] taskDatas)
        {
            if (string.IsNullOrEmpty(LastMissionName))
                return;

            if (taskDatas != null)
                tasks.AddRange(taskDatas);



            SetTasksActive();
        }

        public void AddTask(TaskData taskdata, int placeAtIndex = -1)
        {
            if (string.IsNullOrEmpty(LastMissionName))
                return;

            if (placeAtIndex < 0 || placeAtIndex >= tasks.Count)
            {
                tasks.Add(taskdata);
            }
            else
            {
                TaskData temp = tasks[placeAtIndex];
                tasks[placeAtIndex] = taskdata;
                tasks.Add(temp);
            }

            SetTasksActive();

        }

        void SetTasksActive()
        {

            for (int i = missionTasks.Count; i < ActiveTaskLimit; i++)
            {
                SetActive(0, i * 0.35f);
            }

        }

        public void SetActive(int task, float delay = 0.4f)
        {
            if (task >= tasks.Count || task < 0)
                return;

            MissionTask t = Instantiate(taskPrefab, tasksParent);
            t.SetUp(tasks[task]);
            t.ShowWithDelay(delay);
            missionTasks.Add(t);
            tasks.RemoveAt(task);
        }


        public void RemoveTask(int TaskNo)
        {
            if (TaskNo >= missionTasks.Count || TaskNo < 0)
                return;


            Destroy(missionTasks[TaskNo].gameObject);
            missionTasks.RemoveAt(TaskNo);

            SetTasksActive();
        }

        public void ChangeTaskStatus(TaskProgressType NewType, int index)
        {
            if (index >= missionTasks.Count || index < 0)
                return;

            if (NewType == TaskProgressType.completed)
            {
                if (missionTasks[index] != null)
                    missionTasks[index].HideWithDelay(0.8f, false);

                OnCompleteingTask(index);
            }
            else
            {
                if (missionTasks[index] != null)
                {
                    if (missionTasks[index].taskData.progressType == TaskProgressType.completed)
                    {
                        missionTasks[index].gameObject.SetActive(true);
                        missionTasks[index].Show();
                        missionTasks[index].ChangeType(NewType);
                    }
                    else
                        missionTasks[index].ChangeTypeWithAnim(NewType);
                }
                
            }

            missionTasks[index].taskData.progressType = NewType;
        }

        void OnCompleteingTask(int taskIndex)
        {
            if (missionTasks[taskIndex].taskData.progressType != TaskProgressType.completed)
            {
                SetActive(0);
            }

            missionTasks[taskIndex].ChangeTypeWithAnim(TaskProgressType.completed);
        }
    }
}