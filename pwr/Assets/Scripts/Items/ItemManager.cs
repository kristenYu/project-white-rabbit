using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    public GameObject[] furnitureArray;

    //Singleton 
    private static ItemManager instance;
    // Read-only public access
    public static ItemManager Instance => instance;

    private void Awake()
    {
        // Does another instance already exist?
        if (instance && instance != this)
        {
            // Destroy myself
            Destroy(gameObject);
            return;
        }

        // Otherwise store my reference and make me DontDestroyOnLoad
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //This could slow down the game if there is too much furniture, but I think this will be okay for now 
        //if needed, can look into Resources.Async load, and storing item list in a file 
        furnitureArray = Resources.LoadAll<GameObject>("Prefabs/Furniture");
    }

}
