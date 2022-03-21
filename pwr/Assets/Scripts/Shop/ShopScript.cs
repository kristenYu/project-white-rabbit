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
    public Button ExitToMainButton;
    public Button ExitBtn;

    //Access Items Database
    public GameObject itemManagerObject;
    private ItemManager itemManager;

    //Shop item panel elements
    public Button ItemBtn;
    public Image ItemImage;
    public Text ItemName;
    public Text ItemCost;
    public Button[] ShopItem;
    public List<GameObject> currentStoreItems = new List<GameObject>();

    //test object for loading random item for purchase
    private GameObject furniture;

    // Start is called before the first frame update
    void Start()
    {
        //load the item manager
        itemManager = itemManagerObject.GetComponent<ItemManager>();

        //Setting Button to move back to main, put Button outside!!!
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);

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
            int randNum = Random.Range(0, itemManager.furnitureArray.Length);
            while (currentStoreItems.Contains(itemManager.furnitureArray[randNum]) == true)
            {
                randNum = Random.Range(0, itemManager.furnitureArray.Length);
                print("In List");
            }
            currentStoreItems.Add(itemManager.furnitureArray[randNum]);
            furniture = itemManager.furnitureArray[randNum];
            SetItemButton(item);
            ItemBtn = item.GetComponent<Button>();
            ItemBtn.onClick.AddListener(ItemPurchased);
        }
    }

    //Item setup
    void SetItemButton(Button Item)
    {
        ItemImage = Item.transform.GetChild(0).GetComponent<Image>();
        ItemName = Item.transform.GetChild(1).GetComponent<Text>();
        ItemCost = Item.transform.GetChild(2).GetComponent<Text>();

        ItemImage.sprite = furniture.GetComponent<Item>().itemSprite;
        ItemName.text = furniture.GetComponent<Item>().stringName;
        ItemCost.text = furniture.GetComponent<Item>().cost.ToString();
    }
    void ItemPurchased()
    {
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
