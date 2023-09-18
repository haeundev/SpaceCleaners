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

    public GameObject statusDescUI;
    public TMP_Text statusDesc;
    
    [SerializeField] private float duration = 3f;

    public GameObject[] spawnIngredients;
    private GameObject[] craftingIngredients = new GameObject[2];

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
    public GameObject upgradedObj;
    public Transform upgradedObjPos;
    
    private CraftingRecipe recipe;
    private bool isReadyToCraft = false;

    public GameObject leftCraftingLight;
    public GameObject rightCraftingLight;

    private void Awake()
    {
        OuterSpaceEvent.OnDebrisCaptured += OnDebrisCaptured;
        OuterSpaceEvent.OnCraftingReady += OnCraftingReady;
        
        slider = processingSlider.GetComponent<Slider>();
        // craftSlider = craftingSliderUI.GetComponent<Slider>();
        // craftingText.text = "Place to Craft";
    }

    // Start is called before the first frame update
    void Start()
    {
        leftCraftingLight.SetActive(false);
        rightCraftingLight.SetActive(false);
        
        statusDesc.text = "PROCESSING...";
        StartCoroutine(ProcessingBar(duration)); //for testing

        List<string> temp = new List<string>();
        temp.Add("Plastic");
        temp.Add("Metal");
        
        recipe = new CraftingRecipe(temp, upgradedObj);
        recipe.PrintAll();
    }

    private void OnDestroy()
    {
        OuterSpaceEvent.OnDebrisCaptured -= OnDebrisCaptured;
        OuterSpaceEvent.OnCraftingReady -= OnCraftingReady;
        
    }

    void OnDebrisCaptured(DebrisType _, GameObject __)
    {
        //StartCoroutine(ProcessingBar(duration)); //실제는 이거!!
    }
    
    void OnCraftingReady(string ingredientType, bool isReady)
    {
        if (isReady)
        {
            recipe.Detected(ingredientType);
            bool isCraftable = recipe.IsAllMatched();
            if (isCraftable && !recipe.isDoneCrafting)
            {
                statusDesc.text = "READY TO CRAFT";
                isReadyToCraft = true;
            }
            //일단은 위치시키는 거로 + 버튼 누르는거 
            
        }
        else
        {
            recipe.UnDetected(ingredientType);
            processingSlider.SetActive(false);
            statusDesc.text = "PLACE TO CRAFT";
            isReadyToCraft = false;
        }
    }

    public void OnButtonPressed()
    {
        if (isReadyToCraft)
        {
            StartCoroutine(CraftingBar(duration)); //실제는 이거!!
        }
    }
    
    IEnumerator CraftingBar(float duration)
    {
        processingSlider.SetActive(true);
        statusDesc.text = "CRAFTING...";
        
        leftCraftingLight.SetActive(true);
        rightCraftingLight.SetActive(true);
        
        float time = 0;
        while (time < duration)
        {
            slider.value = time / duration;
            time += Time.deltaTime;
            yield return null; //yield return new WaitForEndOfFrame();
        }

        leftCraftingLight.SetActive(false);
        rightCraftingLight.SetActive(false);
        Destroy(craftingIngredients[0]);
        Destroy(craftingIngredients[1]);
        
        statusDesc.text = "CRAFTING COMPLETED";
        processingSlider.SetActive(false);
        recipe.isDoneCrafting = true;
        Instantiate(recipe.upgradedObj, upgradedObjPos);
        isReadyToCraft = false;

    }

    IEnumerator ProcessingBar(float duration)
    {
        statusDescUI.SetActive(true);
        float time = 0;
        while (time < duration)
        {
            slider.value = time / duration;
            time += Time.deltaTime;
            yield return null; //yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 2; i++)
        {
            craftingIngredients[i] = Instantiate(spawnIngredients[i], spawnIngredientsPos[i]);//Instantiate(spawnIngredients[i], spawnIngredientsPos[i].transform.position, Quaternion.identity);
        }
        
        processingSlider.SetActive(false);
        statusDesc.text = "PLACE TO CRAFT";
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
