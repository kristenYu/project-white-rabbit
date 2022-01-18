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


    // Start is called before the first frame update
    void Start()
    {
        //this will have to be preloaded
        currentDay = 1;
        currentTOD = TOD.Day;

        currentTimer = 0.0f;

        //These might have to be balanced later
        //dayDuration = 840.0f; //14 min
        dayDuration = 10.0f;
        //twilightDuration = 60.0f; //1 min
        twilightDuration = 10.0f;
        //nightDuration = 240.0f; //4 min
        nightDuration = 10.0f;
       
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
