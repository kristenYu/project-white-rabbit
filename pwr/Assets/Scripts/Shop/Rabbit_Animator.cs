using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Rabbit_Animator : MonoBehaviour
{
    public enum AnimState
    {
        idle = 0,
        talk, 
        wave
    }
    
    
    public Sprite[] idleAnim;
    public Sprite[] talkAnim;
    public Sprite[] waveAnim;
    public Sprite[] currentAnim;
    public int animIndex;

    public GameObject speechBubble;
    public TextMeshProUGUI speechText;


    public AnimState animState;
    private Image image;

    public bool isContextScene;


    private float switchFrame;
    private float timer; 

    void Start()
    {
        animState = AnimState.idle;
        switchFrame = 0.9f; // 1 second between frames
        timer = 0.0f; //start at 0
        image = GetComponent<Image>();
        animIndex = 0;
        setAnimation(animState);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; 
        if(timer >= switchFrame)
        {
            //iterate to next anim sprite
            if(animIndex == currentAnim.Length-1)
            {
                setAnimation(AnimState.idle);
                animIndex = 0; 
            }
            else
            {
                animIndex += 1; 
            }
            image.sprite = currentAnim[animIndex];

            //reset timer
            timer = 0.0f;
        }
        if(isContextScene)
        {
            setAnimation(AnimState.wave);
        }
        
    }

    public void setAnimation(AnimState state)
    {
        switch(state)
        {
            case AnimState.idle:
                currentAnim = idleAnim;
                speechBubble.SetActive(false);
                break;
            case AnimState.talk:
                speechBubble.SetActive(true);
                currentAnim = talkAnim;
                break;
            case AnimState.wave:
                speechBubble.SetActive(false);
                currentAnim = waveAnim;
                break;
            default:
                currentAnim = idleAnim;
                break;
        }
    }
}
