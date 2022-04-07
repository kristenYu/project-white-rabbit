using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomQuestAlgorithm : QuestAlgorithmBase
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private Quest[] questsToGive; 

    
    //QuestAlgorithmBase functions
    public override Quest[] GetQuests(int questNum, Quest[] questDataBase)
    {
        questsToGive = null;
        questsToGive = new Quest[questNum];
        for(int i = 0; i < questNum; i++)
        {
           questsToGive[i] = questDataBase[Random.Range(0, questDataBase.Length)];
        }

        return questsToGive;
    }
    public override void OnQuestAccepted()
    {

    }
    public override void OnQuestSubmitted()
    {

    }
    public override void OnQuestClosed()
    {

    }
}
