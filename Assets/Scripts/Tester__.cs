using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AG3959
{

    public class Tester__ : MonoBehaviour
    {
        Animator playerAnim;
        private bool isToggled = false;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))

            {
                print("Spell");
                playerAnim.SetBool("Spell", true);
            }
            else
            {
                print("Not Spell");
                playerAnim.SetBool("Spell", false);
            }
        }
    }
}