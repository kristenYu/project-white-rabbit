using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_plant_tutorial : MonoBehaviour
{
    public bool hasPlanted;
    public WorldController worldController;


    private static howto_plant_tutorial instance;
    // Read-only public access
    public static howto_plant_tutorial Instance => instance;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(worldController.activeCropList.Count > 0)
        {
            hasPlanted = true;
            Destroy(this.gameObject);
        }    
    }
}
