/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DOFprojFPS{

public class GridSlot : MonoBehaviour
{
    public bool free;
    public int x,y;

    public Image image;

    public bool multicell;
    
    public EquipmentPanel equipmentPanel;

    private void OnEnable()
    {
        if (image == null)
            image = GetComponent<Image>();
    }
}
}