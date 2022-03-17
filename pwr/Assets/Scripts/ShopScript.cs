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
    private List<int> CurrentItems;

    //Shop item panel elements
    public Button ItemBtn;
    public Image ItemImage;
    public Text ItemName;
    public Text ItemCost;
    public List<Button> ItemBtnList = new List<Button>();

    //test object for loading random item for purchase
    public GameObject furniture;

    // Start is called before the first frame update
    void Start()
    {
        //load the item manager
        itemManager = itemManagerObject.GetComponent<ItemManager>();

        //Setting Button to move back to main, put Button outside!!!
        ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);

        //load store items and set it on the store
        int randNum = Random.Range(0, itemManager.furnitureArray.Length);
        furniture = itemManager.furnitureArray[randNum];
        Debug.Log(furniture.name);
        //set items
        ItemBtn = ItemBtn.GetComponent<Button>();
        ItemImage.sprite = furniture.GetComponent<Item>().itemSprite;
        ItemName.text = furniture.GetComponent<Item>().stringName;
        ItemCost.text = furniture.GetComponent<Item>().cost.ToString();
        /*icon = furniture.
        ItemBtn.onClick.AddListener(ItemPurchased);*/
    }

    // Update is called once per frame
    void Update()
    {
        //set furniture for day

        //purchase item

        //sell item

    }

    void SetItemUI()
    {

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
