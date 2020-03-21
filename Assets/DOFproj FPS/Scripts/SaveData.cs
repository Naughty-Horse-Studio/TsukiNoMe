/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace DOFprojFPS
{
    public class SaveData : MonoBehaviour
    {
        AssetsDatabase assetsDatabase;
        FPSController controller;
        static bool loadDataTrigger = false;

        public Text saveLoadMessage;

        private void Start()
        {
            controller = FindObjectOfType<FPSController>();

            assetsDatabase = FindObjectOfType<AssetsDatabase>();

            if (loadDataTrigger)
            {
                controller.enabled = false;

                Load();
                loadDataTrigger = false;
            }
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5) && !PlayerStats.isPlayerDead)
            {
                Save();
                FadeMessage._text.text = "Game saved";
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                loadDataTrigger = true;
            }
        }
        
        public void Save()
        {
            var stat = FindObjectOfType<PlayerStats>();
            var camera_rot = Camera.main.transform.rotation;
            var controller = FindObjectOfType<FPSController>();
            PlayerStatsData p_data = new PlayerStatsData(stat.health, stat.useConsumeSystem, stat.hydratation, stat.hydratationSubstractionRate, stat.thirstDamage, stat.hydratationTimer, stat.satiety, stat.satietySubstractionRate, stat.hungerDamage, stat.satietyTimer, stat.playerPosition, stat.playerRotation, camera_rot, controller.targetDirection, controller._mouseAbsolute, controller._smoothMouse);
            string player_data = JsonUtility.ToJson(p_data);

            //print(player_data);

            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData", player_data);

            var weaponManager = FindObjectOfType<WeaponManager>();
            
            var sceneItems = FindObjectsOfType<ItemHandler>();
            List<string> items = new List<string>();
            List<int> stacksize = new List<int>();
            List<Vector2> itemGridPos = new List<Vector2>();
            List<Vector2> itemRectPos = new List<Vector2>();

            foreach (var UIitem in sceneItems)
            {
                items.Add(UIitem.item.title);
                if (UIitem.item.weaponAmmoCount != 0)
                {
                    print(UIitem.item.title + " :" + UIitem.item.weaponAmmoCount);
                    stacksize.Add(UIitem.item.weaponAmmoCount);
                }
                else
                    stacksize.Add(UIitem.item.stackSize);
                itemGridPos.Add(new Vector2(UIitem.x, UIitem.y));
                itemRectPos.Add(UIitem.GetComponent<RectTransform>().anchoredPosition);
            }
            
            var _i = items.ToArray();
            var _s = stacksize.ToArray();
            var _p = itemGridPos.ToArray();

            InventoryData inventoryData = new InventoryData(_i, _s, _p);
            string _inventoryData = JsonUtility.ToJson(inventoryData);
            File.WriteAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_inventoryData", _inventoryData);

            var allSceneItems = FindObjectsOfType<Item>();

            List<Item> enabledItems = new List<Item>();

            foreach(var item in allSceneItems)
            {
                if (item.isActiveAndEnabled)
                    enabledItems.Add(item);
            }
            
            LevelData itemsLevelData = new LevelData();

            itemsLevelData.itemName = new string[enabledItems.ToArray().Length];
            itemsLevelData.itemPos = new Vector3[enabledItems.ToArray().Length];
            itemsLevelData.itemRot = new Quaternion[enabledItems.ToArray().Length];
            itemsLevelData.itemStackSize = new int[enabledItems.ToArray().Length];
            itemsLevelData.equipedWeapon = weaponManager.activeWeapon == null ? "" : weaponManager.activeWeapon.weaponName;

            for (int i = 0; i < enabledItems.ToArray().Length; i++)
            {
                itemsLevelData.itemName[i] = enabledItems.ToArray()[i].title;
                itemsLevelData.itemPos[i] = enabledItems.ToArray()[i].transform.position;
                itemsLevelData.itemRot[i] = enabledItems.ToArray()[i].transform.rotation;
                if (enabledItems.ToArray()[i].GetComponent<Item>().type == ItemType.weaponPrimary || enabledItems.ToArray()[i].GetComponent<Item>().type == ItemType.weaponSecondary)
                    itemsLevelData.itemStackSize[i] = enabledItems.ToArray()[i].weaponAmmoCount;
                else
                {
                    itemsLevelData.itemStackSize[i] = enabledItems.ToArray()[i].stackSize;
                }
            }
            
            NPC[] npc = FindObjectsOfType<NPC>();

            if (npc != null)
            {

                itemsLevelData.npcName = new string[npc.Length];
            itemsLevelData.npcPos = new Vector3[npc.Length];
            itemsLevelData.npcRot = new Quaternion[npc.Length];
            itemsLevelData.npcCurrentTarget = new Vector3[npc.Length];
            itemsLevelData.npcLookAtTarget = new Vector3[npc.Length];

                for (int n = 0; n < npc.Length; n++)
                {
                    itemsLevelData.npcName[n] = npc[n].NPCNameInDatabase;
                    itemsLevelData.npcPos[n] = npc[n].gameObject.transform.position;
                    itemsLevelData.npcRot[n] = npc[n].gameObject.transform.rotation;

                    //if (npc[n].curretTarget != null)
                    //    itemsLevelData.npcCurrentTarget[n] = npc[n].curretTarget.position;

                    itemsLevelData.npcLookAtTarget[n] = npc[n].lookPosition;
                }
            }

            ZombieNPC[] zombies = FindObjectsOfType<ZombieNPC>();

            if (zombies != null)
            {
                itemsLevelData.zombiePos = new Vector3[zombies.Length];
                itemsLevelData.zombieRot = new Quaternion[zombies.Length];
                itemsLevelData.zombieIsWorried = new bool[zombies.Length];

                for(int z = 0; z < zombies.Length; z++)
                {
                    itemsLevelData.zombiePos[z] = zombies[z].transform.position;
                    itemsLevelData.zombieRot[z] = zombies[z].transform.rotation;
                    itemsLevelData.zombieIsWorried[z] = zombies[z].GetComponent<ZombieNPC>().isWorried;
                }
            }

            string _itemsLevelData = JsonUtility.ToJson(itemsLevelData);
            print(_itemsLevelData);
            File.WriteAllText(Application.persistentDataPath +"/"+SceneManager.GetActiveScene().name+"_itemsLevelData", _itemsLevelData);
    }
        

        //When we load game. Player actually takes all the items from save like regular pickups. Because of that we have a lot of pickup sounds at the same time and it sounds like noise
        //We will invoke this method after 1 second to enable volume again
        public void UnmutePickupEffectsOnLoad() { AudioListener.volume = 1; }

        public void Load()
        {
            if (JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData")) == null)
            {
                print("No save data found data found");
                return;
            }

            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_playerData"));

            AudioListener.volume = 0;

            //Player stats
            var playerStats = FindObjectOfType<PlayerStats>();
            playerStats.health = data.health;
            playerStats.useConsumeSystem = data.useConsumeSystem;
            playerStats.hydratation = data.hydratation;
            playerStats.hydratationSubstractionRate = data.hydratationSubstractionRate;
            playerStats.thirstDamage = data.thirstDamage;
            playerStats.hydratationTimer = data.hydratationTimer;
            playerStats.satiety = data.satiety;
            playerStats.satietySubstractionRate = data.satietySubstractionRate;
            playerStats.hungerDamage = data.hungerDamage;
            playerStats.satietyTimer = data.satietyTimer;

            controller.targetDirection = data.targetDirection;
            controller._mouseAbsolute = data.mouseAbsolute;
            controller._smoothMouse = data.smoothMouse;

            Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            player.position = data.playerPosition;
            player.rotation = data.playerRotation;

            Transform cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();
            cameraHolder.rotation = data.camRotation;

            //Clear all objects from scene before you will instantiate saved
            var itemsToDestroy = FindObjectsOfType<Item>();
            var npcToDestroy = FindObjectsOfType<NPC>();
            var zombiesToDestroy = FindObjectsOfType<ZombieNPC>();

            foreach (var item in itemsToDestroy)
            {
                Destroy(item.gameObject);
            }
            

            foreach (var npc in npcToDestroy)
            {
                Destroy(npc.gameObject);
            }

            foreach(var zombie in zombiesToDestroy)
            {
                Destroy(zombie.gameObject);
            }
            ///

            //Inventory
            DTInventory inventory = FindObjectOfType<DTInventory>();
            
            InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_inventoryData"));

            var inventoryItems = inventoryData.itemNames;
            var stackSize = inventoryData.stackSize;
            var itemPos = inventoryData.itemGridPos;

            bool isAutoEquipEnabled = inventory.autoEquipItems;

            inventory.autoEquipItems = false;

            if (inventoryItems != null)
            {
                for (int i = 0; i < inventoryItems.Length; i++)
                {
                    var findItem = assetsDatabase.FindItem(inventoryItems[i]);

                    if (findItem != null)
                    {
                        var item = Instantiate(findItem);

                        if (item.type == ItemType.weaponPrimary || item.type == ItemType.weaponSecondary)
                        {
                            item.weaponAmmoCount = stackSize[i];
                        }
                        else
                        {
                            item.stackSize = stackSize[i];
                        }
                        inventory.AddItem(item, (int)itemPos[i].x, (int)itemPos[i].y);
                    }
                    else
                    {
                        Debug.Log("Missing item. Check if it exists in the ItemsDatabase inspector");
                    }
                }
            }



            inventory.autoEquipItems = isAutoEquipEnabled;

            LevelData itemsLevelData = JsonUtility.FromJson<LevelData>(File.ReadAllText(Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + "_itemsLevelData"));
            
            for (int i = 0; i < itemsLevelData.itemName.Length; i++)
            {
                if (itemsLevelData.itemName[i] != null)
                {
                    try{
                        var item = Instantiate(assetsDatabase.FindItem(itemsLevelData.itemName[i]));
                        item.transform.position = itemsLevelData.itemPos[i];
                        item.transform.rotation = itemsLevelData.itemRot[i];

                        if (item.GetComponent<Item>().type == ItemType.weaponPrimary || item.GetComponent<Item>().type == ItemType.weaponSecondary)
                        {
                            item.weaponAmmoCount = itemsLevelData.itemStackSize[i];
                        }
                        else
                        {
                            item.stackSize = itemsLevelData.itemStackSize[i];
                        }
                    }
                    catch
                    {
                        print("Item you try to restore from save: " + itemsLevelData.itemName[i] + " is null or not exist in database");
                    }
                }
            }

            for (int k = 0; k < itemsLevelData.npcName.Length; k++)
            {
                    var _npc = Instantiate(assetsDatabase.FindNPC(itemsLevelData.npcName[k]));
                _npc.GetComponent<NavMeshAgent>().enabled = false;
                    _npc.transform.position = itemsLevelData.npcPos[k];
                    _npc.transform.rotation = itemsLevelData.npcRot[k];
                    //_npc.GetComponent<NPC>().curretTarget.position = itemsLevelData.npcCurrentTarget[k];
                    _npc.GetComponent<NPC>().lookPosition = itemsLevelData.npcLookAtTarget[k];

                _npc.GetComponent<NavMeshAgent>().enabled = true;
            }

            for(int z = 0; z < itemsLevelData.zombiePos.Length; z++)
            {
                var _zombie = Instantiate(assetsDatabase.ReturnZombie());
                _zombie.GetComponent<NavMeshAgent>().enabled = false;
                _zombie.transform.position = itemsLevelData.zombiePos[z];
                _zombie.transform.rotation = itemsLevelData.zombieRot[z];
                _zombie.GetComponent<ZombieNPC>().isWorried = itemsLevelData.zombieIsWorried[z];
                _zombie.GetComponent<NavMeshAgent>().enabled = true;
            }

            var weaponManager = FindObjectOfType<WeaponManager>();

            if(itemsLevelData.equipedWeapon != "")
            {
                print("Try to enable last active weapon on load " + itemsLevelData.equipedWeapon);
                weaponManager.activeWeapon = weaponManager.GetWeapon(itemsLevelData.equipedWeapon);
                weaponManager.activeWeapon.gameObject.SetActive(true);

                List<ItemHandler> items = FindObjectOfType<DTInventory>().characterItems;

                Item item = null;

                print("I need to find suitable item to give activeWeapon 'Item' arg. Weapon name is" + weaponManager.activeWeapon.weaponName);

                foreach(ItemHandler _item in inventory.characterItems)
                {
                    if (_item.item.title == itemsLevelData.equipedWeapon)
                    {
                        item = _item.item;
                        print("Found suitable item for active Weapon"); 
                        break;
                    }
                }

                if(item != null)
                    weaponManager.activeWeapon.currentItem = item;
            }

            controller.enabled = true;
            
            Invoke("UnmutePickupEffectsOnLoad", 1f);

            FadeMessage._text.text = "Game loaded";
        }
    }
    
    public class PlayerStatsData
    {
        public int health;
        public bool useConsumeSystem;
        public int hydratation;
        public float hydratationSubstractionRate;
        public int thirstDamage;
        public float hydratationTimer;
        public int satiety;
        public float satietySubstractionRate;
        public int hungerDamage;
        public float satietyTimer;

        public Vector3 playerPosition;
        public Quaternion playerRotation;

        public Quaternion camRotation;

        public Vector2 targetDirection;
        public Vector2 mouseAbsolute;
        public Vector2 smoothMouse;

        public PlayerStatsData(int health, bool useConsumeSystem, int hydratation, float hydratationSubstractionRate, int thirstDamage,
                               float hydratationTimer, int satiety, float satietySubstractionRate, int hungerDamage, float satietyTimer,
                               Vector3 playerPosition, Quaternion playerRotation, Quaternion camRotation, Vector2 targetDirection, Vector2 mouseAbsolute, Vector2 smoothMouse)
        {
            this.health = health;
            this.useConsumeSystem = useConsumeSystem;
            this.hydratation = hydratation;
            this.hydratationSubstractionRate = hydratationSubstractionRate;
            this.thirstDamage = thirstDamage;
            this.hydratationTimer = hydratationTimer;
            this.satiety = satiety;
            this.satietySubstractionRate = satietySubstractionRate;
            this.hungerDamage = hungerDamage;
            this.satietyTimer = satietyTimer;
            this.playerPosition = playerPosition;
            this.playerRotation = playerRotation;
            this.camRotation = camRotation;
            this.targetDirection = targetDirection;
            this.mouseAbsolute = mouseAbsolute;
            this.smoothMouse = smoothMouse;
        }
    }

    public class WeaponManagerData
    {
        public string primaryWeaponString;
        public string secondaryWeaponString;
        public int primaryWeaponAmmo;
        public int secondaryWeaponAmmo;

        public int activeSlot;

        public WeaponManagerData(string primaryWeaponString, string secondaryWeaponString, int primaryWeaponAmmo, int secondaryWeaponAmmo, int activeSlot)
        {
            this.primaryWeaponString = primaryWeaponString;
            this.secondaryWeaponString = secondaryWeaponString;
            this.primaryWeaponAmmo = primaryWeaponAmmo;
            this.secondaryWeaponAmmo = secondaryWeaponAmmo;
            this.activeSlot = activeSlot;
        }
    }

    public class InventoryData
    {
        public string[] itemNames;
        public int[] stackSize;
        public Vector2[] itemGridPos;

        public InventoryData( string[] itemNames, int[] stackSize, Vector2[] itemGridPos)
        {
            this.itemNames = itemNames;
            this.stackSize = stackSize;
            this.itemGridPos = itemGridPos;
        }
    }

    public class LevelData
    {
        //Inventory items

        public Vector3[] itemPos;
        public Quaternion[] itemRot;
        public string[] itemName;
        public string equipedWeapon;
        //For ammo items, for usable items quantity will be equal to zero
        public int[] itemStackSize;

        //NPC

        public string[] npcName;
        public Vector3[] npcPos;
        public Quaternion[] npcRot;
        public Vector3[] npcCurrentTarget;
        public Vector3[] npcLookAtTarget;

        //Zombies

        public Vector3[] zombiePos;
        public Quaternion[] zombieRot;
        public bool[] zombieIsWorried;
    }
    
}