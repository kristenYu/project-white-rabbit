using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
//using System;

public class PlayerController : MonoBehaviour
{
    //Access World Database 
    public GameObject itemManagerObject;
    private ItemManager itemManager;
    public GameObject worldControllerObject;
    private WorldController worldController; 

    //planting seeds
    private GameObject cropObject;
    private Crop cropScript; 
    private Seed seedScript;

    //Recipes 
    public List<Recipe> knownRecipes;
    private GameObject currentRecipeUIObject; 
    private bool isKnownRecipe;
    private Recipe currentRecipe;

    //Recipe and Cooking UI
    public GameObject cookingUI;
    public GameObject cookingUIScrollBar;
    public GameObject recipeUIPrefab;
    private List<GameObject> knownRecipeUIObjects;
    private const float knownRecipeUIOffset = 60.0f;
    private const float cookingUIContentMaximum = 260.0f; 
    private GameObject cookingUIContent;
    private Image recipeUIImage;
    private Button recipeUIButton; 
    private TextMeshProUGUI recipeUIText;
    public Image[] recipeUIIngredientImageArray;
    public Item recipeIngredientItem;

    //Cooking
    private List<GameObject> targetIngredients;
    private GameObject currentIngredient;
    private bool isIngredientInInventory;
    public GameObject cookedFoodObject;
    public bool CookedRecipeFlag; 
    public int recipeIndex; 
    

    //inventory 
    public GameObject[] inventory;
    public Image[] inventoryItemImageArray;
    public Image[] inventoryHudImageArray; 
    public GameObject activeItem;
    public Sprite activeItemSprite;
    public Sprite inventorySprite;
    public int currentInventoryIndex;
    public int previousInventoryIndex; 
    private const int inventorySize = 10;
    private GameObject tempObject;
    private Item currentItem;

    //Quests 
    public const int maxActiveQuests = 3; 
    public Quest[] activeQuests;
    public int currentQuestIndex;
    private bool isActiveQuestsAtMaximum;
    public GameObject[] questHudObjectArray;
    public int questHudCurrentIndex;
    private int activeQuestNum;
  //  private QuestAlgorithmBase currentQuestAlgorithm;
  //  public GameObject[] questAlgorithms; //Should be set to the number of algorithms 

    //Currency
    public int currency;
    public TextMeshProUGUI currencyText;

    //place furniture 
    public GameObject furnitureObject;
    private Furniture furnitureScript;
    private SpriteRenderer furnitureSpriteRenderer;
    public bool placeFurnitureFlag;

    //harvestables
    private Harvestable harvestableScript;
    public bool hasHarvestedItem;
    public string justHarvestedName; 

    //Movement
    public bool isShouldMove;
    private Rigidbody2D body;
    private float horizontal;
    private float vertical;
    //this is needed for keyboard movement, not needed for controller
    private float moveLimiter = 0.7f;
    public float runSpeed = 7.0f;
    private Vector2 previousDirection;
    RaycastHit2D hit;

    //Animator
    Animator anim; 

    //juice
    public GameObject interactPopup;
    public Sprite interactPlantSprite;
    public Sprite interactBasicSprite;
    public Sprite interactCookSprite; 
    private int layerMask;
    public GameObject HUD;

    //sound effects
    public AudioSource playerAudioSource;
    public AudioSource cameraAudioSource; 
    public AudioClip walkingClip;
    public AudioClip plantingClip;
    public AudioClip bushRustleclip;
    public AudioClip mushroomHarvestClip;
    public AudioClip inventoryRustleClip;
    public AudioClip placeFurnitureClip;
    public AudioClip rotateFurnitureClip;
    public AudioClip canCookClip;
    public AudioClip cannotCookClip;
    public AudioClip harvestPlantClip;


    //Debug
    private GameObject testObject; 

    //Singleton 
    private static PlayerController instance;
    // Read-only public access
    public static PlayerController Instance => instance;

    //player profile for Passage 
    public int[] actionFrequencyArray;
    public int questAlgorithm;

    //tutorial
    public bool hasPlacedFurniture;
    public bool hasShopped;
    public bool hasDebugCooking;

    //Telemetry 
    public Telemetry_Util telemetryUtil;
    string UUID;
    System.DateTime dt = System.DateTime.Now;
    //session variable from php 
    private string sessionQuestAlgorithm; 

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

        //telemetry
      //  telemetryUtil = GameObject.FindGameObjectWithTag("telemetry").GetComponent<Telemetry_Util>(); 
        StartCoroutine(telemetryUtil.PostData("Event:SessionStart"));
        StartCoroutine(selectQuestAlgorithm());
        previousInventoryIndex = 0;
        currentInventoryIndex = 0;

        hasDebugCooking = false;

    }

    void Start()
    {
       // telemetryUtil = GameObject.FindGameObjectWithTag("telemetry").GetComponent<Telemetry_Util>();
        currency = 500;
        inventory = new GameObject[inventorySize];

        recipeIndex = 0;

        body = GetComponent<Rigidbody2D>();
        previousDirection = Vector2.down;

        cookingUIContent = cookingUI.transform.GetChild(0).GetChild(0).gameObject;
        cookingUIContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
        knownRecipeUIObjects = new List<GameObject>();
        targetIngredients = new List<GameObject>();
        CookedRecipeFlag = false;
        hasHarvestedItem = false; 


        itemManager = itemManagerObject.GetComponent<ItemManager>();
        worldController = worldControllerObject.GetComponent<WorldController>();
        placeFurnitureFlag = false;

        currentQuestIndex = 0;
        questHudCurrentIndex = 0;
        activeQuests = new Quest[maxActiveQuests];
        for(int i = 0; i < questHudObjectArray.Length; i++)
        {
            questHudObjectArray[i].SetActive(false);
        }
      
        anim = GetComponent<Animator>();

        foreach(Image itemImage in inventoryItemImageArray)
        {
            itemImage.gameObject.SetActive(false);
        }

        layerMask = 1 << 2;
        layerMask = ~layerMask;

        actionFrequencyArray = new int[(int)QuestBoard.QuestType.invalid];
        isShouldMove = true;

        //clear inventory array 
        for(int i = 0; i < inventorySize; i++)
        {
            deactivateItem(i);
        }
        setActiveItem(previousInventoryIndex);

        hasPlacedFurniture = false;

        //sound 
        cameraAudioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        playerAudioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        ShowActiveItem();

        if (isShouldMove)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }
        else
        {
            playerAudioSource.Stop();
        }
        //player animation
        if (horizontal != 0 && vertical != 0)
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", 0);
        }
        else
        {
            anim.SetFloat("Horizontal", horizontal);
            anim.SetFloat("Vertical", vertical);
        }

        if (horizontal == 0 && vertical == 0)
        {
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", true);
        }

        if (activeItem != null)
        {
            
            anim.SetBool("IsHolding", true);
        }
        else
        {
            anim.SetBool("IsHolding", false);
        }

        //Interaction raycasts
        if (horizontal < 0)
        {
            if(!playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(walkingClip);
            }
            
            hit = drawRay(Vector2.left, false);
            previousDirection = Vector2.left;
        }
        else if (horizontal > 0)
        {
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(walkingClip);
            }
            hit = drawRay(Vector2.right, false);
            previousDirection = Vector2.right;
        }
        else if (vertical < 0)
        {
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(walkingClip);
            }
            hit = drawRay(Vector2.down, false);
            previousDirection = Vector2.down;
        }
        else if (vertical > 0)
        {
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.PlayOneShot(walkingClip);
            }
            hit = drawRay(Vector2.up, false);
            previousDirection = Vector2.up;
        }
        else
        {
            playerAudioSource.Stop();
            hit = drawRay(previousDirection, false);
        }

        //Rotate furniture
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (activeItem != null)
            {
                if (activeItem.tag == "furniture")
                {
                    cameraAudioSource.PlayOneShot(rotateFurnitureClip);
                    //TODO: This could be filled with bugs! It gets the furniture object from the last instantiated object
                    furnitureObject = activeItem;
                    furnitureSpriteRenderer = furnitureObject.GetComponent<SpriteRenderer>();
                    furnitureScript = furnitureObject.GetComponent<Furniture>();
                    if (furnitureScript.currentIndex < furnitureScript.maxIndex)
                    {
                        furnitureScript.currentIndex += 1;
                    }
                    else
                    {
                        furnitureScript.currentIndex = 0;
                    }
                    furnitureSpriteRenderer.sprite = furnitureScript.spriteArray[furnitureScript.currentIndex];
                    StartCoroutine(telemetryUtil.PostData("Interaction:RotateFurniture" + furnitureScript.stringName));
                }
            }
        }
       
        //Option to Sprint 
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            runSpeed = 9.0f;
        }
        else
        {
            runSpeed = 7.0f;
        }
        
        //inventory QOL Update
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(5);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(6);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(7);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            cameraAudioSource.PlayOneShot(inventoryRustleClip);
            setActiveItem(9);
        }

        //interact with objects
        if (hit)
        {
            if(hit.transform.tag != "non_interact")
            {
                interactPopup.SetActive(true);
                interactPopup.GetComponent<SpriteRenderer>().sprite = interactBasicSprite;
            }
            if(hit.transform.tag == "cooking")
            {
                cookingUI.SetActive(true);
                interactPopup.GetComponent<SpriteRenderer>().sprite = interactCookSprite;
            }
            else if (hit.transform.tag == "planting")
            {
                if (activeItem != null && activeItem.tag == "seed")
                {
                    interactPopup.SetActive(true);
                    interactPopup.GetComponent<SpriteRenderer>().sprite = interactPlantSprite;
                }
                else
                {
                    interactPopup.SetActive(false);
                }
            }
            else if(hit.transform.tag == "crop")
            {
                if(hit.transform.gameObject.GetComponent<Crop>().currentStage == Crop.CropStage.FullyGrown)
                {
                    interactPopup.SetActive(true);
                }
                else
                {
                    interactPopup.SetActive(false);
                }    
            }
            else if(hit.transform.tag == "placeable")
            {
                if(activeItem != null)
                {
                    if (activeItem.tag == "furniture")
                    {
                        interactPopup.SetActive(true);
                        interactPopup.GetComponent<SpriteRenderer>().sprite = interactBasicSprite;
                    }
                    else
                    {
                        interactPopup.SetActive(false);
                    }
                }
                else
                {
                    interactPopup.SetActive(false);
                }
               
            }


            if (Input.GetKeyDown(KeyCode.E))
            {
                //transitions scenes 
                if (hit.transform.gameObject.tag == "new_scene")
                {
                    if(hit.transform.gameObject.name == "Home" || hit.transform.gameObject.name == "TutorialHome")
                    {
                        playerAudioSource.Stop();
                        //assumes that the object matches the name of the scene you want to load
                        this.transform.position = new Vector3(-4.5f, -6.5f, 0f); //HARDCODED VALUE TO THE OPENING LOCATION
                        SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                        StartCoroutine(telemetryUtil.PostData("Transition:Home"));
                    }
                    if(hit.transform.gameObject.name == "Main")
                    {
                        
                        SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                        this.transform.position = new Vector3(-7.5f, 2.5f, 0f);
                        StartCoroutine(telemetryUtil.PostData("Transition:Main"));
                    }
                    if(hit.transform.gameObject.name == "Tutorial")
                    {
                        SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                        this.transform.position = new Vector3(123.5f, -4.25f, 0f); //HARDCODED VALUE FOR TUTORIAL LOCATION
                        StartCoroutine(telemetryUtil.PostData("Transition:Tutorial"));
                    }
                    //Hide hud
                    if (hit.transform.gameObject.name == "Shop" 
                        || hit.transform.gameObject.name == "QuestBoard" 
                        || hit.transform.gameObject.name == "TutorialShop" 
                        || hit.transform.gameObject.name =="TutorialQuestBoard") 
                    {
                        body.velocity = new Vector2(0.0f, 0.0f);
                        HUD.SetActive(false);
                        playerAudioSource.Stop();
                        SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                        StartCoroutine(telemetryUtil.PostData("Transition:" + hit.transform.gameObject.name));
                    }


                }
                else if (hit.transform.gameObject.tag == "debug_seed")
                {
                    testObject = itemManager.seedArray[Random.Range(0, itemManager.seedArray.Length - 1)];
                    testObject.GetComponent<SpriteRenderer>().enabled = true;
                    AddObjectToInventory(testObject);
                }
                //plants a seed if the active item is a seed
                else if (hit.transform.gameObject.tag == "planting")
                {
                    if (activeItem != null)
                    {
                        if (activeItem.tag == "seed")
                        {
                            cameraAudioSource.volume = 0.5f;
                            cameraAudioSource.PlayOneShot(plantingClip);
                            cameraAudioSource.volume = 0.2f;
                            seedScript = activeItem.GetComponent<Seed>();
                            cropObject = Instantiate(seedScript.crop, hit.transform.position, Quaternion.identity);
                            cropObject.GetComponent<SpriteRenderer>().enabled = true;
                            //add crop object to world controller 
                            cropScript = cropObject.GetComponent<Crop>();
                            cropScript.worldController = this.worldControllerObject.GetComponent<WorldController>();
                            worldController.activeCropList.Add(cropObject);
                            //remove activeitem from inventory 
                            RemoveObjectFromInventory(currentInventoryIndex);
                            activeItem = null;
                            //for Passage
                            actionFrequencyArray[(int)QuestBoard.QuestType.plant] += 1;
                            StartCoroutine(telemetryUtil.PostData("Interaction:Plant" + cropScript.cropname));

                        }
                    }
                }
                //Harvest food from a crop that is fully grown 
                else if(hit.transform.gameObject.tag == "crop")
                {
                    //check if the crop can be harvested and harvests if it can 
                    cropScript = hit.transform.gameObject.GetComponent<Crop>();
                    if(cropScript.currentStage == Crop.CropStage.FullyGrown)
                    {
                        cameraAudioSource.PlayOneShot(harvestPlantClip);
                        AddObjectToInventory(cropScript.HarvestCrop());
                        StartCoroutine(telemetryUtil.PostData("Interaction:HarvestCrop" + cropScript.cropname));
                    }
                    //actionFrequencyArray[(int)QuestBoard.QuestType.harvest] += 1;

                }
                else if(hit.transform.gameObject.tag == "harvestable")
                {
                    harvestableScript = hit.transform.gameObject.GetComponent<Harvestable>();
                    AddObjectToInventory(harvestableScript.harvestedItem);
                    worldController.harvestableList.Remove(hit.transform.gameObject);
                    hasHarvestedItem = true; //flag is turned off in harvest event listner
                    justHarvestedName = hit.transform.gameObject.GetComponent<Harvestable>().stringName;
                    Object.Destroy(hit.transform.gameObject);
                    actionFrequencyArray[(int)QuestBoard.QuestType.harvest] += 1;
                    if(harvestableScript.stringName == "mushroom")
                    {
                        cameraAudioSource.PlayOneShot(mushroomHarvestClip);
                        StartCoroutine(telemetryUtil.PostData("Interaction:HarvestMushroom"));
                    }
                    else
                    {
                        cameraAudioSource.PlayOneShot(bushRustleclip);
                        StartCoroutine(telemetryUtil.PostData("Interaction:HarvestBerry"));
                    }
                }
                else if (hit.transform.gameObject.tag == "debug_furniture")
                {
                    //Does not get added to inventory for now - will figure out flow later
                    testObject = itemManager.furnitureArray[Random.Range(0, itemManager.furnitureArray.Length - 1)];
                    CreateFurnitureObjectAndAddToInventory(testObject);
                }
                //places furniture if active item is furniture
                else if (hit.transform.gameObject.tag == "placeable")
                {
                    if(activeItem != null)
                    {
                        if (activeItem.tag == "furniture")
                        {
                            StartCoroutine(telemetryUtil.PostData("Interaction:PlaceFurniture" + activeItem.gameObject.GetComponent<Furniture>().stringName));
                            cameraAudioSource.PlayOneShot(placeFurnitureClip);
                            RemoveObjectFromInventory(activeItem);
                            PlaceFurniture(hit);
                            actionFrequencyArray[(int)QuestBoard.QuestType.place] += 1;
                            placeFurnitureFlag = true;
                            hasPlacedFurniture = true;
                            
                        }
                    }
                }
                //picks up furniture
                else if(hit.transform.gameObject.tag == "furniture")
                {
                    StartCoroutine(telemetryUtil.PostData("Interaction:PickupFurniture" + hit.transform.gameObject.GetComponent<Furniture>().stringName));
                    AddObjectToInventory(hit.transform.gameObject);
                    PickUpFurniture(hit);
                }
                else if(hit.transform.gameObject.tag == "debug_unlock_recipes")
                {
                    hasDebugCooking = true;
                    Debug_UnlockAllRecipes();
                }
            }
           
        }
        else
        {
            interactPopup.SetActive(false);
            cookingUI.SetActive(false);
        }

        //update player currency to view in HUD
        currencyText.text = currency.ToString();

        //check for shop enabled and disable player movement
        if (SceneManager.GetActiveScene().name == "Shop")
        {
            this.GetComponent<PlayerController>().enabled = false;
            //GameObject.Find("WorldController").GetComponent<WorldController>().enabled = false;
        }
        
    }
	
    private void FixedUpdate()
    {
        
        if(isShouldMove)
        {
            //Movemement 
            if (horizontal != 0 && vertical != 0) // Check for diagonal movement
            {
                // limit movement speed diagonally, so you move at 70% speed
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }
            body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        }
        else
        {
            body.velocity = new Vector2(0.0f, 0.0f);
        }
    }

    public void setActiveItem(int index)
    {
        previousInventoryIndex = currentInventoryIndex;
        currentInventoryIndex = index;
        activeItem = inventory[index];
        inventoryHudImageArray[previousInventoryIndex].GetComponent<Image>().sprite = inventorySprite;
        inventoryHudImageArray[currentInventoryIndex].GetComponent<Image>().sprite = activeItemSprite;
    }

    public void deactivateItem(int index)
    {
        inventoryHudImageArray[index].GetComponent<Image>().sprite = inventorySprite;
        activeItem = null;
    }

    public void SetNextOpenInventory()
    {
        for(int i = 0; i < inventorySize; i++)
        {
            if(inventory[i] ==  null)
            {
                currentInventoryIndex = i;
                break;
            }
        }
    }    
	
    public bool AddObjectToInventory(GameObject item)
    {
        SetNextOpenInventory();
        if (item == null)
        {
            Debug.LogWarning("Attempted to add a null item");
            return false;
        }
        if(currentInventoryIndex < inventorySize)
        {
            if(inventory[currentInventoryIndex] == null)
            {
                currentItem = item.GetComponent<Item>();
                if (currentItem == null)
                {
                    Debug.LogWarning(item.name + " is not an item class; Cannot add to inventory");
                    return false; 
                }
                else
                {
                    if (item.gameObject.tag == "furniture")
                    {
                        tempObject = Instantiate(item, this.transform.position, Quaternion.identity);
                        tempObject.gameObject.transform.parent = this.transform;
                        tempObject.SetActive(false);

                        inventory[currentInventoryIndex] = tempObject;
                    }
                    else
                    {
                        inventory[currentInventoryIndex] = item;
                    }
                    inventoryItemImageArray[currentInventoryIndex].sprite = currentItem.itemSprite;
                    inventoryItemImageArray[currentInventoryIndex].gameObject.SetActive(true);
                    activeItem = inventory[currentInventoryIndex];
                   
                    
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("Inventory at CurrentInventoryIndex is not null");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("CurrentInventoryIndex is greater than inventory Size");
            return false;
        }
    }
	
    public bool AddObjectToInventory(GameObject item, int indexAt)
    {
        if (indexAt > inventorySize)
        {
            return false;
        }
        else if(inventory[indexAt] != null)
        {
            return false; 
        }
        else
        {
            currentItem = item.GetComponent<Item>();
            if (currentItem == null)
            {
                Debug.LogWarning(item.name + " is not an item class; Cannot add to inventory");
                return false;
            }
            else
            {
                inventory[indexAt] = item;
                inventoryItemImageArray[currentInventoryIndex].sprite = currentItem.itemSprite;
                inventoryItemImageArray[currentInventoryIndex].gameObject.SetActive(true);
                currentInventoryIndex = indexAt;
                activeItem = item;
                return true;
            }
        }
    }
	
    //only removes the first instance of the object in the list
    public GameObject RemoveObjectFromInventory(GameObject item)
    {
        tempObject = null;
        int savedIndex;
        for(int i = 0; i < inventorySize; i++)
        {
            if (inventory[i] == item)
            {
                tempObject = inventory[i];
                savedIndex = i;
                inventory[i] = null;
                currentInventoryIndex = i;
                break;
            }
        }
        inventoryItemImageArray[currentInventoryIndex].gameObject.SetActive(false);
        return tempObject;
    }

    public GameObject RemoveObjectFromInventory(int indexAt)
    {
        tempObject = inventory[indexAt];
        inventory[indexAt] = null;
        inventoryItemImageArray[indexAt].gameObject.SetActive(false);
        currentInventoryIndex = indexAt; 
        return tempObject;
    }

    private void ShowActiveItem()
    {
        if(activeItem != null)
        {
            if (activeItem.gameObject.tag == "furniture")
            {
                if (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "Tutorial" || SceneManager.GetActiveScene().name == "TutorialHome")
                {
                    activeItem.SetActive(true);
                    activeItem.transform.localPosition = Vector3.zero + new Vector3(previousDirection.x, previousDirection.y, 0);
                }
            }  
        }
        if (inventory[previousInventoryIndex] != null)
        {
            if(previousInventoryIndex != currentInventoryIndex)
            {
                if (inventory[previousInventoryIndex].tag == "furniture")
                {
                    inventory[previousInventoryIndex].SetActive(false);
                }
            }
           
        }
    }
    private void CreateFurnitureObjectAndAddToInventory(GameObject furniture)
    {
        furnitureObject = Instantiate(furniture, this.transform.position, Quaternion.identity);
        furnitureObject.transform.SetParent(this.transform);
        furnitureObject.transform.localPosition = new Vector3(previousDirection.x, previousDirection.y, 0);
        furnitureObject.transform.gameObject.layer = 2; //Ignore Raycast Layer
        AddObjectToInventory(furnitureObject);
    }

    private void CreateFurnitureObject(GameObject furniture)
    {
        furnitureObject = Instantiate(furniture, this.transform.position, Quaternion.identity);
        furnitureObject.transform.SetParent(this.transform);
        furnitureObject.transform.localPosition = new Vector3(previousDirection.x, previousDirection.y, 0);
        furnitureObject.transform.gameObject.layer = 2; //Ignore Raycast Layer
    }

    private void DestroyFurnitureObject(GameObject furniture)
    {
        Destroy(furniture);
    }

    private void PlaceFurniture(RaycastHit2D hit2D)
    {
        furnitureObject = activeItem;
        furnitureObject.transform.parent = null;
        furnitureObject.transform.position = hit2D.transform.position;
        furnitureObject.transform.gameObject.layer = 1; //default layer
        worldController.placedFurnitureObjects.Add(furnitureObject);
        //have to do clean up with active item
        activeItem = null;
    }
    private void PickUpFurniture(RaycastHit2D hit2D)
    {
        furnitureObject = hit2D.transform.gameObject;
        foreach(GameObject furniture in worldController.placedFurnitureObjects)
        {
            if(furniture == furnitureObject)
            {
                worldController.placedFurnitureObjects.Remove(furnitureObject);
                break;
            }
        }
        Destroy(furnitureObject);
        //Destroy(hit2D.transform.gameObject);
        //furnitureObject.transform.SetParent(this.transform);
        //furnitureObject.transform.localPosition = new Vector3(previousDirection.x, previousDirection.y, 0);
        //furnitureObject.transform.gameObject.layer = 2; //Ignore Raycast Layer
    }
    private RaycastHit2D drawRay(Vector2 direction, bool debug)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.0f, layerMask);
        if(debug)
        {
            Debug.DrawRay(transform.position, direction, Color.white, 1.0f);
        }
        return hit; 
    }
	
   

	 //functions to add and remove currency...is this...right??? ehhHHhHHH
    public void addCurrency(int addAmount)
    {
        currency = currency + addAmount;
    }

    public bool removeCurrency(int removeAmount)
    {
        if (currency < removeAmount)
        {
            return false;
        }
        else
        {
            currency = currency - removeAmount;
            return true;
        }
	}
    //Handle Recipe Unlocks
    public bool CheckRecipeUnlocked(Recipe recipe)
    {
        isKnownRecipe = false; 
        foreach(Recipe r in knownRecipes)
        {
            if(r.equals(recipe))
            {
                isKnownRecipe = true;
                break; 
            }
        }
        return isKnownRecipe;
    }
    public void UnlockRecipe(Recipe recipe, int index)
    {
        knownRecipes.Add(recipe);
        currentRecipeUIObject = Instantiate(recipeUIPrefab, this.transform.position, Quaternion.identity);
        currentRecipeUIObject.transform.position = new Vector3(0.0f, (cookingUIContentMaximum - (knownRecipeUIOffset*(index+1))), 90.0f);
        currentRecipeUIObject.transform.SetParent(cookingUIContent.transform, false);
        SetRecipeUI(currentRecipeUIObject, recipe);
        knownRecipeUIObjects.Add(currentRecipeUIObject);
    }
    private void Debug_UnlockAllRecipes()
    {
        Debug.Log("Unlock all recipes");
        for(int i = 0; i < itemManager.recipeArray.Length; i++)
        {
            currentRecipe = itemManager.recipeArray[i].GetComponent<Recipe>();
            if (!CheckRecipeUnlocked(currentRecipe))
            {
                UnlockRecipe(currentRecipe, i);
            }
        }
    }

    private void SetRecipeUI(GameObject recipeUIObject, Recipe recipe)
    {
        //this matches the order in the prefab, but if the prefab changes this code will break
        //recipeUIBackground = recipeUIObject.transform.gameObject.GetComponent<Image>();
        recipeUIButton = recipeUIObject.GetComponent<Button>();
        recipeUIText = recipeUIObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        recipeUIImage = recipeUIObject.transform.GetChild(1).gameObject.GetComponent<Image>();
        recipeUIIngredientImageArray = recipeUIObject.transform.GetChild(2).GetComponentsInChildren<Image>();
        
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            Debug.Log(recipe.ingredients[i]);
            for (int j = 0; j < itemManager.foodArray.Length; j++)
            {
                Debug.Log(itemManager.foodArray[j].GetComponent<Item>().stringName);
                if (recipe.ingredients[i].Equals(itemManager.foodArray[j].GetComponent<Item>().stringName))
                {
                    Debug.Log(recipe.ingredients[i]);
                    Debug.Log(itemManager.foodArray[j]);
                    recipeIngredientItem = itemManager.foodArray[j].GetComponent<Item>();
                    break;
                }
            }
            Debug.Log(recipeIngredientItem);
            recipeUIIngredientImageArray[i].overrideSprite = recipeIngredientItem.itemSprite;
        }

        recipeUIImage.overrideSprite = recipe.itemSprite;
        recipeUIText.text = recipe.stringName;
        recipeUIButton.onClick.AddListener(delegate { CookRecipe(recipe); });


        recipeUIObject.GetComponent<RecipeButton_UI>().recipe = recipe; 
        //recipeUIObject.GetComponent<RecipeUI>().recipe = recipe;
    }

    private bool CookRecipe(Recipe recipe)
    {
        //Debug.Log("On click registered");
        targetIngredients.Clear();
        //check if the player has everything in the ingredients 
        foreach(string ingredient in recipe.ingredients)
        {
            isIngredientInInventory = false; 
            for(int i = 0; i < inventorySize; i++)
            {
                if (inventory[i] == null)
                {
                    continue;
                }
                if (inventory[i].GetComponent<Item>().stringName == ingredient)
                {
                    targetIngredients.Add(inventory[i]);
                    currentIngredient = RemoveObjectFromInventory(i);
                    isIngredientInInventory = true;
                    break;
                }
            }

            if(isIngredientInInventory == false)
            {
                cameraAudioSource.PlayOneShot(cannotCookClip);
                Debug.Log("Recipe Cannot be made because player does not have the correct ingredients");
                //return the items to the inventory 
                foreach(GameObject item in targetIngredients)
                {
                    AddObjectToInventory(item);
                }

                return false; 
            }
        }
        cameraAudioSource.PlayOneShot(canCookClip);
        cookedFoodObject = Instantiate(recipe.cookedFood);
        AddObjectToInventory(cookedFoodObject);
        CookedRecipeFlag = true;
        currentInventoryIndex = 0;
        StartCoroutine(telemetryUtil.PostData("Interaction:Cook" + recipe.name));
        return true;
    }    

    public bool CheckIfRecipeCanBeCooked(Recipe recipe)
    {
        //TODO: copied code - worry about later
        targetIngredients.Clear();
        foreach (string ingredient in recipe.ingredients)
        {
            isIngredientInInventory = false;
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventory[i] == null)
                {
                    continue;
                }
                if (inventory[i].GetComponent<Item>().stringName == ingredient)
                {
                    targetIngredients.Add(inventory[i]);
                    currentIngredient = RemoveObjectFromInventory(i);
                    isIngredientInInventory = true;
                    break;
                }
            }

            if (isIngredientInInventory == false)
            {
                Debug.Log("Recipe Cannot be made because player does not have the correct ingredients");
                //return the items to the inventory 
                foreach (GameObject item in targetIngredients)
                {
                    AddObjectToInventory(item);
                }

                return false;
            }
        }
        return true;
    }

    private void GetNextOpenQuestIndex()
    {
        isActiveQuestsAtMaximum = true; 
        for(int i = 0; i < maxActiveQuests; i++)
        {
            if(activeQuests[i] == null)
            {
                currentQuestIndex = i;
                isActiveQuestsAtMaximum = false;
                Debug.Log("PlayerController.CurrentQuestIndex: " + i);
                break;
            }
            else if (activeQuests[i].questType == QuestBoard.QuestType.invalid) 
            {
                currentQuestIndex = i;
                isActiveQuestsAtMaximum = false;
                break; 
            }
        }
    }

    public void AddQuestToActiveArray(Quest quest)
    {
        GetNextOpenQuestIndex(); 
        if(isActiveQuestsAtMaximum)
        {
            Debug.Log("Quests are maximum; cannot accept another quest");
        }
        else
        {
            activeQuests[currentQuestIndex] = quest; 
        }    

    }

    public int CountNumberOfActiveQuests()
    {
        activeQuestNum = 0; 
        for(int i = 0; i < activeQuests.Length; i++)
        {
            if(activeQuests[i] != null && activeQuests[i].questType != QuestBoard.QuestType.invalid)
            {
                activeQuestNum++;
            }
        }
        return activeQuestNum;
    }

    public void RemoveQuestFromActiveQuestsArray(Quest quest)
    {
        for(int i = 0; i < activeQuests.Length; i++)
        {
            if(activeQuests[i].questName == quest.questName)
            {
                activeQuests[i] = null;
                break;
            }
        }
    }


    IEnumerator selectQuestAlgorithm()
    {
        Debug.Log("Starting Get request");

        //hack to force validate certificate 
        var cert = new CertificateValidator();

        //UnityWebRequest www = UnityWebRequest.Get("https://inc0293516.cs.ualberta.ca/cgi-bin/createUUID.cgi");
        //UnityWebRequest www = UnityWebRequest.Get("https://inc0293516.cs.ualberta.ca/get_unique_id.php");
        //UnityWebRequest www = UnityWebRequest.Get("https://inc0293516.cs.ualberta.ca");
        UnityWebRequest www = UnityWebRequest.Get("https://inc0293516.cs.ualberta.ca/get_aid.php");

        www.certificateHandler = cert;
        yield return www.SendWebRequest();

        Debug.Log("Get Request recieved");
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {

            Debug.Log("Version 4");
            sessionQuestAlgorithm = www.downloadHandler.text;
            Debug.Log(sessionQuestAlgorithm);
            //UUID = www.downloadHandler.text;
            if (sessionQuestAlgorithm.Contains("null"))
            {
                Debug.Log("Previous Quest Algorithm is null");
                questAlgorithm = UnityEngine.Random.Range(0, 3);
            }
            else if (sessionQuestAlgorithm.Contains("Random"))
            {
                Debug.Log("Previous Quest Algorithm is random");
                questAlgorithm = UnityEngine.Random.Range(1, 3);
            }
            else if (sessionQuestAlgorithm.Contains("RLAID"))
            {
                Debug.Log("Previous Quest Algorithm is RLAID");
                //select from 0 and 1, and if it selects 1 then set it to 2 instead
                questAlgorithm = UnityEngine.Random.Range(0, 2);
                if (questAlgorithm == 1)
                {
                    questAlgorithm = 2;
                }
            }
            else if (sessionQuestAlgorithm.Contains("Passage"))
            {
                Debug.Log("Previous Quest Algorithm is Passage");
                questAlgorithm = UnityEngine.Random.Range(0, 2);
            }
            else
            {
                Debug.Log("sessionQuestAlgorithm is an unexpected value " + sessionQuestAlgorithm);
            }

            switch (questAlgorithm)
            {
                case 0:
                    //StartCoroutine(GetAssetBundle());
                    StartCoroutine(telemetryUtil.PostData("QuestAlgorithm:Random"));
                    StartCoroutine(telemetryUtil.SaveAID("Random"));
                    break;
                case 1:
                    //StartCoroutine(GetAssetBundle());
                    StartCoroutine(telemetryUtil.PostData("QuestAlgorithm:RLAID"));
                    StartCoroutine(telemetryUtil.SaveAID("RLAID"));
                    break;
                case 2:
                    //StartCoroutine(GetAssetBundle());
                    StartCoroutine(telemetryUtil.PostData("QuestAlgorithm:Passage"));
                    StartCoroutine(telemetryUtil.SaveAID("Passage"));
                    break;
                default:
                    Debug.Log("ERROR: quest algorithm set to incorrect value. Expected value is 0, 1 or 2. Value is " + questAlgorithm.ToString());
                    break;
            }
        }
    }
}





