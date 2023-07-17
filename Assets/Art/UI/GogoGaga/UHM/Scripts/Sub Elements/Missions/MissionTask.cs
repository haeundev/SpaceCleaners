using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class MissionTask : MonoBehaviour
    {
        public TextMeshProUGUI taskText;
        public Image taskStatusIcon;

        AnimationController animCont;

        [Header ("Defualt Tasks Images")]
        public Sprite pendingImage;
        public Sprite failedImage;
        public Sprite skippedImage;
        public Sprite compeleteImage;


       public TaskData taskData { get; set; }
        private void Awake()
        {
            animCont = GetComponent<AnimationController>();
        }


        public void SetUp(TaskData data)
        {
            if (animCont == null)
                animCont = GetComponent<AnimationController>();

            taskData = data;
            taskText.text = data.taskText;
            SetImage(data.progressType);

        }

        public void Show()
        {
            if (animCont != null)
                animCont.Play("Show");
        }

        public void ShowWithDelay(float delay)
        {
            Invoke("Show", delay);
        }

        

        Coroutine hidingCorotuine;
        public void HideWithDelay(float delay , bool remove = false)
        {
            if(hidingCorotuine != null)
                StopCoroutine(hidingCorotuine);

            if (gameObject.activeInHierarchy)
                hidingCorotuine = StartCoroutine(HidingCorotuine(delay, remove));
        }



        IEnumerator HidingCorotuine(float delay , bool remove)
        {
            yield return new WaitForSeconds(delay);

            if (animCont != null)
                animCont.Play("Hide");

            yield return new WaitForSeconds(0.8f);

            if (remove)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

         void SetImage(TaskProgressType type)
        {
            switch (type)
            {
                case TaskProgressType.pending:
                    taskText.color = Color.white;
                    taskStatusIcon.sprite = pendingImage;
                    break;
                case TaskProgressType.failed:
                    taskText.color = Color.red;
                    taskStatusIcon.sprite = failedImage;
                    break;
                case TaskProgressType.skipped:
                    taskText.color = Color.yellow;
                    taskStatusIcon.sprite = skippedImage;
                    break;
                case TaskProgressType.completed:
                    taskStatusIcon.sprite = compeleteImage;
                    taskText.color = Color.green;
                    break;
                default:
                    break;
            }
        }

        public void ChangeTypeWithAnim(TaskProgressType newType)
        {
            if (animCont != null)
                animCont.Play("Change");
            SetImage(newType);
        }
        public void ChangeType(TaskProgressType newType)
        {
            if (hidingCorotuine != null)
                StopCoroutine(hidingCorotuine);

            SetImage(newType);
        }

    }


    [System.Serializable]
    public class TaskData
    {
        public TaskProgressType progressType;
        public string taskText;

        public TaskData(string taskText, TaskProgressType progressType)
        {
            this.progressType = progressType;
            this.taskText = taskText;
        }

        public TaskData(string taskText)
        {
            this.progressType = TaskProgressType.pending;
            this.taskText = taskText;
        }

        public TaskData()
        {
            taskText = "";
            progressType = TaskProgressType.pending;
        }
    }

    public enum TaskProgressType
    {
        pending,
        failed,
        skipped,
        completed,
    }
}