using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LilDudeAnimationScript : MonoBehaviour 
{
    Animator playerAnim;
    private bool isToggled = false;

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isToggled)
        {
            isToggled = !isToggled;
            Debug.Log("Toggled on");
            playerAnim.SetBool("Dance", true);

        }

        else
        {
            Debug.Log("Toggled Off");
            playerAnim.SetBool("Dance", false);
        }






        /* if (Input.GetKeyDown(KeyCode.D))

        {
            print("Dance");
            playerAnim.SetBool("Dance", true);
        }
        else
        {
            print("Not Dance");
            playerAnim.SetBool("Dance", false);
        } */
    }
}
