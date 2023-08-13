using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHUD : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = (int)slider.value;
    }

    public void SetSliderMaxValue(int maxVal)
    {
        slider.maxValue = maxVal;
        currentHealth = maxVal;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void MonsterTakeDamage(int damage)
    {
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
