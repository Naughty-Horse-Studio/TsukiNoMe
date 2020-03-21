/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DOFprojFPS;

public class ItemHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector]
    public int x, y, width, height;
    
    Image image;

    public Item item;

    RectTransform m_rect;
    
    internal DTInventory inventory;

    PointerEventData dragEventData;
    bool drag;

    Vector2 lastPosition;
    [HideInInspector]
    public Vector2 finalPosition;
    private Text stackText;

    private void OnEnable()
    {
        if (m_rect == null) m_rect = GetComponent<RectTransform>();
        if (inventory == null) inventory = FindObjectOfType<DTInventory>();
        if (image == null) image = GetComponent<Image>();
        if (stackText == null) stackText = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (drag)
        {
            m_rect.pivot = new Vector2(0.5f, 0.5f);//(Mathf.Lerp(m_rect.pivot.x, 0.5f, Time.deltaTime * 30), Mathf.Lerp(m_rect.pivot.y, 0.5f, Time.deltaTime * 30));
            m_rect.position = new Vector2(dragEventData.position.x, dragEventData.position.y) ;//Vector2.Lerp(m_rect.position, dragEventData.position, Time.deltaTime * 30);
        }
        else
        {
            m_rect.pivot = new Vector2(Mathf.Lerp(m_rect.pivot.x, 0f, Time.deltaTime * 10), Mathf.Lerp(m_rect.pivot.y, 1f, Time.deltaTime * 10));
            m_rect.anchoredPosition = Vector2.Lerp(m_rect.anchoredPosition, finalPosition, Time.deltaTime * 10);
        }

        if(item.stackable)
        {
            stackText.text = item.stackSize.ToString();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!InputManager.useMobileInput)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventory.inventoryInteractionPanel.gameObject.SetActive(true);
                inventory.inventoryInteractionPanel.GetComponent<RectTransform>().position = eventData.position;
                inventory.inventoryInteractionPanel.UIItem = this;
            }
            else
            {
                inventory.inventoryInteractionPanel.gameObject.SetActive(false);
            }
        }
        else
        {
            inventory.inventoryInteractionPanel.gameObject.SetActive(true);
            inventory.inventoryInteractionPanel.GetComponent<RectTransform>().position = eventData.position;
            inventory.inventoryInteractionPanel.UIItem = this;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inventory.tooltip.gameObject.SetActive(false);

        if (inventory.inventoryInteractionPanel.isActiveAndEnabled)
        {
            inventory.inventoryInteractionPanel.gameObject.SetActive(false);
        }

        transform.SetParent(inventory.transform);

        image.raycastTarget = false;
        lastPosition = m_rect.anchoredPosition;

        var slot = inventory.FindSlotByIndex(x, y);

        if (slot.equipmentPanel != null)
        {
            if(slot.equipmentPanel.equipedItem != null)
                slot.equipmentPanel.equipedItem = null;
        }

        inventory.MarkSlots(x, y, width, height, true);

        inventory.DrawColorsForHoveredSlots(x, y, width, height);

        /*
        foreach(var handler in inventory.GetComponentsInChildren<ItemHandler>())
        {
            handler.image.raycastTarget = false;
        }*/

        //To render our image above other UI elements, we put it transform to the end of our transforms list
        transform.SetAsLastSibling();
    }

    GridSlot hoveredSlot;

    public void OnDrag(PointerEventData eventData)
    {
        dragEventData = eventData;
        drag = true;
        
        var hoveredGameObject = eventData.pointerCurrentRaycast.gameObject;
        if (hoveredGameObject != null)
        {
            if (hoveredGameObject.GetComponent<GridSlot>() != null && hoveredSlot != hoveredGameObject.GetComponent<GridSlot>())
            {
                hoveredSlot = hoveredGameObject.GetComponent<GridSlot>();
            }
            else if (hoveredGameObject.GetComponent<GridSlot>() != null && hoveredSlot == hoveredGameObject.GetComponent<GridSlot>() && hoveredGameObject.GetComponent<GridSlot>().equipmentPanel != null)
            {
                inventory.DrawRegularSlotsColors();
                inventory.DrawColorsForHoveredSlots(hoveredSlot.x, hoveredSlot.y, width, height, item, hoveredGameObject.GetComponent<GridSlot>().equipmentPanel.allowedItemType);
            }
            else if (hoveredGameObject.GetComponent<GridSlot>() != null && hoveredSlot == hoveredGameObject.GetComponent<GridSlot>() && hoveredGameObject.GetComponent<GridSlot>().equipmentPanel == null)
            {
                inventory.DrawRegularSlotsColors();
                inventory.DrawColorsForHoveredSlots(hoveredSlot.x, hoveredSlot.y, width, height);
            }
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;

        foreach (var handler in inventory.GetComponentsInChildren<ItemHandler>())
        {
            handler.image.raycastTarget = true;
        }

        image.raycastTarget = true;

        inventory.DrawRegularSlotsColors();
        
        var eventDataRaycast = eventData.pointerCurrentRaycast;
        
        if(item.stackable == true 
            && eventDataRaycast.gameObject.GetComponent<ItemHandler>()!=null 
            && eventDataRaycast.gameObject.GetComponent<ItemHandler>().item.id == item.id 
            && eventDataRaycast.gameObject.GetComponent<ItemHandler>().item.stackSize + item.stackSize <= item.maxStackSize)
        {
            eventDataRaycast.gameObject.GetComponent<ItemHandler>().item.stackSize += item.stackSize;
            inventory.RemoveItem(this);
            Destroy(gameObject);
            return;
        }

        
        if(eventDataRaycast.gameObject == null && inventory.dragOutsideToDrop)
        {
            inventory.DropItem(this);
        }

        else
        if (eventDataRaycast.gameObject != null && eventDataRaycast.gameObject.GetComponent<GridSlot>())
        {
            var slot = eventData.pointerCurrentRaycast.gameObject.GetComponent<GridSlot>();
         
            if (inventory.CheckFreeSpaceAtSlot(slot.x, slot.y, width, height) && slot.free && slot.equipmentPanel == null)
            {
                transform.SetParent(slot.transform.parent);

                finalPosition = inventory.FindSlotByIndex(slot.x, slot.y).GetComponent<RectTransform>().anchoredPosition;

                x = slot.x;
                y = slot.y;
                
                inventory.MarkSlots(slot.x, slot.y, width, height, false);
            }
            else if(inventory.CheckFreeSpaceAtSlot(slot.x, slot.y, width, height) && slot.free && slot.equipmentPanel != null && slot.equipmentPanel.allowedItemType == item.type)
            {
                transform.SetParent(slot.transform.parent);

                finalPosition = inventory.FindSlotByIndex(slot.x, slot.y).GetComponent<RectTransform>().anchoredPosition;

                x = slot.x;
                y = slot.y;

                slot.equipmentPanel.equipedItem = item;

                inventory.MarkSlots(slot.x, slot.y, width, height, false);
            }
            else
            {
                finalPosition = lastPosition;
                inventory.MarkSlots(x, y, width, height, false);
            }
        }
        else
        { 
            finalPosition = lastPosition;
            inventory.MarkSlots(x, y, width, height, false);
        }
        
        //image.raycastTarget = true;
    }
    
    /*
    public Vector2 CalculateShiftedAxes()
    {
        int mod_x = 0, mod_y = 0;

        if (width == 1) mod_x = 1;
        if (width / width == 0) mod_x = hoveredSlot.x - (width / 2);
        else if (width / width != 0) mod_x = hoveredSlot.x - ((width - 1) / 2);

        if (height == 1) mod_y = 1;
        if (height / height == 0) mod_y = hoveredSlot.y - (height / 2);
        else if (height / height != 0) mod_y = hoveredSlot.y - ((height - 1) / 2);

        return new Vector2(mod_x, mod_y);
    }*/

        private float timer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventory.tooltip.GenerateContent(item);
        Invoke("ShowTooltip", 0.5f);
    }

    public void ShowTooltip()
    {
        inventory.tooltip.UpdatePos();
        inventory.tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke("ShowTooltip");
        inventory.tooltip.gameObject.SetActive(false);
    }
}
