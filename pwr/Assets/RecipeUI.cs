using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{

    public PlayerController playerController;
    public Recipe recipe;
    public Button button;

    private ColorBlock cb;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        button = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.CheckIfRecipeCanBeCooked(recipe))
        {
            //Changes the button's Highlighted color to the new color.
            cb = button.colors;
            cb.highlightedColor = new Color(203, 224, 174);
            button.colors = cb;
        }
        else
        {
            //Changes the button's Highlighted color to the new color.
            cb = button.colors;
            cb.highlightedColor = new Color(224, 185, 174);
            button.colors = cb;
        }
    }
}
