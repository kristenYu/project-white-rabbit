using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToMove1 : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip tutorialMoveClip;
    public AudioSource audioSource;
    void Start()
    {
        audioSource.PlayOneShot(tutorialMoveClip);
    }
}
