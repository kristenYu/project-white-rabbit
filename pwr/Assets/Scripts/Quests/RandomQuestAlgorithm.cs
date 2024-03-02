using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomQuestAlgorithm : QuestAlgorithmBase
{
    private Quest[] questsToGive;
    private List<Quest> potentialQuests; 

    public override void SetUpAlgorithm()
    {
        potentialQuests = new List<Quest>();
    }

    //QuestAlgorithmBase functions
    public override Quest[] GetQuests(int questNum, Quest[] questDataBase)
    {
        questsToGive = null;
        questsToGive = new Quest[questNum];

        potentialQuests.Clear();
        for(int i = 0; i < questDataBase.Length; i++)
        {
            potentialQuests.Add(questDataBase[i]);
        }
        for(int i = 0; i < questNum; i++)
        {
            questsToGive[i] = potentialQuests[Random.Range(0, potentialQuests.Count)];
            potentialQuests.Remove(questsToGive[i]);
        }

        return questsToGive;
    }
    public override void OnQuestAccepted(Quest quest)
    {

    }
    public override void OnQuestSubmitted()
    {

    }
    public override void OnQuestClosed()
    {

    }
}
