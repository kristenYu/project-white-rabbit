using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_interact_tutorial : MonoBehaviour
{
    public bool hasInteracted;
    public PlayerController playerController; 
    // Start is called before the first frame update
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
