using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinScreen : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public WorldController worldController;
    public TextMeshProUGUI dayStatText;
    public TextMeshProUGUI plantsStatText;
    public TextMeshProUGUI cookStatText;
    public TextMeshProUGUI furnitureStatText;
    public TextMeshProUGUI harvestablesStatText;
    public AudioSource worldControllerAudioSource;
    public AudioClip winScreenSong;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        player.GetComponent<SpriteRenderer>().enabled = false;

        worldController = GameObject.FindGameObjectWithTag("world_c").GetComponent<WorldController>();
        worldControllerAudioSource = GameObject.FindGameObjectWithTag("world_c").GetComponent<AudioSource>();
        worldControllerAudioSource.clip = winScreenSong;
        worldControllerAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        dayStatText.text = worldController.currentDay.ToString();
        plantsStatText.text = playerController.actionFrequencyArray[(int)QuestBoard.QuestType.plant].ToString();
        cookStatText.text = playerController.actionFrequencyArray[(int)QuestBoard.QuestType.cook].ToString();
        furnitureStatText.text = playerController.actionFrequencyArray[(int)QuestBoard.QuestType.place].ToString();
        harvestablesStatText.text = playerController.actionFrequencyArray[(int)QuestBoard.QuestType.harvest].ToString();
    }
}
