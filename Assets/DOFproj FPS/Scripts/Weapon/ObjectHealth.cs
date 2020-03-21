/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;

namespace DOFprojFPS
   {
    public class ObjectHealth : MonoBehaviour {

        //Blank class used for interaction between player damage and reciever

        public float health = 100;
        
        public bool instantiateAfterDeath = false;

        public GameObject objToInstantiate;

        void Update()
        {
            if (health < 0)
            {
                if(instantiateAfterDeath)
                    Instantiate(objToInstantiate, transform.position, transform.rotation);

                Destroy(gameObject);
            }
        }
    }
}
