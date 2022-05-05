using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestAlgorithmBase : MonoBehaviour
{
    //set up for the algorithm as needed, runs at start()
    public abstract void SetUpAlgorithm();
    //asks for quests from the quest algorithm
    public abstract Quest[] GetQuests(int questNum, Quest[] questDataBase);
    //notifies the quest algorithm that a quest has been accepted
    public abstract void OnQuestAccepted(Quest quest);
    //notifies the quest algorithm that a quest has been submitted
    public abstract void OnQuestSubmitted();
    //notifies the quest algorithm that the player closed the quest board
    public abstract void OnQuestClosed(); 

}
