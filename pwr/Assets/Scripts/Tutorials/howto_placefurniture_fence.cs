using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_placefurniture_fence : MonoBehaviour
{
    public bool hasPlacedFurniture;
    PlayerController playerController;

    //Singleton 
    private static howto_placefurniture_fence instance;
    // Read-only public access
    public static howto_placefurniture_fence Instance => instance;

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
        if(playerController.hasPlacedFurniture)
        {
            Destroy(this.gameObject);
        }
    }
}
