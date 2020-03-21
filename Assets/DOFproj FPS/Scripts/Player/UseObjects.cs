/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.UI;

namespace DOFprojFPS
{
    public class UseObjects : MonoBehaviour
    {
        [Tooltip("The distance within which you can pick up item")]
        public float distance = 1.5f;
        
        private GameObject use;
        private GameObject useCursor;
        private Text useText;

        private InputManager input;
        private DTInventory inventory;

        private Button useButton;

        public static bool useState;

        private WeaponManager weaponManager;

        private void Start()
        {
            useCursor = GameObject.Find("UseCursor");
            useText = useCursor.GetComponentInChildren<Text>();
            useCursor.SetActive(false);

            inventory = FindObjectOfType<DTInventory>();
            input = FindObjectOfType<InputManager>();

            weaponManager = FindObjectOfType<WeaponManager>();

            if (InputManager.useMobileInput)
            {
                useButton = GameObject.Find("UseButton").GetComponent<Button>();
                useButton.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            Pickup();
        }

        public void Pickup()
        {
            RaycastHit hit;

            //Hit an object within pickup distance
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                if (hit.collider.tag == "Item")
                {
                    useState = true;
                    //Get an item which we want to pickup
                    use = hit.collider.gameObject;
                    useCursor.SetActive(true);

                    if (InputManager.useMobileInput)
                        useButton.gameObject.SetActive(true);

                    if (use.GetComponent<Item>())
                    {
                        useText.text = use.GetComponent<Item>().title;
                        if (!InputManager.useMobileInput)
                        {
                            if (Input.GetKeyDown(input.Use))
                            {
                                inventory.AddItem(use.GetComponent<Item>());
                                use = null;
                            }
                        }
                        if(InputManager.useMobileInput)
                        {
                            var item = use.GetComponent<Item>();
                            useButton.onClick.RemoveAllListeners();
                            useButton.onClick.AddListener(() => { inventory.AddItem(item); });
                            use = null;
                            return;
                        }
                            
                    }
                }
                else
                    useState = false;
                {
                    //Clear use object if there is no an object with "Item" tag
                    use = null;
                    useCursor.SetActive(false);

                    if(InputManager.useMobileInput)
                        useButton.gameObject.SetActive(false);

                    useText.text = "";
                }
            }
            else
            {
                useState = false;
                useCursor.SetActive(false);

                if (InputManager.useMobileInput)
                    useButton.gameObject.SetActive(false);

                useText.text = "";
            }
        }
    }
}
