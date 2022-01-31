using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{

    [MenuItem("My Generators/Generate Furniture Prefab From Texture2D Selection")]
    static void GenerateFurniturePrefab()
    {
        Debug.Log("Generating Crops " + Selection.count + " Prefabs...");
        Texture2D[] textureArray = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        foreach ( Texture2D texture in textureArray)
        {
            string localPath = "Assets/Resources/Prefabs/Furniture/" + texture.name + ".prefab";

            //load in sprites
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Furniture/" + texture.name);  
            if(sprites.Length != 4)
            {
                Debug.LogError("Expected 4 sprites in texture " + texture.name);
            }

            //generate new prefab object
            GameObject prefabGameObject = new GameObject();
            prefabGameObject.AddComponent<Furniture>();
            Furniture furnitureScript = prefabGameObject.GetComponent<Furniture>();
            //assumes the ordering that can be found in the chair sprites - make sure to reorder these as necessary
            furnitureScript.backSprite = sprites[0];
            furnitureScript.frontSprite = sprites[1];
            furnitureScript.leftSprite = sprites[2];
            furnitureScript.rightSprite = sprites[3];
            furnitureScript.stringName = texture.name;
            furnitureScript.itemSprite = furnitureScript.frontSprite;

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

            foreach(Sprite sp in sprites)
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
}
