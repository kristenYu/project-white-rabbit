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
    private Button ExitBtn;

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
    public GameObject[] ShopItemObj;
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
        foreach (GameObject obj in ShopItemObj)
        {
            Button button = obj.GetComponent<Button>();
            int randNum = Random.Range(0, itemManager.furnitureArray.Length);

            while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
            {
                randNum = Random.Range(0, itemManager.furnitureArray.Length);
            }
            currentStoreItems.Add(itemManager.furnitureArray[randNum]);
            SetItemButton(obj, itemManager.furnitureArray[randNum]);
            button.onClick.AddListener(delegate { ItemPurchased(itemManager.furnitureArray[randNum]); });
        }
    }

    void UpdatePanel(GameObject objToUpdate, GameObject updateObj)
    {
        itemImage = objToUpdate.transform.GetChild(0).GetComponent<Image>();
        itemName = objToUpdate.transform.GetChild(1).GetComponent<Text>();
        itemCost = objToUpdate.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = updateObj.transform.GetChild(0).GetComponent<Image>().sprite;
        itemName.text = updateObj.transform.GetChild(1).GetComponent<Text>().text;
        itemCost.text = updateObj.transform.GetChild(2).GetComponent<Text>().text;
    }

    //Item Setup
    void SetItemButton(GameObject obj, GameObject currentItem)
    {
        itemImage = obj.transform.GetChild(0).GetComponent<Image>();
        itemName = obj.transform.GetChild(1).GetComponent<Text>();
        itemCost = obj.transform.GetChild(2).GetComponent<Text>();

        itemImage.sprite = currentItem.GetComponent<Item>().itemSprite;
        itemName.text = currentItem.GetComponent<Item>().stringName;
        itemCost.text = currentItem.GetComponent<Item>().cost.ToString();
    }  

    void ItemPurchased(GameObject item){

        //check if player can purchase item, if so remove item from store and add to player inventory
        int cost = item.GetComponent<Item>().cost;
        if (GameObject.Find("Player").GetComponent<PlayerController>().removeCurrency(cost)) 
        {
            GameObject.Find("Player").GetComponent<PlayerController>().AddObjectToInventory(item);
            for(int obj = 0; obj < ShopItemObj.Length; obj++)
            {
                //Debug.Log();
                if (ShopItemObj[obj].transform.GetChild(1).GetComponent<Text>().text == item.name) {
                    for (int i = obj; i < ShopItemObj.Length; i++)
                    {
                        print(ShopItemObj.Length);
                        if ((i+1) >= ShopItemObj.Length || ShopItemObj[i+1].activeSelf == false)
                        { 
                            ShopItemObj[i].SetActive(false);
                            print("Removed");
                        }   
                        else
                        {
                            print("moved?");
                            GameObject temp = ShopItemObj[i];
                            ShopItemObj[i] = ShopItemObj[i + 1];
                            //ShopItemObj[i + 1] = temp;
                            UpdatePanel(temp, ShopItemObj[i]);
                        }
                    }
                }
            }
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
