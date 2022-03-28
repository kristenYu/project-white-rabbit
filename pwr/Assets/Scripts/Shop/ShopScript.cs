using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*Plan for script:
     * Click Item to buy to purchase and add to inventory
     * Click Item in Inventory to sell
     * --- how to apply script to other prefabs? Make new script???
     * 
     * Click arrows to move between catagories 
     * --- Will need to turn panels on/off
     * 
     * Exit Shop to Main Scene
     * -- remove HUD in background! should be able to disable it!
     * 
     * Items refresh everyday!
     */

public class ShopScript : MonoBehaviour
{
    //Exit Shop
    public GameObject ExitToMainButton;
    private Button ExitBtn;

    //Access Items Database and Player Controller
    public GameObject[] item_managerArray;
    public ItemManager itemManager;
    public GameObject playerControllerObject;
    private PlayerController playerController;

    //Shop item panel elements
    public Button ItemBtn;
    public Image ItemImage;
    public Text ItemName;
    public Text ItemCost;
    public Button[] ShopItem;
    public List<GameObject> currentStoreItems = new List<GameObject>();

    //test object for loading random item for purchase
    private GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        //load the item manager and player controller
        playerController = playerControllerObject.GetComponent<PlayerController>();

        //Setting Button to move back to main
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);

        //Using Tags to access itemManager...
        item_managerArray = GameObject.FindGameObjectsWithTag("item_manager");
        foreach(GameObject manager in item_managerArray)
        {
            itemManager = manager.GetComponent<ItemManager>();
        }
        //Load store items and set it in the store 
        SetPanels();
    }

    // Update is called once per frame
    void Update()
    {
        

        //set furniture for day

        //purchase item

        //sell item

    }

    //Shop Panel setup
    void SetPanels()
    {
        //set furniture panel
        foreach (Button item in ShopItem)
        {
            Debug.Log(itemManager.furnitureArray.Length);
            int randNum = Random.Range(0, itemManager.furnitureArray.Length);
            
            while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
            {
                randNum = Random.Range(0, itemManager.furnitureArray.Length);
            }
            currentStoreItems.Add(itemManager.furnitureArray[randNum]);
            obj = itemManager.furnitureArray[randNum];
            SetItemButton(item,obj);
            ItemBtn = item.GetComponent<Button>();
            ItemBtn.onClick.AddListener(delegate{ItemPurchased(item,obj);});
        }
    }

    //Item Setup
    void SetItemButton(Button item, GameObject obj)
    {
        ItemImage = item.transform.GetChild(0).GetComponent<Image>();
        ItemName = item.transform.GetChild(1).GetComponent<Text>();
        ItemCost = item.transform.GetChild(2).GetComponent<Text>();

        ItemImage.sprite = obj.GetComponent<Item>().itemSprite;
        ItemName.text = obj.GetComponent<Item>().stringName;
        ItemCost.text = obj.GetComponent<Item>().cost.ToString();
    }

    //Item Remove
    void RemoveItemButton(Button item)
    {
        ItemImage = item.transform.GetChild(0).GetComponent<Image>();
        ItemName = item.transform.GetChild(1).GetComponent<Text>();
        ItemCost = item.transform.GetChild(2).GetComponent<Text>();

        ItemImage.sprite = null;
        ItemName.text = null;
        ItemCost.text = null;
    }
    
    void ItemPurchased(Button item, GameObject obj){
        int cost = obj.GetComponent<Item>().cost;
        print(playerController.GetComponent<PlayerController>().removeCurrency(cost));
        if (playerController.GetComponent<PlayerController>().removeCurrency(cost))
        {
            RemoveItemButton(item);
        }
        Debug.Log("Pressed!");
    }

    void ItemSold()
    {

    }
    void ExitButtonClicked()
    {
        //load main and enable player movement
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = true;
    }

}
