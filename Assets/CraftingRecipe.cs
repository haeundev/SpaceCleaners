using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe
{
    protected List<string> ingredients = new List<string>();
    protected List<bool> matched = new List<bool>();
    public GameObject upgradedObj;
    public bool isDoneCrafting;
    
    public CraftingRecipe(List<string> ingredients, GameObject upgradedObj)
    {
        this.ingredients = ingredients;
        

        // matched = new bool[this.ingredients.Length];
        for (int i = 0; i < ingredients.Count; i++)
        {
            matched.Add(false);
        }
        
        this.upgradedObj = upgradedObj;
        this.isDoneCrafting = false;
    }

    public void PrintAll()
    {
        for (int i = 0; i < this.ingredients.Count; i++)
        {
            Debug.Log(this.ingredients[i]);
            //Debug.Log(this.matched[i]);
        }
    }
    
    public void Detected(string ingredient) //collider 들어오면!
    {
        int index = this.ingredients.IndexOf(ingredient);
        if (index != -1)
        {
            matched[index] = true;
        }
    }
    
    public void UnDetected(string ingredient) //collider 빠져나가면!
    {
        int index = this.ingredients.IndexOf(ingredient);
        if (index != -1)
        {
            matched[index] = false;
        }
    }
    
    public bool IsAllMatched()
    {
        for (int i = 0; i < this.matched.Count; i++)
        {
            if (!matched[i])
            {
                return false;
            }
        }
    
        return true;
    }
}