/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using UnityEngine;

namespace MLSpace
{
    public class BeamControl : MonoBehaviour
    {
        private Material m_Mat;                 // material to modify
        private float m_FadeDist = 10.0f;       // material fade distance
        private bool m_initialized = false;     // is component initialized ?

        /// <summary>
        /// Initialize component
        /// </summary>
        public void Initialize()
        {
            if (m_initialized) return;

            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (!mr) { Debug.LogError("Cannot find 'MeshRenderer' component."); return; }

            m_Mat = mr.material;
            if (!m_Mat) { Debug.LogError("Object cannot be null."); return; }

            m_FadeDist = m_Mat.GetFloat("_FadeDist");


            m_initialized = true;
        }

        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_initialized) { Debug.LogError("component not initialized."); return; }

            Ray ray = new Ray(transform.position, transform.forward);
            float dist = m_FadeDist;
            RaycastHit rhit;
            if (Physics.Raycast(ray, out rhit, m_FadeDist))
            {
                dist = rhit.distance;
            }
            m_Mat.SetFloat("_FadeDist", dist);
        }
    } 
}
