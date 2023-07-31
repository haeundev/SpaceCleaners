using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseIngredientCount : MonoBehaviour
{

    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    public void AddIngredientCount()
    {
        if(gameObject.tag == "Leaf")
        {
            gameManager.ingredientCount[0]++;
            gameManager.ingredientCountText[0].text = gameManager.ingredientCount[0] + "/5";
            print("Leaf: "+ gameManager.ingredientCount[0]);
        }
        else if(gameObject.tag == "Flower")
        {
            gameManager.ingredientCount[1]++;
            gameManager.ingredientCountText[1].text = gameManager.ingredientCount[1] + "/5";
            print("Flower: "+ gameManager.ingredientCount[1]);
        }
        else if(gameObject.tag == "Bug")
        {
            gameManager.ingredientCount[2]++;
            gameManager.ingredientCountText[2].text = gameManager.ingredientCount[2] + "/5";
            print("Bug: "+ gameManager.ingredientCount[2]);
        }
        else if(gameObject.tag == "Oxygen")
        {
            gameManager.oxygenAmount += 10.0f;
            print("Oxygen: "+ gameManager.oxygenAmount);
        }
        
    }
}
