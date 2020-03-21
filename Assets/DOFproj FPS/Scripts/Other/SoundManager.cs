/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip pickupSound;
    public AudioClip inventoryOpenSound;
    public AudioClip clickSound;
    public AudioClip weaponHide;
    public AudioClip weaponUnhide;

    public AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Pickup()
    {
        source.PlayOneShot(pickupSound);
    }

    public void InventoryOpen()
    {
        source.PlayOneShot(inventoryOpenSound);
    }

    public void Click()
    {
        source.PlayOneShot(clickSound);
    }

    public void WeaponPicking(bool hide)
    {
        if(hide)
            source.PlayOneShot(weaponHide);
        else

            source.PlayOneShot(weaponUnhide);
    }

}
