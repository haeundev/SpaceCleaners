using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int[] ingredientCount = new int[3]; //0: leaf, 1: flower, 2: bug
    int maxIngredientCount = 1;

    public Transform keySpawnPos;
    public GameObject keyItem;

    public float oxygenAmount;

    public TMP_Text[] ingredientCountText;

    bool isKeySpawned = false;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isKeySpawned && ingredientCount[0] >= maxIngredientCount && ingredientCount[1] >= maxIngredientCount && ingredientCount[2] >= maxIngredientCount)
        {
            Instantiate(keyItem, keySpawnPos.position, keySpawnPos.rotation);
            isKeySpawned = true;
        }
    }

}
