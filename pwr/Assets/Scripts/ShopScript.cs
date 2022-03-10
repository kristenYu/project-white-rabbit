using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/*Plan for script:
     * Click Item to buy to purchase and add to inventory
     * Click Item in Inventory to sell
     * --- For above two, should a little menu pop up? Or just click to buy if have funds/sell and gain money?
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

    //Shop item panel button
    public Button Item1;
    public Button Item2;
    public Button Item3;

    // Start is called before the first frame update
    void Start()
    {
        //Setting Button to move back to main
        Button ExitBtn = ExitToMainButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitButtonClicked);

        //set items
        Button ItemBtn1 = Item1.GetComponent<Button>();
        ItemBtn1.onClick.AddListener(ItemPurchased);

       /* Button ItemBtn2 = Item2.GetComponent<Button>();
        ItemBtn2.onClick.AddListener(ItemPurchased);

        Button ItemBtn3 = Item3.GetComponent<Button>();
        ItemBtn3.onClick.AddListener(ItemPurchased);*/
    }

    // Update is called once per frame
    void Update()
    {
        //purchase item

        //sell item

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
