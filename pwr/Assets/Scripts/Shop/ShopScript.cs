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
    public ItemManager itemManager;
    public GameObject playerControllerObject;
    private PlayerController playerController;
    public ShopSaveData shopSaveData;
    private int randNum;
    public WorldController worldController; 

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
    int cost;

    //whiteRabbit Juice
    public Rabbit_Animator rabbitAnimator; 
   

    //recipe management
    Recipe recipe;

    //Save data 



    // Start is called before the first frame update
    void Start()
    {
        //load the item manager, player controller, and save data
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        itemManager = GameObject.FindGameObjectWithTag("item_manager").GetComponent<ItemManager>();
        shopSaveData = GameObject.FindGameObjectWithTag("shop_save").GetComponent<ShopSaveData>();
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>(); 

        //store state
        storeState = StoreState.Furniture; //default 
        currentItemObjectArray = new GameObject[shopSaveData.getMaxStoreItems()]; //const value of 3 
        FurnitureTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Furniture); });
        SeedTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Seeds); });
        RecipeTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Recipes); });
        SellTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Sell); });

        //Exit Button
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);



        //Load store items and set it in the store 
      //  GetNewItemsFromItemManager(storeState, currentItemObjectArray);
       // SetStorePanels();

    }

    // Update is called once per frame
    void Update()
    {
        if(checkIfShopShouldUpdate())
        {
            GetNewItemsFromItemManager(storeState, currentItemObjectArray);
            worldController.isNewDay = false;
        }
        else
        {
            SetStorePanels();
        }
    }

    //Shop Panel setup
    private void SetStorePanels()
    {
        if(storeState != StoreState.Sell)
        {
            SetItemsFromSavedItemArray(storeState, currentItemObjectArray);
        }

        /*
        for(int j = 0; j < currentItemObjectArray.Length; j++)
        {
            SetItemButton(ShopItemObj[j], currentItemObjectArray[j]);
            itemBtn = ShopItemObj[j].GetComponent<Button>();
            itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[j]); });
        }
        */

        //HACK - THERE IS AN INDEX OUT OF BOUNDS ERROR THAT WONT GO AWAY ---- USING THE FOR LOOP CAUSES AN ERROR
        SetItemButton(ShopItemObj[0], currentItemObjectArray[0]);
        itemBtn = ShopItemObj[0].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[0]); });

        SetItemButton(ShopItemObj[1], currentItemObjectArray[1]);
        itemBtn = ShopItemObj[1].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[1]); });

        SetItemButton(ShopItemObj[2], currentItemObjectArray[2]);
        itemBtn = ShopItemObj[2].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[2]); });
    }

    private void updateStorePanels()
    {
        if (storeState != StoreState.Sell)
        {
            SetItemsFromSavedItemArray(storeState, currentItemObjectArray);
        }

        for (int i = 0; i < currentItemObjectArray.Length; i++)
        {
            SetItemButton(ShopItemObj[i], currentItemObjectArray[i]);
            itemBtn = ShopItemObj[i].GetComponent<Button>();
        }
    }

    private void UpdateSinglePanel(GameObject objToUpdate, GameObject updateObj)
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

    private void ItemPurchased(GameObject item)
    {
        //check if player can purchase item, if so remove item from store and add to player inventory
        cost = item.GetComponent<Item>().cost;
        if (playerController.removeCurrency(cost)) 
        {
            if(storeState == StoreState.Recipes)
            {
                recipe = item.GetComponent<Recipe>();
                playerController.UnlockRecipe(recipe, playerController.recipeIndex);
                playerController.recipeIndex++;
            }
            else
            {
                playerController.AddObjectToInventory(item);
            }
            rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
            rabbitAnimator.speechText.text = "Thanks for buying something!";
        }
        else
        {
            //message saying no money :(, make into a pop up in game later?
            rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
            rabbitAnimator.speechText.text = "Sorry you don't have enough money";
        }
    }

    private void ItemSold()
    {

    }
    private void ExitButtonClicked()
    {
        //load main and enable player movement
        playerController.enabled = true;
        playerController.HUD.SetActive(true);
        playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    private void SetItemsFromSavedItemArray(StoreState state, GameObject[] itemArray)
    {
        switch(state)
        {
            case StoreState.Furniture:
                for(int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.furnitureArray[i];
                }
                break;
            case StoreState.Recipes:
                for(int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.recipeArray[i];
                }
                break;
            case StoreState.Seeds:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.seedArray[i];
                }
                break;
            case StoreState.Sell:
                break;
        }
    }

    private void GetNewItemsFromItemManager(StoreState state, GameObject[] itemArray)
    {
        //TODO: Make sure the same item doesn't load twice

        //load a new furniture Array
        for(int i = 0; i < shopSaveData.furnitureArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.furnitureArray.Length);
            shopSaveData.furnitureArray[i] = itemManager.furnitureArray[randNum];
        }
        //load a new seeds array 
        for(int i = 0; i < shopSaveData.seedArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.seedArray.Length);
            shopSaveData.seedArray[i] = itemManager.seedArray[randNum];
        }
        //load a new recipes array 
        for(int i = 0; i < shopSaveData.recipeArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.recipeArray.Length);
            shopSaveData.recipeArray[i] = itemManager.recipeArray[randNum];
        }

        switch (state)
        {
            case StoreState.Furniture:
                for(int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.furnitureArray[i];
                }
                break;
            case StoreState.Recipes:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.recipeArray[i];
                }
                break;
            case StoreState.Seeds:
                for (int i = 0; i < itemArray.Length; i++)
                {
                    itemArray[i] = shopSaveData.seedArray[i];
                }
                break;
            case StoreState.Sell:
                break;
            default:
                break;
        }
    }

    private void SetStoreState(StoreState state)
    {
        storeState = state;
        updateStorePanels();
    }

    private bool checkIfShopShouldUpdate()
    {
        return worldController.isNewDay; 
    }

}
