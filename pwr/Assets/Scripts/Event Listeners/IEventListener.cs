using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventListener
{
    //Register the event listener with the event listener manager
    public void RegisterToManager();

    //Tells the listener when to start listening to the gameobject 
    public void OnStartListening();

    //Clean up for when the Event Listener should be destroyed
    public void OnEndListening();
    
    //Is called when the event is updated
    public void OnEventUpdate();

    //Is called when the event is completed
    public void OnEventCompleted();

    //set one event listener equal to the other 
    public void Equals();
}
