using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListenerManager : MonoBehaviour
{

    public List<GameObject> activeEventListeners;

    public GameObject testEventListener;

    private GameObject currentEventListener;
    private GameObject eventListenerTemplate;
    private AEventListener eventListenter; 

    // Start is called before the first frame update
    void Start()
    {
        activeEventListeners = new List<GameObject>();
        CreateEventListener(testEventListener);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateEventListener(GameObject eventListenerType)
    {
        eventListenerTemplate = new GameObject();

        Debug.Log(eventListenerType.GetType());



       // currentEventListener = Instantiate(eventListenerObject, this.transform.position, Quaternion.identity);
        //currentEventListener.transform.SetParent(this.transform);

        //activeEventListeners.Add(currentEventListener);
    }

}
