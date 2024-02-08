using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_plant_tutorial : MonoBehaviour
{
    public bool hasPlanted;
    public WorldController worldController; 
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
