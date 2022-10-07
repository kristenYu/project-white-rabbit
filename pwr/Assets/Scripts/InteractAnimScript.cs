using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAnimScript : MonoBehaviour
{
    public Sprite sprite0;
    public Sprite sprite1;

    public Sprite[] spriteAnimArray;
    public int spriteAnimIndex; 

    private SpriteRenderer spriteRenderer;
    private float switchFrame;
    public float timer; 

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = 0.0f;
        switchFrame = 0.9f; 
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer > switchFrame)
        {
            if(spriteAnimIndex == spriteAnimArray.Length - 1)
            {
                spriteAnimIndex = 0; 
            }
            else
            {
                spriteAnimIndex++;
            }
            spriteRenderer.sprite = spriteAnimArray[spriteAnimIndex];
            timer = 0.0f; 
        }
    }
}
