/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.Events;

namespace DOFprojFPS
{
    public class InventoryManager : MonoBehaviour
    {
        [System.Serializable]
        public class OnInventoryOpen : UnityEvent { }
        [System.Serializable]
        public class OnInventoryClose : UnityEvent { }

        Canvas canvas;
        FPSController controller;
        InputManager input;

        public static bool showInventory = false;
        public bool isOpen = true;

        public OnInventoryOpen OnOpen;
        public OnInventoryClose OnClose;
        
        public enum ActiveMode { craft, inventory }

        public ActiveMode mode = ActiveMode.inventory;

        public GameObject inventoryPanel;
        public GameObject craftPanel;

        public Animator panel;

        public Tooltip infoPanel;

        private void OnEnable()
        {
            canvas = GetComponent<Canvas>();
            controller = FindObjectOfType<FPSController>();
            input = FindObjectOfType<InputManager>();

            InventoryClose();
        }

        private void Update()
        {
            if (Input.GetKeyDown(input.Inventory) && !PlayerStats.isPlayerDead && !InputManager.useMobileInput)
            {
                showInventory = !showInventory;
            }

            if (showInventory)
            {
                InventoryOpen();
            }
            else
            {
                InventoryClose();
            }

            if (mode == ActiveMode.inventory)
            {
                if (inventoryPanel && craftPanel)
                {
                    inventoryPanel.SetActive(true);
                    craftPanel.SetActive(false);
                }
            }
            else if (mode == ActiveMode.craft)
            {
                if (inventoryPanel && craftPanel)
                {
                    inventoryPanel.SetActive(false);
                    craftPanel.SetActive(true);
                }
            }
        }
    
        public void SetInventory()
        {
            mode = ActiveMode.inventory;
        }

        public void SetCraft()
        {
            mode = ActiveMode.craft;
        }

        private void InventoryOpen()
        {
            if (isOpen)
                return;
            else
            {
                canvas.enabled = true;
                controller.lockCursor = false;
                //blurEffect.enabled = true;
                OnOpen.Invoke();
                isOpen = true;

            }
        }
        private void InventoryClose()
        {
            if (!isOpen)
                return;
            else
            {
                canvas.enabled = false;
                controller.lockCursor = true;
                //blurEffect.enabled = false;
                OnClose.Invoke();
                isOpen = false;
            }
        }

        public void MobileToggle()
        {
            showInventory = !showInventory;
        }
    }
}