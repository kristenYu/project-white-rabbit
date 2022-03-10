using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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

    //inventory 
    public GameObject[] inventory;
    public Image[] inventoryHUDObjects;
    public GameObject activeItem; 
    public int currentInventoryIndex;
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
    private BoxCollider2D furnitureBoxCollider;
    

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
        currentInventoryIndex = 0; 

        body = GetComponent<Rigidbody2D>();
        previousDirection = Vector2.down;

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
        //inventory
        ShowActiveItem();

        //player animation
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

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

        //interact with objects
        if (hit)
        {
            if(hit.transform.tag != "non_interact")
            {
                interactPopup.SetActive(true);
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
                    testObject = itemManager.seedArray[Random.Range(0, itemManager.seedArray.Length - 1)];
                    setActiveItem(testObject);
                    SetNextOpenInventory(); 
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
                            //remove item from activeItem 
                            RemoveObjectFromInventory(activeItem);
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
                    setActiveItem(testObject);
                    AddObjectToInventory(testObject);
                    furnitureObject = Instantiate(activeItem, this.transform.position, Quaternion.identity);
                    furnitureObject.transform.SetParent(this.transform);
                    furnitureObject.transform.localPosition = new Vector3(previousDirection.x, previousDirection.y, 0);
                    furnitureObject.transform.gameObject.layer = 2; //Ignore Raycast Layer
                    worldController.placedFurnitureObjects.Add(furnitureObject);

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
                    setActiveItem(hit.transform.gameObject);
                    if(activeItem != null)
                    {
                        PickUpFurniture(hit);
                        SetNextOpenInventory();
                        AddObjectToInventory(activeItem);
                    }                    
                }
            }
           
        }
        else
        {
            interactPopup.SetActive(false);
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

    public void setActiveItem(GameObject item)
    {
        /*
        if(activeItem == null)
        {
            activeItem = item;
        }
        */
        activeItem = item; 
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
        if(item == null)
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
                    if (currentInventoryIndex < inventorySize)
                    {
                        currentInventoryIndex++;
                    }
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
        if(indexAt > inventorySize)
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
                if (currentInventoryIndex < inventorySize)
                {
                    currentInventoryIndex++;
                }
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
                furnitureObject.transform.localPosition = Vector3.zero + new Vector3(previousDirection.x, previousDirection.y, 0);
            }
        }
    }
    private void PlaceFurniture(RaycastHit2D hit2D)
    {
        furnitureObject.transform.parent = null;
        furnitureObject.transform.position = hit2D.transform.position;
        furnitureObject.transform.gameObject.layer = 1; //default layer
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
	
}


