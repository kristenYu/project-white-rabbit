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
    private int previousRandNum;
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

    //recipe selling objects;
    public Image recipeIngredientImage1;
    public Image recipeIngredientImage2;
    public Image recipeIngredientImage3;
    private Recipe currentRecipe;
    private Image[] recipeIngredientImageArray; 


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

    //SaveData
    private List<int> randomNumberList; 

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
        randomNumberList = new List<int>();
        recipeIngredientImageArray = new Image[3]; //hardcoded for the maximum number of ingredients in a recipe
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


        if(shopSaveData.furnitureArray[0] == null)
        {
            PopulateNewItemsForShopSaveData(storeState, currentItemObjectArray);
        }
        else
        {
            LoadShopSaveData(storeState, currentItemObjectArray, soldItems);
        }
        updateStorePanels();
        setItemButtonDelegates();
        SetItemSoldPanels(storeState);
        sellingUIPanel.SetActive(false);
        UpdateSellingItemsPanel();

        playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = playerController.currency.ToString();
        if (checkIfShopShouldUpdate())
        {
            PopulateNewItemsForShopSaveData(storeState, currentItemObjectArray);
            shopSaveData.soldItemList.Clear();
            worldController.isNewDay = false;
            updateStorePanels();
            setItemButtonDelegates();
        }
    }

    private void setItemButtonDelegates()
    {
        //THIS IS NOT A HACK. THIS IS HOW UNITY WORKS 
        //Delegates are compiled at compile time so the argument to the delegate cannot be created using an array
        //this results in a index out of bounds error if put in a for loop.
        SetItemButtonUI(shopUIObjectArray[0], currentItemObjectArray[0]);
        itemBtn = shopUIObjectArray[0].GetComponent<Button>();
        itemBtn.onClick.RemoveAllListeners();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[0]); });

        SetItemButtonUI(shopUIObjectArray[1], currentItemObjectArray[1]);
        itemBtn = shopUIObjectArray[1].GetComponent<Button>();
        itemBtn.onClick.RemoveAllListeners();
        itemBtn.onClick.AddListener(delegate { ItemPurchased(currentItemObjectArray[1]); });

        SetItemButtonUI(shopUIObjectArray[2], currentItemObjectArray[2]);
        itemBtn = shopUIObjectArray[2].GetComponent<Button>();
        itemBtn.onClick.RemoveAllListeners();
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
            LoadShopSaveData(storeState, currentItemObjectArray, soldItems);
            SetItemSoldPanels(storeState);

            for (int i = 0; i < currentItemObjectArray.Length; i++)
            {
                SetItemButtonUI(shopUIObjectArray[i], currentItemObjectArray[i]);
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
    private void SetItemButtonUI(GameObject obj, GameObject currentItem)
    {
        itemImage = obj.transform.GetChild(0).GetComponent<Image>();
        itemName = obj.transform.GetChild(1).GetComponent<Text>();
        itemCost = obj.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = currentItem.GetComponent<Item>().itemSprite;
        itemName.text = currentItem.GetComponent<Item>().stringName;
        itemCost.text = currentItem.GetComponent<Item>().cost.ToString();

        if(storeState == StoreState.Recipes)
        {
            recipeIngredientImage1 = obj.transform.GetChild(4).GetComponent<Image>();
            recipeIngredientImage2 = obj.transform.GetChild(5).GetComponent<Image>();
            recipeIngredientImage3 = obj.transform.GetChild(6).GetComponent<Image>();

            recipeIngredientImage1.enabled = true;
            recipeIngredientImage2.enabled = true;
            recipeIngredientImage3.enabled = true;

            recipeIngredientImageArray[0] = recipeIngredientImage1;
            recipeIngredientImageArray[1] = recipeIngredientImage2;
            recipeIngredientImageArray[2] = recipeIngredientImage3;

            currentRecipe = currentItem.GetComponent<Recipe>();
            for (int i = 0; i < currentRecipe.ingredients.Length; i++)
            {
                for (int j = 0; j < itemManager.foodArray.Length; j++)
                {
                    if (currentRecipe.ingredients[i].Equals(itemManager.foodArray[j].GetComponent<Item>().stringName))
                    {
                        recipeIngredientImageArray[i].overrideSprite = itemManager.foodArray[j].GetComponent<Item>().itemSprite;
                        break;
                    }
                }
            }

            //HARDCODED VALUES
            if(currentRecipe.ingredients.Length == 2)
            {
                recipeIngredientImage3.enabled = false;
            }

        }
        else
        {
            recipeIngredientImage1 = obj.transform.GetChild(4).GetComponent<Image>();
            recipeIngredientImage2 = obj.transform.GetChild(5).GetComponent<Image>();
            recipeIngredientImage3 = obj.transform.GetChild(6).GetComponent<Image>();

            recipeIngredientImage1.enabled = false;
            recipeIngredientImage2.enabled = false;
            recipeIngredientImage3.enabled = false;
        }
    }  

    private void ItemPurchased(GameObject item)
    {
        Debug.Log("Item Purchased Fired");
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
            SetItemSoldPanels(storeState);
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

    private void ItemSold(GameObject item, GameObject sellingUIObject, int index)
    {
        //add currency to player
        playerController.addCurrency(item.GetComponent<Item>().sellingPrice);
        //remove item from inventory 
        playerController.RemoveObjectFromInventory(index);
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
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[0], sellingUIObjectArray[0], 0); });
                }
                else if (i == 1)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[1], sellingUIObjectArray[1], 1); });
                }
                else if (i == 2)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[2], sellingUIObjectArray[2], 2); });
                }
                else if (i == 3)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[3], sellingUIObjectArray[3], 3); });
                }
                else if (i == 4)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[4], sellingUIObjectArray[4], 4); });
                }
                else if (i == 5)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[5], sellingUIObjectArray[5], 5); });
                }
                else if (i == 6)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[6], sellingUIObjectArray[6], 6); });
                }
                else if (i == 7)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[7], sellingUIObjectArray[7], 7); });
                }
                else if (i == 8)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[8], sellingUIObjectArray[8], 8); });
                }
                else if (i == 9)
                {
                    sellingUIObjectArray[i].GetComponent<Button>().onClick.AddListener(delegate { ItemSold(playerController.inventory[9], sellingUIObjectArray[9], 9); });
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
    private void SetItemSoldPanels(StoreState state)
    {
        for(int i = 0; i < currentItemObjectArray.Length; i++)
        {
            if(state == StoreState.Sell)
            {
                soldPanelArray[i].SetActive(false);
            }
            else if (soldItems.Contains(currentItemObjectArray[i]))
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
    private void LoadShopSaveData(StoreState state, GameObject[] itemArray, List<GameObject> soldItemList)
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

    private void PopulateNewItemsForShopSaveData(StoreState state, GameObject[] itemArray)
    {
        randomNumberList.Clear();
        //load a new furniture Array
        for(int i = 0; i < shopSaveData.furnitureArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.furnitureArray.Length);
            while(randomNumberList.Contains(randNum))
            {
                randNum = Random.Range(0, itemManager.furnitureArray.Length);
            }
            randomNumberList.Add(randNum);
            //previousRandNum = randNum;
            shopSaveData.furnitureArray[i] = itemManager.furnitureArray[randNum];
        }
        //load a new seeds array 
        randomNumberList.Clear();
        for (int i = 0; i < shopSaveData.seedArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.seedArray.Length);
            while (randomNumberList.Contains(randNum))
            {
                randNum = Random.Range(0, itemManager.seedArray.Length);
            }
            randomNumberList.Add(randNum);
            shopSaveData.seedArray[i] = itemManager.seedArray[randNum];
        }
        //load a new recipes array 
        randomNumberList.Clear();
        for (int i = 0; i < shopSaveData.recipeArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.recipeArray.Length);
            while (randomNumberList.Contains(randNum))
            {
                randNum = Random.Range(0, itemManager.recipeArray.Length);
            }
            randomNumberList.Add(randNum);
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
