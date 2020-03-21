/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;

namespace DOFprojFPS
{
    public class BalisticProjectile : MonoBehaviour
    {
        [HideInInspector]
        public float initialVelocity = 360;
        [HideInInspector]
        public float airResistance = 0.1f;

        private float time;

        private float livingTime = 1f;

        Vector3 lastPosition;

        public Weapon weapon;

        private void OnEnable()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * initialVelocity, ForceMode.VelocityChange);

            lastPosition = transform.position;
        }

        private void Update()
        {
            time += Time.deltaTime;

            RaycastHit hit;
            if (Physics.Linecast(lastPosition, transform.position, out hit))
            {
                weapon.ApplyHit(hit);

                gameObject.SetActive(false);
            }

            lastPosition = transform.position;

            if (time > livingTime)
            {
                gameObject.SetActive(false);
            }
        }


        private void OnDisable()
        {
            time = 0;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = Vector3.zero;
        }
    }
}
