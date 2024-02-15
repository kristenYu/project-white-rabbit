using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class howto_shop_tutorial : MonoBehaviour
{

    public ShopScript shopScript;
    public bool hasShopped; 

    //Singleton 
    private static howto_shop_tutorial instance;
    // Read-only public access
    public static howto_shop_tutorial Instance => instance;

    void Awake()
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
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "TutorialShop")
        {
            shopScript = GameObject.FindGameObjectWithTag("shop").GetComponent<ShopScript>();
        }
        if (shopScript != null)
        {
            if (shopScript.tutorialBool)
            {
                hasShopped = true;
                Destroy(this.gameObject);
            }
        }   
    }
}
