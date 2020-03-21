/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS {
    public class EquipmentPanel : MonoBehaviour
    {
        public Item equipedItem;
        public Item lastItem;

        [HideInInspector]
        public GridSlot mainSlot;

        [Range(1, 100)]
        public int width, height;

        public ItemType allowedItemType;

        [Header("Using ids ignore allowedItemType. Only specified id items will be equiped")]
        public int[] allowedIds;

        public KeyCode activateKey;

        private WeaponManager weaponManager;

        private void Start()
        {
            weaponManager = FindObjectOfType<WeaponManager>();
        }

        private void Update()
        {
            if(equipedItem != null && lastItem == null)
            {
                lastItem = equipedItem;
            }

            if(equipedItem == null && lastItem != null)
            {
                weaponManager.UneqipWeapon(lastItem);
                lastItem = null;
            }
        }
    }
}