using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs;

public class CookingEventListener : AEventListener
{
    public CookingStruct structToCheck;
    private CookingEventListener otherCEL;

    private PlayerController playerController; 

    //Initial number of recipe types to cook
    public int startingNumRecipes;
    //Starting number + the target number of crops to check for
    public int checkNumRecipes;
    //Current Number of recipes made
    public int currentNumRecipes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckCookedRecipe();
    }

    public override void OnStartListening()
    {
        startingNumRecipes = 0;
        checkNumRecipes = structToCheck.targetValue;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>(); 
    }
    public override void OnEndListening()
    {
        Debug.Log("End Listening");
    }
    public override void OnEventUpdate()
    {
        Debug.Log("Update Event");
    }
    public override void OnEventCompleted()
    {
        Debug.Log("Event Has been Completed");
    }
    public override void Equals(AEventListener otherEventListener)
    {
        otherCEL = (CookingEventListener)otherEventListener;
        this.structToCheck.ingredientType = otherCEL.structToCheck.ingredientType;
        this.structToCheck.targetValue = otherCEL.structToCheck.targetValue;
    }

    public void SetCookingEventListener(string ingredientType, int targetValue)
    {
        structToCheck.ingredientType = ingredientType;
        structToCheck.targetValue = targetValue;
    }

    //TODO: UNHACK THIS this is a hardcoded mess
    private void CheckCookedRecipe()
    {
        if (playerController.CookedRecipeFlag == true)
        {
            switch(playerController.cookedFoodObject.name)
            {
                case "french fries":
                    if(structToCheck.ingredientType == "potato")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "potato chips":
                    if (structToCheck.ingredientType == "potato")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "tomato soup":
                    if (structToCheck.ingredientType == "tomato")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "veggie soup":
                    if (structToCheck.ingredientType == "tomato" || 
                        structToCheck.ingredientType == "carrot" || 
                        structToCheck.ingredientType == "onion")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "spaghetti sauce":
                    if (structToCheck.ingredientType == "tomato" ||
                        structToCheck.ingredientType == "onion")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "onion soup":
                    if (structToCheck.ingredientType == "greenonion" ||
                        structToCheck.ingredientType == "onion")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "potato buns":
                    if (structToCheck.ingredientType == "greenonion" ||
                        structToCheck.ingredientType == "potato")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "veggie dumplings":
                    if (structToCheck.ingredientType == "greenonion" ||
                       structToCheck.ingredientType == "carrot" ||
                       structToCheck.ingredientType == "onion")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
                case "lettuce sandwich":
                    if (structToCheck.ingredientType == "lettuce" ||
                       structToCheck.ingredientType == "carrot" ||
                       structToCheck.ingredientType == "tomato")
                    {
                        currentNumRecipes++;
                        playerController.CookedRecipeFlag = false;
                    }
                    break;
            }
            if(currentNumRecipes >= checkNumRecipes)
            {
                IsEventCompleted = true;
            }
            else
            {
                IsEventCompleted = false;
            }
        }
    }
}

/*
french fries,100, potato, potato
potato chips,200, potato, potato, potato
tomato soup,200, tomato, tomato, tomato 
veggie soup,200, tomato, carrot, onion
spaghetti sauce,100, tomato, onion
onion soup,200, onion, onion, greenonion
potato buns,200, potato, potato, greenonion
veggie dumplings,200, greenonion, onion, carrot
lettuce sandwich,200, lettuce, carrot, tomato
*/