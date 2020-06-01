/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using UnityEngine;
using System;

namespace MLSpace
{
    public class CameraControl : MonoBehaviour
    {

        /// <summary>
        /// Speed of camera movement
        /// </summary>
        [Tooltip("Speed of camera movement.")]
        public float speed = 2.0f;

        /// <summary>
        /// Speed of camera rotation
        /// </summary>
        [Tooltip("Speed of camera rotation.")]
        public float angularSpeed = 100.0f;

        private float m_totalXAngleDeg = 0;     // accumulated camera rotation on x axis
        private float m_totalYAngleDeg = 0;     // accumulated camera rotation on y axis


        // Use this for initialization
        void Start()
        {
            Vector3 euler = transform.rotation.eulerAngles;
            m_totalXAngleDeg = euler.x;
            m_totalYAngleDeg = euler.y;
            transform.rotation = Quaternion.Euler(euler.x, euler.y, 0); // straighten camera
        }

        // update camera transform
        void FixedUpdate()
        {
            if (!Input.GetMouseButton(0))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;


            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            bool u = Input.GetKey(KeyCode.E);
            bool e = Input.GetKey (KeyCode .Q );

            float angleAroundY = Input.GetAxisRaw("Mouse X");
            float angleAroundX = -Input.GetAxisRaw("Mouse Y");

            float f = v * speed * Time.deltaTime;
            float s = h * speed * Time.deltaTime;
            float up = Convert.ToSingle (u) * speed * Time.deltaTime;
            float down = Convert.ToSingle(e) * speed * Time.deltaTime;

            transform.position += transform.forward * f;
            transform.position += transform.right * s;
            transform.position += transform.up * up;
            transform.position -= transform.up * down;



            float currentAngleY = angleAroundY * Time.deltaTime * angularSpeed;
            float currentAngleX = angleAroundX * Time.deltaTime * angularSpeed;


            m_totalXAngleDeg += currentAngleX;
            m_totalYAngleDeg += currentAngleY;

            Quaternion rotation =
                Quaternion.Euler
                (
                    m_totalXAngleDeg,
                    m_totalYAngleDeg,
                    0
                );
            transform.rotation = rotation;
        }

    } 
}
