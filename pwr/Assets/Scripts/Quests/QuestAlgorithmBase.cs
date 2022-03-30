using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestAlgorithmBase : MonoBehaviour
{
    //asks for quests from the quest algorithm
    public abstract Quest[] GetQuests(int questNum);
    //notifies the quest algorithm that a quest has been accepted
    public abstract void OnQuestAccepted();
    //notifies the quest algorithm that a quest has been submitted
    public abstract void OnQuestSubmitted();
    //notifies the quest algorithm that the player closed the quest board
    public abstract void OnQuestClosed(); 

}
