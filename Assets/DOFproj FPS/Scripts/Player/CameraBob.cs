/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS
{
    public class CameraBob : MonoBehaviour
    {
        FPSController controller;
        Animator animator;
        InputManager manager;

        void Start()
        {
            controller = FindObjectOfType<FPSController>();
            animator = GetComponent<Animator>();
            manager = FindObjectOfType<InputManager>();
        }

        // Update is called once per frame
        void Update()
        {
            animator.SetBool("isMoving", CheckMovement());

            if(!controller.crouch)
            animator.SetBool("Run", Input.GetKey(manager.Run));
        }

        public bool CheckMovement()
        {
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                return true;
            }

            return false;
        }
    }
}
