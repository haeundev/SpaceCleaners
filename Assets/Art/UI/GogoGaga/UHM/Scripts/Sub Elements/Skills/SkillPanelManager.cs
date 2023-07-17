using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class SkillPanelManager : MonoBehaviour
    {

        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descText;
        public Slider ProgressBar;
        public Image Icon;

        [Header("Default Properties")]
        public Sprite DefaultIcon;

        AnimationController AnimCont;

        Dictionary<string, SkillProgressProperties> SkillProgressDatas = new Dictionary<string, SkillProgressProperties>();


        const int ValueSetSpeed = 10;

        private void Awake()
        {
            AnimCont = GetComponent<AnimationController>();

        }

        public void CreateSkillNotification(SkillProgressProperties newSkillData, bool ShowAfterUpdating)
        {
            if (SkillProgressDatas.ContainsKey(newSkillData.Title))
            {
                Debug.LogError("Already exists such skill data");
                return;
            }

            SkillProgressDatas.Add(newSkillData.Title, newSkillData);

            if (ShowAfterUpdating)
                ShowSkillNotification(newSkillData.Title);
        }

        public void ShowSkillNotification(string Title, int newProgressVal = -1)
        {

            if (!SkillProgressDatas.ContainsKey(Title))
            {
                Debug.LogError("No such skill progress exist! Please create a skill progress first");
                return;
            }

            if (newProgressVal >= 0)
            {
                SkillProgressDatas[Title].ChangeProgressValue(newProgressVal);
            }

            if (ShowingCorotuine != null)
                StopCoroutine(ShowingCorotuine);

            ShowingCorotuine = StartCoroutine(ShowingProgress());

            

            titleText.text = SkillProgressDatas[Title].Title;

            ProgressBar.value = SkillProgressDatas[Title].GetPrevVal();
            ProgressBar.maxValue = SkillProgressDatas[Title].maxValue;

            SkillProgressDatas[Title].progressValue = Mathf.Clamp(SkillProgressDatas[Title].progressValue, 0, SkillProgressDatas[Title].maxValue);

            

            if (SetValueCorotuine != null)
                StopCoroutine(SetValueCorotuine);

            SetValueCorotuine = StartCoroutine(SetValue(Title));

            if (string.IsNullOrEmpty(SkillProgressDatas[Title].Description))
            {
                descText.text = "+" + SkillProgressDatas[Title].progressValue + "/" + SkillProgressDatas[Title].maxValue;
            }
            else
            {
                descText.text = SkillProgressDatas[Title].Description;
            }

            if (UltimateHudManager.CheckIfMissingOrNull(SkillProgressDatas[Title].IconImage))
            {
                Icon.sprite = DefaultIcon;
            }
            else
            {
                Icon.sprite = SkillProgressDatas[Title].IconImage;
            }
        }

        Coroutine SetValueCorotuine;
        IEnumerator SetValue(string Title)
        {
            float val = SkillProgressDatas[Title].GetPrevVal();
            float finalVal = SkillProgressDatas[Title].progressValue;

            yield return new WaitForSeconds(0.5f);





            if (finalVal > val)
            {
                while (val < finalVal)
                {
                    yield return new WaitForEndOfFrame();
                    val += Mathf.Abs((val - finalVal) * ValueSetSpeed * Time.deltaTime);
                    ProgressBar.value = val;

                }
            }

            else if (finalVal < val)
            {
                while (val > finalVal)
                {
                    yield return new WaitForEndOfFrame();
                    val -= Mathf.Abs((val - finalVal) * ValueSetSpeed * Time.deltaTime);
                    ProgressBar.value = val;
                }
            }
        }

        Coroutine ShowingCorotuine;
        IEnumerator ShowingProgress()
        {
            if (AnimCont != null)
                AnimCont.Play("Show");

            yield return new WaitForSeconds(3f);

            if (AnimCont != null)
                AnimCont.Play("Hide");


            yield return new WaitForSeconds(1f);

            gameObject.SetActive(false);
        }

        public void UpdateSkillProgressData(SkillProgressProperties newData, bool ShowAfterUpdating = true)
        {
            if (!SkillProgressDatas.ContainsKey(newData.Title))
            {
                Debug.LogError("There is no such skill!");
                return;
            }

            SkillProgressDatas[newData.Title] = newData;

            if (ShowAfterUpdating)
                ShowSkillNotification(newData.Title);
        }

        public void RemoveSkillProgress(string Title)
        {
            if (SkillProgressDatas.ContainsKey(Title))
            {
                SkillProgressDatas.Remove(Title);
            }
        }

        public void RemoveAllSkillProgress()
        {
            SkillProgressDatas.Clear();
        }


        public float GetValue(string title)
        {
            if (!SkillProgressDatas.ContainsKey(title))
                return 0;

            return SkillProgressDatas[title].progressValue;
        }
    }


    [System.Serializable]
    public class SkillProgressProperties
    {
        public string Title;
        public float progressValue;
        private float prevVal;
        public float maxValue;
        public string Description;
        public Sprite IconImage;

        public SkillProgressProperties()
        {

        }

        public SkillProgressProperties(
                string title,
                int progressValue,
                int maxValue, 
                string description,
                Sprite iconImage)

        {
            Title = title;
            this.progressValue = progressValue;
            this.maxValue = maxValue;
            Description = description;
            IconImage = iconImage;
        }

        public SkillProgressProperties(
                string title,
                int progressValue,
                int maxValue, 
                string description)
        {
            Title = title;
            this.progressValue = progressValue;
            this.maxValue = maxValue;
            Description = description;
        }

        public SkillProgressProperties(
                string title,
                int progressValue, 
                int maxValue)
        {
            Title = title;
            this.progressValue = progressValue;
            this.maxValue = maxValue;
            Description = "";
            IconImage = null;
        }

        public float GetPrevVal()
        {
            return prevVal;
        }

        public void ChangeProgressValue(int newVal)
        {
            prevVal = progressValue;
            progressValue = newVal;
        }
    }
}