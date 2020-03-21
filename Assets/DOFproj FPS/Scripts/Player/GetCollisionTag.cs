/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;

namespace DOFprojFPS
{
    public class GetCollisionTag : MonoBehaviour
    {
        public string contactTag;

        private void Update()
        {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, -transform.up, out hit, 4f))
            {
                contactTag = hit.collider.tag;
            }
        }
    }
}