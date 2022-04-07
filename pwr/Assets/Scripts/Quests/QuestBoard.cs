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
    private Dictionary<QuestType, AEventListener> questtypeToEventListenerMap;
    private PlantingEventListener plantingEventListener;

    //Database loading 
    public Quest[] QuestDataBase;
    public Quest currentQuest;
    public TextAsset[] questFiles;
    public int questDatabaseIndex;
   

    
    private void Awake()
    {
        questDatabaseIndex = 0;
        StartCoroutine(LoadQuestDatabase());
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

    private IEnumerator LoadQuestDatabase()
    {
        //expects that the file is in Resources/Data/Quests

        questFiles = Resources.LoadAll<TextAsset>("Data/Quests");
        QuestDataBase = new Quest[questFiles.Length];
        foreach(TextAsset questFile in questFiles)
        {
            //All quest data except for the event listener - the event listener is instantiated when the quest is accepted;
            currentQuest = JsonUtility.FromJson<Quest>(questFile.ToString());
            QuestDataBase[questDatabaseIndex] = currentQuest;
            questDatabaseIndex++;
        }



        yield return null;
    }
}
