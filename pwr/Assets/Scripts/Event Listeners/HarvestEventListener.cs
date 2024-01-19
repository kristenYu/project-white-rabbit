using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs; 




public class HarvestEventListener : AEventListener
{
    public HarvestStruct structToCheck;
    private HarvestEventListener otherHEL;
    private PlayerController playerController;

    public int currentHarvestedNum;
    public int targetHarvestedNum; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForHarvest();
    }

    public override void OnStartListening()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.hasHarvestedItem = false; 
        currentHarvestedNum = 0;
        targetHarvestedNum = structToCheck.targetValue; 
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
        otherHEL = (HarvestEventListener)otherEventListener;
        this.structToCheck.name = otherHEL.structToCheck.name;
        this.structToCheck.targetValue = otherHEL.structToCheck.targetValue;
    }

    public void SetHarvestStruct(HarvestStruct targetStruct)
    {
        structToCheck.name = targetStruct.name;
        structToCheck.targetValue = targetStruct.targetValue;
    }

    public void SetHarvestEventListener(string name, int targetValue)
    {
        structToCheck.name = name;
        structToCheck.targetValue = targetValue;
    }

    private void CheckForHarvest()
    {
        //flag is turned on in player controller script
        if (playerController.hasHarvestedItem)
        {
            if (playerController.justHarvestedName == structToCheck.name)
            { 
                currentHarvestedNum++;
            }
        }
        if (currentHarvestedNum >= targetHarvestedNum)
        {
            IsEventCompleted = true;
        }

        else if (currentHarvestedNum > 0)
        {
            IsEventHasBeenUpdated = true;
        }
        playerController.hasHarvestedItem = false;
    }
}
