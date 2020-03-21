/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;

namespace DOFprojFPS {
    
    public class LadderTrigger : MonoBehaviour {

        private FPSController controller;

        private void Start()
        {
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<FPSController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                controller.isClimbing = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                controller.isClimbing = false;
            }
        }
    }
}