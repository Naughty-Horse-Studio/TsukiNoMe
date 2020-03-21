/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using System.Collections.Generic;

namespace DOFprojFPS
{
    public class WeaponManager : MonoBehaviour
    {
        //A public list which get all aviliable weapons on Start() and operate with them
        public List<Weapon> weapons;

        public bool UseNonPhysicalReticle = true;
        public List<EquipmentPanel> equipmentPanel;

        [Tooltip("Scope image used for riffle aiming state")]
        public GameObject scopeImage;

        [Tooltip("Animator that contain pickup animation")]
        public Animator weaponHolderAnimator;

        [HideInInspector]
        public GameObject tempGameobject;
        
        private Transform swayTransform;

        private DTInventory inventory;

        [HideInInspector]
        public Weapon activeWeapon;

        private Weapon weaponToUnhide;

        private SoundManager soundManager;

        public string grenadeItemName = "Grenade";

        private void Awake()
        {
            swayTransform = FindObjectOfType<Sway>().GetComponent<Transform>();
            soundManager = FindObjectOfType<SoundManager>();

            foreach (Weapon weapon in swayTransform.GetComponentsInChildren<Weapon>(true))
            {
                weapons.Add(weapon);
            }

            inventory = FindObjectOfType<DTInventory>();
        }

        private void Update()
        {
            if(!PlayerStats.isPlayerDead)
            SlotInput();

            if(activeWeapon != null && activeWeapon.weaponName == "Grenade" && inventory.CheckIfItemExist("Grenade") == false)
            {
                weaponHolderAnimator.Play("Hide");
                Invoke("HideWeapon", 0.5f);
            }
        }

        public void AutoEquip(Item item)
        {
            if(activeWeapon == null)
            {
                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == item.title)
                    {
                        soundManager.WeaponPicking(false);
                        weapon.currentItem = item;
                        weapon.gameObject.SetActive(true);
                        activeWeapon = weapon;
                        activeWeapon.currentItem = item;
                        weaponHolderAnimator.Play("Unhide");
                    }
                }
            }
        }

        public void MobileSlotInput(int equipmentPanelIndexInPanels)
        {
            if (equipmentPanel != null && !ChangingWeapon)
            {
                        if (activeWeapon != null && activeWeapon.name == equipmentPanel[equipmentPanelIndexInPanels].equipedItem.title)
                        {
                            return;
                        }

                        ChangingWeapon = true;
                        weaponHolderAnimator.Play("Hide");
                        Invoke("HideWeapon", 0.5f);

                        foreach (var weapon in weapons)
                        {
                            if (weapon.weaponName == equipmentPanel[equipmentPanelIndexInPanels].equipedItem.title)
                            {
                                weaponToUnhide = weapon;
                                weaponToUnhide.currentItem = equipmentPanel[equipmentPanelIndexInPanels].equipedItem;
                            }
                        }

                        Invoke("UnhideWeapon", 0.6f);
            }
        }
        
        public void SlotInput()
        {
            if(Input.GetKeyDown(KeyCode.G) && inventory.CheckIfItemExist("Grenade"))
            {
                ChangingWeapon = true;
                weaponHolderAnimator.Play("Hide");
                Invoke("HideWeapon", 0.5f);

                foreach (var weapon in weapons)
                {
                    if (weapon.weaponName == grenadeItemName)
                    {
                        weaponToUnhide = weapon;
                    }
                }

                Invoke("UnhideWeapon", 1f);
            }

            if (equipmentPanel != null && !ChangingWeapon)
            {
                for (int i = 0; i < equipmentPanel.Count; i++)
                {
                    if(Input.GetKeyDown(equipmentPanel[i].activateKey) && equipmentPanel[i].equipedItem != null)
                    {

                        if(activeWeapon != null && activeWeapon.name == equipmentPanel[i].equipedItem.title)
                        {
                            return;
                        }
                        
                            ChangingWeapon = true;
                            weaponHolderAnimator.Play("Hide");
                            Invoke("HideWeapon", 0.5f);
                        
                        foreach (var weapon in weapons)
                        {
                            if(weapon.weaponName == equipmentPanel[i].equipedItem.title)
                            {
                                weaponToUnhide = weapon;
                                weaponToUnhide.currentItem = equipmentPanel[i].equipedItem;
                            }
                        }

                        Invoke("UnhideWeapon", 0.6f);
                    }
                }
            }
        }

        public Weapon GetWeapon(Item item)
        {
            foreach(Weapon weapon in weapons)
            {
                if(weapon.weaponName == item.title)
                {
                    return weapon;
                }
            }
            return null;
        }

        public Weapon GetWeapon(string name)
        {
            foreach(Weapon weapon in weapons)
            {
                if(weapon.weaponName == name)
                {
                    return weapon;
                }
            }
            return null;
        }
        
        public void HideWeaponOnDeath()
        {
            weaponHolderAnimator.SetLayerWeight(1, 0);
            weaponHolderAnimator.Play("Hide");
        }

        public void UnhideWeaponOnRespawn()
        {
            weaponHolderAnimator.SetLayerWeight(1, 1);
            weaponHolderAnimator.Play("Hide");
        }

        public bool ChangingWeapon;

        public void UneqipWeapon(Item item)
        {
            foreach(var weapon in weapons)
            {
                if (item == null)
                    print("Unequip weapon: items you try to unequip is null");

                if(item.title == weapon.weaponName && weapon.gameObject.activeInHierarchy)
                {
                    weaponHolderAnimator.Play("Hide");
                    Invoke("HideWeapon", 0.5f);
                }
            }
        }

        public void HideWeapon()
        {
            if (activeWeapon != null)
            {
                activeWeapon.gameObject.SetActive(false);
                activeWeapon = null;
                soundManager.WeaponPicking(true);
            }
        }

        public void UnhideWeapon()
        {
            if (weaponToUnhide != null)
            {
                weaponToUnhide.gameObject.SetActive(true);
                activeWeapon = weaponToUnhide;
            }
            soundManager.WeaponPicking(false);
            weaponHolderAnimator.Play("Unhide");
            weaponToUnhide = null;
            ChangingWeapon = false;
        }

        public void HideAll()
        {
            print("Hide weapon works");

            if (activeWeapon != null)
            {
                activeWeapon.gameObject.SetActive(false);

                weaponHolderAnimator.Play("Hide");
            }
        }
    }
}
