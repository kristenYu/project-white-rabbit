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

    //Initial number of specific target crops planted in the world
    public  int startingNumTargetCrops;
    //Starting number of crops + the target number of crops to check for
    public int checkNumTargetCrops;
    //Current Number of target crop in the world 
    public int currentNumTargetCrops; 

    // Start is called before the first frame update
    void Start()
    {
        OnStartListening(); 
    }

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

    }
    public override void OnEventUpdate()
    {

    }
    public override void OnEventCompleted()
    {

    }

    public void SetPlantSeedStruct(PlantSeedStruct targetStruct)
    {
        structToCheck.name = targetStruct.name;
        structToCheck.targetValue = targetStruct.targetValue;
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
