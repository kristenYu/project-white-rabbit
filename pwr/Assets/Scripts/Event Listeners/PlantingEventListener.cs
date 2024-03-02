using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs;
using TMPro;

public class PlantingEventListener : AEventListener
{
    public PlayerController playerController;
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
        playerController = this.GetComponentInParent<PlayerController>();

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
                //search for an active placing quest
                for (int j = 0; j < playerController.activeQuests.Length; j++)
                {
                    
                    if(playerController.activeQuests[j] != null)
                    {
                        if (playerController.activeQuests[j].questType == QuestBoard.QuestType.plant)
                        {
                            if (playerController.activeQuests[j].eventListenerData[0].Contains(structToCheck.name))
                            {
                                //find the place quest that matches in the UI
                                for (int k = 0; k < playerController.questHudObjectArray.Length; k++)
                                {
                                    //check is quest name is the same
                                    if (playerController.activeQuests[j].questName.Contains(playerController.questHudObjectArray[k].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text))
                                    {
                                        playerController.questHudObjectArray[k].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = currentNumTargetCrops.ToString();
                                    }
                                }
                            }
                        }
                    }
                   
                }
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
