using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public enum CropStage {
        Sprout = 0,
        SmallPlant,
        LargePlant,
        FullyGrown,
        Harvested
    }

    //basic values
    public string cropname;
    //needs to be divisible by 3 in order to match the growing stages
    public int daysUntilReady;
    //0 is the sprout stage, 3 is the fully grown stage, 4 is the harvested stage; 
    public Sprite[] SpriteGrowingArray;
    public CropStage currentStage;
    public GameObject food;

    //growing variables
    public WorldController worldController; 
    public bool isReadyToGrow;
    public int startingDay;
    public int targetDay;
    public WorldController.TOD startingTOD;
    private const int growingStages = 3;
    public int growRate;
    public int nextGrowthStage;

    //sprite rendering 
    private Sprite currentSprite;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        daysUntilReady = 3;
        currentStage = CropStage.Sprout;
        currentSprite = SpriteGrowingArray[(int)currentStage];
        spriteRenderer = GetComponent<SpriteRenderer>();

        isReadyToGrow = false;
        startingDay = worldController.currentDay;
        startingTOD = worldController.currentTOD;
        targetDay = startingDay + daysUntilReady;
        growRate = daysUntilReady / growingStages;
        nextGrowthStage = startingDay + growRate;
    }

    // Update is called once per frame
    void Update()
    {
        checkIsReadyToGrow();
    }
    public void checkIsReadyToGrow()
    {
        if(worldController.currentDay == nextGrowthStage && worldController.currentTOD == startingTOD)
        {
            Debug.Log("Crop is ready to grow");
            isReadyToGrow = true;
            if(nextGrowthStage < targetDay)
            {
                nextGrowthStage += growRate;
            }
        }
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
                spriteRenderer.sprite = currentSprite;
                break;
            case CropStage.SmallPlant:
                currentStage = CropStage.LargePlant;
                currentSprite = SpriteGrowingArray[(int)currentStage];
                spriteRenderer.sprite = currentSprite;
                break;
            case CropStage.LargePlant:
                currentStage = CropStage.FullyGrown;
                currentSprite = SpriteGrowingArray[(int)currentStage];
                spriteRenderer.sprite = currentSprite;
                break;
            default:
                break;
        }
    }    
}

