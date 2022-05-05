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
    public string[] eventListenerData;

    public string printString;

    public Quest(string questName, AEventListener eventListener, int reward)
    {
        this.questName = questName;
        this.eventListener = eventListener;
        this.reward = reward; 
    }

    public Quest()
    {
        this.questName = "test";
        this.questType = QuestBoard.QuestType.invalid;
        this.reward = 100;
    }

    public string PrintPretty()
    {
        printString = "";
        printString += "Quest name: " + questName + "\n";
        printString += "Quest type: " + questType + "\n";
        printString += "Reward: " + reward + "\n";
        foreach(string data in eventListenerData)
        {
            printString += "data 1: " + data + "\n";
        }
        return printString;
    }
    
}
