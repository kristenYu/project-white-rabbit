using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Networking;

public class QuestBoard : MonoBehaviour
{
    //this enum is used for the reinforcement learning algorithm and for PaSSAGE
    public enum QuestType
    {
        plant = 0,
        cook,
        place,
        harvest,
        invalid, //this is always the maximum number of quest categories
    }

    public enum QuestBoardState
    {
        submit = 0,
        accept,
    }

    //telemetry
    public Telemetry_Util telemetryUtil;



    //
    public Button exitButton;
    public GameObject playerObject;
    private PlayerController playerController;
    private PlantingEventListener plantingEventListener;
    private CookingEventListener cookingEventListener;
    private PlaceEventListener placeEventListener;
    private HarvestEventListener harvestEventListener;

    //Algorithm Toggles 
    public Toggle randomToggle;
    public Toggle cmabToggle;
    public Toggle passageToggle;

    //questboard state
    public QuestBoardState currentQuestboardState;
    public Button submitQuestsButton;
    public Button acceptQuestsButton;
    public TextMeshProUGUI questDescriptionText;
    public int[] questAcceptedAlreadyArray;

    //Questboard Quests 
    public const int numberOfQuests = 3;
    public Quest[] displayQuests;
    private QuestAlgorithmBase currentQuestAlgorithm;
    private GameObject currentQuestGameObject;
    public Quest[] questsToSubmit;
    private int result;
    private int questAlgorithmIndex;

    //quest setup 
    public QuestSetupScript questSetupScript;
    public Quest[] usableQuestArray;
    private bool skipQuest;
    private int usableQuestArrayIndex;

    //Quest UI 
    public GameObject[] questUIObjects;//Should always be matching the number of quests - 0 is left most, max is right most
    public GameObject[] questAccecptPanelObjects; //Should always be matching the number of quests - 0 is the left most, max is the right most
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
    public TextMeshProUGUI questHudTargetNumText;
    public TextMeshProUGUI questHudCurrentNumText;

    //player current coins
    public TextMeshProUGUI currentCoins;

    //update quest on the day 
    public WorldController worldController;

    //sound 
    public AudioSource audioSource;
    public AudioClip questTabClip;
    public AudioClip exitButtonClip;
    public AudioClip acceptQuestClip;
    public AudioClip submitQuestClip;

    private void Awake()
    {




    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.GetComponent<AudioSource>().Stop();
        playerController = playerObject.GetComponent<PlayerController>();
        playerObject.GetComponent<SpriteRenderer>().enabled = false;
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        telemetryUtil = GameObject.FindGameObjectWithTag("telemetry").GetComponent<Telemetry_Util>();
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
        questSetupScript = GameObject.FindGameObjectWithTag("quest_setup").GetComponent<QuestSetupScript>();

        //ASSUMES THAT THE CANVAS OBJECT IS THE SECOND OBJECT ON THE PLAYER OBJECT
        questHudIndex = 0;
        questHudGameobjectArray = new GameObject[numberOfQuests];
        foreach (Transform child in playerObject.transform.GetChild(1).transform)
        {
            if (child.gameObject.tag == "quest_hud")
            {
                questHudGameobjectArray[questHudIndex] = child.gameObject;
                questHudIndex++;
            }
        }
        playerController.isShouldMove = false;
        currentQuestHudIndex = 0;
        currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

        //Turn of selection of algorithm 
        /*
        //default algorithm is random 
        randomToggle.isOn = true;
        randomToggle.onValueChanged.AddListener(delegate { OnRandomToggleChanged(randomToggle); });
        cmabToggle.isOn = false;
        cmabToggle.onValueChanged.AddListener(delegate { onCMABToggleChanged(cmabToggle); });
        passageToggle.isOn = false;
        passageToggle.onValueChanged.AddListener(delegate { onPassageToggleChanged(passageToggle); });
        questAlgorithmIndex = playerController.questAlgorithm;
        */


        //setup button delegates
        exitButton.onClick.AddListener(ExitScene);
        submitQuestsButton.onClick.AddListener(SetupSubmitQuestsUI);
        acceptQuestsButton.onClick.AddListener(SetupAcceptQuestsUI);
        questsToSubmit = new Quest[PlayerController.maxActiveQuests];


        currentQuestboardState = QuestBoardState.submit;
        SetupSubmitQuestsUI();
        questAcceptedAlreadyArray = new int[numberOfQuests];
        //Set up accepted panels 
        for (int i = 0; i < questAccecptPanelObjects.Length; i++)
        {
            questAccecptPanelObjects[i].SetActive(false);
            questAcceptedAlreadyArray[i] = 0;
        }

        //get the current quest algorithm 
        currentQuestAlgorithm = questSetupScript.questAlgorithms[playerController.questAlgorithm].GetComponent<QuestAlgorithmBase>();

        //set the current amount of coins
        currentCoins.text = playerController.currency.ToString();
    }

    // Update is called once per frame
    void Update()
    {


    }
    public void ExitScene()
    {
        audioSource.PlayOneShot(exitButtonClip);
        playerController.enabled = true;
        playerController.HUD.SetActive(true);
        currentQuestAlgorithm.OnQuestClosed();

        playerController.isShouldMove = true;
        playerObject.GetComponent<SpriteRenderer>().enabled = true;
        if (SceneManager.GetActiveScene().name == "QuestBoard")
        {
            playerObject.transform.position = new Vector3(-0.5f, 10.5f, 0f);
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Transition:Main"));
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        else if (SceneManager.GetActiveScene().name == "TutorialQuestBoard")
        {
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Transition:Tutorial"));
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);


        }

    }

    public void PopulateQuestBoard()
    {
        //Get Quests to show
        //questAlgorithmIndex = playerController.questAlgorithm;
        //SetCorrectQuestToggle(questAlgorithmIndex);
        //currentQuestAlgorithm = questSetupScript.questAlgorithms[questAlgorithmIndex].GetComponent<QuestAlgorithmBase>();
        usableQuestArray = new Quest[questSetupScript.questDataBase.Length - playerController.CountNumberOfActiveQuests()];
        usableQuestArrayIndex = 0;
        for (int i = 0; i < questSetupScript.questDataBase.Length; i++)
        {
            skipQuest = false;
            for (int j = 0; j < playerController.activeQuests.Length; j++)
            {
                if(playerController.activeQuests[j] == null)
                {
                    continue;
                }
                if (questSetupScript.questDataBase[i].questName == playerController.activeQuests[j].questName)
                {
                    skipQuest = true;
                }
            }
            if (!skipQuest)
            {
                usableQuestArray[usableQuestArrayIndex] = questSetupScript.questDataBase[i];
                usableQuestArrayIndex++;
            }
        }
        displayQuests = currentQuestAlgorithm.GetQuests(numberOfQuests, usableQuestArray);

        for (int i = 0; i < displayQuests.Length; i++)
        {
            SetQuestUI(questUIObjects[i], displayQuests[i], i);
            switch(i)
            {
                case 0:
                    StartCoroutine(telemetryUtil.PostData("Quest:PositionQuest:" + displayQuests[i].questName + ":Left"));
                    break;
                case 1:
                    StartCoroutine(telemetryUtil.PostData("Quest:PositionQuest:" + displayQuests[i].questName + ":Middle"));
                    break;
                case 2:
                    StartCoroutine(telemetryUtil.PostData("Quest:PositionQuest:" + displayQuests[i].questName + ":Right"));
                    break;
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
        questAcceptButton.onClick.RemoveAllListeners();
        questAcceptButton.onClick.AddListener(delegate { AcceptQuest(quest, UIObjectPosition); });
    }

    public void SetSubmitQuestUI(GameObject submitQuestUIObject, Quest quest)
    {
        //assumes that the prefab is used to create the submit quest UI in a particular order
        questSubmitButton = submitQuestUIObject.GetComponent<Button>();
        submitQuestName = submitQuestUIObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        submitQuestName.text = quest.questName;
        questSubmitButton.onClick.AddListener(delegate { SubmitQuest(quest, submitQuestUIObject); });
    }

    public void SetupSubmitQuestsUI()
    {
        audioSource.PlayOneShot(questTabClip);
        currentQuestboardState = QuestBoardState.submit;
        questDescriptionText.text = "Quests to Submit:";
        //Show all active quests
        for (int i = 0; i < playerController.activeQuests.Length; i++)
        {
            //this is going to look weird - needs a clean up pass
            if(playerController.activeQuests[i] != null)
            {
                if (playerController.activeQuests[i].questType == QuestType.invalid)
                {
                    submitQuestUIObjects[i].SetActive(false);
                }
                else
                {
                    submitQuestUIObjects[i].SetActive(true);
                    SetSubmitQuestUI(submitQuestUIObjects[i], playerController.activeQuests[i]);
                    switch (playerController.activeQuests[i].questType)
                    {
                        case QuestType.cook:
                            //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                            submitQuestUIObjects[i].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ((CookingEventListener)playerController.activeQuests[i].eventListener).checkNumRecipes.ToString();
                            submitQuestUIObjects[i].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = ((CookingEventListener)playerController.activeQuests[i].eventListener).currentNumRecipes.ToString();
                            break;
                        case QuestType.harvest:
                            //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                            submitQuestUIObjects[i].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ((HarvestEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                            submitQuestUIObjects[i].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = ((HarvestEventListener)playerController.activeQuests[i].eventListener).currentHarvestedNum.ToString();
                            break;
                        case QuestType.place:
                            //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                            submitQuestUIObjects[i].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ((PlaceEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                            submitQuestUIObjects[i].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = ((PlaceEventListener)playerController.activeQuests[i].eventListener).currentNumPlaced.ToString();
                            break;
                        case QuestType.plant:
                            //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                            submitQuestUIObjects[i].transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ((PlantingEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                            submitQuestUIObjects[i].transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = (((PlantingEventListener)playerController.activeQuests[i].eventListener).currentNumTargetCrops - ((PlantingEventListener)playerController.activeQuests[i].eventListener).startingNumTargetCrops).ToString();
                            break;

                    }
                    if (playerController.activeQuests[i].eventListener.IsEventCompleted)
                    {
                        submitQuestUIObjects[i].GetComponent<Button>().enabled = true;
                        //submitQuestUIObjects[i].GetComponent<Image>().color = Color.green;
                        submitQuestUIObjects[i].GetComponent<Image>().color = new Color(0.5377715f, 0.6887516f, 0.8584906f, 1f);
                        //submitQuestUIObjects[i].GetComponent<Image>().color = new Color(69f, 154f, 63f, 255f);
                    }
                    else
                    {
                        submitQuestUIObjects[i].GetComponent<Button>().enabled = false;
                        submitQuestUIObjects[i].GetComponent<Image>().color = new Color(0.735849f, 0.4396859f, 0.3637593f, 1f);
                        //submitQuestUIObjects[i].GetComponent<Image>().color = new Color(191f, 99f, 68f, 255f);
                        //submitQuestUIObjects[i].SetActive(false);
                    }
                }
            }
  
        }

        //turn off accept quest UI
        for (int i = 0; i < questUIObjects.Length; i++)
        {
            questUIObjects[i].SetActive(false);
            questAccecptPanelObjects[i].SetActive(false);
        }
        if(currentQuestAlgorithm != null)
        {
            currentQuestAlgorithm.OnQuestClosed();
        }

        //HARDCODED FOR THE PANEL BEING THE FIRST CHILD OF THE BUTTON
        submitQuestsButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        acceptQuestsButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(telemetryUtil.PostData("QuestBoardState:Submit"));

    }

    public void SetupAcceptQuestsUI()
    {
        audioSource.PlayOneShot(questTabClip);
        if (currentQuestboardState == QuestBoardState.submit)
        {
            PopulateQuestBoard();
        }
        currentQuestboardState = QuestBoardState.accept;
        questDescriptionText.text = "Quests to Accept:";
        //turn on accept quest UI
        for (int i = 0; i < questUIObjects.Length; i++)
        {
            questUIObjects[i].SetActive(true);
            if (questAcceptedAlreadyArray[i] == 1)
            {
                questAccecptPanelObjects[i].SetActive(true);
            }
        }
        //turn off submit quest UI
        for (int i = 0; i < submitQuestUIObjects.Length; i++)
        {
            submitQuestUIObjects[i].SetActive(false);
        }
        //HARDCODED FOR THE PANEL BEING THE FIRST CHILD OF THE BUTTON
        submitQuestsButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        acceptQuestsButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(telemetryUtil.PostData("Quests:QuestBoardState:Accept"));
    }
    //TODO: Turn this into functions
    public void AcceptQuest(Quest quest, int UIObjectPosition)
    {
        if (playerController.CountNumberOfActiveQuests() == PlayerController.maxActiveQuests)
        {
            Debug.Log("Maximum Quests Reached");
        }
        else
        {
            audioSource.PlayOneShot(acceptQuestClip);
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
                    //SetQuestHudObject(quest);

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

                    //SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

                    break;
                case QuestType.harvest:
                    if (!Int32.TryParse(quest.eventListenerData[1], out result))
                    {
                        Debug.LogError("Accept Quest Failed for quest " + quest.questName + "; could not instantiate HarvestEventListener. Check eventListenerData is formatted as [\"targetPlant\", \"targetValue\"]");
                    }
                    currentQuestGameObject = new GameObject();
                    currentQuestGameObject.name = quest.questName + "_EL";
                    currentQuestGameObject.transform.SetParent(playerObject.transform);
                    currentQuestGameObject.AddComponent<HarvestEventListener>();

                    harvestEventListener = currentQuestGameObject.GetComponent<HarvestEventListener>();
                    harvestEventListener.SetHarvestEventListener(quest.eventListenerData[0], Int32.Parse(quest.eventListenerData[1]));

                    quest.eventListener = harvestEventListener;
                    quest.eventListener.OnStartListening();
                    playerController.AddQuestToActiveArray(quest);
                    //SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();
                    break;
                default:
                    break;

            }

            for (int i = 0; i < playerController.activeQuests.Length; i++)
            {
                if (playerController.activeQuests[i] != null)
                {
                    if(playerController.activeQuests[i].questType == QuestType.invalid)
                    {
                        questHudGameobjectArray[i].SetActive(false);
                    }
                    else
                    {
                        SetQuestHudObject(playerController.activeQuests[i], i);
                    }
                    
                }
                else
                {
                    questHudGameobjectArray[i].SetActive(false);
                }
            }

            questAccecptPanelObjects[UIObjectPosition].SetActive(true);
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Quest:Accept:" + quest.questName));
            //questAcceptedAlreadyArray[UIObjectPosition] = 1;
        }



    }
    public void SubmitQuest(Quest quest, GameObject UIObject)
    {
        audioSource.PlayOneShot(submitQuestClip);
        playerController.addCurrency(quest.reward);
        playerController.RemoveQuestFromActiveQuestsArray(quest);
        currentQuestAlgorithm.OnQuestSubmitted();

        //destroy eventlistener 
        Destroy(quest.eventListener.gameObject);

        //remove UI elements
        UIObject.SetActive(false);
        for (int i = 0; i < questHudGameobjectArray.Length; i++)
        {
            if (questHudGameobjectArray[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == quest.questName)
            {
                questHudIndexToRemove = i;
            }
        }
        questHudGameobjectArray[questHudIndexToRemove].SetActive(false);
        playerController.questHudCurrentIndex = questHudIndexToRemove;
        currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();

        //set the current amount of coins
        currentCoins.text = playerController.currency.ToString();

        //telemetry
        StartCoroutine(telemetryUtil.PostData("Quest:Submit:" + quest.questName));
    }

    private void OnRandomToggleChanged(Toggle toggle)
    {
        if (toggle.isOn)
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
        if (index == 0)
        {
            randomToggle.isOn = true;
            cmabToggle.isOn = false;
            passageToggle.isOn = false;
        }
        else if (index == 1)
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

    private void SetQuestHudObject(Quest quest, int index)
    {   
        //ASSUMES THAT THE QUEST HUD GAME OBJECTS ARE IN A PARTICLAR ORDER - if they are moved this breaks!
        questHudGameobjectArray[index].SetActive(true);
        questHudTMP = questHudGameobjectArray[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        questHudTMP.text = quest.questName;
        questHudReward = questHudGameobjectArray[index].transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        questHudReward.text = quest.reward.ToString();
        questHudTargetNumText = questHudGameobjectArray[index].transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        questHudCurrentNumText = questHudGameobjectArray[index].transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        switch (quest.questType)
        {
            case QuestType.cook:
                //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                questHudTargetNumText.text = ((CookingEventListener)quest.eventListener).structToCheck.targetValue.ToString();
                questHudCurrentNumText.text = ((CookingEventListener)quest.eventListener).currentNumRecipes.ToString();
                break;
            case QuestType.harvest:
                //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                questHudTargetNumText.text = ((HarvestEventListener)quest.eventListener).structToCheck.targetValue.ToString();
                questHudCurrentNumText.text = ((HarvestEventListener)quest.eventListener).currentHarvestedNum.ToString();
                break;
            case QuestType.place:
                //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                questHudTargetNumText.text = ((PlaceEventListener)quest.eventListener).structToCheck.targetValue.ToString();
                questHudCurrentNumText.text = ((PlaceEventListener)quest.eventListener).currentNumPlaced.ToString();
                break;
            case QuestType.plant:
                //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                questHudTargetNumText.text = ((PlantingEventListener)quest.eventListener).structToCheck.targetValue.ToString();
                questHudCurrentNumText.text = ((PlantingEventListener)quest.eventListener).currentNumTargetCrops.ToString();
                break;

        }
    }
}

