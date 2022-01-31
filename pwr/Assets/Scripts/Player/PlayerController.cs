using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Access World Database 
    public GameObject itemManagerObject;
    private ItemManager itemManager;
    public GameObject[] seedArray;
    public GameObject cropObject;
    public Seed seedScript;

    //inventory 
    public GameObject[] inventory;
    public GameObject ActiveItem; 
    public int currentInventoryIndex;
    private int inventorySize;
    private GameObject tempObject;


    //Movement
    private Rigidbody2D body;
    private float horizontal;
    private float vertical;
    //this is needed for keyboard movement, not needed for controller
    private float moveLimiter = 0.7f;
    public float runSpeed = 7.0f;
    public Vector2 previousDirection;

    //juice
    public GameObject interactPopup;

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
        inventorySize = 5;
        inventory = new GameObject[inventorySize];
        currentInventoryIndex = 0; 

        body = GetComponent<Rigidbody2D>();
        previousDirection = Vector2.down;

        itemManager = itemManagerObject.GetComponent<ItemManager>();
        seedArray = itemManager.seedArray;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //raycast 
        RaycastHit2D hit;
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

        if(hit)
        {
            interactPopup.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("register E key press");
                if (hit.transform.gameObject.tag == "new_scene")
                {
                    //assumes that the object matches the name of the scene you want to load
                    SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
                }
                else if (hit.transform.gameObject.tag == "debug_seed")
                {
                    //Does not get added to inventory for now - will figure out flow later
                    ActiveItem = itemManager.seedArray[0];
                }
                else if (hit.transform.gameObject.tag == "planting")
                {
                    if(ActiveItem != null)
                    {
                        if (ActiveItem.tag == "seed")
                        {
                            seedScript = ActiveItem.GetComponent<Seed>();
                            Instantiate(seedScript.crop, hit.transform.position, Quaternion.identity);
                            //remove item from activeItem 
                            ActiveItem = null;
                        }
                    }
                }

            }
        }
        else
        {
            interactPopup.SetActive(false);
        }
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
            inventory[indexAt] = item;
            return true;
        }
    }

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
            }
        }
        return tempObject;
    }

    public GameObject RemoveObjectFromInventory(int indexAt)
    {
        tempObject = inventory[indexAt];
        inventory[indexAt] = null;
        return tempObject;
    }


    private RaycastHit2D drawRay(Vector2 direction, bool debug)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.0f);
        if(debug)
        {
            Debug.DrawRay(transform.position, direction, Color.white, 1.0f);
        }
        return hit; 
    }
}


