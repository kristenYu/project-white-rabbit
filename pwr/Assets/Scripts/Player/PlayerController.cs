using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public Button testButton;

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
    public GameObject recipeUIPrefab;
    private List<GameObject> knownRecipeUIObjects;
    private const float knownRecipeUIOffset = -1.5f;
    private GameObject cookingUIContent;
    private Image recipeUIImage;
    private Button recipeUIButton; 
    private TextMeshProUGUI recipeUIText;

    //Cooking
    private List<GameObject> targetIngredients;
    private GameObject currentIngredient;
    private bool isIngredientInInventory;
    private GameObject cookedFoodObject; 

    //inventory 
    public GameObject[] inventory;
    public Image[] inventoryHUDObjects;
    public GameObject activeItem; 
    public int currentInventoryIndex;
    public int previousInventoryIndex; 
    private const int inventorySize = 10;
    private GameObject tempObject;
    private Item currentItem; 
    

    //Currency
    public int currency;
    public TextMeshProUGUI currencyText;

    //place furniture 
    public GameObject furnitureObject;
    private Furniture furnitureScript;
    private SpriteRenderer furnitureSpriteRenderer;

    //Movement
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
    private int layerMask;

    //Debug
    private GameObject testObject; 

    //Singleton 
    private static PlayerController instance;
    // Read-only public access
    public static PlayerController Instance => instance;


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

    void Start()
    {
        currency = 30;
        inventory = new GameObject[inventorySize];
        previousInventoryIndex = 0; 
        currentInventoryIndex = 0; 

        body = GetComponent<Rigidbody2D>();
        previousDirection = Vector2.down;

        cookingUIContent = cookingUI.transform.GetChild(0).GetChild(0).gameObject;
        knownRecipeUIObjects = new List<GameObject>();
        targetIngredients = new List<GameObject>(); 

        itemManager = itemManagerObject.GetComponent<ItemManager>();
        worldController = worldControllerObject.GetComponent<WorldController>();

        anim = GetComponent<Animator>(); 


        foreach(Image itemImage in inventoryHUDObjects)
        {
            itemImage.gameObject.SetActive(false);
        }

        layerMask = 1 << 2;
        layerMask = ~layerMask;
    }

    void Update()
    {
        ShowActiveItem();

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        
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

        //Interaction raycasts
        if (horizontal < 0)
        {
            hit = drawRay(Vector2.left, false);
            previousDirection = Vector2.left;
        }
        else if (horizontal > 0)
        {
            hit = drawRay(Vector2.right, false);
            previousDirection = Vector2.right;
        }
        else if (vertical < 0)
        {
            hit = drawRay(Vector2.down, false);
            previousDirection = Vector2.down;
        }
        else if (vertical > 0)
        {
            hit = drawRay(Vector2.up, false);
            previousDirection = Vector2.up;
        }
        else
        {
            hit = drawRay(previousDirection, false);
        }

        //Rotate furniture
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (activeItem != null)
            {
                if (activeItem.tag == "furniture")
                {
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
                }
            }
        }

        //inventory QOL Update
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            setActiveItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            setActiveItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            setActiveItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            setActiveItem(3);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            setActiveItem(4);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            setActiveItem(5);
        }
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            setActiveItem(6);
        }
        if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
        {
            setActiveItem(7);
        }
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
        {
            setActiveItem(8);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            setActiveItem(9);
        }

        //interact with objects
        if (hit)
        {
            if(hit.transform.tag != "non_interact")
            {
                interactPopup.SetActive(true);
            }

            if(hit.transform.tag == "cooking")
            {
                cookingUI.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                //transitions scenes 
                if (hit.transform.gameObject.tag == "new_scene")
                {
                    //assumes that the object matches the name of the scene you want to load
                    SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                }
                else if (hit.transform.gameObject.tag == "debug_seed")
                {
                    foreach(GameObject seed in itemManager.seedArray)
                    {
                        if(seed.GetComponent<Seed>().stringName == "potato")
                        {
                            testObject = seed;
                        }
                    }
                    //testObject = itemManager.seedArray[Random.Range(0, itemManager.seedArray.Length - 1)];
                    AddObjectToInventory(testObject);
                }
                //plants a seed if the active item is a seed
                else if (hit.transform.gameObject.tag == "planting")
                {
                    if(activeItem != null)
                    {
                        if (activeItem.tag == "seed")
                        {
                            seedScript = activeItem.GetComponent<Seed>();
                            cropObject = Instantiate(seedScript.crop, hit.transform.position, Quaternion.identity);
                            //add crop object to world controller 
                            cropScript = cropObject.GetComponent<Crop>();
                            cropScript.worldController = this.worldControllerObject.GetComponent<WorldController>();
                            worldController.activeCropList.Add(cropObject);
                            //remove activeitem from inventory 
                            RemoveObjectFromInventory(currentInventoryIndex);
                            activeItem = null;

                        }
                    }
                }
                //Harvest food from a crop that is fully grown 
                else if(hit.transform.gameObject.tag == "crop")
                {
                    //check if the crop can be harvested and harvests if it can 
                    cropScript = hit.transform.gameObject.GetComponent<Crop>();
                    AddObjectToInventory(cropScript.HarvestCrop());

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
                            RemoveObjectFromInventory(activeItem);
                            PlaceFurniture(hit);
                        }
                    }
                }
                //picks up furniture
                else if(hit.transform.gameObject.tag == "furniture")
                {
                    PickUpFurniture(hit);
                    AddObjectToInventory(activeItem);
                }
                else if(hit.transform.gameObject.tag == "debug_unlock_recipes")
                {
                    Debug_UnlockAllRecipes();
                }
                //cooking
                {

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
    }
	
    private void FixedUpdate()
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

    public void setActiveItem(int index)
    {
        previousInventoryIndex = currentInventoryIndex;
        currentInventoryIndex = index;
        activeItem = inventory[index];
    }

    /*
    public void setActiveItem(GameObject item)
    {
        /*
        if(activeItem == null)
        {
            activeItem = item;
        }
        
        activeItem = item; 
    }
    */

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
                    inventory[currentInventoryIndex] = item; 
                    inventoryHUDObjects[currentInventoryIndex].sprite = currentItem.itemSprite;
                    inventoryHUDObjects[currentInventoryIndex].gameObject.SetActive(true);
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
                inventoryHUDObjects[currentInventoryIndex].sprite = currentItem.itemSprite;
                inventoryHUDObjects[currentInventoryIndex].gameObject.SetActive(true);
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
        inventoryHUDObjects[currentInventoryIndex].gameObject.SetActive(false);
        return tempObject;
    }

    public GameObject RemoveObjectFromInventory(int indexAt)
    {
        tempObject = inventory[indexAt];
        inventory[indexAt] = null;
        inventoryHUDObjects[indexAt].gameObject.SetActive(false);
        currentInventoryIndex = indexAt; 
        return tempObject;
    }

    private void ShowActiveItem()
    {
        if(activeItem != null)
        {
            if (activeItem.gameObject.tag == "furniture")
            {
                activeItem.SetActive(true);
                activeItem.transform.localPosition = Vector3.zero + new Vector3(previousDirection.x, previousDirection.y, 0);
            }  
        }
        if (inventory[previousInventoryIndex] != null)
        {
            if (inventory[previousInventoryIndex].tag == "furniture")
            {
                inventory[previousInventoryIndex].SetActive(false);
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
        furnitureObject.transform.SetParent(this.transform);
        furnitureObject.transform.localPosition = new Vector3(previousDirection.x, previousDirection.y, 0);
        furnitureObject.transform.gameObject.layer = 2; //Ignore Raycast Layer
        activeItem = furnitureObject;
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
        currentRecipeUIObject.transform.SetParent(cookingUIContent.transform, false);
        currentRecipeUIObject.transform.position = new Vector3(cookingUIContent.transform.position.x, (cookingUIContent.transform.position.y - (2.25f + knownRecipeUIOffset*index)), 0);
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

        recipeUIImage.overrideSprite = recipe.cookedFood.GetComponent<CookedFood>().itemSprite;
        recipeUIText.text = recipe.stringName;

        recipeUIButton.onClick.AddListener(delegate { CookRecipe(recipe); });
    }

    private bool CookRecipe(Recipe recipe)
    {
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
                Debug.Log("Recipe Cannot be made because player does not have the correct ingredients");
                //return the items to the inventory 
                foreach(GameObject item in targetIngredients)
                {
                    AddObjectToInventory(item);
                }

                return false; 
            }
        }
        cookedFoodObject = Instantiate(recipe.cookedFood);
        AddObjectToInventory(cookedFoodObject);
        return true;
    }    

}



