using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_move_tutorial : MonoBehaviour
{
    public bool hasMovedHor;
    public bool hasMovedVer;

    //Singleton 
    private static howto_move_tutorial instance;
    // Read-only public access
    public static howto_move_tutorial Instance => instance;

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
        hasMovedHor = false;
        hasMovedVer = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            hasMovedHor = true; 
        }
        if(Input.GetAxisRaw("Vertical") != 0)
        {
            hasMovedVer = true; 
        }

        if(hasMovedHor && hasMovedVer)
        {
            Destroy(this.gameObject);
        }
        
    }
}
