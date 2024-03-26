# FarmQuest 

Farm quest is a video game test bed intended to further AI Director and procedural quest generation research. 

Farm Quest is developed on unity 2020.3.24f1. For best results, download this version of unity and open the project. FarmQuest can be forked if you would like to add in other AI directors to the project, or modify the existing project in any other way. 


## How to Play FarmQuest
To play farmquest, use WASD to move, and E to interact. All of the UI can be interacted by clicking with a mouse. 

## Features in FarmQuest 
FarmQuest features a fully designed game loop, with various bits of gameplay. This part of the game is still under development, but the following gameplay is currently available in FarmQuest. 

1. Harvesting mushrooms and berries
1. Buying seeds at a shop 
1. Planting and harvesting vegtables and fruits
1. Cooking food
1. Buying furniture 
1. Decorating the house with furniture 
1. Selling items in the shop 
1. Completing quests for money 
1. A mortage system to use as a goal
1. A tutorial

## AI Directors in FarmQuest
FarmQuest is intended for use as a tool to enable AI director research. The AI director in the game chooses which quest the player sees on the quest board. The Quest board will be populated with a specific AI director, depending on which one is selected. Currently, the AI director is selected at random. FarmQuest comes with 3 AI Directors. A [reinforcement based AI Director](https://webdocs.cs.ualberta.ca/~nathanst/papers/yu2022director.pdf), an AI director that tries to learn from player actions, and a random AI director which randomly selects quests. 

## Adding a new AI Director
All AI Directors need to extend the QuestAlgorithmBase class found in Assets/Scripts/Quests. The QuestBoard script then calls these methods at the appropriate time. 
There are 6 methods in this class: 
1. SetUpAlgorithm - runs during the Start() method
1. GetQuests - asks the algorithm to return quests
1. OnQuestAccepted - runs when a quest has been accepted
1. OnQuestSubmitted - runs when a quest has been submitted for completion
1. OnQuestClosed - runs when the player stops looking at the quest board

Once you have made a new QuestAlgorithm class, you need to add it to the QuestBoard Script. To do this, open the QuestBoard Scene in Assets/Scenes. From there, make a new empty game object, and add the new AI Director script to the object. For example, see the RandomQuestAlgorihtm gameobject in the scene. Then, in the QuestBoard object add the game object you've created as a new element in QuestAlgorithms array.

## Adding a new quest 
All quests are saved as json files. The name of the file does not matter. To make a new quest of an existing type, simply make a new json file. Below is an example of a harvest 10 berry quest. 
```
{"questName": "Harvest 10 Berry",
"questType": 3, 
"reward": 100, 
"eventListenerData": [ "berry", "10"]
}
```
Where 
1. questName is the display name of the quest to the player
1. questType corresponds to the enum QuestType in the QuestBoard script. (Note, invalid should always be the last quest type in the enum)
  * plant is 0
  * cook is 1
  * place is 2
  * harvest is 3
  * invalid is 4
3. reward is the amount of currency the player gets from completing the quest, should be type int
4. eventListenerdData corresponds to the event listener type. This list is parsed by the Questboard in function AcceptQuest and creates a quest from the corresponding values. The values need to be in order as the function expects the data to be in a particular order.
* plant type uses array ["\<type of seed\>", "\<number of seeds to plant\>"]
* cook type uses array ["\<type of ingredient to use\>", "\<number of dishes to cook\>"]
* place uses array  ["\<number of furniture to place\>"]
* harvest uses array   ["\<type of harvestable\>", "\<number of harvests\>"]

## Adding a New Recipe
Recipes can be added to the game, but they require a little bit of work. There are two pieces that need to be done by hand - the recipe JSON and the cooked food sprite. First, the recipe JSON is configured as follows:

```
{"stringName": "lettuce sandwich", 
"ingredients": ["lettuce", "carrot", "tomato"],
"cost": 50,
"cookedFoodSellingPrice": 50
}
```
Where

1. stringName is the name that the player sees
2. The ingredients list is the string names of the ingredients that need to be made. They have to be valid food objects, and there is minimum 1 maximum 3 ingredients
3. The cost is the cost to unlock the recipe as an int
4. The cookedFoodSellingPrice is the price of selling the final cooked food as an int

Second, add in a sprite with the exact name of the cooked food in the cooked food folder in sprites. 

## Adding a New Quest Type 
Quests rely on the Event Listener System to track quests. Quests are tracked by manually sending notifications to Event Listener Objects. Each Type of Quest has it's own event listener class that extends the abstract AEventListener base class. The AEventListener base class is set up as the following: 

```
    //Manager Variables 
    public bool IsEventHasBeenUpdated;
    public bool IsEventCompleted; 

    //Register the event listener with the event listener manager
    public void RegisterToManager()
    {
    }

    //Tells the listener when to start listening to the gameobject 
    public abstract void OnStartListening();

    //Clean up for when the Event Listener should be destroyed
    public abstract void OnEndListening();

    //Is called when the event is updated
    public abstract void OnEventUpdate();

    //Is called when the event is completed
    public abstract void OnEventCompleted();

    //set one event listener equal to the other 
    public abstract void Equals(AEventListener otherEventListener); 
```

There are two booleans to track if the event has been updated or completed, and then various functions for listening and clean up. Each event listener has it's own type of struct, found in the EventListenerStructs script. These structs track the relevant information that needs to be stored by a quest in order to track progress. For example, the plant seed struct has two fields: 

```
 //struct to check when a seed is planted
    [System.Serializable]
    public struct PlantSeedStruct
    {
        public string name;
        public int targetValue; 
    }
```
Where name is the name of the seed that should be tracked, and targetValue is the number of seeds that should be planted. This struct matches the ```eventListenerData``` field in the quest data object. 

When making a new Event Listener class, extend the abstract AEventListenerClass. All of the functions need to be defined, but OnStartLIstening() and Equals() are required to be defined in order for proper functionality to work. For examples, see any of the event listeners currently in the game. 

The EventListeningManager will automatically create event listeners when a quest begins tracking. It then manages the state of each event listener, so it knows when each of the events are considered completed, and will perform clean up of the event listeners when they are no longer needed. 


## Credits 
Art Credits: 
https://helm3t.itch.io/farmlandia-fruit
https://cupnooble.itch.io/
https://ghostpixxells.itch.io/pixelfood

Music by: 
"Farm Village" by Kirk Osamayo from Free Music Archive liscence [CC by 4.0 ](https://creativecommons.org/licenses/by/4.0/)
Mixkit "bonus earned in video game" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "quick jump arcade" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Catching a Basketball" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Crunchy road fast walking loop" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Hard Typewriter Click" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Rooster Crowing in the Morning" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Garden Shovel Stab" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Mouse Click Close" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Typewriter Soft Click" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Mixkit "Slide Click" sound effect by [mixkit liscence](https://mixkit.co/license/#sfxFree)
Sound Effect "bush rustle" from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=6986">Pixabay</a>
Sound Effect "Coins27" from <a href="https://pixabay.com/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=36030">Pixabay</a>
Sound Effect "cloth rustle" from <a href="https://pixabay.com/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=30053">Pixabay</a>
Sound Effect "tear grass" from <a href="https://pixabay.com/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=81384">Pixabay</a>
Sound Effect "place" from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=100513">Pixabay</a>
Sound Effect "Happy Airlines ym-2413" from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=28411">Pixabay</a>
Sound Effect "setting drink down 2" from <a href="https://pixabay.com/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=106992">Pixabay</a>
Sound Effect "big plant growing quickly" from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=43721">Pixabay</a>
Sound Effect "Celery-Chop" from <a href="https://pixabay.com/sound-effects/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=62378">Pixabay</a>
Sound Effect "Walking on a Wooden Floor" fro <Sound Effect from <a href="https://pixabay.com/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=72830">Pixabay</a>
