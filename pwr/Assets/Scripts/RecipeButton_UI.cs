using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeButton_UI : MonoBehaviour
{

    public PlayerController playerController;
    public Recipe recipe;
    public Button button;

    private ColorBlock cb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        button = this.gameObject.GetComponent<Button>();
        if (playerController.CheckIfRecipeCanBeCooked(recipe))
        {
            //Changes the button's Highlighted color to the new color.
            cb = button.colors;
            //cb.highlightedColor = Color.green;
            cb.highlightedColor = new Color(0.5377715f, 0.6887516f, 0.8584906f, 1f);
            button.colors = cb;
        }
        else
        {
            //Changes the button's Highlighted color to the new color.
            cb = button.colors;
            //cb.highlightedColor = Color.red;
            cb.highlightedColor = new Color(0.735849f, 0.4396859f, 0.3637593f, 1f);
            button.colors = cb;
        }
    }
}
