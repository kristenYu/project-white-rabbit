using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCookingTutorial : MonoBehaviour
{

    public PlayerController playerController;

    //Singleton 
    private static DebugCookingTutorial instance;
    // Read-only public access
    public static DebugCookingTutorial Instance => instance;

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


    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.hasDebugCooking)
        {
            Destroy(this.gameObject);
        }
    }
}
