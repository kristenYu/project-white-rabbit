using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLAIDQuestAlgorithm : QuestAlgorithmBase
{

    //TODO: This needs to be made DoNotDestroyOnLoad();

    public int questNum;
    public int questCategories;
    //setup array 
    private int[] questCategoryArray;
    
    //player profile 
    private string currentKey;
    private Dictionary<string, int> banditCountDictionary;
    private Dictionary<string, float> banditRewardDictionary;
    private Dictionary<string, float> qValueDictionary;

    //generating a cba job (calculate qValue and generate job)
    private float value;
    private float adjustedReward;
    private const float explorationConstant = 0.5f; // called c pseudocode for the algorithm
    private float maxValue;
    private List<string> possibleKeys;

    //rewarding the bandit m
    //this array is the length of the questCategories 
    //Index indicates the quest category, and value indicates the number that were accepted
    private int[] rewardArray;
    private int currentReward;
    private int currentNumberOfAcceptedQuests;

    //select quests 
    private int time;
    private string job;
    private Quest[] questsToGive;
    private List<Quest> questsOfType;
    private int questTypeCatch;
    private List<Quest> potentialQuests;


    //set up for the algorithm as needed 
    public override void SetUpAlgorithm()
    {
        questNum = 3;
        time = 0;
        questCategories = (int)QuestBoard.QuestType.invalid;
        potentialQuests = new List<Quest>();
        questCategoryArray = new int[questCategories];
        rewardArray = new int[questCategories];
        for(int i =0; i < questCategories; i++)
        {
            questCategoryArray[i] = i;
        }
        resetRewardArray();
        //sets up the player profile to 0s
        banditCountDictionary = new Dictionary<string, int>();
        banditRewardDictionary = new Dictionary<string, float>();
        qValueDictionary = new Dictionary<string, float>();
        possibleKeys = new List<string>();
        questsOfType = new List<Quest>();
        CombinationRepetition(questCategoryArray, questCategories, questNum);
    }
    //asks for quests from the quest algorithm
    public override Quest[] GetQuests(int questNum, Quest[] questDataBase)
    {
        questsToGive = new Quest[questNum];
        job = generateCBAJob(time);
        Debug.Log(job);
        potentialQuests.Clear();
        for(int i = 0; i < questDataBase.Length; i++)
        {
            potentialQuests.Add(questDataBase[i]);
        }

        for(int i = 0; i < job.Length; i++)
        {
            questTypeCatch = (int)job[i] - 48;
            questsOfType = GetAllQuestsOfType(questTypeCatch, potentialQuests.ToArray());
            questsToGive[i] = questsOfType[Random.Range(0, questsOfType.Count)];
            potentialQuests.Remove(questsToGive[i]);
            //questsOfType.Remove(questsToGive[i]);
        }
        
        return questsToGive;
    }
    //notifies the quest algorithm that a quest has been accepted
    public override void OnQuestAccepted(Quest quest)
    {
        rewardArray[(int)quest.questType] += 1;
    }
    //notifies the quest algorithm that a quest has been submitted
    public override void OnQuestSubmitted()
    {

    }
    //notifies the quest algorithm that the player closed the quest board
    public override void OnQuestClosed()
    {
        rewardBandit(rewardArray);
        resetRewardArray(); 
    }


    private void CombinationRepetitionUtil(int[] chosen, int[] arr, int index, int r, int start, int end)
    {
        // Since index has become r, current combination is
        // ready to be printed, print
        if (index == r)
        {
            currentKey = "";
            for (int i = 0; i < r; i++)
            {
                currentKey += arr[chosen[i]];
            }
            banditCountDictionary.Add(currentKey, 0);
            banditRewardDictionary.Add(currentKey, 0.0f);
            qValueDictionary.Add(currentKey, 0.0f);
            return;
        }

        // One by one choose all elements (without considering
        // the fact whether element is already chosen or not)
        // and recur
        for (int i = start; i <= end; i++)
        {
            chosen[index] = i;
            CombinationRepetitionUtil(chosen, arr, index + 1, r, i, end);
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

    private void resetRewardArray()
    {
        for(int i = 0; i < rewardArray.Length; i++)
        {
            rewardArray[i] = 0;
        }
    }
    //update the bandit based on the current rewardArray; 
    private void rewardBandit(int[] reward)
    {
        Debug.Log(banditCountDictionary.Count);
        foreach(KeyValuePair<string, int> kvp in banditCountDictionary)
        {
            currentReward = 0;
            for (int i = 0; i < questNum; i++)
            {
                currentNumberOfAcceptedQuests = (int)kvp.Key[i];
                if (reward[i] != 0 && reward[i] <= currentNumberOfAcceptedQuests)
                {
                    currentReward += reward[i];
                }
            }

            if (currentReward != 0)
            {
                banditRewardDictionary[kvp.Key] = currentReward;
            }
        }
    }
    private string generateCBAJob(int time)
    {
        maxValue = 0;
        foreach(KeyValuePair<string, float> kvp in banditRewardDictionary)
        {
            if (banditCountDictionary[kvp.Key] == 0)
            {
                value = qValueDictionary[kvp.Key];
                possibleKeys.Add(kvp.Key);
            }
            else
            {
                adjustedReward = banditRewardDictionary[kvp.Key] - qValueDictionary[kvp.Key];
                //Alpha is 1/n, where n is the number of times it has been selected
                qValueDictionary[kvp.Key] = qValueDictionary[kvp.Key] + 1.0f / banditCountDictionary[kvp.Key] * adjustedReward;
                //UCB selection
                value = qValueDictionary[kvp.Key] + explorationConstant * Mathf.Sqrt(Mathf.Log(time) / (banditCountDictionary[kvp.Key]));

                if (value >  maxValue)
                {
                    maxValue = value;
                    possibleKeys.Clear();
                    possibleKeys.Add(kvp.Key);
                }
                else if (value == maxValue)
                {
                    possibleKeys.Add(kvp.Key);
                }
            }
        }

        //possible keys is the super arm so we want to iterate the count for the whole arm
        for(int i = 0; i < possibleKeys.Count; i++)
        {
            banditCountDictionary[possibleKeys[i]] += 1;
        }
        return possibleKeys[Random.Range(0, possibleKeys.Count)];
    }

    private List<Quest> GetAllQuestsOfType(int type, Quest[] questDataBase)
    {
        questsOfType.Clear();
        foreach(Quest quest in questDataBase)
        {
            if((int) quest.questType == type)
            {
                questsOfType.Add(quest);
            }
        }
        return questsOfType;
    }
}
