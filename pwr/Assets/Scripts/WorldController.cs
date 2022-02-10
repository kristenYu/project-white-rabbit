using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour
{
    public enum TOD{
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
        foreach(GameObject furniture in placedFurnitureObjects)
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
        foreach(GameObject crop in activeCropList)
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
