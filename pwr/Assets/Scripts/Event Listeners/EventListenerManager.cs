using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventListenerStructs; 


public class EventListenerManager : MonoBehaviour
{

    public List<GameObject> activeEventListeners;

    public GameObject testEventListener;
    public AEventListener testEventListenerType; 

    private AEventListener currentEventListener;
    private GameObject eventListenerObject; 
    //private GameObject eventListenerTemplate;
    //private AEventListener eventListener; 

    // Start is called before the first frame update
    void Start()
    {
        activeEventListeners = new List<GameObject>();
        CreateEventListener(testEventListenerType);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForUpdatedOrCompletedEventListener();
    }
    public void CreateEventListener(AEventListener eventListenerToCreate)
    {
        eventListenerObject = new GameObject(eventListenerToCreate.GetType().ToString());
        eventListenerObject.AddComponent(eventListenerToCreate.GetType());
        eventListenerObject.transform.SetParent(this.transform);
        currentEventListener = eventListenerObject.GetComponent<AEventListener>();
        currentEventListener.Equals(eventListenerToCreate);
        activeEventListeners.Add(eventListenerObject);
    }

    public void CheckForUpdatedOrCompletedEventListener()
    {

        for (int i = 0; i < activeEventListeners.Count; i++)
        {
            currentEventListener = activeEventListeners[i].GetComponent<AEventListener>();
            if (currentEventListener.IsEventCompleted == true)
            {
                currentEventListener.OnEventCompleted();
                currentEventListener.OnEndListening();
                eventListenerObject = activeEventListeners[i]; 
                activeEventListeners.RemoveAt(i);
                Destroy(eventListenerObject);
            }
            else if (currentEventListener.IsEventHasBeenUpdated == true)
            {
                currentEventListener.OnEventUpdate();
                currentEventListener.IsEventHasBeenUpdated = false;
            }
        }
    }
}
