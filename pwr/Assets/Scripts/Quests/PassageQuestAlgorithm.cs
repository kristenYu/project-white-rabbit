using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassageQuestAlgorithm : QuestAlgorithmBase
{
    private Quest[] questsToGive;

    //Player profile
    private int[] playerActionFrequency;
    private GameObject player;
    private PlayerController playerController;


    //Create the 1D tree for the dot product calculation later 
    private string currentKey;
    private Dictionary<string, int[]> questProposalTree;
    private int questNum;
    private int[] questProposalValues;
    public int questCategories;
    private int[] questCategoryArray;


    //Select the best quest proposal
    private int currentValue; 
    private int maxValue; 
    private List<string> possibleKeys;
    private string job;
    private List<Quest> questsOfType;



    //set up for the algorithm as needed, runs at start()
    public override void SetUpAlgorithm()
    {
        questNum = 3;
        questCategories = (int)QuestBoard.QuestType.invalid;
        playerActionFrequency = new int[questCategories];
        questProposalValues = new int[questCategories];

        questCategoryArray = new int[questCategories];
        for (int i = 0; i < questCategories; i++)
        {
            questCategoryArray[i] = i;
        }

        questProposalTree = new Dictionary<string, int[]>();
        CombinationRepetition(questCategoryArray, questCategories, questNum);

    }
    //asks for quests from the quest algorithm
    public override Quest[] GetQuests(int questNum, Quest[] questDataBase)
    {
        UpdateplayerActionFrequency();
        job = SearchTreeForBestAction();
        for (int i = 0; i < job.Length; i++)
        {
            questsOfType = GetAllQuestsOfType((QuestBoard.QuestType)job[i], questDataBase);
            questsToGive[i] = questsOfType[Random.Range(0, questsOfType.Count)];
            questsOfType.Remove(questsToGive[i]);
        }
        return questsToGive;
    }
    //notifies the quest algorithm that a quest has been accepted
    public override void OnQuestAccepted(Quest quest)
    {

    }
    //notifies the quest algorithm that a quest has been submitted
    public override void OnQuestSubmitted()
    {

    }
    //notifies the quest algorithm that the player closed the quest board
    public override void OnQuestClosed()
    {

    }

    public void UpdateplayerActionFrequency()
    {
        //gets the current quest profile
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();

        for (int i = 0; i < playerActionFrequency.Length; i++)
        {
            playerActionFrequency[i] = playerController.actionFrequencyArray[i];
        }
    }

    private void CombinationRepetitionUtil(int[] chosen, int[] arr,
       int index, int r, int start, int end)
    {
        // Since index has become r, current combination is
        // ready to be printed, print
        if (index == r)
        {
            currentKey = "";
            for(int i = 0; i < questProposalValues.Length; i++)
            {
                questProposalValues[i] = 0;
            }
            for (int i = 0; i < r; i++)
            {
                currentKey += arr[chosen[i]];
                questProposalValues[arr[chosen[i]]] ++; //the index is the quest type, the value is the number of that type
            }

            questProposalTree.Add(currentKey, questProposalValues);
            return;
        }

        // One by one choose all elements (without considering
        // the fact whether element is already chosen or not)
        // and recur
        for (int i = start; i <= end; i++)
        {
            chosen[index] = i;
            CombinationRepetitionUtil(chosen, arr, index + 1,
                    r, i, end);
        }
        return;
    }

    private void CombinationRepetition(int[] arr, int n, int r)
    {
        // Allocate memory
        int[] chosen = new int[r + 1];

        // Call the recursive function
        CombinationRepetitionUtil(chosen, arr, 0, r, 0, n - 1);
    }

    private string SearchTreeForBestAction()
    {
        foreach (KeyValuePair<string, int[]> kvp in questProposalTree)
        {
            currentValue = DotProduct(questProposalTree[kvp.Key], playerActionFrequency);
            if(currentValue > maxValue)
            {
                maxValue = currentValue;
                possibleKeys.Clear();
                possibleKeys.Add(kvp.Key);
            }
            else if (currentValue == maxValue)
            {
                possibleKeys.Add(kvp.Key);
            }
        }
        return possibleKeys[Random.Range(0, possibleKeys.Count)];
    }

    private int DotProduct(int[] matrix1, int[] matrix2)
    {
        int currentProduct = 0; 
        for(int i = 0; i < matrix1.Length; i++)
        {
            currentProduct += matrix1[i] * matrix2[i];
        }
        return currentProduct;
    }

    private List<Quest> GetAllQuestsOfType(QuestBoard.QuestType type, Quest[] questDataBase)
    {
        questsOfType.Clear();
        foreach (Quest quest in questDataBase)
        {
            if (quest.questType == type)
            {
                questsOfType.Add(quest);
            }
        }
        return questsOfType;
    }
}
