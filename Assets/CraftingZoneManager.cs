using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingZoneManager : MonoBehaviour
{
    public GameObject processingSlider;
    private Slider slider;
    [SerializeField] private float duration = 3f;

    public GameObject[] spawnIngredients;

    public Transform[] spawnIngredientsPos;
    
    // public Transform[] craftIngredientsPos;
    //
    // private RaycastHit[] hit = new RaycastHit[2];
    //
    // private string[] ingredientA = {"Metal", "Plastic"};
    // private string[] ingredientB = {"Plastic", "Metal"};
    //
    // private int indexA = -1;
    // private int indexB = -1;
    //
    // private bool isReadyCraft = false;
    //
    // //Crafting UI
    // public GameObject craftingSliderUI;
    // private Slider craftSlider;
    // public TMP_Text craftingText;
    //
    // public GameObject[] craftedObj;
    // public Transform craftedObjPos;

    private void Awake()
    {
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
        
        slider = processingSlider.GetComponent<Slider>();
        // craftSlider = craftingSliderUI.GetComponent<Slider>();
        // craftingText.text = "Place to Craft";
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ProcessingBar(duration)); //for testing
    }

    void OnDebrisCaptured(DebrisType _, GameObject __)
    {
        //StartCoroutine(ProcessingBar(duration)); //실제는 이거!!
    }

    IEnumerator ProcessingBar(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            slider.value = time / duration;
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 2; i++)
        {
            Instantiate(spawnIngredients[i], spawnIngredientsPos[i].transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (Physics.Raycast(craftIngredientsPos[0].position, Vector3.up, out hit[0], 0.2f)) //transform.position
    //     {
    //         indexA = Array.IndexOf(ingredientA, hit[0].transform.gameObject.tag);
    //     }
    //     else
    //     {
    //         indexA = -1;
    //     }
    //     
    //     if (Physics.Raycast(craftIngredientsPos[1].position, Vector3.up, out hit[1], 0.2f)) //transform.position
    //     {
    //         indexB = Array.IndexOf(ingredientB, hit[1].transform.gameObject.tag);
    //     }
    //     else
    //     {
    //         indexB = -1;
    //     }
    //     
    //     if (indexA != -1 && indexA == indexB)
    //     {
    //         print("Able to craft!");
    //         craftingText.text = "Ready to Craft";
    //         isReadyCraft = true;
    //     }
    // }

    // public void OnCraftButtonPressed()
    // {
    //     if (isReadyCraft)
    //     {
    //         print("crafting~");
    //         craftingText.text = "Crafting...";
    //         StartCoroutine(CraftingBar(duration));
    //         
    //     }
    // }
    //
    // public void OnCraftButtonReleased()
    // {
    //     isReadyCraft = false;
    // }
    //
    // IEnumerator CraftingBar(float duration)
    // {
    //     float time = 0;
    //     while (time < duration)
    //     {
    //         craftSlider.value = time / duration;
    //         time += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     
    //     print("craft complete~");
    //     craftingText.text = "Craft Complete";
    //     
    //     Instantiate(craftedObj[indexA], craftedObjPos.transform.position, Quaternion.identity);
    //     
    // }
    
    
}
