using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSaveData : MonoBehaviour
{

    //Do not destroy on load 
    //Singleton 
    private static ShopSaveData instance;
    // Read-only public access
    public static ShopSaveData Instance => instance;

    //arrays to save items; only update when a day has passed
    public GameObject[] furnitureArray;
    public GameObject[] seedArray;
    public GameObject[] recipeArray;
    private int maxStoreItems = 3;


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

        //comes in on awake instead of start to ensure save data is set up before the shop script tries to access it
        recipeArray = new GameObject[maxStoreItems];
        seedArray = new GameObject[maxStoreItems];
        furnitureArray = new GameObject[maxStoreItems];
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public int getMaxStoreItems()
    {
        return maxStoreItems;
    }

}
