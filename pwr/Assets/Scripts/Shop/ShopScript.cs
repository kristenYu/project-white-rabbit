using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopScript : MonoBehaviour
{
    //Exit Shop
    public Button ExitToMainButton;
    public Button ExitBtn;

    //Access Items Database and Player Controller
    public GameObject[] item_managerArray;
    public ItemManager itemManager;
    public GameObject playerControllerObject;
    private PlayerController playerController;

    //Shop item panel elements
    public Button itemBtn;
    public Image itemImage;
    public Text itemName;
    public Text itemCost;
    public GameObject[] ShopItem;
    public List<GameObject> currentStoreItems = new List<GameObject>();

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
        SetPanels(false);
    }

    // Update is called once per frame
    void Update()
    {
        //set furniture for day

        //purchase item

        //sell item
    }

    //Shop Panel setup
    void SetPanels(bool itemPurchased)
    {
        if (itemPurchased == false)
        {
            foreach (GameObject obj in ShopItem)
            {
                obj.SetActive(true);
                Button button = obj.GetComponent<Button>();
                int randNum = Random.Range(0, itemManager.furnitureArray.Length);

                while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
                {
                    randNum = Random.Range(0, itemManager.furnitureArray.Length);
                }

                currentStoreItems.Add(itemManager.furnitureArray[randNum]);
                SetItemButton(button, itemManager.furnitureArray[randNum]);
                //itemBtn = button.GetComponent<Button>();
                button.onClick.AddListener(delegate { ItemPurchased(button, itemManager.furnitureArray[randNum]); });
            }
        }

        else
        {
            foreach (GameObject obj in ShopItem)
            {
                obj.SetActive(false);
            }
            print("currentStoreItems = " + currentStoreItems.Count);
            for (int i = 0; i < currentStoreItems.Count; i++)
            {
                Debug.Log(i);
                if (currentStoreItems.Count > 0)
                {
                    print(currentStoreItems.Count);
                    ShopItem[i].SetActive(true);
                    itemBtn = ShopItem[i].GetComponent<Button>();
                    Debug.Log(currentStoreItems);
                    SetItemButton(itemBtn, currentStoreItems[i]);
                    //ShopItem[i].GetComponent<Button>().onClick.AddListener(delegate { ItemPurchased(itemBtn, currentStoreItems[i]); });
                }
            }
            /*{
                print("currentStoreItems = " + currentStoreItems.Count);
                ShopItem[i].SetActive(true);
                itemBtn = ShopItem[i].GetComponent<Button>();
                SetItemButton(itemBtn, currentStoreItems[i]);
                Debug.Log(i,this);
                //ShopItem[i].GetComponent<Button>().onClick.AddListener(delegate { ItemPurchased(itemBtn, currentStoreItems[i]); });
            }*/
        }
    


        /*//check if we are setting NEW panels or setting after purchase
        print("Current = " + currentStoreItems.Count);
        if (currentStoreItems.Count == 0)
        {
            //set furniture panel
            foreach (GameObject obj in ShopItem)
            {
                obj.SetActive(true);
                Button button = obj.GetComponent<Button>();
                int randNum = Random.Range(0, itemManager.furnitureArray.Length);

                while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
                {
                    randNum = Random.Range(0, itemManager.furnitureArray.Length);
                }            
                currentStoreItems.Add(itemManager.furnitureArray[randNum]);
                SetItemButton(button, itemManager.furnitureArray[randNum]);
                //itemBtn = button.GetComponent<Button>();
                button.onClick.AddListener(delegate { ItemPurchased(button, itemManager.furnitureArray[randNum]); });
            }
        }
        else
        {
            for (int i = 0; i < currentStoreItems.Count; i++)
            {
                print("currentStoreItems" + currentStoreItems.Count);
                ShopItem[i].SetActive(true);
                itemBtn = ShopItem[i].GetComponent<Button>();
                SetItemButton(itemBtn, currentStoreItems[i]);
                Debug.Log(i);
                ShopItem[i].GetComponent<Button>().onClick.AddListener(delegate { ItemPurchased(itemBtn, currentStoreItems[i]); });
            }
        }*/
    }

    //Item Setup
    void SetItemButton(Button item, GameObject obj)
    {
        itemImage = item.transform.GetChild(0).GetComponent<Image>();
        itemName = item.transform.GetChild(1).GetComponent<Text>();
        itemCost = item.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = obj.GetComponent<Item>().itemSprite;
        itemName.text = obj.GetComponent<Item>().stringName;
        itemCost.text = obj.GetComponent<Item>().cost.ToString();
    }

    //Item Remove
    void RemoveItemButton(Button item)
    {
        itemImage = item.transform.GetChild(0).GetComponent<Image>();
        itemName = item.transform.GetChild(1).GetComponent<Text>();
        itemCost = item.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = null;
        itemName.text = null;
        itemCost.text = null;
    }
    
    void ItemPurchased(Button button, GameObject item){

        print("item purchased = " + item);
        //check if player can purchase item, if so remove item from store and add to player inventory
        //GameObject item = itemManager.furnitureArray[randNum];
        int cost = item.GetComponent<Item>().cost;
        if (GameObject.Find("Player").GetComponent<PlayerController>().removeCurrency(cost)) 
        {
            GameObject.Find("Player").GetComponent<PlayerController>().AddObjectToInventory(item);
            RemoveItemButton(button);
            //button.enabled = false;
            currentStoreItems.Remove(item);
            //item.SetActive(false);
            SetPanels(true);
        }
        else
        {
            //message saying no money :(, make into a pop up in game later?
            print("Sorry! You don't have enough money!!");
        }
    }

    void ItemSold()
    {

    }
    void ExitButtonClicked()
    {
        //load main and enable player movement
        print("Exit Clicked!");
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = true;
    }

}
