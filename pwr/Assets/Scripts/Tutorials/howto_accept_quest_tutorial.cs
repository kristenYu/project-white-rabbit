using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howto_accept_quest_tutorial : MonoBehaviour
{
    public PlayerController playerController;
    public bool hasAcceptedQuest; 

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.CountNumberOfActiveQuests() > 0)
        {
            hasAcceptedQuest = true;
            Destroy(this.gameObject);
        }
    }
}
