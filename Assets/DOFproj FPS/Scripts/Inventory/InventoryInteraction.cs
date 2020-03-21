/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.UI;

namespace DOFprojFPS
{
    public class InventoryInteraction : MonoBehaviour
    {
        [HideInInspector]
        public ItemHandler UIItem;
        private DTInventory inventory;
        
        private void Start()
        {
            inventory = FindObjectOfType<DTInventory>();
        }

        public void RemoveItem()
        {
            inventory.DropItem(UIItem);
            this.gameObject.SetActive(false);
        }

        public void Useitem()
        {
            if (UIItem.item.type == ItemType.consumable)
            {
                
                inventory.UseItem(UIItem, false);
            }
            gameObject.SetActive(false);
        }

        public void UnstackItem()
        {
            inventory.SubstractStack(UIItem);
            gameObject.SetActive(false);
        }
    }
}
