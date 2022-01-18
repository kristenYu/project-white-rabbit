using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabGenerator : MonoBehaviour
{

    [MenuItem("My Generators/Generate Furniture Prefab From Texture2D Selection")]
    static void GenerateFurniturePrefab()
    {
        Debug.Log("Generating Furniture " + Selection.count + " Prefabs...");
        Texture2D[] textureArray = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        foreach ( Texture2D texture in textureArray)
        {
            //Create unique prefab location - prevents prefabs of the same name from being created
            string localPath = "Assets/Prefabs/Furniture/" + texture.name + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            //load in sprites
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Furniture/" + texture.name);  
            if(sprites.Length != 4)
            {
                Debug.LogError("Expected 4 sprites in texture " + texture.name);
            }

            //generate new prefab object
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<Furniture>();
            Furniture furnitureScript = gameObject.GetComponent<Furniture>();
            //assumes the ordering that can be found in the chair sprites - make sure to reorder these as necessary
            furnitureScript.backSprite = sprites[0];
            furnitureScript.frontSprite = sprites[1];
            furnitureScript.leftSprite = sprites[2];
            furnitureScript.rightSprite = sprites[3];
            furnitureScript.stringName = texture.name;
            furnitureScript.itemSprite = furnitureScript.frontSprite;

            // Create the new Prefab
            bool isCreatedPrefab;
            PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out isCreatedPrefab);
            if(!isCreatedPrefab)
            {
                Debug.LogWarning(texture.name + " could not be made into a prefab");
            }

            //Clean up for the scene
            DestroyImmediate(gameObject);

        }
        Debug.Log("Finished Making Furniture Prefabs");
    }

    // Disable the menu item if no selection is in place.
    [MenuItem("My Generators/Generate Furniture Prefab From Texture2D Selection", true)]
    static bool ValidateCreatePrefab()
    {
        return Selection.activeGameObject != null && !EditorUtility.IsPersistent(Selection.activeGameObject);
    }
}
