using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopScript : MonoBehaviour
{
    public enum StoreState{
        Furniture = 0, 
        Seeds,
        Recipes, 
        Sell
    }

    //Exit Shop
    public Button ExitToMainButton;
    private Button ExitBtn;

    //Access Items Database and Player Controller
    public GameObject[] item_managerArray;
    public ItemManager itemManager;
    public GameObject playerControllerObject;
    private PlayerController playerController;
    private int randNum;

    //store state 
    public StoreState storeState;
    public Button FurnitureTabButton;
    public Button SeedTabButton;
    public Button RecipeTabButton;
    public Button SellTabButton; 

    //Shop item panel elements
    public Button itemBtn;
    public Image itemImage;
    public Text itemName;
    public Text itemCost;
    public GameObject[] ShopItemObj;
    public List<GameObject> currentStoreItems = new List<GameObject>();
    private GameObject[] currentItemObjectArray;
    public const int maxStoreItems = 3; 

    // Start is called before the first frame update
    void Start()
    {
        //load the item manager and player controller
        playerController = playerControllerObject.GetComponent<PlayerController>();

        //Setting Button to move back to main
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);

        //store state
        storeState = StoreState.Furniture; //default 
        FurnitureTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Furniture); });
        SeedTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Seeds); });
        RecipeTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Recipes); });
        SellTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Sell); });

        //item panes 
        currentItemObjectArray = new GameObject[maxStoreItems]; //const value of 3 

        //Using Tags to access itemManager...
        item_managerArray = GameObject.FindGameObjectsWithTag("item_manager");
        foreach(GameObject manager in item_managerArray)
        {
            itemManager = manager.GetComponent<ItemManager>();
        }
        //Load store items and set it in the store 
        UpdateStorePanels(false);
    }

    // Update is called once per frame
    void Update()
    {
        //set furniture for day


        //purchase item

        //sell item
    }

    //Shop Panel setup
    private void UpdateStorePanels(bool itemPurchased)
    {
        foreach (GameObject obj in ShopItemObj)
        {
            if(storeState != StoreState.Sell)
            {
                GetItemsFromItemManager(storeState, currentItemObjectArray);
            }

            for(int i = 0; i < currentItemObjectArray.Length; i++)
            {
                SetItemButton(obj, currentItemObjectArray[i]);
                itemBtn = obj.GetComponent<Button>();
                itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[i]); });
            }

            /*--------------------------------------
            randNum = Random.Range(0, itemManager.furnitureArray.Length);

            while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
            {
                randNum = Random.Range(0, itemManager.furnitureArray.Length);
            }
            currentStoreItems.Add(itemManager.furnitureArray[randNum]);
            SetItemButton(obj, itemManager.furnitureArray[randNum]);
            itemBtn.onClick.AddListener(delegate { ItemPurchased(itemManager.furnitureArray[randNum]); });
            */
        }
    }

    private void UpdatePanel(GameObject objToUpdate, GameObject updateObj)
    {
        itemImage = objToUpdate.transform.GetChild(0).GetComponent<Image>();
        itemName = objToUpdate.transform.GetChild(1).GetComponent<Text>();
        itemCost = objToUpdate.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = updateObj.transform.GetChild(0).GetComponent<Image>().sprite;
        itemName.text = updateObj.transform.GetChild(1).GetComponent<Text>().text;
        itemCost.text = updateObj.transform.GetChild(2).GetComponent<Text>().text;
    }

    //Item Setup
    private void SetItemButton(GameObject obj, GameObject currentItem)
    {
        itemImage = obj.transform.GetChild(0).GetComponent<Image>();
        itemName = obj.transform.GetChild(1).GetComponent<Text>();
        itemCost = obj.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = currentItem.GetComponent<Item>().itemSprite;
        itemName.text = currentItem.GetComponent<Item>().stringName;
        itemCost.text = currentItem.GetComponent<Item>().cost.ToString();
    }  

    private void ItemPurchased(GameObject item){

        //check if player can purchase item, if so remove item from store and add to player inventory
        int cost = item.GetComponent<Item>().cost;
        //TODO: Replace with find by tag
        playerControllerObject = GameObject.Find("Player");
        playerController = playerControllerObject.GetComponent<PlayerController>();
        if (playerController.removeCurrency(cost)) 
        {
            playerController.AddObjectToInventory(item);
            for(int obj = 0; obj < ShopItemObj.Length; obj++)
            {
                if (ShopItemObj[obj].transform.GetChild(1).GetComponent<Text>().text == item.name) {
                    for (int i = obj; i < ShopItemObj.Length; i++)
                    {
                        print(ShopItemObj.Length);
                        if ((i+1) >= ShopItemObj.Length || ShopItemObj[i+1].activeSelf == false)
                        { 
                            ShopItemObj[i].SetActive(false);
                            print("Removed");
                        }   
                        else
                        {
                            print("moved?");
                            GameObject temp = ShopItemObj[i];
                            ShopItemObj[i] = ShopItemObj[i + 1];
                            //ShopItemObj[i + 1] = temp;
                            UpdatePanel(temp, ShopItemObj[i]);
                        }
                    }
                }
            }
        }
        else
        {
            //message saying no money :(, make into a pop up in game later?
            print("Sorry! You don't have enough money!!");
        }
    }

    private void ItemSold()
    {

    }
    private void ExitButtonClicked()
    {
        //load main and enable player movement
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        playerControllerObject = GameObject.Find("Player");
        playerController = playerControllerObject.GetComponent<PlayerController>();
        playerController.enabled = true;
        playerController.HUD.SetActive(true);
    }

    private void GetItemsFromItemManager(StoreState state, GameObject[] itemArray)
    {
        switch (state)
        {
            case StoreState.Furniture:
                for(int i = 0; i < itemArray.Length; i++)
                {
                    randNum = Random.Range(0, itemManager.furnitureArray.Length);
                    itemArray[i] = itemManager.furnitureArray[randNum];
                }
                break;
            case StoreState.Recipes:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    randNum = Random.Range(0, itemManager.recipeArray.Length);
                    itemArray[i] = itemManager.recipeArray[randNum];
                }
                break;
            case StoreState.Seeds:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    randNum = Random.Range(0, itemManager.seedArray.Length);
                    itemArray[i] = itemManager.seedArray[randNum];
                }
                break;
            default:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    randNum = Random.Range(0, itemManager.furnitureArray.Length);
                    itemArray[i] = itemManager.furnitureArray[randNum];
                }
                break;
        }
    }

    private void SetStoreState(StoreState state)
    {
        storeState = state;
        UpdateStorePanels(false);
    }

}
