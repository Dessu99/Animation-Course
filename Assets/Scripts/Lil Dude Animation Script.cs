using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class LilDudeAnimationScript : MonoBehaviour 
{
    Animator playerAnim;
    private bool isToggled = false;
    private bool Skele = false;
    private KeyCode[] kys = new KeyCode[]
    {
        KeyCode.D,
        KeyCode.F
    };
    private bool[] toggles = new bool[2];

    private void Awake()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
      /*  for (int i = 0; i < kys.Length; i++)
        {
            if (Input.GetKeyDown(kys[i]))
                toggles[i] = !toggles[i];
        }
        for (int i = 0; i < toggles.Length; i++)
            
            playerAnim.SetBool("Dance", toggles[i]);

            isToggled = !isToggled;


        if (Input.GetKeyDown(KeyCode.F))

            isToggled = !isToggled;

        if (isToggled)
        {
            Debug.Log("Toggled on");
            playerAnim.SetBool("SkeleDance", true);

        }

        else
        {
            Debug.Log("Toggled Off");
            playerAnim.SetBool("SkeleDance", false);
        }
        */




         if (Input.GetKeyDown(KeyCode.D))

        {
            print("Dance");
            playerAnim.SetBool("Dance", true);
        }
        else
        {
            print("Not Dance");
            playerAnim.SetBool("Dance", false);
        } 
    }
}
