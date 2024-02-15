using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_cook_tutorial : MonoBehaviour
{
    public PlayerController playerController;
    public bool hasCooked;
    // Start is called before the first frame update

    //Singleton 
    private static howto_cook_tutorial instance;
    // Read-only public access
    public static howto_cook_tutorial Instance => instance;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
       for(int i = 0; i < playerController.inventory.Length; i++)
       {
            if(playerController.inventory[i] != null)
            {
                if (playerController.inventory[i].gameObject.GetComponent<CookedFood>() != null)
                {
                    hasCooked = true;
                }
            }
       }
       if(hasCooked)
       {
           Destroy(this.gameObject);
       }
    }
}
