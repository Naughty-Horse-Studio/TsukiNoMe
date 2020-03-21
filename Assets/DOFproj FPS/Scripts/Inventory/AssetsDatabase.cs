/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOFprojFPS {

    public class AssetsDatabase : MonoBehaviour
    {
        public List<GameObject> items;
        public List<GameObject> NPCs;
        public GameObject zombie;

        public Item FindItem(string name)
        {
            foreach (var item in items)
            {
                if (item.GetComponent<Item>().title == name)
                {
                    return item.GetComponent<Item>();
                }
            }

            print("Find item with arg: " + name + " Item not found in database");
            return null;
        }
        
        public GameObject FindNPC(string name)
        {
            foreach (var npc in NPCs)
            {
                if (npc.GetComponent<NPC>().NPCNameInDatabase == name)
                {
                    return npc;
                }
            }
            
            return null;
        }

        public GameObject ReturnZombie()
        {
            return zombie;
        }
    }
}
