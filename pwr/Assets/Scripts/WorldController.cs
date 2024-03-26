using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour
{
    public enum TOD {
        Day = 0, //10 minutes
        Twilight, // 1 minute
        Night // 4 minutes
    }

    public int currentDay;
    public TOD currentTOD;
    public float currentTimer;

    //HUD Variables
    public TextMeshProUGUI TODText;
    public RawImage TODImage;
    public Texture[] TODIcons = new Texture[3];

    //managing changing time of day
    public float dayDuration;
    public float twilightDuration;
    public float nightDuration;

    //crop management
    public List<GameObject> activeCropList;
    private Crop currentCropScript;
    private List<GameObject> destroyCropList; 

    //furniture management 
    public List<GameObject> placedFurnitureObjects;
    private Furniture currentFurnitureScript;
    private GameObject currentFurnitureObject;

    //shop Management 
    public ShopScript shopScript;
    public bool isNewDay;

    //quest management 
    public bool isNewDayQuests;

    //harvestable
    public List<GameObject> mushroomHarvestableList;
    public List<GameObject> berryHarvestableList;
    public const int maxMushroomSpawn = 5;
    public const int maxBerrySpawn = 5;
    public bool shouldSpawnMushroom;
    public bool shouldSpawnBerries;
    public int mushroomAmountToSpawn; 
    public int berryAmountToSpawn; 
    public HarvestableSpawner mushroomSpawner;
    public HarvestableSpawner berrySpawner;

    //filters 
    public GameObject nightFilter;
    public GameObject twilightFilter;
    public Vector3 foregroundPosition;
    public Vector3 backgroundPosition;

    //Tutorial
    public List<GameObject> tutorialCropList;
    public GameObject tutorialHarvestable;
    public Telemetry_Util telemetryUtil;
    public GameObject[] invalidTutorialCropArray;
    public bool hasSetCropToFulllyGrown;

    //audio
    public AudioSource audioSource;
    public AudioClip newDayClip;
    public AudioClip growCropsClip;
    public bool hasPlayedGrowCropsClip;

    //Singleton 
    private static WorldController instance;
    // Read-only public access
    public static WorldController Instance => instance;

    private void Awake()
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
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        //this will have to be preloaded
        currentDay = 1;
        currentTOD = TOD.Day;
        TODText.text = "Day " + currentDay;
        TODImage.texture = TODIcons[0];

        currentTimer = 0.0f;

        //bool for reseting the shop
        isNewDay = false;
        //bool for resetting quests
        isNewDayQuests = false;


        //Days are one minute total to complete
        dayDuration = 30.0f;
        twilightDuration = 10.0f;
        nightDuration = 20.0f;

        //activeCropList = new List<GameObject>();
        destroyCropList = new List<GameObject>();        
        //harvestableList = new List<GameObject>();
        

        nightFilter = GameObject.FindGameObjectWithTag("night_filter");
        twilightFilter = GameObject.FindGameObjectWithTag("twilight_filter");
        foregroundPosition = new Vector3(nightFilter.transform.position.x, nightFilter.transform.position.y, 10f);
        backgroundPosition = new Vector3(nightFilter.transform.position.x, nightFilter.transform.position.y, -10f);

        telemetryUtil = GameObject.FindGameObjectWithTag("telemetry").GetComponent<Telemetry_Util>();
      

        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        hasPlayedGrowCropsClip = false;
        hasSetCropToFulllyGrown = false;

        mushroomAmountToSpawn = 3;
        berryAmountToSpawn = 3;
        //spawn mushrooms on start
        /*
        if (SceneManager.GetActiveScene().name == "Main")
        {
            mushroomSpawner = GameObject.FindGameObjectWithTag("mushroom_spawner").GetComponent<HarvestableSpawner>();
            berrySpawner = GameObject.FindGameObjectWithTag("berry_spawner").GetComponent<HarvestableSpawner>();

            mushroomSpawner.SpawnNewHarvestable(mushroomAmountToSpawn, mushroomHarvestableList, this);
            berrySpawner.SpawnNewHarvestable(berryAmountToSpawn, berryHarvestableList, this);
        }
        */
       
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;

        checkValidSceneForCrops();
        checkValidSceneForFurniture();
        checkValidSceneForHarvestables();
        checkValidSceneForFilters();
        growActiveCrops();
        updateTOD(currentTimer);

        if(SceneManager.GetActiveScene().name == "Tutorial")
        {
            //tutorial 
            invalidTutorialCropArray = GameObject.FindGameObjectsWithTag("crop");
            for (int i = 0; i < invalidTutorialCropArray.Length; i++)
            {
                if (!tutorialCropList.Contains(invalidTutorialCropArray[i]))
                {
                    if (!activeCropList.Contains(invalidTutorialCropArray[i]))
                    {
                        Destroy(invalidTutorialCropArray[i]);
                    }

                }
            }
            if(!hasSetCropToFulllyGrown)
            {
                foreach (GameObject tutorialCrop in tutorialCropList)
                {
                    tutorialCrop.GetComponent<Crop>().currentStage = Crop.CropStage.FullyGrown;
                    tutorialCrop.GetComponent<Crop>().isReadyToGrow = false;
                    tutorialCrop.GetComponent<SpriteRenderer>().sprite = tutorialCrop.GetComponent<Crop>().SpriteGrowingArray[3];
                }
                hasSetCropToFulllyGrown = true;
            }
        } 
    }

    public void checkValidSceneForFurniture()
    {
        foreach (GameObject furniture in placedFurnitureObjects)
        {
            if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "TutorialHome")
            {
                furniture.GetComponent<SpriteRenderer>().enabled = true;
                furniture.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                furniture.GetComponent<SpriteRenderer>().enabled = false;
                furniture.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void checkValidSceneForCrops()
    {
        foreach (GameObject crop in activeCropList)
        {
            if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Tutorial")
            {
                crop.GetComponent<SpriteRenderer>().enabled = true;
                crop.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                crop.GetComponent<SpriteRenderer>().enabled = false;
                crop.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public void checkValidSceneForHarvestables()
    {
        foreach (GameObject harvestable in mushroomHarvestableList)
        {
            if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Tutorial")
            {
                if(harvestable != null)
                {
                    setHarvestable(harvestable, true);
                }
              
            }
            else
            {
                if(harvestable != null)
                {
                    setHarvestable(harvestable, false);
                }
            }
        }
        foreach (GameObject harvestable in berryHarvestableList)
        {
            if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Tutorial")
            {
                if (harvestable != null)
                {
                    setHarvestable(harvestable, true);
                }
                
            }
            else
            {
                if (harvestable != null)
                {
                    setHarvestable(harvestable, false);
                }
            }
        }
    }

    public void checkValidSceneForFilters()
    {
        
        if (SceneManager.GetActiveScene().name == "Main" || SceneManager.GetActiveScene().name == "Tutorial")
        {
            twilightFilter = GameObject.FindGameObjectWithTag("twilight_filter");
            nightFilter = GameObject.FindGameObjectWithTag("night_filter");
         
            if (currentTOD == TOD.Day)
            {
                nightFilter.transform.position = backgroundPosition;
                twilightFilter.transform.position = backgroundPosition; 
            }
            else if (currentTOD == TOD.Twilight)
            {
                nightFilter.transform.position = backgroundPosition;
                twilightFilter.transform.position = foregroundPosition; 
             }
            else if(currentTOD == TOD.Night)
            {
                nightFilter.transform.position = foregroundPosition;
                twilightFilter.transform.position = backgroundPosition; 
             }
            
        }
        
    }

    public void growActiveCrops()
    {
        foreach (GameObject crop in activeCropList)
        {
            currentCropScript = crop.GetComponent<Crop>();
            if (currentCropScript.isReadyToGrow)
            {
                currentCropScript.GrowCrop();
                currentCropScript.isReadyToGrow = false; 
            }
            else if(crop.GetComponent<Crop>().currentStage == Crop.CropStage.Harvested)
            {
                destroyCropList.Add(crop);
            }
        }
        for(int i = 0; i < destroyCropList.Count; i++)
        {
            activeCropList.Remove(destroyCropList[i]);
            Destroy(destroyCropList[i]);
        }
        destroyCropList.Clear();
    }

    public void updateTOD(float timer)
    {
        if (currentTOD == TOD.Day)
        {
            if (timer >= dayDuration)
            {
                foreach (GameObject crop in activeCropList)
                {
                    currentCropScript = crop.GetComponent<Crop>();
                    if (currentCropScript.isReadyToGrow)
                    {
                        audioSource.PlayOneShot(growCropsClip);
                        break;
                    }
                }
                    currentTOD = TOD.Twilight;
                currentTimer = 0.0f;
                TODImage.texture = TODIcons[1];
            }
        }
        else if (currentTOD == TOD.Twilight)
        {
            foreach (GameObject crop in activeCropList)
            {
                currentCropScript = crop.GetComponent<Crop>();
                if (currentCropScript.isReadyToGrow)
                {
                    audioSource.PlayOneShot(growCropsClip);
                    break;
                }
            }
            if (timer >= twilightDuration)
            {
                currentTOD = TOD.Night;
                currentTimer = 0.0f;
                TODImage.texture = TODIcons[2];
            }
        }
        else if (currentTOD == TOD.Night)
        {
            if (timer >= nightDuration)
            {
                foreach (GameObject crop in activeCropList)
                {
                    currentCropScript = crop.GetComponent<Crop>();
                    if (currentCropScript.isReadyToGrow)
                    {
                        audioSource.PlayOneShot(growCropsClip);
                        break;
                    }
                }
                audioSource.PlayOneShot(newDayClip);
                currentTOD = TOD.Day;
                currentTimer = 0.0f;
                currentDay++;
                TODText.text = "Day " + currentDay;
                TODImage.texture = TODIcons[0];
                //set to false in the shop script
                isNewDay = true;
                //set to true in quest script
                isNewDayQuests = true;
                //count hte number of berreis and mushrooms to spawn
                if (maxMushroomSpawn - mushroomHarvestableList.Count > HarvestableSpawner.harvestableSpawnNumber)
                {
                    mushroomAmountToSpawn = HarvestableSpawner.harvestableSpawnNumber;
                }
                else
                {
                    mushroomAmountToSpawn = maxMushroomSpawn - mushroomHarvestableList.Count;
                }

                if (maxBerrySpawn - berryHarvestableList.Count > HarvestableSpawner.harvestableSpawnNumber)
                {
                    berryAmountToSpawn = HarvestableSpawner.harvestableSpawnNumber;
                }
                else
                {
                    berryAmountToSpawn = maxBerrySpawn - berryHarvestableList.Count;
                }
                if(SceneManager.GetActiveScene().name == "Main")
                {
                    berrySpawner = GameObject.FindGameObjectWithTag("berry_spawner").GetComponent<HarvestableSpawner>();
                    mushroomSpawner = GameObject.FindGameObjectWithTag("mushroom_spawner").GetComponent<HarvestableSpawner>();
                    berrySpawner.SpawnNewHarvestable(berryAmountToSpawn, berryHarvestableList, this);
                    mushroomSpawner.SpawnNewHarvestable(mushroomAmountToSpawn, mushroomHarvestableList, this);
                    berryAmountToSpawn = 0;
                    mushroomAmountToSpawn = 0;
                }

                StartCoroutine(telemetryUtil.PostData("Event:Day" + currentDay.ToString()));
            }
        }
    }

    public void setDurationsForTesting(float day, float twilight, float night)
    {
        dayDuration = day;
        twilightDuration = twilight;
        nightDuration = night; 
    }

    private void setHarvestable(GameObject harvestable, bool value)
    {
        harvestable.GetComponent<SpriteRenderer>().enabled = value;
        harvestable.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = value;
        harvestable.GetComponent<BoxCollider2D>().enabled = value;
    }
}
