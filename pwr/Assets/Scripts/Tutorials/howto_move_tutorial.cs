using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_move_tutorial : MonoBehaviour
{
    public bool hasMovedHor;
    public bool hasMovedVer;
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
