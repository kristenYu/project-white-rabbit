using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_animation_script : MonoBehaviour
{
    public Animator animator; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("IsTriggerTestAnimation", true);
        }
    }
}
