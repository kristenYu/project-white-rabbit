using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteratableUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       if(SceneManager.GetActiveScene().name == "Main")
       {
            this.GetComponent<SpriteRenderer>().enabled = true; 
       }
       else
       {
            this.GetComponent<SpriteRenderer>().enabled = false;
       }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
