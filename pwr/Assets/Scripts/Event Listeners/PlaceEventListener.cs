using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs;

public class PlaceEventListener : AEventListener
{
    public PlaceStruct structToCheck;
    private CookingEventListener otherPlEL;

    private PlayerController playerController;

    //Initial number of furniture to place
    public int startingNumPlaced;
    //Starting number + the target number of furniture to place
    public int checkNumPlaced;
    //Current Number of furniture placed
    public int currentNumPlaced;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlacedFurniture();
    }

    public override void OnStartListening()
    {
        startingNumPlaced = 0;
        checkNumPlaced = structToCheck.targetValue;
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
        otherPlEL = (CookingEventListener)otherEventListener;
        this.structToCheck.targetValue = otherPlEL.structToCheck.targetValue;
    }

    public void SetPlaceEventListener(int targetValue)
    {
        structToCheck.targetValue = targetValue;
    }

    private void CheckPlacedFurniture()
    {
        if(playerController.placeFurnitureFlag == true)
        {
            currentNumPlaced++;
            playerController.placeFurnitureFlag = false;
        }

        if (currentNumPlaced == checkNumPlaced)
        {
            IsEventCompleted = true;
        }
        else if (currentNumPlaced > checkNumPlaced)
        {
            IsEventHasBeenUpdated = true;
        }
    }
}
