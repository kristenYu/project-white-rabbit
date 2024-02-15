using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHack : MonoBehaviour
{
    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
