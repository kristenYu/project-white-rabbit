using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToInteract1 : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip audioClip;
    public AudioSource audioSource;
    public AudioSource previousAudioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (previousAudioSource != null)
            {
                if (previousAudioSource.isPlaying)
                {
                    previousAudioSource.Stop();
                }
            }
            
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

    }
 
    
}
