/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS {

    public class HeadBob : MonoBehaviour
    {
        Animator animator;
        InputManager input;

        public float runHeadBobSpeed = 2;

        private void Start()
        {
            animator = GetComponent<Animator>();
            input = FindObjectOfType<InputManager>();
        }

        private void Update()
        {
            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"));

            animator.SetBool("Run", input.IsRunning());
        }
    }
}