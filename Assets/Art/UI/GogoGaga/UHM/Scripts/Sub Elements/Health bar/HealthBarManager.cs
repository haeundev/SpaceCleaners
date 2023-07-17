using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GogoGaga.UHM
{
    public class HealthBarManager : MonoBehaviour
    {

        public Slider healthBar;
        public Slider extraHealthBar;

        public TextMeshProUGUI healthText;

        HeathData healthData;
        AnimationController AnimCont;
        Coroutine HideCorotuine;


        const int ValueSetSpeed = 10;
        private void Awake()
        {
            AnimCont = GetComponent<AnimationController>();
        }

        public void Set(HeathData _heathData)
        {
            if (_heathData.maxHealth == 0)
            {
                Debug.LogError("Please use max health greater then 0!");
                return;
            }


            healthBar.maxValue = _heathData.maxHealth;
            healthBar.value = _heathData.health;


            if (_heathData.maxExtraHealth == 0)
                extraHealthBar.gameObject.SetActive(false);
            else
            {
                extraHealthBar.gameObject.SetActive(true);
                extraHealthBar.maxValue = _heathData.maxExtraHealth;
                extraHealthBar.value = _heathData.extraHealth;
            }


            healthText.text = _heathData.health.ToString();

            healthData = _heathData;


            if (HideCorotuine != null)
                StopCoroutine(HideCorotuine);

            AnimCont.Play("Show");
        }


        public void Remove()
        {
            healthData = new HeathData();
            Hide();
        }

        public void Hide()
        {
            HideCorotuine = StartCoroutine(Hiding());
        }

        public IEnumerator Hiding()
        {
            yield return new WaitForEndOfFrame();

            AnimCont.Play("Hide");

            yield return new WaitForSeconds(1);

            gameObject.SetActive(false);
        }




        public void SetMaxHeatlh(float newMaxHealth)
        {
            healthBar.maxValue = newMaxHealth;
            healthData.maxHealth = newMaxHealth;
        }

        public void AddHealth(float add)
        {
            SetHealth(healthData.health + add);
        }

        public void SetHealth(float health)
        {
            /*
            LeanTween.value(healthData.health, health, 0.7f).setEase(LeanTweenType.easeOutBack)
                .setOnUpdate((float val) =>
                {
                    healthBar.value = val;
                });

            */

            health = Mathf.Clamp(health,0, healthData.maxHealth);

            healthText.text = health.ToString();

            if (SettingValCorotuine != null)
                StopCoroutine(SettingValCorotuine);

            if (gameObject.activeInHierarchy)
                SettingValCorotuine = StartCoroutine(SetValue(health));
        }

        Coroutine SettingValCorotuine;
        IEnumerator SetValue(float newHealth)
        {
            float val = healthData.health;
            healthData.health = newHealth;


            if (newHealth > val)
            {
                while (val < newHealth)
                {
                    yield return new WaitForEndOfFrame();
                    val += Mathf.Abs((val - newHealth) * ValueSetSpeed * Time.deltaTime);
                    healthBar.value = val;
                }
            }

            else if(newHealth < val)
            {
                while (val > newHealth)
                {
                    yield return new WaitForEndOfFrame();
                    val -= Mathf.Abs((val - newHealth) * ValueSetSpeed * Time.deltaTime);
                    healthBar.value = val;
                }
            }
        }

        public void SetExtraMaxHeatlh(float newMaxHealth)
        {
            extraHealthBar.maxValue = newMaxHealth;
            healthData.maxExtraHealth = newMaxHealth;
        }

        public void AddExtraHealth(float Add)
        {
            SetExtraValue(healthData.extraHealth + Add);
        }

        public void SetExtraHealth(float extraHealth)
        {
            if (healthData.maxExtraHealth == 0)
            {
                extraHealthBar.gameObject.SetActive(false);
                return;
            }
            else
                extraHealthBar.gameObject.SetActive(true);

            /*
            LeanTween.value(healthData.extraHealth, extraHealth, 0.7f).setEase(LeanTweenType.easeOutBack)
               .setOnUpdate((float val) =>
               {
                   extraHealthBar.value = val;
               });
            */

            extraHealth = Mathf.Clamp(extraHealth, 0, healthData.maxHealth);


            if (SettingValExtraCorotuine != null)
                StopCoroutine(SettingValExtraCorotuine);

            SettingValExtraCorotuine = StartCoroutine(SetExtraValue(extraHealth));
        }

        Coroutine SettingValExtraCorotuine;
        IEnumerator SetExtraValue(float newHealth)
        {
            float val = healthData.extraHealth;
            healthData.extraHealth = newHealth;


            if (newHealth > val)
            {
                while (val < newHealth)
                {
                    yield return new WaitForEndOfFrame();
                    val += Mathf.Abs((val - newHealth) * ValueSetSpeed * Time.deltaTime);
                    extraHealthBar.value = val;
                }
            }

            else if (newHealth < val)
            {
                while (val > newHealth)
                {
                    yield return new WaitForEndOfFrame();
                    val -= Mathf.Abs((val - newHealth) * ValueSetSpeed * Time.deltaTime);
                    extraHealthBar.value = val;
                }
            }
        }

        public void SetColor(Color upperBarColor, Color lowerBarColor)
        {
            healthBar.fillRect.GetComponent<Image>().color = upperBarColor;
            extraHealthBar.fillRect.GetComponent<Image>().color = lowerBarColor;
        }



    }

    [System.Serializable]
    public struct HeathData
    {
        public float health;
        public float maxHealth;
        public float extraHealth;
        public float maxExtraHealth;




        public HeathData(
                float health,
                float maxHealth,
                float extraHealth = 0,
                float maxExtraHealth = 0)
        {
            this.health = health;
            this.maxHealth = maxHealth;
            this.extraHealth = extraHealth;
            this.maxExtraHealth = maxExtraHealth;
        }
    }

}