using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class QuestBoard : MonoBehaviour
{
    //this enum is used for the reinforcement learning algorithm
    public enum QuestType
    {
        invalid = 0,
        plant,
    }

    public Button exitButton;
    public GameObject playerObject;
    private PlayerController playerController;
    private Dictionary<Quest.QuestType, AEventListener> questtypeToEventListenerMap;
    private PlantingEventListener plantingEventListener;

    //Database loading 
    public Quest[] QuestDataBase;
    private TextAsset questFile;
    private string[] questList;
    private char[] endlineDelims;
    private string[] questComponents;
   


    private void Awake()
    {
        endlineDelims = new[] { '\r', '\n' };
        StartCoroutine(LoadQuestDatabase("Quests.txt"));
    }

    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(ExitScene);
        playerController = playerObject.GetComponent<PlayerController>();
        playerController.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitScene()
    {
        playerController.enabled = true;
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private IEnumerator LoadQuestDatabase(string filename)
    {
        //expects that the file is in resources/data
        //moving the file out of resources completely will cause unity to not be able to find the file at all 
        questFile = Resources.Load<TextAsset>("Data/" + filename);
        questList = questFile.text.Split(endlineDelims, StringSplitOptions.RemoveEmptyEntries);

        foreach (string quest in questList)
        {
            questComponents = quest.Split(',');
            //order is questname, amount of reward currency, quest type (as an int), plantstruct.targetnum, plantstruct.targetname

        }


        yield return null;
    }

    private void setupQuestDictionary()
    {
        questtypeToEventListenerMap.Add(Quest.QuestType.plant, plantingEventListener);
    }
}
