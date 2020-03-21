/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMessage : MonoBehaviour
{
    public static Text _text;

    public float fadeSpeed = 3;
    float decrease = 1;

    private void OnEnable()
    {
        _text = GetComponent<Text>();
    }

    void Update()
    {
        if(_text.text != "")
        {
            decrease -= Time.deltaTime * fadeSpeed;
            _text.color = new Color(1, 1, 1, decrease);
        }
        if(_text.color.a <= 0)
        {
            decrease = 1;
            _text.color = Color.white;
            _text.text = "";
            
        }
    }
}
