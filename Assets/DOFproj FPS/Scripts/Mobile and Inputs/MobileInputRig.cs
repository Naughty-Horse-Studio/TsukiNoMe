/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS
{
    public class MobileInputRig : MonoBehaviour
    {
        WeaponManager manager;
        
        public bool setFire = false;

        private void Start()
        {
            manager = FindObjectOfType<WeaponManager>();
        }

        private void Update()
        {
            if (setFire)
                Fire();
        }
        
        public void SetFire(bool state)
        {
            setFire = state;
        }

        public void Fire()
        {
                manager.activeWeapon.Fire();
        }

        public void Aim()
        {
            if (manager.activeWeapon != null)
            {
                manager.activeWeapon.AimMobile();
            }
        }

        public void Reload()
        {
            if (manager.activeWeapon != null)
            {
                manager.activeWeapon.ReloadBegin();
            }
        }
    }
}