using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class InventoryMountInfo
{
}

[System.Serializable]
public class InventoryWeaponMountInfo : InventoryMountInfo
{
  //  public InventoryItemWeapon Weapon = null;
    [Range(0.0f, 100.0f)]
    public float Condition = 100.0f;
    [Range(0, 100)]
    public int InGunRounds = 0;
}

[System.Serializable]
public class InventoryAmmoMountInfo : InventoryMountInfo
{
 //   public InventoryItemAmmo Ammo = null;
    public int Rounds = 0;
}

[System.Serializable]
public class InventoryBackpackMountInfo : InventoryMountInfo
{
    public InventoryItem Item = null;
}

// Special Unity Events for broadcasting a weapon change event
[System.Serializable]
public class InventoryWeaponChangeEvent : UnityEvent<InventoryWeaponMountInfo> { }
[System.Serializable]
//public class InventoryWeaponDropEvent : UnityEvent<InventoryItemWeapon> { }

// ------------------------------------------------------------------------------------------------
// Class    :   Inventory
// Desc     :   Base class for implementations of Inventories using this system
// ------------------------------------------------------------------------------------------------
public abstract class Inventory : ScriptableObject
{
    public InventoryWeaponChangeEvent OnWeaponChange  = new InventoryWeaponChangeEvent();
   // public InventoryWeaponDropEvent   OnWeaponDropped = new InventoryWeaponDropEvent();

    // Standard API
    public abstract InventoryWeaponMountInfo    GetWeapon   (int mountIndex);
    public abstract InventoryAmmoMountInfo      GetAmmo     (int mountIndex);
    public abstract InventoryBackpackMountInfo  GetBackpack (int mountIndex);

    // Audio properties / Functions
    public abstract bool                        autoPlayOnPickup { get; set; }
    public abstract InventoryItemAudio          GetAudioRecording(int recordingIndex);
    public abstract int                         GetActiveAudioRecording();
    public abstract int                         GetAudioRecordingCount();
    public abstract bool                        PlayAudioRecording(int recordingIndex);
    public abstract void                        StopAudioRecording();

    public abstract void                        DropAmmoItem    (int mountIndex, bool playAudio = true);
    public abstract void                        DropBackpackItem(int mountIndex, bool playAudio = true);
    public abstract void                        DropWeaponItem  (int mountIndex, bool playAudio = true);

    public abstract bool                        UseBackpackItem (int mountIndex, bool playAudio = true);
    public abstract bool                        ReloadWeapon    (int mountIndex, bool playAudio = true);
    public abstract bool                        AddItem         (CollectableItem collectableItem, bool playAudio = true);
    public abstract void                        AssignWeapon    (int mountIndex, InventoryWeaponMountInfo mountInfo);

 //   public abstract int                         GetAvailableAmmo(InventoryItemAmmo ammo, bool includeWeapon = false);
    public abstract bool                        IsReloadAvailable(int weaponMountIndex);
    public abstract InventoryMountInfo          Search(InventoryItem matchItem);
    public abstract int                         Remove(InventoryItem matchItem);
    public abstract bool                        RemoveWeapon(int mountIndex);
    public abstract bool                        RemoveBackpack(int mountIndex);
    public abstract bool                        RemoveAmmo(int mountIndex);
   
    // Low Level Mount Array access
    public abstract List<InventoryWeaponMountInfo> GetAllWeapons();
    public abstract List<InventoryAmmoMountInfo> GetAllAmmo();
    public abstract List<InventoryBackpackMountInfo> GetAllBackpack();
}
