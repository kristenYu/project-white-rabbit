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
     * --- Weird error where player character can move...might want to disable background?? or something??
     * 
     * Items refresh everyday!
     */

public class ShopScript : MonoBehaviour
{
    //Exit Shop
    public Button ExitToMain;

    // Start is called before the first frame update
    void Start()
    {
        //Remove movement for player
        //GameObject.Find("Player").GetComponent(PlayerMovement).enabled = false;

        //Setting Button to move back to main
        Button ExitBtn = ExitToMain.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    void ButtonClicked()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
        //Restart movement for player

    }

}
