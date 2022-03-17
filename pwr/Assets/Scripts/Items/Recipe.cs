using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : Item
{
    public string[] ingredients;
    public GameObject cookedFood;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool equals(Recipe otherRecipe)
    {
        //this is only conditioned off of string name since all objects should be made using the generator
        //This has the potential to cause errors if recipe objects are not made using the generator
        //TODO: Enforce better equality
        if(this.stringName == otherRecipe.stringName)
        {
            return true; 
        }
        else
        {
            return false; 
        }
    }
}
