using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : Item
{
    // Start is called before the first frame update

    public Sprite[] spriteArray;
    public int currentIndex; 
    public int maxIndex = 3; 

    void Start()
    {
        currentIndex = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
