using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_interact_tutorial : MonoBehaviour
{
    public bool hasInteracted;
    public PlayerController playerController;
    // Start is called before the first frame update
    //Singleton 
    private static howto_interact_tutorial instance;
    // Read-only public access
    public static howto_interact_tutorial Instance => instance;

    void Awake()
    {

        // Does another instance already exist?
        if (instance && instance != this)
        {
            // Destroy myself
            Destroy(gameObject);
            return;
        }

        // Otherwise store my reference and make me DontDestroyOnLoad
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        hasInteracted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.inventory[0] != null)
        {
            hasInteracted = true;
            Destroy(this.gameObject);
        }

       
    }

  
}
