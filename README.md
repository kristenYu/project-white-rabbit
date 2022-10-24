# FarmQuest 

Farm quest is a video game test bed intended to further AI Director and procedural quest generation research. 

Farm Quest is developed on unity 2020.3.24f1. For best results, download this version of unity and open the project. FarmQuest can be forked if you would like to add in other AI directors to the project. 

##How to Play FarmQuest
To play farmquest, use WASD to move, and E to interact. All of the UI can be interacted by clicking with a mouse. 

##Features in FarmQuest 
FarmQuest features a fully designed game loop, with various bits of gameplay. This part of the game is still under development, but the following gameplay is currently available in FarmQuest. 

1. Harvesting mushrooms 
1. Buying seeds at a shop 
1. Planting and harvesting vegtables and fruits
1. Cooking food
1. Buying furniture 
1. Decorating the house with furniture 
1. Selling items in the shop 
1. Completing quests for money 
1. A mortage system to use as a goal

##Adding a new AI Director
All AI Directors need to extend the QuestAlgorithmBase class found in Assets/Scripts/Quests. The QuestBoard script then calls these methods at the appropriate time. 
There are 6 methods in this class: 
1. SetUpAlgorithm - runs during the Start() method
1. GetQuests - asks the algorithm to return quests
1. OnQuestAccepted - runs when a quest has been accepted
1. OnQuestSubmitted - runs when a quest has been submitted for completion
1. OnQuestClosed - runs when the player stops looking at the quest board

Once you have made a new QuestAlgorithm class, you need to add it to the QuestBoard Script. To do this, open the QuestBoard Scene in Assets/Scenes. From there, make a new empty game object, and add the new AI Director script to the object. For example, see the RandomQuestAlgorihtm gameobject in the scene. Then, in the QuestBoard object add the game object you've created as a new element in QuestAlgorithms array.



##Credits 
Art Credits: 
https://helm3t.itch.io/farmlandia-fruit
https://cupnooble.itch.io/
https://ghostpixxells.itch.io/pixelfood

Music by: 
Music by Pixabay




