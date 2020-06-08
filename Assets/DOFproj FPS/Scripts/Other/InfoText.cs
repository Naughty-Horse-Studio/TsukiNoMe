using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoText : MonoBehaviour
{
    /// <summary>
    /// Information text UI.
    /// </summary>
    public UnityEngine.UI.Text InfoUI;

    /// <summary>
    /// Text shown when clicked F1.
    /// </summary>
    [Multiline]
    public string InfoTxt = "Press F1 to hide controls";

    private bool m_ShowInfo = false;                // show controls info text flag



    // Unity Update method
    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        // toggle text info
        if (Input.GetKeyDown(KeyCode.F1))
            m_ShowInfo = !m_ShowInfo;

        if (InfoUI)
        {
            if (m_ShowInfo)
            {
                InfoUI.text = InfoTxt;
            }
            else
            {
                InfoUI.text = "Press F1 to show controls";
            }
        }
    }
}
