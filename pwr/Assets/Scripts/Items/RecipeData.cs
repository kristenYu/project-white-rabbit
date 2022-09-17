using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RecipeData
{
    //Stores serializable data to transfer to the recipe component 
    public string stringName;
    public string[] ingredients;
    public int cost;
    public int cookedFoodSellingPrice;
}
