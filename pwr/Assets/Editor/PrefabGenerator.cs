using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System; 


public class PrefabGenerator : MonoBehaviour
{

    [MenuItem("My Generators/Generate Furniture Prefab From Texture2D Selection")]
    static void GenerateFurniturePrefab()
    {
        Debug.Log("Generating Crops " + Selection.count + " Prefabs...");
        Texture2D[] textureArray = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        foreach (Texture2D texture in textureArray)
        {
            string localPath = "Assets/Resources/Prefabs/Furniture/" + texture.name + ".prefab";


            //load in sprites
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Furniture/" + texture.name);


            /*if(sprites.Length != 4)
            {
                Debug.LogError("Expected 4 sprites in texture " + texture.name);
            } */

            //generate new prefab object
            GameObject prefabGameObject = new GameObject();
            prefabGameObject.AddComponent<Furniture>();
            prefabGameObject.AddComponent<SpriteRenderer>();
            prefabGameObject.AddComponent<BoxCollider2D>();
            prefabGameObject.tag = "furniture";
            SpriteRenderer spriteRenderer = prefabGameObject.GetComponent<SpriteRenderer>();
            Furniture furnitureScript = prefabGameObject.GetComponent<Furniture>();
            BoxCollider2D boxCollider = prefabGameObject.GetComponent<BoxCollider2D>();
            //assumes the ordering that can be found in the chair sprites - make sure to reorder these as necessary
            furnitureScript.spriteArray = sprites;
            furnitureScript.stringName = texture.name;
            string[] nameStrings = texture.name.Split(' ');
            if (nameStrings[1] == "Chair")
            {
                furnitureScript.cost = 150; //hardcoded value 
                furnitureScript.sellingPrice = 100; //hardcoded value
            }
            else if (nameStrings[1] == "Bed")
            {
                furnitureScript.cost = 200; //hardcoded value 
                furnitureScript.sellingPrice = 150; //hardcoded value
            }
            else if (nameStrings[1] == "Lamp")
            {
                furnitureScript.cost = 50; //hardcoded value 
                furnitureScript.sellingPrice = 30; //hardcoded value
            }
            else if (nameStrings[1] == "Plant" || nameStrings[1] == "Flower")
            {
                furnitureScript.cost = 70; //hardcoded value 
                furnitureScript.sellingPrice = 50; //hardcoded value
            }

            furnitureScript.itemSprite = furnitureScript.spriteArray[0];


            spriteRenderer.sprite = furnitureScript.spriteArray[0];
            spriteRenderer.sortingLayerName = "Foreground";
            spriteRenderer.sortingOrder = 1;

            boxCollider.size = new Vector2(1, 1);

            // Create the new Prefab
            bool isCreatedPrefab;
            PrefabUtility.SaveAsPrefabAsset(prefabGameObject, localPath, out isCreatedPrefab);
            if (!isCreatedPrefab)
            {
                Debug.LogWarning(texture.name + " could not be made into a prefab");
            }

            //Clean up for the scene
            DestroyImmediate(prefabGameObject);

        }
        Debug.Log("Finished Making Furniture Prefabs");
    }


    // Disable the menu item if the proper items are not selected
    [MenuItem("My Generators/Generate Furniture Prefab From Texture2D Selection", true)]
   static bool ValidateCreatePrefab()
    {
        return Selection.activeObject != null && Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length > 0; 
    }


    [MenuItem("My Generators/Generate Crop Gameobjects from Crop Tiles")]
    static void GenerateAllCropPrefab()
    {
        Debug.Log("Generating Crop, Food, and Seed Prefabs...");
        Texture2D[] textureArray = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
        Texture2D texture; 

        if (textureArray.Length > 1)
        {
            Debug.LogError("Too many Selections for the generator");
        }
        else
        {
            texture = textureArray[0];
            //load in sprites
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Plants/" + texture.name);
            List<Sprite> validSpriteList = new List<Sprite>();
            Sprite[] foodSprites = Resources.LoadAll<Sprite>("Sprites/Plants/food");


            foreach (Sprite sp in sprites)
            {
                string[] spritestrings = sp.name.Split('_');
                if(spritestrings.Length == 3)
                {
                    validSpriteList.Add(sp);
                }
            }
            int maxCropStage = 5;
            Sprite[,] groupedSpriteArray = new Sprite[validSpriteList.Count/maxCropStage,maxCropStage];
            int currentIndex = 0;
            int currentSpriteIndex = 0;
            for(int i = 0; i < validSpriteList.Count; i++)
            {
                groupedSpriteArray[currentIndex, currentSpriteIndex] = validSpriteList[i];
                if(currentSpriteIndex == maxCropStage-1)
                {
                    currentSpriteIndex = 0;
                    currentIndex++; 
                    
                }
                else
                {
                    currentSpriteIndex++; 
                }
            }

            for(int i = 0; i < groupedSpriteArray.GetLength(0); i++)
            {
                //get relavent string data
                string[] spriteStrings = groupedSpriteArray[i, 0].name.Split('_');
                Sprite basicSeedSprite = Resources.Load<Sprite>("Sprites/Plants/seed_bag");


                string seedPath = "Assets/Resources/Prefabs/Seeds/" + spriteStrings[1] + "_seed.prefab";
                string cropPath = "Assets/Resources/Prefabs/Crops/" + spriteStrings[1] + "_crop.prefab";
                string foodPath = "Assets/Resources/Prefabs/Food/" + spriteStrings[1] + "_food.prefab";

                //generate new gameobjectss
                GameObject seedObject = new GameObject();
                GameObject cropObject = new GameObject();
                GameObject foodObject = new GameObject();

                //seed prefab
                seedObject.name = spriteStrings[1] + "_seed";
                seedObject.tag = "seed";
                seedObject.AddComponent<Seed>();
                seedObject.AddComponent<SpriteRenderer>(); 
                Seed seedScript = seedObject.GetComponent<Seed>();
                seedScript.stringName = spriteStrings[1];
                seedScript.itemSprite = basicSeedSprite;
                SpriteRenderer seedRenderer = seedObject.GetComponent<SpriteRenderer>();
                seedRenderer.sprite = seedScript.itemSprite;
                seedRenderer.sortingLayerName = "Foreground";

                //crop prefab
                cropObject.name = spriteStrings[1] + "_crop";
                cropObject.tag = "crop";
                cropObject.AddComponent<Crop>();
                cropObject.AddComponent<BoxCollider2D>();
                cropObject.AddComponent<SpriteRenderer>(); 
                Crop cropScript = cropObject.GetComponent<Crop>();
                cropScript.cropname = spriteStrings[1];
                cropScript.SpriteGrowingArray = new Sprite[groupedSpriteArray.GetLength(1)];
                //getlength (1) returns the width of the array -> GetLength() returns [length, width] 
                for(int j = 0; j < groupedSpriteArray.GetLength(1); j++)
                {
                    cropScript.SpriteGrowingArray[j] = groupedSpriteArray[i,j];
                }
                SpriteRenderer cropRenderer = cropObject.GetComponent <SpriteRenderer>();
                cropRenderer.sprite = cropScript.SpriteGrowingArray[0];
                cropRenderer.sortingLayerName = "Foreground";

                //food prefab
                foodObject.name = spriteStrings[1] + "_food";
                foodObject.tag = "food";
                foodObject.AddComponent<Food>();
                foodObject.AddComponent<SpriteRenderer>(); 
                Food foodScript = foodObject.GetComponent<Food>();
                foodScript.stringName = spriteStrings[1]; ;
                foreach(Sprite sp in foodSprites)
                {
                    if (sp.name == spriteStrings[1])
                    {
                        foodScript.itemSprite = sp; 
                    }
                }
                SpriteRenderer foodRenderer = foodObject.GetComponent<SpriteRenderer>();
                foodRenderer.sprite = foodScript.itemSprite;
                foodRenderer.sortingLayerName = "Foreground";

                // Create the new Prefabs
                //Food Prefab
                bool isCreatedPrefab;
                GameObject foodPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(foodObject, foodPath, InteractionMode.UserAction, out isCreatedPrefab);
                if (!isCreatedPrefab)
                {
                    Debug.LogWarning(spriteStrings[1] + " could not be made into a crop prefab");
                }
                //Clean up for the scene
                DestroyImmediate(foodObject);

                //Crop Prefab
                //adds the food prefab object to the crop prefab
                cropScript.food = foodPrefab;
                GameObject cropPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(cropObject, cropPath, InteractionMode.UserAction, out isCreatedPrefab);
                if (!isCreatedPrefab)
                {
                    Debug.LogWarning(spriteStrings[1] + " could not be made into a crop prefab");
                }
                //Clean up for the scene
                DestroyImmediate(cropObject);

                //Seed Prefab
                //Adds the crop prefab to the seed prefab
                seedScript.crop = cropPrefab;
                GameObject seedPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(seedObject, seedPath, InteractionMode.UserAction, out isCreatedPrefab);
                if (!isCreatedPrefab)
                {
                    Debug.LogWarning(spriteStrings[1] + " could not be made into a seed prefab");
                }
                //Clean up for the scene
                DestroyImmediate(seedObject);



               
            }
        }

        Debug.Log("Finished Making Crop Prefabs");
    }

    [MenuItem("My Generators/Generate Recipes from Recipe JSON")]
    static void GenerateRecipePrefabsFromJson()
    {
        Debug.Log("Generating Recipes from data ...");
        TextAsset[] recipeDataFiles = Resources.LoadAll<TextAsset>("Data/Recipes/");
        RecipeData[] recipeDatabase = new RecipeData[recipeDataFiles.Length];
        RecipeData currentRecipeData;



        foreach (TextAsset recipeFile in recipeDataFiles)
        {
            //All quest data except for the event listener - the event listener is instantiated when the quest is accepted
            currentRecipeData = JsonUtility.FromJson<RecipeData>(recipeFile.ToString());
            string cookedFoodPath = "Assets/Resources/Prefabs/Cooked Food/" + currentRecipeData.stringName + "_cookedFood.prefab";
            string recipePath = "Assets/Resources/Prefabs/Recipes/" + currentRecipeData.stringName + "_recipe.prefab";

            //create cooked food prefab
            GameObject cookedFoodObject = new GameObject();
            cookedFoodObject.AddComponent<CookedFood>();
            CookedFood cookedFoodScript = cookedFoodObject.GetComponent<CookedFood>();
            cookedFoodScript.stringName = currentRecipeData.stringName;
            cookedFoodScript.sellingPrice = currentRecipeData.cookedFoodSellingPrice;
            Sprite cookedFoodSprite = Resources.Load<Sprite>("Sprites/CookedFood/" + currentRecipeData.stringName);
            cookedFoodScript.itemSprite = cookedFoodSprite;
            //expects there to be a sprite with the same name of the recipe
            cookedFoodObject.AddComponent<SpriteRenderer>();
            SpriteRenderer cookedFoodRenderer = cookedFoodObject.GetComponent<SpriteRenderer>();
            cookedFoodRenderer.sprite = cookedFoodScript.itemSprite;
            cookedFoodRenderer.sortingLayerName = "foreground";
            //So that the cooked food can be placed and displayed
            cookedFoodObject.tag = "furniture";

            //Create Recipe Prefab
            GameObject recipeObject = new GameObject();
            recipeObject.AddComponent<Recipe>();
            Recipe recipeScript = recipeObject.GetComponent<Recipe>();
            recipeScript.stringName = currentRecipeData.stringName;
            recipeScript.ingredients = currentRecipeData.ingredients;
            recipeScript.cost = currentRecipeData.cost;
            recipeScript.itemSprite = cookedFoodSprite;

            // Create the new Prefabs
            //Cooked Food Prefab
            bool isCreatedPrefab;
            GameObject cookedFoodPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(cookedFoodObject, cookedFoodPath, InteractionMode.UserAction, out isCreatedPrefab);
            if (!isCreatedPrefab)
            {
                Debug.LogWarning(currentRecipeData.stringName + " could not be made into a cooked food prefab");
            }
            //Clean up for the scene
            DestroyImmediate(cookedFoodObject);

            //recipe Prefab
            recipeScript.cookedFood = cookedFoodPrefab; //add cooked food prefab to recipe prefab
            GameObject recipePrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(recipeObject, recipePath, InteractionMode.UserAction, out isCreatedPrefab);
            if (!isCreatedPrefab)
            {
                Debug.LogWarning(currentRecipeData.stringName + " could not be made into a recipe prefab");
            }
            DestroyImmediate(recipeObject);
        }
    }


    [MenuItem("My Generators/DONOTUSE Generate Recipes from Recipe Data")]
    static void GenerateRecipePrefabs()
    {
        //Recipes need to be formated name of <recipe name>, <selling price>, <ingredient 1>, <ingredient 2>, <ingredient 3>
        //This script parses by new line, so to add a new recipe to the list you just need to add a new line
        Debug.Log("Generating Recipes from data ...");
        TextAsset[] textAssetList = Selection.GetFiltered<TextAsset>(SelectionMode.Assets);

       
        foreach (TextAsset textAsset in textAssetList)
        {
            
            string data = textAsset.text;
            //string[] recipeList = data.Split('\n');
            char[] delims = new[] { '\r', '\n' };
            string[] recipeList = data.Split(delims, StringSplitOptions.RemoveEmptyEntries);
            foreach (string recipe in recipeList)
            {


                string[] recipeComponents = recipe.Split(',');

                string cookedFoodPath = "Assets/Resources/Prefabs/Cooked Food/" + recipeComponents[0] + "_cookedFood.prefab";
                string recipePath = "Assets/Resources/Prefabs/Recipes/" + recipeComponents[0] + "_recipe.prefab";

                //create cooked food prefab
                GameObject cookedFoodObject = new GameObject();
                cookedFoodObject.AddComponent<CookedFood>();
                CookedFood cookedFoodScript = cookedFoodObject.GetComponent<CookedFood>();
                cookedFoodScript.stringName = recipeComponents[0];
                cookedFoodScript.sellingPrice = int.Parse(recipeComponents[1]);
                Sprite cookedFoodSprite = Resources.Load<Sprite>("Sprites/CookedFood/" + recipeComponents[0]);
                cookedFoodScript.itemSprite = cookedFoodSprite;
                //expects there to be a sprite with the same name of the recipe
                cookedFoodObject.AddComponent<SpriteRenderer>();
                SpriteRenderer cookedFoodRenderer = cookedFoodObject.GetComponent<SpriteRenderer>();
                cookedFoodRenderer.sprite = cookedFoodScript.itemSprite;
                cookedFoodRenderer.sortingLayerName = "foreground";
                //So that the cooked food can be placed and displayed
                cookedFoodObject.tag = "furniture";
                

                //create recipe prefab 
                int offset = 2; 
                GameObject recipeObject = new GameObject();
                recipeObject.AddComponent<Recipe>();
                Recipe recipeScript = recipeObject.GetComponent<Recipe>();
                recipeScript.stringName = recipeComponents[0];

                recipeScript.ingredients = new string[recipeComponents.Length - offset];
                for(int i = 0; i < recipeScript.ingredients.Length; i++)
                {
                    recipeScript.ingredients[i] = recipeComponents[i + offset];
                }


                // Create the new Prefabs
                //Cooked Food Prefab
                bool isCreatedPrefab;
                GameObject cookedFoodPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(cookedFoodObject, cookedFoodPath, InteractionMode.UserAction, out isCreatedPrefab);
                if (!isCreatedPrefab)
                {
                    Debug.LogWarning(recipeComponents[0] + " could not be made into a cooked food prefab");
                }
                //Clean up for the scene
                DestroyImmediate(cookedFoodObject);

                //recipe Prefab
                recipeScript.cookedFood = cookedFoodPrefab; //add cooked food prefab to recipe prefab
                GameObject recipePrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(recipeObject, recipePath, InteractionMode.UserAction, out isCreatedPrefab);
                if (!isCreatedPrefab)
                {
                    Debug.LogWarning(recipeComponents[0] + " could not be made into a recipe prefab");
                }
                DestroyImmediate(recipeObject);

            }
        }
        Debug.Log("Finished Creating Recipe and Cooked Food Prefabs");
    }

    // Disable the menu item if the proper items are not selected
    [MenuItem("My Generators/Generate Recipes from Recipe Data", true)]
    static bool ValidateCreateRecipes()
    {
        return Selection.activeObject != null && Selection.GetFiltered<TextAsset>(SelectionMode.Assets).Length > 0;
    }

}



