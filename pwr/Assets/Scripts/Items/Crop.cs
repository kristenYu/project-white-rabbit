using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public enum CropStage{
        Sprout = 0,
        SmallPlant, 
        LargePlant, 
        FullyGrown, 
        Harvested
    }


    public string cropname;
    public int daysUntilReady;
    //0 is the sprout stage, 3 is the fully grown stage, 4 is the harvested stage; 
    public Sprite[] SpriteGrowingArray;
    public CropStage currentStage;
    public GameObject food;

    private Sprite currentSprite;

    // Start is called before the first frame update
    void Start()
    {
        daysUntilReady = 3;
        currentStage = CropStage.Sprout;
        currentSprite = SpriteGrowingArray[(int)currentStage];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject HarvestCrop()
    {
        if(currentStage == CropStage.FullyGrown)
        {
            return food; 
        }
        else
        {
            return null; 
        }
    }

    public void GrowCrop()
    {
        switch(currentStage)
        {
            case CropStage.Sprout:
                currentStage = CropStage.SmallPlant;
                currentSprite = SpriteGrowingArray[(int)currentStage];
                break;
            case CropStage.SmallPlant:
                currentStage = CropStage.LargePlant;
                currentSprite = SpriteGrowingArray[(int)currentStage];
                break;
            case CropStage.LargePlant:
                currentStage = CropStage.FullyGrown;
                currentSprite = SpriteGrowingArray[(int)currentStage];
                break;
            default:
                break;
        }

    }    
}

