using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_harvest_tutorial : MonoBehaviour
{

    public bool hasHarvested;
    public WorldController worldController;
    public PlayerController playerController;
    // Start is called before the first frame update

    //Singleton 
    private static howto_harvest_tutorial instance;
    // Read-only public access
    public static howto_harvest_tutorial Instance => instance;

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
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < worldController.tutorialCropList.Count; i++)
        {
            if(worldController.tutorialCropList[i].GetComponent<Crop>().currentStage == Crop.CropStage.Harvested)
            {
                hasHarvested = true;
            }
        }

        if(worldController.tutorialHarvestable == null)
        {
            hasHarvested = true;
        }
        

        if(hasHarvested)
        {
            Destroy(this.gameObject);
        }
    }
}
