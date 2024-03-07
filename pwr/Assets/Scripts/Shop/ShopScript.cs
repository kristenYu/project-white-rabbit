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

    //telemetry 
    public Telemetry_Util telemetryUtil; 

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

    //quest tracking UI
    public GameObject[] questTrackingUIArray;
    private TextMeshProUGUI questTrackingUIQuestName;
    private TextMeshProUGUI questTrackingUITargetNumText;
    private TextMeshProUGUI questTrackingUICurrentNumText;

    //Tutorial
    public bool tutorialBool; //set to true if a player sucessfully buys or sells something

    //sound 
    private AudioSource playerAudioSource;
    public AudioSource audioSource;
    public AudioClip changeTabClip;
    public AudioClip exitButtonClip;
    public AudioClip buyItemClip;
    public AudioClip cantBuyItemClip;
    public AudioClip sellItemClip;
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
        telemetryUtil = GameObject.FindGameObjectWithTag("telemetry").GetComponent<Telemetry_Util>();
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
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
        moneyOwed = shopSaveData.mortage;
        
       if (shopSaveData.furnitureArray[0] == null)
       {
            Debug.Log("populate new store data");
           PopulateNewItemsForShopSaveData(storeState, currentItemObjectArray);
       }
        else
        {
            Debug.Log("Load store data");
            LoadShopSaveData(storeState, currentItemObjectArray, soldItems);
        }

        FurnitureTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
       SeedTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
       RecipeTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
       SellTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);

       updateStorePanels();
       setItemButtonDelegates();
       SetItemSoldPanels(storeState);
       sellingUIPanel.SetActive(false);
       UpdateSellingItemsPanel();

       playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);

       //tutorial
       tutorialBool = false;

       for(int i = 0; i < questTrackingUIArray.Length; i++)
       {
           //HARDCODED VALUE FOR THE ORDER OF THE PREFAB - DO NOT CHANGE ORDER OF PREFAB
           questTrackingUIQuestName = questTrackingUIArray[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
           questTrackingUITargetNumText = questTrackingUIArray[i].transform.GetChild(4).GetComponent<TextMeshProUGUI>();
           questTrackingUICurrentNumText = questTrackingUIArray[i].transform.GetChild(5).GetComponent<TextMeshProUGUI>();
           if (playerController.activeQuests[i].questType == QuestBoard.QuestType.invalid)
           {
               questTrackingUIArray[i].SetActive(false);
           }
           else
           {
               questTrackingUIArray[i].SetActive(true);
               questTrackingUIQuestName.text = playerController.activeQuests[i].questName;
               switch (playerController.activeQuests[i].questType)
               {
                   case QuestBoard.QuestType.cook:
                       //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                       questTrackingUITargetNumText.text = ((CookingEventListener)playerController.activeQuests[i].eventListener).checkNumRecipes.ToString();
                       questTrackingUICurrentNumText.text = ((CookingEventListener)playerController.activeQuests[i].eventListener).currentNumRecipes.ToString();
                       break;
                   case QuestBoard.QuestType.harvest:
                       //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                       questTrackingUITargetNumText.text = ((HarvestEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                       questTrackingUICurrentNumText.text = ((HarvestEventListener)playerController.activeQuests[i].eventListener).currentHarvestedNum.ToString();
                       break;
                   case QuestBoard.QuestType.place:
                       //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                       questTrackingUITargetNumText.text = ((PlaceEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                       questTrackingUICurrentNumText.text = ((PlaceEventListener)playerController.activeQuests[i].eventListener).currentNumPlaced.ToString();
                       break;
                   case QuestBoard.QuestType.plant:
                       //HARDCODED VALUE TO THE ORDER OF THE PREFAB
                       questTrackingUITargetNumText.text = ((PlantingEventListener)playerController.activeQuests[i].eventListener).structToCheck.targetValue.ToString();
                       questTrackingUICurrentNumText.text = ((PlantingEventListener)playerController.activeQuests[i].eventListener).currentNumTargetCrops.ToString();
                       break;

               }
           }
       }



       //telemetry
       StartCoroutine(telemetryUtil.PostData("Shop:StartCoins" + playerController.currency.ToString()));
       
       

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

        itemName.text = currentItem.GetComponent<Item>().stringName;
        itemCost.text = currentItem.GetComponent<Item>().cost.ToString();

        if (storeState == StoreState.Seeds)
        {
            itemImage.sprite = currentItem.GetComponent<Seed>().shopSprite;       
        }
        else
        {
            itemImage.sprite = currentItem.GetComponent<Item>().itemSprite;
        }

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
        audioSource.PlayOneShot(buyItemClip);
        //check if player can purchase item, if so remove item from store and add to player inventory
        cost = item.GetComponent<Item>().cost;
        if (playerController.currency >= cost) 
        {
            if(!playerController.IsInventoryFull())
            {
                if (storeState == StoreState.Recipes)
                {
                    recipe = item.GetComponent<Recipe>();
                    playerController.UnlockRecipe(recipe, playerController.recipeIndex);
                    playerController.recipeIndex++;
                    playerController.removeCurrency(cost);
                    shopSaveData.soldItemList.Add(item);
                    soldItems.Add(item);
                    SetItemSoldPanels(storeState);
                    rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
                    rabbitAnimator.speechText.text = "Thanks for buying something!";

                    //tutorial
                    //tutorialBool = true;

                    //telemetry 
                    StartCoroutine(telemetryUtil.PostData("Shop:Bought" + item.GetComponent<Item>().stringName));
                }
                else if (storeState == StoreState.Seeds)
                {
                    if (playerController.GetOpenInventory() < maxSeedPurchased)
                    {
                        audioSource.PlayOneShot(cantBuyItemClip);
                        rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
                        rabbitAnimator.speechText.text = "Sorry, you don't have any room in your inventory.";
                        //telemetry
                        StartCoroutine(telemetryUtil.PostData("Shop:TriedBought" + item.GetComponent<Item>().stringName));
                    }
                    else
                    {
                        for (int i = 0; i < maxSeedPurchased; i++)
                        {
                            playerController.AddObjectToInventory(item);
                        }
                        playerController.removeCurrency(cost);
                        shopSaveData.soldItemList.Add(item);
                        soldItems.Add(item);
                        SetItemSoldPanels(storeState);
                        rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
                        rabbitAnimator.speechText.text = "Thanks for buying something!";

                        //tutorial
                        //tutorialBool = true;

                        //telemetry 
                        StartCoroutine(telemetryUtil.PostData("Shop:Bought" + item.GetComponent<Item>().stringName));
                    }
                }
                else
                {
                    playerController.removeCurrency(cost);
                    playerController.AddObjectToInventory(item);
                    shopSaveData.soldItemList.Add(item);
                    soldItems.Add(item);
                    SetItemSoldPanels(storeState);
                    rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
                    rabbitAnimator.speechText.text = "Thanks for buying something!";

                    //tutorial
                    //tutorialBool = true;

                    //telemetry 
                    StartCoroutine(telemetryUtil.PostData("Shop:Bought" + item.GetComponent<Item>().stringName));
                }
               
            }
            else
            {
                audioSource.PlayOneShot(cantBuyItemClip);
                rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
                rabbitAnimator.speechText.text = "Sorry, you don't have any room in your inventory.";
                //telemetry
                StartCoroutine(telemetryUtil.PostData("Shop:TriedBought" + item.GetComponent<Item>().stringName));
            }
            
        }
        else
        {
            audioSource.PlayOneShot(cantBuyItemClip);
            //message saying no money :(, make into a pop up in game later?
            rabbitAnimator.setAnimation(Rabbit_Animator.AnimState.talk);
            rabbitAnimator.speechText.text = "Sorry you don't have enough money";
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Shop:TriedBought" + item.GetComponent<Item>().stringName));
        }
    }

    private void ItemSold(GameObject item, GameObject sellingUIObject, int index)
    {
        audioSource.PlayOneShot(sellItemClip);
        //add currency to player
        if(item != null)
        {
            playerController.addCurrency(item.GetComponent<Item>().sellingPrice);
            //remove item from inventory 
            playerController.RemoveObjectFromInventory(index);
            for(int i = 0; i < playerController.transform.childCount; i++)
            {
                if(playerController.transform.GetChild(i).gameObject == item)
                {
                    Destroy(playerController.transform.GetChild(i).gameObject);
                }
            }   
            //hide button 
            sellingUIObject.SetActive(false);

            //tutorial
            tutorialBool = true;

            //telemetry
            StartCoroutine(telemetryUtil.PostData("Shop:Sold" + item.GetComponent<Item>().stringName));
        }
        
       
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
            if(amountToPay <= playerController.currency && amountToPay > 0)
            {
                audioSource.PlayOneShot(sellItemClip);
                if(amountToPay < moneyOwed)
                {
                    moneyOwed -= amountToPay;
                    playerController.currency -= amountToPay;
                    moneyOwedText.text = moneyOwed.ToString();
                }
                else
                {
                    moneyOwed = 0;
                    playerController.currency -= amountToPay;
                    moneyOwedText.text = moneyOwed.ToString();
                    shopSaveData.mortage = moneyOwed;
                    SceneManager.LoadScene("Win", LoadSceneMode.Single);
                }
                //telemetry
                StartCoroutine(telemetryUtil.PostData("Shop:Pay" + amountToPay.ToString()));
            }
            StartCoroutine(telemetryUtil.PostData("Shop:TriedPay" + amountToPay.ToString()));
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
        audioSource.PlayOneShot(exitButtonClip);
        playerControllerObject.transform.position = new Vector3(8.5f, -2f, 0f);
        playerController.HUD.SetActive(true);
        playerController.enabled = true;
        playerController.isShouldMove = true;
        shopSaveData.mortage = moneyOwed;
        if(SceneManager.GetActiveScene().name == "TutorialShop")
        {
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Transition:Tutorial"));
        }
        else if (SceneManager.GetActiveScene().name == "Shop")
        {
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Transition:Main"));
        }
        else if (SceneManager.GetActiveScene().name == "test")
        {
            SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Transition:Tutorial"));
        }

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
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Shop:AvailableFurniture" + shopSaveData.furnitureArray[i].GetComponent<Furniture>().stringName));
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
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Shop:AvailableSeed" + shopSaveData.seedArray[i].GetComponent<Seed>().stringName));
        }
        //load a new recipes array 
        randomNumberList.Clear();
        for (int i = 0; i < shopSaveData.recipeArray.Length; i++)
        {
            randNum = Random.Range(0, itemManager.recipeArray.Length);
            if(SceneManager.GetActiveScene().name == "TutorialShop")
            {
                while (randomNumberList.Contains(randNum))
                {
                    randNum = Random.Range(0, itemManager.recipeArray.Length);
                }
            }
            else
            {
                while (randomNumberList.Contains(randNum) || playerController.CheckRecipeUnlocked(itemManager.recipeArray[randNum].GetComponent<Recipe>()))
                {
                    randNum = Random.Range(0, itemManager.recipeArray.Length);
                }
            }
            randomNumberList.Add(randNum);
            shopSaveData.recipeArray[i] = itemManager.recipeArray[randNum];
            //telemetry
            StartCoroutine(telemetryUtil.PostData("Shop:AvailableRecipe" + shopSaveData.recipeArray[i].GetComponent<Recipe>().stringName));
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
        audioSource.PlayOneShot(changeTabClip);
        updateStorePanels();
        UpdateSellingItemsPanel();

        switch(state)
        {
            case StoreState.Furniture:
                //HARDCODED TO EXPECT AN IMAGE CHILD AT CHILD 0
                FurnitureTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                SeedTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                RecipeTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SellTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case StoreState.Seeds:
                //HARDCODED TO EXPECT AN IMAGE CHILD AT CHILD 0
                FurnitureTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SeedTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                RecipeTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SellTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case StoreState.Recipes:
                //HARDCODED TO EXPECT AN IMAGE CHILD AT CHILD 0
                FurnitureTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SeedTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                RecipeTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                SellTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                break;
            case StoreState.Sell:
                //HARDCODED TO EXPECT AN IMAGE CHILD AT CHILD 0
                FurnitureTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SeedTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                RecipeTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SellTabButton.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                break;
        }

    }
    private bool checkIfShopShouldUpdate()
    {
        return worldController.isNewDay; 
    }

}
