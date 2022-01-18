using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //managing changing time of day
    private float dayDuration;
    private float twilightDuration;
    private float nightDuration;


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

        currentTimer = 0.0f;

        //These might have to be balanced later
        dayDuration = 840.0f; //14 min
        twilightDuration = 60.0f; //1 min
        nightDuration = 240.0f; //4 min
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer += Time.deltaTime;
        if (currentTOD == TOD.Day)
        {
            if (currentTimer >= dayDuration)
            {
                currentTOD = TOD.Twilight;
                currentTimer = 0.0f;
            }
        }
        else if (currentTOD == TOD.Twilight)
        {
            if (currentTimer >= twilightDuration)
            {
                currentTOD = TOD.Night;
                currentTimer = 0.0f;
            }
        }
        else if (currentTOD == TOD.Night)
        {
            if(currentTimer >= nightDuration)
            {
                currentTOD = TOD.Day;
                currentTimer = 0.0f;
                currentDay++;
            }
        }
    }
}
