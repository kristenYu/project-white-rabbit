using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSetupScript : MonoBehaviour
{
    //Database loading 
    public Quest[] questDataBase;
    public Quest currentQuest;
    public TextAsset[] questFiles;
    public int questDatabaseIndex;

    //quest algorithms 
    public GameObject[] questAlgorithms; //Should be set to the number of algorithms 
    public int questAlgorithmIndex; //quest algorithm to use
    private QuestAlgorithmBase currentQuestAlgorithm;

    //saving which quests have already been accepted 
    //public int[] questAcceptedAlreadyArray;
    

    //Singleton 
    private static QuestSetupScript instance;
    // Read-only public access
    public static QuestSetupScript Instance => instance;

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

        questDatabaseIndex = 0;
        StartCoroutine(LoadQuestDatabase());

        //run set up for all algorithms 
        for (int i = 0; i < questAlgorithms.Length; i++)
        {
            currentQuestAlgorithm = questAlgorithms[i].GetComponent<QuestAlgorithmBase>();
            currentQuestAlgorithm.SetUpAlgorithm();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //questAcceptedAlreadyArray = new int[3];
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private IEnumerator LoadQuestDatabase()
    {
        //expects that the file is in Resources/Data/Quests

        questFiles = Resources.LoadAll<TextAsset>("Data/Quests");
        questDataBase = new Quest[questFiles.Length];
        foreach (TextAsset questFile in questFiles)
        {
            //All quest data except for the event listener - the event listener is instantiated when the quest is accepted;
            currentQuest = JsonUtility.FromJson<Quest>(questFile.ToString());
            questDataBase[questDatabaseIndex] = currentQuest;
            questDatabaseIndex++;
        }
        yield return null;
    }
}
