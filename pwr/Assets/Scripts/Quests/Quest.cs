using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{
    public string questName;
    public QuestBoard.QuestType questType; 
    public AEventListener eventListener; //The proper event listener that should go with the quest
    public int reward; //reward is always currency 

    public Quest(string questName, AEventListener eventListener, int reward)
    {
        this.questName = questName;
        this.eventListener = eventListener;
        this.reward = reward; 
    }

    public Quest()
    {
        this.questName = "test";
        this.questType = 0;
        this.reward = 100;
    }
    
}