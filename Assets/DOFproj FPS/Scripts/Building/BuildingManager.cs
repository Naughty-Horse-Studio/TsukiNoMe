/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DOFprojFPS
{
    public class BuildingManager : MonoBehaviour
    {
        public BuildingScriptableObjects[] buildings;

        public GameObject buttonPrefab;

        public RectTransform contentHolder;

        private DTInventory inventory;
        private ObjectPlacement objectPlacement;

        private void Start()
        {
            foreach (var building in buildings)
            {
                var button = Instantiate(buttonPrefab).GetComponent<Button>();
                button.onClick.AddListener(() => Build(building.buildingCostItems, building.builingCostItemsAmont, building.BuildingGameObject));
                button.GetComponentInChildren<Text>().text = building.BuildingName;
                button.GetComponent<Image>().sprite = building.BuildingIcon;
                button.gameObject.transform.SetParent(contentHolder);
            }

            inventory = FindObjectOfType<DTInventory>();
            objectPlacement = FindObjectOfType<ObjectPlacement>();
        }

        public void Build(GameObject[] requiredItems, int[] requiredItemsValue, GameObject buildObject)
        {
            if (requiredItems == null)
                print("requireditems null");
            else
                print("requireditems ok");

            if (requiredItemsValue == null)
                print("requiredItemsValue null");
            else
                print("requiredItemsValue ok");

            if (buildObject == null)
                print("buildObject null");
            else
                print("buildObject ok");
            
            if (inventory.SearchItemsForBuilding(requiredItems, requiredItemsValue) != null)
            {
                var items = inventory.SearchItemsForBuilding(requiredItems, requiredItemsValue);
                print("Needed object for building found. Building started");
                objectPlacement.itemsToRemove = items;
                objectPlacement.objectToPlace = Instantiate(buildObject);
                InventoryManager.showInventory = false;
            }
            else
            {
                print("Not enough resources to build");
            }
        }
    }
}
