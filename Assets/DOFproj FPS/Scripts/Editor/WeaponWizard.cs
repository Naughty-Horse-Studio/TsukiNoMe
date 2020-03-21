/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace DOFprojFPS
{
    public class WeaponEditor : EditorWindow
    {
        public Texture labelImage;

        public GameObject weaponObject;
        public int stageIndex = 0;

        GameObject gamePrefab;
        GameObject weapon;

        AnimationClip shotAnimation;
        AnimationClip idleAnimation;
        AnimationClip reloadAnimation;
        AnimationClip aimPositionAnimation;
        AnimationClip takeBeginAnimation;
        AnimationClip takeLoopAnimation;
        AnimationClip takeAnimation;
        AnimationClip shot2Animation;
        AnimationClip shot3Animation;

        bool useAnimation;
        bool useExtraAnimation;

        int toolbarInt = 0;
        string[] toolbarStrings = { "Object", "Position", "Animator", "Settings", "FPS Controller"};

        WeaponSettingSO weaponSettingInstance;

        FPSController controller;

        Vector2 scrollValue;

        GameObject weaponPickup;
        GameObject weaponAmmoPickup;

        string weaponNameToPickup;
        int weaponPickupAmmoCount;
        int ammoPickupAmmoCount;

        //*  [MenuItem("Don Philifeh/Weapon wizard")]
        static void Init()
        {
            WeaponEditor _editor = (WeaponEditor)GetWindow(typeof(WeaponEditor));

            _editor.Show();
        }

        void OnGUI()
        {
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            Texture labelImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DOFproj FPS/Scripts/Editor/EditorImages/product_banner.png", typeof(Texture));
            GameObject gamePrefabSource = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/DOFproj FPS/Prefabs/GamePrefab.prefab", typeof(GameObject));

            GUILayout.Label(labelImage);

            EditorGUI.indentLevel++;

            if (toolbarInt == 0)
            {
                Repaint();
                GUILayout.TextArea("Welcome to weapon setting menu!\n\nAssign 3D weapon model to create new weapon", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("All necessary objects will be created automatically after weapon object assign. Delete GamePrefab If you already have one in your scene.", MessageType.Info);

                EditorGUI.indentLevel++;
                weaponObject = (GameObject)EditorGUILayout.ObjectField("Weapon object", weaponObject, typeof(GameObject), true);
                if (weaponObject)
                {
                    if (GUILayout.Button("Next"))
                        toolbarInt++;
                }
                GUILayout.TextArea("Or create GamePrefab to edit existing weapons and FPS Player", EditorStyles.boldLabel);
                if (GUILayout.Button("Create GamePrefab"))
                {
                    if (!GameObject.FindGameObjectWithTag("Player"))
                    {
                        gamePrefab = PrefabUtility.InstantiatePrefab(gamePrefabSource) as GameObject;
                        controller = GameObject.FindObjectOfType<FPSController>();

                        Selection.activeGameObject = gamePrefab;
                        SceneView.lastActiveSceneView.FrameSelected();
                    }
                    toolbarInt = 3;
                }
                Repaint();
            }
            else if (toolbarInt == 1)
            {
                Repaint();
                GUILayout.Label("Adjust the position, rotation and scale of the object relative to the game camera", EditorStyles.boldLabel);
                if (!GameObject.FindGameObjectWithTag("Player"))
                {
                    weapon = null;
                    gamePrefab = PrefabUtility.InstantiatePrefab(gamePrefabSource) as GameObject;

                    controller = GameObject.FindObjectOfType<FPSController>();

                    weapon = Instantiate(weaponObject);
                    weapon.transform.parent = GameObject.Find("Sway").transform;
                    weapon.transform.position = GameObject.Find("WeaponCamera").transform.position;
                    weapon.transform.rotation = GameObject.Find("WeaponCamera").transform.rotation;
                    weapon.transform.localScale = GameObject.Find("WeaponCamera").transform.localScale;
                    weapon.name = weaponObject.name;

                    foreach (Transform _object in weapon.GetComponentInChildren<Transform>())
                    {
                        _object.gameObject.layer = 20;
                    }
                    weapon.layer = 20;

                    weapon.AddComponent<Weapon>();
                    weapon.AddComponent<AudioSource>();

                    Selection.activeGameObject = weapon;
                    SceneView.lastActiveSceneView.FrameSelected();
                    Repaint();
                }

                if (weapon != null)
                {
                    EditorGUILayout.HelpBox("Create children helpers and move 'Muzzle flash transform' to the end of weapon muzzle and 'Shell transform' to the slide or hole where shells are ejected", MessageType.Info, true);
                    if (GUILayout.Button("Create helpers for weapon"))
                    {
                        var MuzzleFlashTransform = Instantiate(new GameObject());
                        MuzzleFlashTransform.name = "Muzzle flash transform";
                        MuzzleFlashTransform.transform.parent = weapon.transform;
                        MuzzleFlashTransform.transform.localPosition = Vector3.zero;
                        var ShellTransform = Instantiate(new GameObject());
                        ShellTransform.name = "Shell transform";
                        ShellTransform.transform.parent = weapon.transform;
                        ShellTransform.transform.localPosition = Vector3.zero;

                        weapon.GetComponent<Weapon>().muzzleFlashTransform = MuzzleFlashTransform.transform;
                        weapon.GetComponent<Weapon>().shellTransform = ShellTransform.transform;
                        Repaint();
                    }
                }

                if (GUILayout.Button("Next"))
                    toolbarInt++;
            }
            else if (toolbarInt == 2)
            {
                Repaint();

                if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<Weapon>())
                {
                    weapon = Selection.activeGameObject;
                    Repaint();
                }
                else
                {
                    weapon = null;
                    Repaint();
                }

                if (weapon == null)
                {
                    if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Weapon>())
                    {
                        weapon = Selection.activeGameObject;
                        Repaint();
                    }
                    else
                        return;
                }

                if (weapon != null)
                {
                    if (!weapon.GetComponent<Animator>())
                    {
                        shotAnimation = (AnimationClip)EditorGUILayout.ObjectField("Shot animation", shotAnimation, typeof(AnimationClip), false);
                        idleAnimation = (AnimationClip)EditorGUILayout.ObjectField("Idle animation", idleAnimation, typeof(AnimationClip), false);
                        reloadAnimation = (AnimationClip)EditorGUILayout.ObjectField("Reload animation", reloadAnimation, typeof(AnimationClip), false);
                        EditorGUILayout.HelpBox("Animation with aim position should have only one frame with iron sight view", MessageType.Info, true);
                        aimPositionAnimation = (AnimationClip)EditorGUILayout.ObjectField("Aim position animation", aimPositionAnimation, typeof(AnimationClip), false);
                        useExtraAnimation = EditorGUILayout.Toggle("Use extra animations", useExtraAnimation);
                        Repaint();

                        EditorGUILayout.HelpBox("You can apply extra animation here. If you have not some, don't worry you make it later in animator", MessageType.Info, true);
                            takeBeginAnimation = (AnimationClip)EditorGUILayout.ObjectField("Take begin animation", takeBeginAnimation, typeof(AnimationClip), false) ;
                            takeLoopAnimation = (AnimationClip)EditorGUILayout.ObjectField("Take loop animation", takeLoopAnimation, typeof(AnimationClip), false) ;
                            takeAnimation = (AnimationClip)EditorGUILayout.ObjectField("Take animation", takeAnimation, typeof(AnimationClip), false);
                            shot2Animation = (AnimationClip)EditorGUILayout.ObjectField("Second shot", shot2Animation, typeof(AnimationClip), false);
                            shot3Animation = (AnimationClip)EditorGUILayout.ObjectField("Third shot", shot3Animation, typeof(AnimationClip), false);


                        EditorGUILayout.HelpBox("Animator component should exist for proper Weapon component work!", MessageType.Warning, true);
                        if (GUILayout.Button("Create animator"))
                        {
                            var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/DOFproj FPS/Animations/" + weapon.name + " animator.controller");
                          
                            controller.AddLayer("Aim");
                            controller.AddParameter("Aim", AnimatorControllerParameterType.Bool);
                            controller.AddParameter("Use", AnimatorControllerParameterType.Bool);
                            controller.AddParameter("Take", AnimatorControllerParameterType.Trigger);

                            var rootStateMachine = controller.layers[0].stateMachine;
                            var aimLayer = controller.layers[1].stateMachine;

                            var aimParameter = controller.parameters[0];
                            var useParameter = controller.parameters[1];
                            var takeTrigger = controller.parameters[2];

                            var shot = rootStateMachine.AddState("Shot");
                            var idle = rootStateMachine.AddState("Idle");
                            var reload = rootStateMachine.AddState("Reload");
                            var defaultPosition = aimLayer.AddState("Default position");
                            var aimPosition = aimLayer.AddState("Aim");
                            var takeBegin = rootStateMachine.AddState("Take begin");
                            var takeLoop = rootStateMachine.AddState("Take loop");
                            var take = rootStateMachine.AddState("Take");
                            
                            var shot1 = rootStateMachine.AddState("Shot1");
                            var shot2 = rootStateMachine.AddState("Shot2");
                            
                                shot.motion = shotAnimation;
                                idle.motion = idleAnimation;
                                reload.motion = reloadAnimation;
                                aimPosition.motion = aimPositionAnimation;
                            
                                takeBegin.motion = takeBeginAnimation;
                                takeLoop.motion = takeLoopAnimation;
                                take.motion = takeAnimation;

                                if (shot2Animation != null)
                                    shot1.motion = shot2Animation;
                                else
                                    shot1.motion = shotAnimation;

                                if (shot2.motion != null)
                                    shot2.motion = shot3Animation;
                                else
                                    shot2.motion = shotAnimation;

                            idle.name = "Idle";
                            shot.name = "Shot";
                            reload.name = "Reload";
                            aimPosition.name = "Aim"; takeBegin.name = "Take begin";
                            takeLoop.name = "Take loop";
                            take.name = "Take";
                            shot1.name = "Shot1";
                            shot2.name = "Shot2";

                            rootStateMachine.defaultState = idle;
                            aimLayer.defaultState = defaultPosition;

                            controller.layers[1].defaultWeight = 1;

                            var shotToIdleTransition = shot.AddTransition(idle).hasExitTime = true;
                            var reloadToIdleTransition = reload.AddTransition(idle).hasExitTime = true;

                            idle.AddTransition(takeBegin).AddCondition(AnimatorConditionMode.If, 1.0f, "Use");
                            takeBegin.AddTransition(takeLoop).hasExitTime = true;

                            takeLoop.AddTransition(take).AddCondition(AnimatorConditionMode.If, 1.0f, "Take");
                            take.AddTransition(idle).hasExitTime = true;

                            aimPosition.AddTransition(defaultPosition).AddCondition(AnimatorConditionMode.IfNot, 1f, "Aim");
                            defaultPosition.AddTransition(aimPosition).AddCondition(AnimatorConditionMode.If, 1f, "Aim");

                            shot1.AddTransition(idle).hasExitTime = true;
                            shot2.AddTransition(idle).hasExitTime = true;
                            takeLoop.AddTransition(idle).AddCondition(AnimatorConditionMode.IfNot, 1f, "Use");
                            
                            controller.layers[1].defaultWeight = 1;

                            weapon.AddComponent<Animator>().runtimeAnimatorController = controller;
                            Repaint();
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Animator component already exist on the weapon object! You can assign animations to the animator controller states, if you already not.", MessageType.Warning, true);
                        Repaint();
                    }
                }
                else
                {
                    GUILayout.Label("Select Weapon component in the hierarchy or create new in Object tab!", EditorStyles.boldLabel);
                    Repaint();
                }
            }
            else if (toolbarInt == 3)
            {
                Repaint();
                if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Weapon>())
                    weapon = Selection.activeGameObject;
                else
                    weapon = null;

                if (weapon != null)
                {
                    scrollValue = GUILayout.BeginScrollView(scrollValue, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height/1.2f));
                    if (weapon.GetComponent<Weapon>().weaponSetting)
                    {
                        Editor editor = Editor.CreateEditor(weapon.GetComponent<Weapon>().weaponSetting);
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        if (GUILayout.Button("No weapon settings found. Create new weapon setting sriptable object?"))
                        {
                            WeaponSettingSO weaponSetting = CreateInstance<WeaponSettingSO>();
                            AssetDatabase.CreateAsset(weaponSetting, "Assets/DarkTree FPS/WeaponData/" + weapon.name + " settings.asset");
                            weaponSettingInstance = (WeaponSettingSO)AssetDatabase.LoadAssetAtPath("Assets/DOFproj FPS/WeaponData/" + weapon.name + " settings.asset", typeof(WeaponSettingSO));
                            weapon.GetComponent<Weapon>().weaponSetting = weaponSettingInstance;
                            Repaint();
                        }
                    }
                    GUILayout.EndScrollView();
                }
                else
                {
                    GUILayout.Label("Select Weapon component in the hierarchy to edit!", EditorStyles.boldLabel);
                    Repaint();
                }
            }

            else if (toolbarInt == 4)
            {
                if(controller == null)
                controller = GameObject.FindObjectOfType<FPSController>();

                    if (controller)
                    {
                        Editor editor = Editor.CreateEditor(controller);
                        editor.DrawDefaultInspector();
                    }
                    else
                {
                    EditorGUILayout.HelpBox("Can't find FPS Controller in the scene! Is it exist in the scene?", MessageType.Warning, true);
                }
            }

            /*
            else if(toolbarInt == 5)
            {
                EditorGUILayout.HelpBox("Here you can create pickups for weapons. Select the desired weapon in the hierarchy and attach the necessary components to the fields below.", MessageType.Info, true);
                EditorGUI.indentLevel++;
                GUILayout.Label("Object which will be used for weapon pickup");
                weaponPickup = (GameObject)EditorGUILayout.ObjectField("Weapon object", weaponPickup, typeof(GameObject), true);
                GUILayout.Label("Weapon name. Must be the same as in the weapon settings");
                weaponNameToPickup = EditorGUILayout.TextField(weaponNameToPickup);
                GUILayout.Label("Weapon ammo count");
                weaponPickupAmmoCount = EditorGUILayout.IntField(weaponPickupAmmoCount);

                if (GUILayout.Button("Create weapon pickup"))
                {
                    var _weaponPickupObject = Instantiate(weaponPickup) as GameObject;
                    _weaponPickupObject.AddComponent<WeaponPickup>();
                    _weaponPickupObject.AddComponent<BoxCollider>();
                    _weaponPickupObject.AddComponent<Rigidbody>();
                    _weaponPickupObject.tag = "Item";
                    
                    _weaponPickupObject.GetComponent<WeaponPickup>().weaponNameToEquip = weaponNameToPickup;
                    _weaponPickupObject.GetComponent<WeaponPickup>().ammoInWeaponCount = weaponPickupAmmoCount;
                    _weaponPickupObject.name = weaponNameToPickup + " pickup";

                    Selection.activeGameObject = _weaponPickupObject;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
            }*/
        }
    }
}
