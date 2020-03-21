/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DOFprojFPS
{
    public class DTInventory : MonoBehaviour
    {
        GridLayoutGroup grid;

        public Image cell;

        public int cellSize = 70;
        public int padding = 4;
        
        [Header("Inventory size")]
        public int column = 5;
        public int row = 4;

        [Header("Loot grid size")]
        public int l_column = 4;
        public int l_row = 4;

        //List for slots functionality
        public List<GridSlot> slots;

        public GameObject UIItemPrefab;
        
        public List<ItemHandler> characterItems = new List<ItemHandler>();

        public Color normalCellColor;
        public Color hoveredCellColor;
        public Color blockedCellColor;

        public EquipmentPanel[] equipmentPanels;

        public RectTransform lootPanel;

        public InventoryInteraction inventoryInteractionPanel;
        
        [HideInInspector]
        public List<ItemHandler> ammoItems;

        [HideInInspector]
        public Tooltip tooltip;

        public bool dragOutsideToDrop;

        private Transform player;
        private WeaponManager weaponManager;

        public bool autoEquipItems = true;

        void Awake()
        {
            inventoryInteractionPanel = FindObjectOfType<InventoryInteraction>();
            weaponManager = FindObjectOfType<WeaponManager>();
            player = GameObject.Find("Player").GetComponent<Transform>();

            Initialize();
        }

        private void Start()
        {
            tooltip = FindObjectOfType<Tooltip>();
            tooltip.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (characterItems != null)
            {
                ammoItems = new List<ItemHandler>();

                foreach (var UIItem in characterItems)
                {
                    if(UIItem != null && UIItem.item.type == ItemType.ammo)
                        ammoItems.Add(UIItem);
                }
            }
        }

        public void Initialize()
        {
            if(GetComponentsInChildren<GridSlot>() != null)
            {
                foreach(var previewSlot in GetComponentsInChildren<GridSlot>())
                {
                    DestroyImmediate(previewSlot);
                }
            }

            foreach(var equipPanel in equipmentPanels)
            {
                if(equipPanel.GetComponentsInChildren<GridSlot>() != null)
                {
                    foreach(var previewSlot in GetComponentsInChildren<GridSlot>())
                    {
                        DestroyImmediate(previewSlot);
                    }
                }
            }

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    var _cell = Instantiate(cell);
                    _cell.rectTransform.SetParent(transform);
                    _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                    _cell.rectTransform.anchoredPosition = new Vector2((cellSize * i) + padding, (-cellSize * j) + padding);
                    _cell.rectTransform.localScale = new Vector2(1, 1);

                    _cell.name = i + "," + j;

                    var slot = _cell.GetComponent<GridSlot>();
                    slot.free = true;
                    slot.image.color = normalCellColor;
                    slot.x = i; slot.y = j;
                    slots.Add(slot);
                }
            }

            if(lootPanel != null)
            {
                for (int i = 0; i < l_row; i++)
                {
                    for (int j = 0; j < l_column; j++)
                    {
                        var _cell = Instantiate(cell);
                        _cell.rectTransform.SetParent(lootPanel);
                        _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                        _cell.rectTransform.anchoredPosition = new Vector2((cellSize * i) + padding, (-cellSize * j) + padding);
                        _cell.rectTransform.localScale = new Vector2(1, 1);

                        _cell.name = i + "," + j;

                        var slot = _cell.GetComponent<GridSlot>();
                        slot.free = true;
                        slot.image.color = normalCellColor;
                        slot.x = i + 9000; slot.y = j + 9000;
                        slots.Add(slot);
                    }
                }
            }

            if (equipmentPanels != null)
            {
                for (int k = 0; k < equipmentPanels.Length; k++)
                {
                    for (int i = 0; i < equipmentPanels[k].width; i++)
                    {
                        for (int j = 0; j < equipmentPanels[k].height; j++)
                        {

                            var _cell = Instantiate(cell);
                            _cell.rectTransform.SetParent(equipmentPanels[k].transform);
                            _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                            _cell.rectTransform.anchoredPosition = new Vector2((cellSize * i) + padding, (-cellSize * j) + padding);
                            _cell.rectTransform.localScale = new Vector2(1, 1);

                            _cell.name = i + (k + 1) * 100 + "," + j + (k + 1) * 100;

                            var slot = _cell.GetComponent<GridSlot>();
                            slot.free = true;
                            slot.image.color = normalCellColor;
                            slot.equipmentPanel = equipmentPanels[k];
                            slot.x = i + (k + 1) * 100; slot.y = j + (k + 1) * 100;
                            slots.Add(slot);

                            if (i == 0 && j == 0)
                            {
                                equipmentPanels[k].mainSlot = slot;
                            }
                        }
                    }
                }
            }

            /*
            foreach(var slot in slots)
            {
                slot.transform.SetParent(transform);
            }*/

            transform.SetAsLastSibling();
        }

        [ContextMenu("Draw preview slots")]
        public void DrawPreview()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    var _cell = Instantiate(cell);
                    _cell.rectTransform.SetParent(transform);
                    _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                    _cell.rectTransform.anchoredPosition = new Vector2((cellSize * i) + padding, (-cellSize * j) + padding);

                    _cell.name = i + "," + j;

                    _cell.GetComponent<Image>().color = normalCellColor;
                }
            }

            if (equipmentPanels != null)
            {
                for (int k = 0; k < equipmentPanels.Length; k++)
                {
                    for (int i = 0; i < equipmentPanels[k].width; i++)
                    {
                        for (int j = 0; j < equipmentPanels[k].height; j++)
                        {
                            var _cell = Instantiate(cell);
                            _cell.rectTransform.SetParent(equipmentPanels[k].transform);
                            _cell.rectTransform.sizeDelta = new Vector2(cellSize - padding, cellSize - padding);
                            _cell.rectTransform.anchoredPosition = new Vector2((cellSize * i) + padding, (-cellSize * j) + padding);

                            _cell.name = i + (k + 1) * 100 + "," + j + (k + 1) * 100;

                            _cell.GetComponent<Image>().color = normalCellColor;
                        }
                    }
                }
            }
        }

        [ContextMenu("Clear preview slots")]
        public void ClearPreview()
        {
            if (GetComponentInChildren<GridSlot>() != null)
            {
                foreach (var previewSlot in GetComponentsInChildren<GridSlot>())
                {
                    DestroyImmediate(previewSlot);
                }
            }

            foreach (var equipPanel in equipmentPanels)
            {
                if (equipPanel.GetComponentInChildren<GridSlot>() != null)
                {
                    foreach (var previewSlot in GetComponentsInChildren<GridSlot>())
                    {
                        DestroyImmediate(previewSlot);
                    }
                }
            }
        }

        public bool AddItem(Item item)
        {
            if (CheckFreeSpaceForAllSlots(item.width, item.height))
            {
                var slot = CheckFreeSpaceForAllSlots(item.width, item.height);
                var item_test = Instantiate(cell);
                var item_handler = item_test.gameObject.AddComponent<ItemHandler>();
                
                item_test.GetComponent<Image>().rectTransform.SetParent(transform);
                item_test.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(cellSize * item.width - padding, cellSize * item.height - padding);
                item_test.GetComponent<Image>().rectTransform.anchoredPosition = slot.GetComponent<RectTransform>().anchoredPosition;

                item_handler.item = item;

                item_handler.finalPosition = slot.GetComponent<RectTransform>().anchoredPosition;
                item_handler.GetComponent<Image>().sprite = item.icon;
                item_handler.x = slot.x;
                item_handler.y = slot.y;
                item_handler.width = item.width;
                item_handler.height = item.height;
                item_handler.inventory = this;

                MarkSlots(slot.x, slot.y, item.width, item.height, false);

                Destroy(item_test.GetComponent<GridSlot>());

                characterItems.Add(item_handler);

                /*
                if(item.haveScopeAddon)
                {
                    CreateImageForAddon(item_handler, item.scopeImage, item.scopeImagePosition, item.scopeImageSize);
                }*/
                

                item.gameObject.SetActive(false);

                if (autoEquipItems)
                {
                    foreach (var equipmentPanel in equipmentPanels)
                    {
                        if (equipmentPanel.allowedItemType == item_handler.item.type && equipmentPanel.equipedItem == null)
                        {
                            EquipItem(equipmentPanel, item_handler);
                            MarkSlots(slot.x, slot.y, item.width, item.height, true);
                        }
                    }
                }


                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to add item to special grid slot. Primarly used for Save/Load to restore items order after load
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="x">X grid position</param>
        /// <param name="y">Y grid position</param>
        /// <returns></returns>
        public bool AddItem(Item item, int x, int y)
        {
            if (CheckFreeSpaceForAllSlots(item.width, item.height))
            {
                var item_test = Instantiate(cell);
                var item_handler = item_test.gameObject.AddComponent<ItemHandler>();
                
                item_test.GetComponent<Image>().rectTransform.SetParent(FindSlotByIndex(x, y).GetComponent<RectTransform>().transform.parent);
                item_test.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(cellSize * item.width - padding, cellSize * item.height - padding);
                item_test.GetComponent<Image>().rectTransform.anchoredPosition = FindSlotByIndex(x, y).GetComponent<RectTransform>().anchoredPosition;

                item_handler.item = item;

                item_handler.GetComponent<Image>().sprite = item.icon;
                item_handler.x = x;
                item_handler.y = y;
                item_handler.width = item.width;
                item_handler.height = item.height;
                item_handler.inventory = this;

                item_handler.finalPosition = FindSlotByIndex(x, y).GetComponent<RectTransform>().anchoredPosition;

                MarkSlots(x, y, item.width, item.height, false);

                Destroy(item_test.GetComponent<GridSlot>());

                characterItems.Add(item_handler);

                /*
                if (item.haveScopeAddon)
                {
                    CreateImageForAddon(item_handler, item.scopeImage, item.scopeImagePosition, item.scopeImageSize);
                }*/

                print("Item added: " + item.title);

                foreach (var panel in equipmentPanels)
                {
                    if(item_handler.x == panel.mainSlot.x && item_handler.y == panel.mainSlot.y)
                        panel.equipedItem = item_handler.item;
                }

                item.gameObject.SetActive(false);

                if (autoEquipItems)
                {
                    foreach (var equipmentPanel in equipmentPanels)
                    {
                        if (equipmentPanel.allowedItemType == item_handler.item.type && equipmentPanel.equipedItem == null)
                        {
                            EquipItem(equipmentPanel, item_handler);
                            MarkSlots(x, y, item.width, item.height, true);
                        }
                    }
                }

                return true;
            }
            return false;
        }

        public bool CheckIfItemExist(string itemName)
        {
            foreach(var item in characterItems)
            {
                if(item.item.title == itemName)
                {
                    return true;
                }
            }

            return false;
        }

        public void CreateImageForAddon(ItemHandler itemHandler, Sprite icon, Vector2 position, Vector2 size)
        {
            var _image = Instantiate(new GameObject());
            var image_rect = _image.AddComponent<Image>();
            image_rect.raycastTarget = false;
            image_rect.sprite = icon;
            image_rect.transform.parent = itemHandler.transform;
            image_rect.rectTransform.sizeDelta = size;
            image_rect.rectTransform.anchoredPosition = position;
        }

        public void DropItem(ItemHandler UIItem)
        {
            int temp_x, temp_y, temp_width, temp_height;

            temp_x = UIItem.x;
            temp_y = UIItem.y;
            temp_width = UIItem.width;
            temp_height = UIItem.height;

            if (UIItem == null)
                print("UIItem is null");

            if (UIItem.item == null)
                print("Item is null");

            UIItem.item.gameObject.SetActive(true);
            UIItem.item.transform.position = Camera.main.transform.position + Camera.main.transform.forward;

            if (equipmentPanels != null)
            {
                foreach (var panel in equipmentPanels)
                {
                    if (panel.equipedItem == UIItem.item)
                    {
                        print("Clear equipment slot : Found item is "+UIItem.item);
                        panel.lastItem = null;
                        panel.equipedItem = null;
                    }

                }
            }
            if (UIItem.item.type == ItemType.weaponPrimary || UIItem.item.type == ItemType.weaponSecondary || UIItem.item.type == ItemType.melee)
            {
                print("Bring item to unequip : " + UIItem.item);
                weaponManager.UneqipWeapon(UIItem.item);
            }


            UIItem.item.gameObject.transform.parent = null;

            Destroy(UIItem.gameObject);


            MarkSlots(temp_x, temp_y, temp_width, temp_height, true);
        }

        // This method is for auto-equip event only. Don't use it anywhere
        public void EquipItem(EquipmentPanel panel, ItemHandler item)
        {
            item.transform.SetParent(panel.mainSlot.transform.parent);


            item.x = panel.mainSlot.x;
            item.y = panel.mainSlot.y;

            MarkSlots(panel.mainSlot.x, panel.mainSlot.y, item.width, item.height, false);
            panel.equipedItem = item.item;
            item.finalPosition = panel.mainSlot.GetComponent<RectTransform>().anchoredPosition;
            weaponManager.AutoEquip(item.item);
        }

        public void RemoveItem(ItemHandler UIItem)
        {
            int temp_x, temp_y, temp_width, temp_height;

            temp_x = UIItem.x;
            temp_y = UIItem.y;
            temp_width = UIItem.width;
            temp_height = UIItem.height;

            characterItems.Remove(UIItem);

            if (ammoItems.Contains(UIItem))
                ammoItems.Remove(UIItem);

            Destroy(UIItem.gameObject);
            Destroy(UIItem.item.gameObject);

            MarkSlots(temp_x, temp_y, temp_width, temp_height, true);
        }
        

        public void MarkSlots(int startSlot_x, int startSlot_y, int width, int height, bool isFree)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    if (FindSlotByIndex(i, j))
                    {
                        var slot = FindSlotByIndex(i, j);
                        slot.free = isFree;

                        if (slot.free) slot.image.color = normalCellColor; else slot.image.color = hoveredCellColor;
                    }
                }
            }
        }

        public GridSlot CheckFreeSpaceAtSlot(int cell_x, int cell_y, int width, int height)
        {
            for (int i = cell_x; i < cell_x + width; i++)
            {
                for (int j = cell_y; j < cell_y + height; j++)
                {
                    if (FindSlotByIndex(i, j) == null || FindSlotByIndex(i, j).free == false)
                    {
                        return null;
                    }
                }
            }

            return FindSlotByIndex(cell_x, cell_y);
        }

        public void DrawRegularSlotsColors()
        {
            foreach (var slot in slots)
            {
                if (slot.free)
                {
                    slot.image.color = normalCellColor;
                }
                else
                {
                    slot.image.color = hoveredCellColor;
                }
            }
        }

        public void DrawColorsForHoveredSlots(int startSlot_x, int startSlot_y, int width, int height)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    var slot = FindSlotByIndex(i, j);

                    if (slot != null)
                    {
                        if (slot.free)
                            slot.image.color = hoveredCellColor;
                        else
                            slot.image.color = blockedCellColor;
                    }
                }
            }
        }

        public void DrawColorsForHoveredSlots(int startSlot_x, int startSlot_y, int width, int height, Item item, ItemType itemType)
        {
            for (int i = startSlot_x; i < startSlot_x + width; i++)
            {
                for (int j = startSlot_y; j < startSlot_y + height; j++)
                {
                    var slot = FindSlotByIndex(i, j);

                    if (slot != null)
                    {
                        if (slot.free && item.type == itemType)
                            slot.image.color = hoveredCellColor;
                        else
                            slot.image.color = blockedCellColor;
                    }
                }
            }
        }

        public GridSlot CheckFreeSpaceForAllSlots(int width, int height)
        {
            foreach (var slot in slots)
            {
                if (CheckFreeSpaceAtSlot(slot.x, slot.y, width, height) && slot.equipmentPanel == null)
                    return CheckFreeSpaceAtSlot(slot.x, slot.y, width, height);
            }

            return null;
        }

        public GridSlot FindSlotByIndex(int x, int y)
        {
            foreach (var slot in slots)
            {
                if (slot.x == x && slot.y == y)
                    return slot;
            }

            return null;
        }

        public List<ItemHandler> SearchItemWithCount(GameObject itemToFind, int itemsValue)
        {
            List<ItemHandler> items = new List<ItemHandler>();

            foreach (var i in characterItems)
            {
                if (i.item.id == itemToFind.GetComponent<Item>().id)
                    items.Add(i);

                if (items.Count == itemsValue)
                    return items;
            }

            return null;
        }

        public List<ItemHandler> SearchItemsForBuilding(GameObject[] itemsToFind, int[] itemsValue)
        {
            int status = 0;

            for (int i = 0; i < itemsValue.Length; i++)
            {
                if (SearchItemWithCount(itemsToFind[i], itemsValue[i]) != null)
                    status++;
            }

            print(status + " : " + itemsToFind.Length);

            if (status == itemsToFind.Length)
            {
                List<ItemHandler> items = new List<ItemHandler>();

                for (int i = 0; i < itemsValue.Length; i++)
                {
                    items.AddRange(SearchItemWithCount(itemsToFind[i], itemsValue[i]));
                }

                return items;
            }
            else
                return null;
        }

        public void SubstractStack(ItemHandler UIitem)
        {
            // If check free space == false -> exit
            if (UIitem.item.stackSize < 2)
                return;
            
            if (UIitem.item.stackable)
            {
                if (UIitem.item.stackSize % 2 == 0)
                {
                    if(CheckFreeSpaceForAllSlots(UIitem.width, UIitem.height) == null)
                    {
                        return;
                    }

                    UIitem.item.gameObject.SetActive(true);

                    var second_item_go = Instantiate(UIitem.item.gameObject);
                    var second_item = second_item_go.GetComponent<Item>();

                    UIitem.item.stackSize /= 2;
                    second_item.stackSize = UIitem.item.stackSize;

                    AddItem(second_item);

                    second_item.name = UIitem.item.name;

                    UIitem.item.gameObject.SetActive(false);
                }
                else if (UIitem.item.stackSize % 2 == 1)
                {
                    if (CheckFreeSpaceForAllSlots(UIitem.width, UIitem.height) == null)
                    {
                        return;
                    }

                    UIitem.item.gameObject.SetActive(true);

                    var second_item_go = Instantiate(UIitem.item.gameObject);
                    var second_item = second_item_go.GetComponent<Item>();

                    UIitem.item.stackSize = (UIitem.item.stackSize - 1) / 2;
                    second_item.stackSize = UIitem.item.stackSize + 1;
                    
                    AddItem(second_item);

                    second_item.name = UIitem.item.name;

                    UIitem.item.gameObject.SetActive(false);
                }
            }
            else
            {
                return;
            }

        }

        public void AutoStack(ItemHandler UIItem)
        {
            List<ItemHandler> items = new List<ItemHandler>();

            foreach (var i in characterItems)
            {
                if (i.item.id == UIItem.item.id)
                {
                    items.Add(i);
                }
            }

            foreach (var i in items)
            {
                if (UIItem.item.stackSize + i.item.stackSize <= UIItem.item.maxStackSize)
                {
                    UIItem.item.stackSize += UIItem.item.stackSize;
                    RemoveItem(i);
                }

                if (UIItem.item.stackSize == UIItem.item.maxStackSize)
                    break;
            }
        }

        public void UseItem(ItemHandler UIItem, bool closeInventory)
        {
            if (UIItem.item.type == ItemType.consumable)
            {
                // If not stackable
                if (!UIItem.item.stackable || UIItem.item.stackSize <= 1)
                {
                    UIItem.item.onUseEvent.Invoke();
                    RemoveItem(UIItem);
                }
                // If stackable
                else
                {
                    UIItem.item.onUseEvent.Invoke();
                    UIItem.item.stackSize -= 1;
                }
            }

            if (closeInventory)
                InventoryManager.showInventory = false;
        }

        public void UseGrenade()
        {
            ItemHandler UIItem = null;

            foreach(var item in characterItems)
            {
                if(item.item.title == "Grenade")
                {
                    UIItem = item;
                    break;
                }
            }

            if (UIItem == null)
                return;

                // If not stackable
                if (!UIItem.item.stackable || UIItem.item.stackSize <= 1)
                {
                    UIItem.item.onUseEvent.Invoke();
                    RemoveItem(UIItem);
                }
                // If stackable
                else
                {
                    UIItem.item.onUseEvent.Invoke();
                    UIItem.item.stackSize -= 1;
                }
            }
            
        }
    }

