using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseIngredientCount : MonoBehaviour
{
    public void AddIngredientCount()
    {
        if(gameObject.tag == "Leaf")
        {
            GameManager.ingredientCount[0]++;
            print("Leaf: "+ GameManager.ingredientCount[0]);
        }
        else if(gameObject.tag == "Flower")
        {
            GameManager.ingredientCount[1]++;
            print("Flower: "+ GameManager.ingredientCount[1]);
        }
        else if(gameObject.tag == "Bug")
        {
            GameManager.ingredientCount[2]++;
            print("Bug: "+ GameManager.ingredientCount[2]);
        }
        else if(gameObject.tag == "Oxygen")
        {
            GameManager.oxygenAmount += 10.0f;
            print("Oxygen: "+ GameManager.oxygenAmount);
        }
        
    }
}
