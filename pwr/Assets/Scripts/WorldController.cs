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
    public GameObject MushroomSpawnArea2;
    public GameObject MushroomSpawnArea3;
    int index;
    int spawnX;
    int spawnY;
    private float mushroomsPosition; 

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

        //Randomly spawning Mushrooms

        for (int i = 0; i < 3; i++)
        {
            float spawnX = UnityEngine.Random.Range(MushroomSpawnArea.gameObject.transform.position.x - (MushroomSpawnArea.gameObject.transform.localScale.x/2), MushroomSpawnArea.gameObject.transform.position.x + (MushroomSpawnArea.gameObject.transform.localScale.x/2));
            float spawnY = UnityEngine.Random.Range(MushroomSpawnArea.gameObject.transform.position.y - (MushroomSpawnArea.gameObject.transform.localScale.y/2), MushroomSpawnArea.gameObject.transform.position.y + (MushroomSpawnArea.gameObject.transform.localScale.y/2));
             Vector3 spawnPosition = new(spawnX, spawnY, 0.0f);
            Instantiate(mushroomfood, spawnPosition, Quaternion.identity);
        }

        if (MushroomSpawnArea == null)
            MushroomSpawnArea = GameObject.FindWithTag("mushroom");

        Instantiate(mushroomfood, MushroomSpawnArea.transform.position, MushroomSpawnArea.transform.rotation);

        for (int i = 0; i < 3; i++)
        {
            float spawnX = UnityEngine.Random.Range(MushroomSpawnArea2.gameObject.transform.position.x - (MushroomSpawnArea2.gameObject.transform.localScale.x / 2), MushroomSpawnArea2.gameObject.transform.position.x + (MushroomSpawnArea2.gameObject.transform.localScale.x / 2));
            float spawnY = UnityEngine.Random.Range(MushroomSpawnArea2.gameObject.transform.position.y - (MushroomSpawnArea2.gameObject.transform.localScale.y / 2), MushroomSpawnArea2.gameObject.transform.position.y + (MushroomSpawnArea2.gameObject.transform.localScale.y / 2));
            Vector3 spawnPosition = new(spawnX, spawnY, 0.0f);
            Instantiate(mushroomfood, spawnPosition, Quaternion.identity);
        }

        if (MushroomSpawnArea2 == null)
            MushroomSpawnArea2 = GameObject.FindWithTag("mushroom");

        Instantiate(mushroomfood, MushroomSpawnArea2.transform.position, MushroomSpawnArea2.transform.rotation);

        for (int i = 0; i < 3; i++)
        {
            float spawnX = UnityEngine.Random.Range(MushroomSpawnArea3.gameObject.transform.position.x - (MushroomSpawnArea3.gameObject.transform.localScale.x / 2), MushroomSpawnArea3.gameObject.transform.position.x + (MushroomSpawnArea3.gameObject.transform.localScale.x / 2));
            float spawnY = UnityEngine.Random.Range(MushroomSpawnArea3.gameObject.transform.position.y - (MushroomSpawnArea3.gameObject.transform.localScale.y / 2), MushroomSpawnArea3.gameObject.transform.position.y + (MushroomSpawnArea3.gameObject.transform.localScale.y / 2));
            Vector3 spawnPosition = new(spawnX, spawnY, 0.0f);
            Instantiate(mushroomfood, spawnPosition, Quaternion.identity);
        }

        if (MushroomSpawnArea3 == null)
            MushroomSpawnArea3 = GameObject.FindWithTag("mushroom");

        Instantiate(mushroomfood, MushroomSpawnArea3.transform.position, MushroomSpawnArea3.transform.rotation);

        //mushrooms don't spawn on top of each other

        PlayerPrefs.SetInt("Spawn X", spawnX);
        PlayerPrefs.SetInt("Spawn Y", spawnY);
        PlayerPrefs.SetFloat("Mushrooms' Position", mushroomsPosition);

        _ = PlayerPrefs.GetInt("Spawn X");
        spawnY = PlayerPrefs.GetInt("Spawn Y");
        _ = PlayerPrefs.GetFloat("Mushrooms' Position");

        GameObject[] mushroomsInTheScene = GameObject.FindGameObjectsWithTag("mushroom");

        float[] mushroomsX = new float[12];
        float[] mushroomsY = new float[12];

        for (int i = 0; i < mushroomsX.Length; i++)
        {
            System.Console.WriteLine(mushroomsX[i]);
            mushroomsInTheScene[i].gameObject.SetActive(false);
            mushroomsInTheScene[index].gameObject.SetActive(true);
            mushroomsY[index] = spawnY;
        }

        for (int i = 0; i < mushroomsY.Length; i++)
        {
            System.Console.WriteLine(mushroomsY[i]);
            mushroomsInTheScene[i].gameObject.SetActive(false);
            mushroomsInTheScene[index].gameObject.SetActive(true);
            mushroomsY[index] = spawnY;
        }

        index += 1;

        Debug.Log(index);

        PlayerPrefs.SetInt("index", index);
        PlayerPrefs.Save();

        if (ColliderDistance2D.mushroomsX[i] < 1)
             Instantiate(mushroomfood);
             Destroy(mushroomsX[i]);

        if(ColliderDistance2D.mushroomsY[i] < 1)
             Instantiate(mushroomfood);
             Destroy(mushroomsY[i]);

    }

    //Debug.log to get value of variable

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;

        checkValidSceneForCrops();
        checkValidSceneForFurniture();
        growActiveCrops();
        updateTOD(currentTimer);
        InvokeRepeating("mushroom", 0, 30.0f);
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

