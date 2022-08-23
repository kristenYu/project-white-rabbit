using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayEventListener : AEventListener
{
    private GameObject worldControllerObject;
    private WorldController worldController;

    private bool IsNewDay;

    // Update is called once per frame
    void Update()
    {
        
    }

    //Tells the listener when to start listening to the gameobject 
    public override void OnStartListening()
    {
        worldControllerObject = GameObject.FindGameObjectWithTag("world_c");
        worldController = worldControllerObject.GetComponent<WorldController>();
    }    

    //Clean up for when the Event Listener should be destroyed
    public override void OnEndListening()
    {

    }

    //Is called when the event is updated
    public override void OnEventUpdate()
    {

    }

    //Is called when the event is completed
    public override void OnEventCompleted()
    {

    }

    //set one event listener equal to the other 
    public override void Equals(AEventListener otherEventListener)
    {

    }
}
