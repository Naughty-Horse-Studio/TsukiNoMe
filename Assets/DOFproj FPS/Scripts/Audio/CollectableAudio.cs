using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DOFprojFPS;

public class CollectableAudio : CollectableItem
{
    // Inpsector Assigned Variables
    [SerializeField] protected Renderer     _screenRenderer = null;         
    [SerializeField] protected Color        _emissiveColor  = Color.gray;

    // Internal
    protected InventoryItemAudio _audioItem = null;

    // ---------------------------------------------------------------------------------------------
    // Name :   Start
    // Desc :   Caches a reference to the associated InventoryAudioItem and sets the texture
    //          of that audio item.
    // ---------------------------------------------------------------------------------------------
    protected override void Start()
    {
        // Call Base Class to register as an interactive item
        base.Start();

        // Cache Inventory Item as correct type
        _audioItem = _inventoryItem as InventoryItemAudio;

        // If we have an audio item reference
        if (_audioItem)
        {
            if (_screenRenderer)
            {
                _screenRenderer.material.SetTexture("_EmissionMap", _audioItem.image);
                _screenRenderer.material.SetColor("_EmissionColor", _emissiveColor);
            }
        }
    }

    // --------------------------------------------------------------------------------------------
    // Name :   GetText (Override)
    // Desc :   Returns the Interactive Text for this Ammo
    // --------------------------------------------------------------------------------------------
    public override string GetText()
    {
        // If text not generated yet
        if (_interactiveText == null)
        {
          if (!_audioItem)
             _interactiveText = "Audio Log: Empty";
          else
             _interactiveText = "Audio Log: " + _audioItem.person + "\n"+_audioItem.subject + "\n" + _audioItem.pickupText;
           
        }

        // Return text
        return _interactiveText;
    }

    // -----------------------------------------------------------------------------
    // Name	:	Activate
    // Desc	:	Overrides the base class functionality that would ordinarilty remove
    //			a collectable item from the scene. In the case of PDAs, we essentially
    //			download the data (remove disk) leaving the PDA in the scene but no
    //			longer functional
    // ------------------------------------------------------------------------------
    public override void Activate(UseObjects useObjects)
    {
        // This is an empty PDA so nothing to take
        if (!_audioItem) return;

        // We need a valid character manager and inventory manager
        if (_inventory != null)
        {
            // Add this item to the inventory
            if (_inventory.AddItem(this, true))
            {
                // Set Empty Text
                _interactiveText = "Audio Log: Empty";

                // Remove Data
                _inventoryItem = _audioItem = null;

                // Disable screen Texture
                if (_screenRenderer)
                {
                    _screenRenderer.material.SetTexture("_EmissionMap", null);
                    _screenRenderer.material.SetColor("_EmissionColor", Color.black);
                }
            }
        }
    }
}
