/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DOFprojFPS
{
    [CustomEditor(typeof(Item))]
    public class ItemCustomInspector : Editor
    {
        Item item;
        Item cachedItem;

        public override void OnInspectorGUI()
        {
            item = target as Item;
            cachedItem = item;
            
            if(item)
            DrawGeneralItem();

            if (item != null)
            {
                if (item.type == ItemType.weaponPrimary || item.type == ItemType.weaponSecondary)
                {
                    item.stackable = false;
                    item.weaponAmmoCount = EditorGUILayout.IntField("Ammo in weapon", item.weaponAmmoCount);
                    EditorGUILayout.HelpBox("You can unload founded weapon by default (clip out magazine). To show unloading effect we will hide weapon's magazine gameobject on unload", MessageType.Info);
                    item.weaponUnloadingItem = (GameObject)EditorGUILayout.ObjectField("Weapon magazine object" ,item.weaponUnloadingItem, typeof(GameObject), true);


                    /* ///WIP FOR 1.4
                    item.haveScopeAddon = EditorGUILayout.Toggle("Have scope addon", item.haveScopeAddon);
                    item.scopeImage = (Sprite)EditorGUILayout.ObjectField("Scope icon", item.scopeImage, typeof(Sprite), false);
                    item.scopeImagePosition = EditorGUILayout.Vector2Field("Scope image position", item.scopeImagePosition);
                    item.scopeImageSize = EditorGUILayout.Vector2Field("Scope image size", item.scopeImageSize);*/
                    
                }
            }

            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            style.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("Save changes?", style))
            {
                EditorUtility.SetDirty(item);
                EditorSceneManager.MarkSceneDirty(item.gameObject.scene);
            }
        }

        public void DrawGeneralItem()
        {
            GUILayout.Label("General item settings", EditorStyles.boldLabel);
            GUILayout.BeginVertical("HelpBox");
            item.title = EditorGUILayout.TextField("Name", item.title);
            item.description = EditorGUILayout.TextField("Description", item.description);
            item.type = (ItemType)EditorGUILayout.EnumPopup("Type", item.type);
            item.icon = (Sprite)EditorGUILayout.ObjectField("Item icon", item.icon, typeof(Sprite), false);
            item.id = EditorGUILayout.IntField("ID", item.id);

            if (GUILayout.Button("Generate random ID?"))
            {
                item.id = Random.Range(0, int.MaxValue);
            }

            GUILayout.BeginVertical("GroupBox");
            GUILayout.Label("Item grid size", EditorStyles.boldLabel);
            item.width = EditorGUILayout.IntField("Width", item.width);
            item.height = EditorGUILayout.IntField("Height", item.height);
            GUILayout.EndVertical();

            if (item.type != ItemType.weaponPrimary && item.type != ItemType.weaponSecondary && item.type != ItemType.melee)
            {
                item.stackable = EditorGUILayout.Toggle("Stackable", item.stackable);

                if (item.stackable)
                {
                    item.stackSize = EditorGUILayout.IntSlider("Item stack size", item.stackSize, 1, 100);
                    item.maxStackSize = EditorGUILayout.IntSlider("Max stack size", item.maxStackSize, 1, 100);
                }
            }

            GUILayout.EndVertical();
        }


    }
}
