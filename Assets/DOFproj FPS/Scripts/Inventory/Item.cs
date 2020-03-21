/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType {none, weaponPrimary, weaponSecondary, melee, ammo, consumable }

namespace DOFprojFPS
{

    public class Item : MonoBehaviour
    {
        [System.Serializable]
        public class OnUseEvent : UnityEvent { }
        [System.Serializable]
        public class OnPickupEvent : UnityEvent { }

        public int id;
        public string title;
        public string description;
        public ItemType type;
        public Sprite icon;

        [Range(1, 10)]
        public int width = 1, height = 1;

        [Header("Stack options")]
        public bool stackable;

        [Range(1, 100)]
        public int maxStackSize = 1;

        [Range(1, 100)]
        public int stackSize = 1;

        [Tooltip("It must be ammo for this weapon")]
        public GameObject weaponUnloadingItem;

        public int weaponAmmoCount;

        /*
        public bool haveScopeAddon;
        public Sprite scopeImage;
        public Vector2 scopeImageSize;
        public Vector2 scopeImagePosition;

        public bool haveSilencerAddon;
        public Sprite silencerImage;
        public Vector2 silencerImageSize;
        public Vector2 silencerImagePosition;*/

        [SerializeField]
        public OnUseEvent onUseEvent;
        [SerializeField]
        public OnPickupEvent onPickupEvent;

        public Item(int id, string title, string description, ItemType itemType)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.type = itemType;
        }

        public Item(Item item)
        {
            this.id = item.id;
            this.title = item.title;
            this.description = item.description;
            this.type = item.type;
        }
    }
}
