/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;

namespace DOFprojFPS {

    public class Slot : MonoBehaviour {
        
        public Weapon storedWeapon;
        public GameObject storedDropObject;

        public bool IsFree()
        {
            if (!storedWeapon)
                return true;
            else
                return false;
        }
    }
}
