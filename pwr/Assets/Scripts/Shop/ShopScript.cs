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
    public GameObject[] shopUIObjectArray;
    public List<GameObject> currentStoreItems = new List<GameObject>();
    private GameObject[] currentItemObjectArray;
    private int cost;
    private const int maxSeedPurchased = 3; 


    //Sold Items List 
    public GameObject[] soldPanelArray;
    public List<GameObject> soldItems;

    //item sellling 
    public TextMeshProUGUI coinText;
    public GameObject[] sellingUIObjectArray;
    public GameObject sellingUIPanel;

    //whiteRabbit Juice
    public Rabbit_Animator rabbitAnimator;

    //Mortage 
    public TextMeshProUGUI moneyOwedText;
    public Button payMortageButton;
    public TMP_InputField amountToPayInput;
    private int amountToPay;
    public int moneyOwed; 

    //recipe management
    Recipe recipe;

    //Save data 

    private void Awake()
    {
        playerControllerObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
    }

    // Start is called before the first frame update
    void Start()
    {
        //load the item manager, player controller, and save data
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerController.isShouldMove = false;
        itemManager = GameObject.FindGameObjectWithTag("item_manager").GetComponent<ItemManager>();
        shopSaveData = GameObject.FindGameObjectWithTag("shop_save").GetComponent<ShopSaveData>();
        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>(); 

        //store state 
        storeState = StoreState.Furniture; //default 
        currentItemObjectArray = new GameObject[shopSaveData.getMaxStoreItems()]; //const value of 3

        //Delegates
        FurnitureTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Furniture); });
        SeedTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Seeds); });
        RecipeTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Recipes); });
        SellTabButton.onClick.AddListener(delegate { SetStoreState(StoreState.Sell); });
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);
        payMortageButton.onClick.AddListener(PayMortage);
        moneyOwedText.text = shopSaveData.mortage.ToString();

        SetStorePanels();
        SetItemSoldPanels();
        sellingUIPanel.SetActive(false);
        UpdateSellingItemsPanel();

        playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = playerController.currency.ToString();
        if(checkIfShopShouldUpdate())
        {
            GetNewItemsFromItemManager(storeState, currentItemObjectArray);
            shopSaveData.soldItemList.Clear();
            worldController.isNewDay = false;
            setItemButtonDelegates();
        }
        else
        {
            SetStorePanels();
            SetItemSoldPanels();
        }
    }

    //Shop Panel setup
    private void SetStorePanels()
    {
        if(storeState != StoreState.Sell)
        {
            SetItemsFromSavedItemArray(storeState, currentItemObjectArray, soldItems);
        }
        else
        {
            
        }

        /*
        for(int j = 0; j < currentItemObjectArray.Length; j++)
        {
            SetItemButton(ShopItemObj[j], currentItemObjectArray[j]);
            itemBtn = ShopItemObj[j].GetComponent<Button>();
            itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[j]); });
        }
        */
    }

    private void setItemButtonDelegates()
    {
        //THIS IS NOT A HACK. THIS IS HOW UNITY WORKS 
        //Delegates are compiled at compile time so the argument to the delegate cannot be created using an array
        //this results in a index out of bounds error if put in a for loop.
        SetItemButton(shopUIObjectArray[0], currentItemObjectArray[0]);
        itemBtn = shopUIObjectArray[0].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[0]); });

        SetItemButton(shopUIObjectArray[1], currentItemObjectArray[1]);
        itemBtn = shopUIObjectArray[1].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[1]); });

        SetItemButton(shopUIObjectArray[2], currentItemObjectArray[2]);
        itemBtn = shopUIObjectArray[2].GetComponent<Button>();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[2]); });
    }

    private void updateStorePanels()
    {
        if (storeState != StoreState.Sell)
        {
            for (int i = 0; i < shopUIObjectArray.Length; i++)
            {
                shopUIObjectArray[i].SetActive(true);
                soldPanelArray[i].SetActive(true);
            }
            sellingUIPanel.SetActive(false);
            SetItemsFromSavedItemArray(storeState, currentItemObjectArray, soldItems);
            SetItemSoldPanels();

            for (int i = 0; i < currentItemObjectArray.Length; i++)
            {
                SetItemButton(shopUIObjectArray[i], currentItemObjectArray[i]);
                itemBtn = shopUIObjectArray[i].GetComponent<Button>();
            }
        }
        else
        {
            for(int i = 0; i < shopUIObjectArray.Length; i++)
            {
                shopUIObjectArray[i].SetActive(false);
                soldPanelArray[i].SetActive(false);
            }
            sellingUIPanel.SetActive(true);
        }
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
            else if(storeState == StoreState.Seeds)
            {
                for(int i = 0; i < maxSeedPurchased; i ++)
                {
                    playerController.AddObjectToInventory(item);
                }
            }
            else
            {
                playerController.AddObjectToInventory(item);
            }
            shopSaveData.soldItemList.Add(item);
            soldItems.Add(item);
            SetItemSoldPanels();
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

    private void ItemSold(GameObject item, GameObject sellingUIObject)
    {
        //add currency to player
        playerController.addCurrency(item.GetComponent<Item>().sellingPrice);
        //remove item from inventory 
        playerController.RemoveObjectFromInventory(item);
        //hide button 
        sellingUIObject.SetActive(false);
    }
    private void UpdateSellingItemsPanel()
    {
        for(int i = 0; i < sellingUIObjectArray.Length; ++i)
        {
            if (playerController.inventory[i] != null)
            {

                sellingUIObjectArray[i].SetActive(true);
                sellingUIObjectArray[i].GetComponent<Image>().sprite = playerController.inventory[i].GetComponent<Item>().itemSprite;
                sellingUIObjectArray[i].GetComponentInChildren<TextMeshProUGUI>().text = playerController.inventory[i].GetComponent<Item>().sellingPrice.ToString();

                //THIS IS NOT A HACK. THIS IS HOW UNITY WORKS (See above)
                //this is ugly but unity needs compile time arguments
                //THIS ASSUMES THAT THE LENGTH OF THE INVENTORY IS 10. IF THAT CHANGES THIS NEEDS TO CHANGE
                if (i == 0)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[0], sellingUIObjectArray[0]); });
                }
                else if (i == 1)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[1], sellingUIObjectArray[1]); });
                }
                else if (i == 2)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[2], sellingUIObjectArray[2]); });
                }
                else if (i == 3)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[3], sellingUIObjectArray[3]); });
                }
                else if (i == 4)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[4], sellingUIObjectArray[4]); });
                }
                else if (i == 5)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[5], sellingUIObjectArray[5]); });
                }
                else if (i == 6)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[6], sellingUIObjectArray[6]); });
                }
                else if (i == 7)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[7], sellingUIObjectArray[7]); });
                }
                else if (i == 8)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[8], sellingUIObjectArray[8]); });
                }
                else if (i == 9)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[9], sellingUIObjectArray[9]); });
                }
            }
            else
            {
                sellingUIObjectArray[i].SetActive(false);
            }
        }
    }

    private void PayMortage()
    {
        if(int.TryParse(amountToPayInput.text.ToString(), out amountToPay))
        {
            if(amountToPay < playerController.currency && amountToPay > 0)
            {
                if(amountToPay < moneyOwed)
                {
                    moneyOwed -= amountToPay;
                    playerController.currency -= amountToPay;
                    moneyOwedText.text = moneyOwed.ToString();
                }
                else
                {
                    moneyOwed = 0;
                    playerController.currency -= moneyOwed;
                    moneyOwedText.text = moneyOwed.ToString();
                }
            }
        }
    }
    private void SetItemSoldPanels()
    {
        for(int i = 0; i < currentItemObjectArray.Length; i++)
        {
            if (soldItems.Contains(currentItemObjectArray[i]))
            {
                soldPanelArray[i].SetActive(true);
            }
            else
            {
                soldPanelArray[i].SetActive(false);
            }    
        }
    }
    private void ExitButtonClicked()
    {
        //load main and enable player movement
        playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);
        playerController.HUD.SetActive(true);
        playerController.enabled = true;
        playerController.isShouldMove = true;
        shopSaveData.mortage = moneyOwed;
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

  
    private void SetItemsFromSavedItemArray(StoreState state, GameObject[] itemArray, List<GameObject> soldItemList)
    {
        soldItemList.Clear();
        foreach(GameObject soldItem in shopSaveData.soldItemList)
        {
            soldItemList.Add(soldItem);
        }

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
