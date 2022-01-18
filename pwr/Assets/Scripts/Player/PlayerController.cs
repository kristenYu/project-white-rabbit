using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
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
        body = GetComponent<Rigidbody2D>();
        previousDirection = Vector2.down; 
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
            if(Input.GetKeyDown(KeyCode.E))
            {
                //assumes that the object matches the name of the scene you want to load
                SceneManager.LoadScene(hit.transform.gameObject.name, LoadSceneMode.Single);
            }
        }
        else
        {
            interactPopup.SetActive(false);
        }
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hit)
            {

                
            }
        }
        */
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


