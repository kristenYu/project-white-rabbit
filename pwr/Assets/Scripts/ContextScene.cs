using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ContextScene : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI welcomeText;

    public string text0;
    public string text1;
    public string text2;

    public int currentTextNumber;

    public AudioSource audioSource;
    public AudioClip audioClip; 

    void Start()
    {
        currentTextNumber = 0;
        welcomeText.text = text0; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            currentTextNumber++;
            if (currentTextNumber == 1)
            {
                audioSource.PlayOneShot(audioClip);
                welcomeText.text = text1;
            }
            else if (currentTextNumber == 2)
            {
                audioSource.PlayOneShot(audioClip);
                welcomeText.text = text2; 
            }
            else if(currentTextNumber >= 3)
            {
                audioSource.PlayOneShot(audioClip);
                SceneManager.LoadScene("Main", LoadSceneMode.Single);
            }
        }
    }
}
