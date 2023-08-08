using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int[] ingredientCount = new int[3]; //0: leaf, 1: flower, 2: bug

    int maxCount = 5;
    public float oxygenAmount;

    public TMP_Text[] ingredientCountText;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ingredientCount[0] == maxCount && ingredientCount[1] == maxCount && ingredientCount[2] == maxCount)
        {
            
        }
    }

}
