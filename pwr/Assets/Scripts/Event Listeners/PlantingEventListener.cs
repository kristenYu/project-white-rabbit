using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs;

public class PlantingEventListener : AEventListener
{
    public PlantSeedStruct structToCheck;
    public GameObject worldControllerObject;
    private WorldController worldController;
    private Crop currentCrop;
    private PlantingEventListener otherPEL;

    //Initial number of specific target crops planted in the world
    public  int startingNumTargetCrops;
    //Starting number of crops + the target number of crops to check for
    public int checkNumTargetCrops;
    //Current Number of target crop in the world 
    public int currentNumTargetCrops; 

    // Update is called once per frame
    void Update()
    {
        CheckForSeedPlanted();
    }
    public override void OnStartListening()
    {
        worldControllerObject = GameObject.FindGameObjectWithTag("world_c");
        worldController = worldControllerObject.GetComponent<WorldController>();

        startingNumTargetCrops = 0; 

        foreach(GameObject cropObject in worldController.activeCropList)
        {
            currentCrop = cropObject.GetComponent<Crop>();
            if(currentCrop.cropname == structToCheck.name)
            {
                startingNumTargetCrops++; 
            }
        }

        checkNumTargetCrops = startingNumTargetCrops + structToCheck.targetValue; 
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
        otherPEL = (PlantingEventListener)otherEventListener;
        this.structToCheck.name = otherPEL.structToCheck.name;
        this.structToCheck.targetValue = otherPEL.structToCheck.targetValue;
    }

    public void SetPlantSeedStruct(PlantSeedStruct targetStruct)
    {
        structToCheck.name = targetStruct.name;
        structToCheck.targetValue = targetStruct.targetValue;
    }

    public void SetPlantSeedEventListener(string name, int targetValue)
    {
        structToCheck.name = name;
        structToCheck.targetValue = targetValue; 
    }
    private void CheckForSeedPlanted()
    {
        currentNumTargetCrops = 0; 
        foreach(GameObject cropObject in worldController.activeCropList)
        {
            currentCrop = cropObject.GetComponent<Crop>(); 
            if(currentCrop.cropname == structToCheck.name)
            {
                currentNumTargetCrops++; 
            }
        }
        if(currentNumTargetCrops == checkNumTargetCrops)
        {
            IsEventCompleted = true; 
        }
        else if(currentNumTargetCrops > startingNumTargetCrops)
        {
            IsEventHasBeenUpdated = true; 
        }
    }
}
