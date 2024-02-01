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
    private Quest[] usableQuestArray;
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

    //update quest on the day 
    WorldController worldController; 
            

   
    private void Awake()
    {


      

    }

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerController = playerObject.GetComponent<PlayerController>();
        playerObject.GetComponent<SpriteRenderer>().enabled = false; 
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
        

        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
        questSetupScript = GameObject.FindGameObjectWithTag("quest_setup").GetComponent<QuestSetupScript>();

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
        //PopulateQuestBoard();

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

        //telemetry
        //StartCoroutine(GetAssetBundle());
        //StartCoroutine(PostData("test"));

        
    }

    // Update is called once per frame
    void Update()
    { 
        

    }
    public void ExitScene()
    {
        playerController.enabled = true;
        playerController.HUD.SetActive(true);
        Debug.Log(currentQuestAlgorithm);
        currentQuestAlgorithm.OnQuestClosed();
       
        playerController.isShouldMove = true;
        playerObject.GetComponent<SpriteRenderer>().enabled = true;
        if(SceneManager.GetActiveScene().name == "QuestBoard")
        {
            playerObject.transform.position = new Vector3(-0.5f, 10.5f, 0f);
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }
        else if(SceneManager.GetActiveScene().name == "TutorialQuestBoard")
        {
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
        for(int i = 0; i < questSetupScript.questDataBase.Length; i++)
        {
            skipQuest = false;
            for(int j = 0; j < playerController.activeQuests.Length; j++)
            {
                if(questSetupScript.questDataBase[i].questName == playerController.activeQuests[j].questName)
                {
                    skipQuest = true;
                }
            }
            if(!skipQuest)
            {
                usableQuestArray[usableQuestArrayIndex] = questSetupScript.questDataBase[i];
                usableQuestArrayIndex++;
            }
        }
        displayQuests = currentQuestAlgorithm.GetQuests(numberOfQuests, questSetupScript.questDataBase);

        for(int i = 0; i < displayQuests.Length; i++)
        {
            SetQuestUI(questUIObjects[i], displayQuests[i], i);
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

    public void SetupSubmitQuestsUI()
    {
        currentQuestboardState = QuestBoardState.submit;
        questDescriptionText.text = "Quests to Submit:";
        //Get quests to submit
        for (int i = 0; i < playerController.activeQuests.Length; i++)
        {
            Debug.Log(i);
            Debug.Log(playerController.activeQuests[i].questName);
            //this is going to look weird - needs a clean up pass
            if (playerController.activeQuests[i].questType == QuestType.invalid)
            {
                Debug.Log(playerController.activeQuests[i] + " is considered invalid");
                submitQuestUIObjects[i].SetActive(false);
            }
            else
            {
                Debug.Log(playerController.activeQuests[i].eventListener.IsEventCompleted);
                //Debug.Log(i);
                //Debug.Log(playerController.activeQuests[i].questName);
                //Debug.Log(playerController.activeQuests[i].eventListener.IsEventCompleted);
                if (playerController.activeQuests[i].eventListener.IsEventCompleted)
                {
                    Debug.Log(playerController.activeQuests[i].questName + " is considered completed");
                    submitQuestUIObjects[i].SetActive(true);
                    SetSubmitQuestUI(submitQuestUIObjects[i], playerController.activeQuests[i]);
                }
                else
                {
                    Debug.Log(playerController.activeQuests[i].questName + " is not considered completed");
                    submitQuestUIObjects[i].SetActive(false);
                }
            }
        }

        //turn off accept quest UI
        for (int i = 0; i < questUIObjects.Length; i++)
        {
            questUIObjects[i].SetActive(false);
            questAccecptPanelObjects[i].SetActive(false);
        }
    }

    public void SetupAcceptQuestsUI()
    {
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
                    SetQuestHudObject(quest);

                    //update feedback for max number of quests 
                    currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();
                    break;
                default:
                    break;
            }
            questAccecptPanelObjects[UIObjectPosition].SetActive(true);
            //questAcceptedAlreadyArray[UIObjectPosition] = 1;
        }

       
       
    }
    public void SubmitQuest(Quest quest, GameObject UIObject)
    {
        playerController.addCurrency(quest.reward);
        Debug.Log(quest);
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
        currentAcceptedQuestNumText.text = playerController.CountNumberOfActiveQuests().ToString();
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

    //Telemetry function 
    IEnumerator GetAssetBundle()
    {
        //hack to force validate certificate 
        var cert = new CertificateValidator();

        UnityWebRequest www = UnityWebRequest.Get("https://localhost:8000");
        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        Debug.Log("Request recieved");
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator PostData(string msg)
    {
        Debug.Log("Starting Post Request");
        var cert = new CertificateValidator();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));


        UnityWebRequest www = UnityWebRequest.Post("https://localhost:8000", formData);
        //NEEDED TO AVOID CERTIFICATE VALIDATION ERROR
        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        Debug.Log("Post Request Recieved");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
