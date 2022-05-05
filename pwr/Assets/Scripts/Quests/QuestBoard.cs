using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class QuestBoard : MonoBehaviour
{
    //this enum is used for the reinforcement learning algorithm and for PaSSAGE
    public enum QuestType
    {
        plant = 0,
        harvest, 
        place,
        invalid, //this is always the maximum number of quest categories
    }

    public Button exitButton;
    public GameObject playerObject;
    private PlayerController playerController;
    private PlantingEventListener plantingEventListener;

    //Database loading 
    public Quest[] questDataBase;
    public Quest currentQuest;
    public TextAsset[] questFiles;
    public int questDatabaseIndex;

    //Questboard Quests 
    public const int numberOfQuests = 3; 
    public Quest[] displayQuests;
    public GameObject[] questAlgorithms; //Should be set to the number of algorithms 
    public int questAlgorithmIndex; //quest algorithm to use
    private QuestAlgorithmBase currentQuestAlgorithm;
    private GameObject currentQuestGameObject;

    public Quest[] questsToSubmit; 

    //Quest UI 
    public GameObject[] QuestUIObjects;//Should always be matching the number of quests - 0 is left most, max is right most
    private TextMeshProUGUI questName;
    private Button questAcceptButton;
    public GameObject[] submitQuestUIObjects; //Should always be matching the maximum number of quests that can be active
    private TextMeshProUGUI submitQuestName;
    private Button questSubmitButton; 

    private void Awake()
    {
        questDatabaseIndex = 0;
        StartCoroutine(LoadQuestDatabase());
    }

    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(ExitScene);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();

        questsToSubmit = new Quest[PlayerController.maxActiveQuests];
        PopulateQuestBoard();

        questAlgorithmIndex = 0; //default value 

        //run set up for all algorithms 
        for(int i = 0; i < questAlgorithms.Length; i++)
        {
            currentQuestAlgorithm = questAlgorithms[i].GetComponent<QuestAlgorithmBase>();
            currentQuestAlgorithm.SetUpAlgorithm();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitScene()
    {
        playerController.enabled = true;
        currentQuestAlgorithm.OnQuestClosed(); 
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private IEnumerator LoadQuestDatabase()
    {
        //expects that the file is in Resources/Data/Quests

        questFiles = Resources.LoadAll<TextAsset>("Data/Quests");
        questDataBase = new Quest[questFiles.Length];
        foreach(TextAsset questFile in questFiles)
        {
            //All quest data except for the event listener - the event listener is instantiated when the quest is accepted;
            currentQuest = JsonUtility.FromJson<Quest>(questFile.ToString());
            questDataBase[questDatabaseIndex] = currentQuest;
            questDatabaseIndex++;
        }
        yield return null;
    }

    public void PopulateQuestBoard()
    {
        //Get Quests to show
        currentQuestAlgorithm = questAlgorithms[questAlgorithmIndex].GetComponent<QuestAlgorithmBase>();
        displayQuests = currentQuestAlgorithm.GetQuests(numberOfQuests, questDataBase);

        for(int i = 0; i < displayQuests.Length; i++)
        {
            Debug.Log(displayQuests[i].questName);
            SetQuestUI(QuestUIObjects[i], displayQuests[i]);
        }

        //Get quests to submit
        for(int i = 0; i < playerController.activeQuests.Length; i++)
        {
            //this is going to look weird - needs a clean up pass
            if (playerController.activeQuests[i].questType == QuestType.invalid)
            {
                submitQuestUIObjects[i].SetActive(false);
            }
            else
            {
                if (playerController.activeQuests[i].eventListener.IsEventCompleted)
                {
                    
                    submitQuestUIObjects[i].SetActive(true);
                    SetSubmitQuestUI(submitQuestUIObjects[i], playerController.activeQuests[i]);
                }
                else
                {
                    submitQuestUIObjects[i].SetActive(false);
                }
            }
           
        }
    }

    public void SetQuestUI(GameObject questUIObject, Quest quest)
    {
        //assumes that the prefab is used to create the quest UI in a particular order
        questAcceptButton = questUIObject.GetComponent<Button>();
        questName = questUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        questName.text = quest.questName; 
        questAcceptButton.onClick.AddListener(delegate {AcceptQuest(quest); });
    }

    public void SetSubmitQuestUI(GameObject submitQuestUIObject, Quest quest)
    {
        //assumes that the prefab is used to create the submit quest UI in a particular order
        questSubmitButton = submitQuestUIObject.GetComponent<Button>(); 
        submitQuestName = submitQuestUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        submitQuestName.text = quest.questName;
        questSubmitButton.onClick.AddListener(delegate { SubmitQuest(quest); });
    }

    public void AcceptQuest(Quest quest)
    {
        //Instantiate Event Listener
        switch(quest.questType)
        {
            case QuestType.plant:
                //expects the plant event listening data to be formatted as name of plant, and target number in the format of strings
                if(!Int32.TryParse(quest.eventListenerData[1], out int result))
                {
                    Debug.LogError("Accept Quest Failed for quest " + quest.questName + "; could not instantiate PlantingEventListener. Check eventListenerData is formatted as [\"targetPlant\", \"targetValue\"]");
                }
                currentQuestGameObject = new GameObject();
                currentQuestGameObject.name = quest.questName + "_EL";
                currentQuestGameObject.transform.SetParent(playerObject.transform); 
                currentQuestGameObject.AddComponent<PlantingEventListener>();
                
                plantingEventListener = currentQuestGameObject.GetComponent<PlantingEventListener>();
                plantingEventListener.SetPlantSeedEventListener(quest.eventListenerData[0], Int32.Parse(quest.eventListenerData[1]));
                
                quest.eventListener = plantingEventListener;
                quest.eventListener.OnStartListening(); 
                playerController.AddQuestToActiveArray(quest);

                currentQuestAlgorithm.OnQuestAccepted(quest);
                break;
            default:
                break; 
        }
    }
    public void SubmitQuest(Quest quest)
    {
        Debug.Log("Submitted quest: " + quest.questName);
        playerController.addCurrency(quest.reward);
        currentQuestAlgorithm.OnQuestSubmitted(); 
    }
}
