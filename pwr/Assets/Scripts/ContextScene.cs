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

    public AudioClip text0AudioClip;
    public AudioClip text1AudioClip;
    public AudioClip text2AudioClip;

    public int currentTextNumber;

    public AudioSource audioSource;
    public AudioClip audioClip;

    public Animator sceneTransition;
    public float transitionTime = 1.0f;

    void Start()
    {
        currentTextNumber = 0;
        welcomeText.text = text0;
        audioSource.PlayOneShot(text0AudioClip); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            currentTextNumber++;
            if (currentTextNumber == 1)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                audioSource.PlayOneShot(audioClip);
                audioSource.PlayOneShot(text1AudioClip);
                welcomeText.text = text1;
            }
            else if (currentTextNumber == 2)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                audioSource.PlayOneShot(text2AudioClip);
                audioSource.PlayOneShot(audioClip);
                welcomeText.text = text2; 
            }
            else if(currentTextNumber >= 3)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                audioSource.PlayOneShot(audioClip);
                StartCoroutine(PlaySceneTransition());
                
            }
        }
    }

    public IEnumerator PlaySceneTransition()
    {
        //play animation
        sceneTransition.SetTrigger("start");
        //wait for animation to stop playing 
        yield return new WaitForSeconds(transitionTime);
        //load scene
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
