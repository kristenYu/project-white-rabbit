using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_harvest_tutorial : MonoBehaviour
{

    public bool hasHarvested;
    public WorldController worldController; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < worldController.tutorialCropList.Count; i++)
        {
            if(worldController.tutorialCropList[i].GetComponent<Crop>().currentStage == Crop.CropStage.Harvested)
            {
                hasHarvested = true;
                Destroy(this.gameObject);
            }
        }
    }
}
