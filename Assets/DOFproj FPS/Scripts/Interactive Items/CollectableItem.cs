using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;
public class CollectableItem : InteractiveItem
{
    // Inspector Assigned
    [SerializeField]
    protected Inventory _inventory = null;

    [Header("Collectable Item Properties")]
    [SerializeField]
    protected InventoryItem _inventoryItem = null;

    protected InventoryItemAudio _inventoryItemAudio = null;
    protected int _index = -1;

    // Properties
    public Inventory inventory
    {
        get { return _inventory; }
        set { _inventory = value; }
    }

    public InventoryItem inventoryItem
    {
        get { return _inventoryItem; }
    }

    // Internals
    protected string _interactiveText = null;

    // --------------------------------------------------------------------------------------------
    // Name :   GetText (Override)
    // Desc :   Returns the Interactive Text for the current item.
    // --------------------------------------------------------------------------------------------
    public override string GetText()
    {
        // If we have created the interactive text yet then do it
        if (_interactiveText == null)
        {
            if (_inventoryItem != null)
                _interactiveText = _inventoryItem.inventoryName + "\n" + _inventoryItem.pickupText;
            else
            {
                _interactiveText = "ERROR: No InventoryItem assigned to " + name;
            }
        }

        return _interactiveText;
    }

    // --------------------------------------------------------------------------------------------
    // Name :   Activate
    // Desc :   The base functionality for Activating a CollectableItem. It adds the item to the
    //          references IInventory
    // --------------------------------------------------------------------------------------------
    public override void Activate(UseObjects useObjects)
    {
     
        if (_inventory != null)
        {
            if (_inventory.AddItem(this))
            {
                // Destroy(gameObject);
             
            }
        }
    }

 

}
