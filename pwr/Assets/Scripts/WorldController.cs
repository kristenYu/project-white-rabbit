using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour
{
    public enum TOD
    {
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

    //furniture management 
    public List<GameObject> placedFurnitureObjects;
    private Furniture currentFurnitureScript;
    private GameObject currentFurnitureObject;


    //mushroom management 
    private GameObject newMushroom;
    public GameObject mushroomfood;
    public GameObject MushroomSpawnArea;

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

        //These might have to be balanced later
        //dayDuration = 480.0f; //14 min
        //twilightDuration = 60.0f; //1 min
        //nightDuration = 240.0f; //4 min

        //TESTING VALUES 
        dayDuration = 10.0f;
        twilightDuration = 10.0f;
        nightDuration = 10.0f;

        activeCropList = new List<GameObject>();

        //Spawning Mushrooms

        for (int i = 0; i < 10; i++)
        {
            float spawnX = UnityEngine.Random.Range(MushroomSpawnArea.gameObject.transform.position.x - (MushroomSpawnArea.gameObject.transform.localScale.x/2), MushroomSpawnArea.gameObject.transform.position.x + (MushroomSpawnArea.gameObject.transform.localScale.x/2));
            float spawnY = UnityEngine.Random.Range(MushroomSpawnArea.gameObject.transform.position.y - (MushroomSpawnArea.gameObject.transform.localScale.y/2), MushroomSpawnArea.gameObject.transform.position.y + (MushroomSpawnArea.gameObject.transform.localScale.y/2));
            Vector3 temporary = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0.0f);
            Instantiate(mushroomfood, temporary, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;

        checkValidSceneForCrops();
        checkValidSceneForFurniture();
        growActiveCrops();
        updateTOD(currentTimer);
    }

    public void checkValidSceneForFurniture()
    {
        foreach (GameObject furniture in placedFurnitureObjects)
        {
            if (SceneManager.GetActiveScene().name == "Home")
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
            if (SceneManager.GetActiveScene().name == "Main")
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
        }
    }

    public void updateTOD(float timer)
    {
        if (currentTOD == TOD.Day)
        {
            if (timer >= dayDuration)
            {
                currentTOD = TOD.Twilight;
                currentTimer = 0.0f;
                TODImage.texture = TODIcons[1];
            }
        }
        else if (currentTOD == TOD.Twilight)
        {
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
                currentTOD = TOD.Day;
                currentTimer = 0.0f;
                currentDay++;
                TODText.text = "Day " + currentDay;
                TODImage.texture = TODIcons[0];
            }
        }
    }

    public void setDurationsForTesting(float day, float twilight, float night)
    {
        dayDuration = day;
        twilightDuration = twilight;
        nightDuration = night;
    }
}

//Spawning Mushrooms 

    /*public bool isTimer;


    public object mushroomfood()
    {
        object x = mushroomSpawn.transform.position.x; //9.67
        object mushroomSpawn = null;
        _ = mushroomSpawn.transform.localScale.x; //10
    }
   }

// spawn mushrooms during day (from code: dayDuration = 10.0f;)

/*private void Update ()
{
timer += Time.deltaTime;

if (timer >= 0)
    {

    }
}*/

//from player script (harvesting action): 

/*     //check if the crop can be harvested and harvests if it can 
                cropScript = hit.transform.gameObject.GetComponent<Crop>();
                AddObjectToInventory(cropScript.HarvestCrop());
               // actionFrequencyArray[(int)QuestBoard.QuestType.harvest] += 1;*/

// pressing E to interact with mushrooms - using keycode??

/*if (Input.GetKeyDown(KeyCode.E))
{

}*/