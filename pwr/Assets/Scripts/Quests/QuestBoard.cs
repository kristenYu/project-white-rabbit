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
        cook,
        place,
        invalid, //this is always the maximum number of quest categories
    }

    public Button exitButton;
    public GameObject playerObject;
    private PlayerController playerController;
    private PlantingEventListener plantingEventListener;
    private CookingEventListener cookingEventListener;
    private PlaceEventListener placeEventListener;

    //Algorithm Toggles 
    public Toggle randomToggle;
    public Toggle cmabToggle;
    public Toggle passageToggle; 

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
    private int result;

    //Quest UI 
    public GameObject[] QuestUIObjects;//Should always be matching the number of quests - 0 is left most, max is right most
    public GameObject[] QuestAcceptPanelObjects; //Should always be matching the number of quests - 0 is the left most, max is the right most
    private TextMeshProUGUI questName;
    private Button questAcceptButton;
    public GameObject[] submitQuestUIObjects; //Should always be matching the maximum number of quests that can be active
    public TextMeshProUGUI currentAcceptedQuestNumText;
    private TextMeshProUGUI submitQuestName;
    private Button questSubmitButton;
    private TextMeshProUGUI rewardText;
    private int questHudIndexToRemove;
    private int questHudIndex;

    //Quest HUD
    public GameObject[] questHudGameobjectArray;
    private TextMeshProUGUI questHudTMP;
    private TextMeshProUGUI questHudReward;
    public int currentQuestHudIndex;

    private void Awake()
    {
        questDatabaseIndex = 0;
        StartCoroutine(LoadQuestDatabase());
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();
        //ASSUMES THAT THE CANVAS OBJECT IS THE SECOND OBJECT ON THE PLAYER OBJECT
        questHudIndex = 0;
        questHudGameobjectArray = new GameObject[numberOfQuests];
        foreach(Transform child in playerObject.transform.GetChild(1).transform)
        {
            if(child.gameObject.tag == "quest_hud")
            {
                questHudGameobjectArray[questHudIndex] = child.gameObject;
                questHudIndex++;
            }
        }
        playerController.isShouldMove = false;
        currentQuestHudIndex = 0;
        currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

        //default algorithm is random 
        randomToggle.isOn = true;
        randomToggle.onValueChanged.AddListener(delegate { OnRandomToggleChanged(randomToggle); });
        cmabToggle.isOn = false;
        cmabToggle.onValueChanged.AddListener(delegate { onCMABToggleChanged(cmabToggle); });
        passageToggle.isOn = false;
        passageToggle.onValueChanged.AddListener(delegate { onPassageToggleChanged(passageToggle); });
        questAlgorithmIndex = playerController.questAlgorithm;
        

        //run set up for all algorithms 
        for(int i = 0; i < questAlgorithms.Length; i++)
        {
            currentQuestAlgorithm = questAlgorithms[i].GetComponent<QuestAlgorithmBase>();
            currentQuestAlgorithm.SetUpAlgorithm();
        }

        exitButton.onClick.AddListener(ExitScene);
        questsToSubmit = new Quest[PlayerController.maxActiveQuests];
        PopulateQuestBoard();

        //Set up accepted panels 
        for(int i = 0; i < QuestAcceptPanelObjects.Length; i++)
        {
            QuestAcceptPanelObjects[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitScene()
    {
        playerController.enabled = true;
        playerController.HUD.SetActive(true);
        currentQuestAlgorithm.OnQuestClosed();
        playerObject.transform.position = new Vector3(-0.5f, 10.5f, 0f);
        playerController.isShouldMove = true;
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
        questAlgorithmIndex = playerController.questAlgorithm;
        SetCorrectQuestToggle(questAlgorithmIndex);
        currentQuestAlgorithm = questAlgorithms[questAlgorithmIndex].GetComponent<QuestAlgorithmBase>();
        displayQuests = currentQuestAlgorithm.GetQuests(numberOfQuests, questDataBase);

        for(int i = 0; i < displayQuests.Length; i++)
        {
            SetQuestUI(QuestUIObjects[i], displayQuests[i], i);
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

    public void SetQuestUI(GameObject questUIObject, Quest quest, int UIObjectPosition)
    {
        //assumes that the prefab is used to create the quest UI in a particular order
        questAcceptButton = questUIObject.GetComponent<Button>();
        questName = questUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        rewardText = questUIObject.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        rewardText.text = quest.reward.ToString();

        questName.text = quest.questName; 
        questAcceptButton.onClick.AddListener(delegate {AcceptQuest(quest, UIObjectPosition); });
    }

    public void SetSubmitQuestUI(GameObject submitQuestUIObject, Quest quest)
    {
        //assumes that the prefab is used to create the submit quest UI in a particular order
        questSubmitButton = submitQuestUIObject.GetComponent<Button>(); 
        submitQuestName = submitQuestUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        submitQuestName.text = quest.questName;
        questSubmitButton.onClick.AddListener(delegate { SubmitQuest(quest, submitQuestUIObject); });
    }


    //TODO: Turn this into functions
    public void AcceptQuest(Quest quest, int UIObjectPosition)
    {
        if(playerController.CountNumberOfActiveQuests() == PlayerController.maxActiveQuests)
        {
            Debug.Log("Maximum Quests Reached");
        }
        else
        {
            //Instantiate Event Listener
            switch (quest.questType)
            {
                case QuestType.plant:
                    //expects the plant event listening data to be formatted as name of plant, and target number in the format of strings
                    if (!Int32.TryParse(quest.eventListenerData[1], out result))
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
                    SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();


                    break;

                case QuestType.cook:
                    //expects the cook event listening data to be formatted as type of ingredient, and target number in the format of strings
                    if (!Int32.TryParse(quest.eventListenerData[1], out result))
                    {
                        Debug.LogError("Accept Quest Failed for quest " + quest.questName + "; could not instantiate PlantingEventListener. Check eventListenerData is formatted as [\"targetPlant\", \"targetValue\"]");
                    }
                    currentQuestGameObject = new GameObject();
                    currentQuestGameObject.name = quest.questName + "_EL";
                    currentQuestGameObject.transform.SetParent(playerObject.transform);
                    currentQuestGameObject.AddComponent<CookingEventListener>();

                    cookingEventListener = currentQuestGameObject.GetComponent<CookingEventListener>();
                    cookingEventListener.SetCookingEventListener(quest.eventListenerData[0], Int32.Parse(quest.eventListenerData[1]));

                    quest.eventListener = cookingEventListener;
                    quest.eventListener.OnStartListening();
                    playerController.AddQuestToActiveArray(quest);
                    currentQuestAlgorithm.OnQuestAccepted(quest);
                    SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

                    break;
                case QuestType.place:
                    //expects place type as an int
                    currentQuestGameObject = new GameObject();
                    currentQuestGameObject.name = quest.questName + "_EL";
                    currentQuestGameObject.transform.SetParent(playerObject.transform);
                    currentQuestGameObject.AddComponent<PlaceEventListener>();

                    placeEventListener = currentQuestGameObject.GetComponent<PlaceEventListener>();
                    placeEventListener.SetPlaceEventListener(Int32.Parse(quest.eventListenerData[0]));

                    quest.eventListener = placeEventListener;
                    quest.eventListener.OnStartListening();
                    playerController.AddQuestToActiveArray(quest);
                    currentQuestAlgorithm.OnQuestAccepted(quest);

                    SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

                    break;
                default:
                    break;
            }
            QuestAcceptPanelObjects[UIObjectPosition].SetActive(true);
        }

       
       
    }
    public void SubmitQuest(Quest quest, GameObject UIObject)
    {
        playerController.addCurrency(quest.reward);
        playerController.RemoveQuestFromActiveQuestsArray(quest);
        currentQuestAlgorithm.OnQuestSubmitted();

        //destroy eventlistener 
        Destroy(quest.eventListener.gameObject);

        //remove UI elements
        UIObject.SetActive(false);
        for (int i = 0; i < questHudGameobjectArray.Length; i++)
        {
           if(questHudGameobjectArray[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == quest.questName)
            {
                questHudIndexToRemove = i; 
            }
        }
        questHudGameobjectArray[questHudIndexToRemove].SetActive(false);
        playerController.questHudCurrentIndex = questHudIndexToRemove;

    }

    private void OnRandomToggleChanged(Toggle toggle)
    {
        if(toggle.isOn)
        {
            //turn others off 
            cmabToggle.isOn = false;
            passageToggle.isOn = false;

            //set algorithm 
            questAlgorithmIndex = 0;
            playerController.questAlgorithm = questAlgorithmIndex;
        }
    }

    private void onCMABToggleChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            //turn others off 
            randomToggle.isOn = false;
            passageToggle.isOn = false;

            //set algorithm 
            questAlgorithmIndex = 1;
            playerController.questAlgorithm = questAlgorithmIndex;
        }
    }

    private void onPassageToggleChanged(Toggle toggle)
    {
        if (toggle.isOn)
        {
            //turn others off 
            randomToggle.isOn = false;
            cmabToggle.isOn = false;

            //set algorithm 
            questAlgorithmIndex = 2;
            playerController.questAlgorithm = questAlgorithmIndex;
        }
    }

    private void SetCorrectQuestToggle(int index)
    {
        if(index == 0)
        {
            randomToggle.isOn = true;
            cmabToggle.isOn = false;
            passageToggle.isOn = false;
        }
        else if(index == 1)
        {
            randomToggle.isOn = false;
            cmabToggle.isOn = true;
            passageToggle.isOn = false;
        }
        else if (index == 2)
        {
            randomToggle.isOn = false;
            cmabToggle.isOn = false;
            passageToggle.isOn = true;
        }
    }

    private void SetQuestHudObject(Quest quest)
    {
        questHudGameobjectArray[playerController.questHudCurrentIndex].SetActive(true);
        //ASSUMES THAT THE QUEST HUD GAME OBJECTS ARE IN A PARTICLAR ORDER - if they are moved this breaks!
        questHudTMP = questHudGameobjectArray[playerController.questHudCurrentIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        questHudTMP.text = quest.questName;
        questHudReward = questHudGameobjectArray[playerController.questHudCurrentIndex].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        questHudReward.text = quest.reward.ToString();
        playerController.questHudCurrentIndex++;
    }
}
