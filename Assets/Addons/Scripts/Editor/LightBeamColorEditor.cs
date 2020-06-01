using UnityEditor;
using UnityEngine;

public class LightBeamColorEditor : MaterialEditor
{
    [SerializeField]
    GUIContent colorgc;
    [SerializeField]
    ColorPickerHDRConfig cpc;

    public override void OnInspectorGUI()
    {
        Material targetMat = target as Material;

        if(colorgc == null)
        {
            colorgc = new GUIContent("Color");
            colorgc.tooltip = "TOOLTIP";
        }
        if(cpc == null)
        {
            cpc = new ColorPickerHDRConfig(0, float.MaxValue, 0, float.MaxValue);
        }


        Color color = targetMat.GetColor("_Color");
        color = EditorGUILayout.ColorField(colorgc, color,true,true,true, cpc);
        targetMat.SetColor("_Color", color);



        base.OnInspectorGUI();



        if (GUI.changed)
        {
            
            EditorUtility.SetDirty(targetMat);
            serializedObject.ApplyModifiedProperties();
        }
    }
}