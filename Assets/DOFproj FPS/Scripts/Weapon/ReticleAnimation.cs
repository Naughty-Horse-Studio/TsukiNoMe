/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleAnimation : MonoBehaviour
{
    public Image top, bottom, left, right;

    public float startPosValue = 40;

    public float shiftAmount;
    public float maxShift;
    public float coolSpeed;

    public static bool shot;
    

    void Update()
    {
        top.rectTransform.anchoredPosition = new Vector2(top.rectTransform.anchoredPosition.x, Mathf.Lerp(top.rectTransform.anchoredPosition.y, startPosValue, Time.deltaTime * coolSpeed));
        bottom.rectTransform.anchoredPosition = new Vector2(bottom.rectTransform.anchoredPosition.x, Mathf.Lerp(bottom.rectTransform.anchoredPosition.y, -startPosValue, Time.deltaTime * coolSpeed));
        left.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(left.rectTransform.anchoredPosition.x,- startPosValue, Time.deltaTime * coolSpeed), left.rectTransform.anchoredPosition.y);
        right.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(right.rectTransform.anchoredPosition.x, startPosValue, Time.deltaTime * coolSpeed), right.rectTransform.anchoredPosition.y);

        if (shot)
        {
            top.rectTransform.anchoredPosition = new Vector2(0, shiftAmount);
            bottom.rectTransform.anchoredPosition = new Vector2(0, -shiftAmount);
            left.rectTransform.anchoredPosition = new Vector2(-shiftAmount,0);
            right.rectTransform.anchoredPosition = new Vector2(shiftAmount,0);

            shot = false;
        }
    }
}
