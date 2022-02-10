using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEventListener : MonoBehaviour, IEventListener
{

    //Manager Variables 
    public bool IsEventHasBeenUpdated;
    public bool IsEventCompleted; 

    //Register the event listener with the event listener manager
    public void RegisterToManager()
    {
        //TODO:
        //Get the manager in the do not destroy on load section of the game, and then register itself with the manager
    }

    //Tells the listener when to start listening to the gameobject 
    public abstract void OnStartListening();

    //Clean up for when the Event Listener should be destroyed
    public abstract void OnEndListening();

    //Is called when the event is updated
    public abstract void OnEventUpdate();

    //Is called when the event is completed
    public abstract void OnEventCompleted();
}
