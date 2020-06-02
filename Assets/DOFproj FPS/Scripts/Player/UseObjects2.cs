/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DOFprojFPS;
using System.Collections.Generic;
//using EZCameraShake;

namespace DOFprojFPS
{
    public class UseObjects2 : MonoBehaviour
    {
        public AudioSource audioComnponent;

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
        [SerializeField] private PlayerStats _playerStats = null;
        [SerializeField] private Camera _camera = null;
        private Collider _collider = null;
        private GameSceneManager _gameSceneManager = null;
        private InteractiveDoor _interactDoor = null;
        private int _interactiveMask = 0;

        private CollectableItem collectableItem = null;
        private InventoryAudioPlayer audioPlayer = null;
        [Header("Audio Recordings")]
        [SerializeField] protected bool _autoPlayOnPickup = true;
        [SerializeField] protected List<InventoryItemAudio> _audioRecordings = new List<InventoryItemAudio>();

        protected List<InventoryItemAudio> _recordings = new List<InventoryItemAudio>();

        // The index of a recording currently being played
        protected int _activeAudioRecordingIndex = -1;
        bool playAudio = false;

        public GameObject _crawlingZombie;

        public GameObject _skipbtn;



       private MainMenuSceneManager sceneManager;
        private void Start()
        {
          


            StartCoroutine(waitAudio());
            //   useCursor = GameObject.Find("UseCursor");
            //   useText = useCursor.GetComponentInChildren<Text>();
            sceneManager = GetComponent<MainMenuSceneManager>();
                      
            _playerStats = FindObjectOfType<PlayerStats>();
            _collider = GetComponent<Collider>();
            _gameSceneManager = GameSceneManager.instance;
            audioPlayer = FindObjectOfType<InventoryAudioPlayer>();
            _interactiveMask = 1 << LayerMask.NameToLayer(layerName: "Interactive");

         //   _interactDoor = useCursor.GetComponent<InteractiveDoor>();
            inventory = FindObjectOfType<DTInventory>();
            input = FindObjectOfType<InputManager>();
            collectableItem = FindObjectOfType<CollectableItem>();
            weaponManager = FindObjectOfType<WeaponManager>();

            //if (InputManager.useMobileInput)
            //{
            //    useButton = GameObject.Find("UseButton").GetComponent<Button>();
            //    useButton.gameObject.SetActive(false);
            //}

            if (_gameSceneManager != null)
            {
                PlayerInfo info = new PlayerInfo();
                info.camera = _camera;
              //  info.useObjects2 = this;
                info.collider = _collider;
                _gameSceneManager.RegisterPlayerInfo(_collider.GetInstanceID(), info);

                //  Debug.Log("GotCha!" + _collider.GetInstanceID().ToString());
            }
        }

        void Update()
        {
          //  HitObjectPriority();

            Pickup();
        }

        private IEnumerator waitAudio()
        {

            yield return new WaitForSeconds(3f);

            audioComnponent.Play();

         //   yield return new WaitForSeconds(15f);

            _skipbtn.SetActive(true);


            yield return new WaitForSeconds(audioComnponent.clip.length);

       //   ApplicationManager instance = ApplicationManager.instance;

            if (_activeAudioRecordingIndex < 0) ApplicationManager.instance.LoadMainMenu();



            Debug.Log("end of sound = " + _activeAudioRecordingIndex);
        }

        private IEnumerator activateZombie()
        {

            yield return new WaitForSeconds(70f);

            _crawlingZombie.SetActive(true);

          
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log("other tag  =" + other.tag);
        //    if (other.tag == "Recording")
        //    {

        //        bool playAudio = true;
        //        InventoryItem invItem = collectableItem.inventoryItem;
        //        AddRecordingItem(invItem as InventoryItemAudio, collectableItem as CollectableAudio, playAudio); ;
        //    }

        //}
        public void Pickup()
        {

            RaycastHit hit;


            //Hit an object within pickup distance
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {

                Debug.DrawRay(transform.position, transform.forward, Color.green);
          //      Debug.Log("other tag  =" + hit.collider.tag);

                if (hit.collider.tag == "Item")
                {
          //          Debug.Log("other tag  =" + hit.collider.tag);
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
                        if (InputManager.useMobileInput)
                        {
                            var item = use.GetComponent<Item>();
                            useButton.onClick.RemoveAllListeners();
                            useButton.onClick.AddListener(() => { inventory.AddItem(item); });
                            use = null;
                            return;
                        }

                    }
                }
                else if (hit.collider.tag == "Recording")
                {

              //      Debug.Log("other tag  =" + hit.collider.tag);
                    if (playAudio ==false)
                     {

                     
                 //      Debug.Log("other tag  =" + hit.collider.tag);

                        playAudio = true;
                        InventoryItem invItem = collectableItem.inventoryItem;
                        AddRecordingItem(invItem as InventoryItemAudio, collectableItem as CollectableAudio, playAudio);

                        StartCoroutine(activateZombie());
                    }
                }
                else
                    useState = false;
                {
                    //Clear use object if there is no an object with "Item" tag
                    use = null;
           //         useCursor.SetActive(false);

                    if (InputManager.useMobileInput)
                        useButton.gameObject.SetActive(false);

                //    useText.text = "";
                }
            }
            else
            {
                useState = false;
             //   useCursor.SetActive(false);

                if (InputManager.useMobileInput)
                    useButton.gameObject.SetActive(false);

           //     useText.text = "";
            }
        }
    

        public void OnAfterDeserialize()
        {
            _recordings.Clear();
            foreach (InventoryItemAudio recording in _audioRecordings)
            {
                _recordings.Add(recording);
            }

            // Reset the audio recording selection
            _activeAudioRecordingIndex = -1;

        }
        // --------------------------------------------------------------------------------------------
        // Name :   AddRecordingItem
        // Desc :   Adds an AudioRecording to the Inventory and begins playing is AutoPlay is enabled
        // --------------------------------------------------------------------------------------------
        protected bool AddRecordingItem(InventoryItemAudio inventoryAudio, CollectableAudio collectableAudio, bool playAudio)
        {
            if (inventoryAudio)
            {
                // Play the pickup sound
                inventoryAudio.Pickup(collectableAudio.transform.position, playAudio);

                // Add audio recording to the list
                _recordings.Add(inventoryAudio);

                // Play on Pick if configured to do so
                if (_autoPlayOnPickup)
                {

                    // Tell Inventory to play this audio recording immediately
                    // This should be the one at end of the recordings list
                    PlayAudioRecording(_recordings.Count - 1);
                }




                // Data successfully retrieved
                return true;
            }

            return false;
        }

        void HitObjectPriority()
        {
            Ray ray;
            RaycastHit hit;
            RaycastHit[] hits;

            // PROCESS INTERACTIVE OBJECTS
            // Is the crosshair over a usuable item or descriptive item...first get ray from centre of screen
            ray = _camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            // Calculate Ray Length
            float rayLength = Mathf.Lerp(1.0f, 1.8f, Mathf.Abs(Vector3.Dot(_camera.transform.forward, Vector3.up)));

            // Cast Ray and collect ALL hits
            hits = Physics.RaycastAll(ray, rayLength, 1 << 26);

            // Process the hits for the one with the highest priorty
            if (hits.Length > 0)
            {
                // Used to record the index of the highest priorty
                int highestPriority = int.MinValue;
                InteractiveItem priorityObject = null;

                // Iterate through each hit
                for (int i = 0; i < hits.Length; i++)
                {
                    // Process next hit
                    hit = hits[i];


                    // Fetch its InteractiveItem script from the database
                    InteractiveItem interactiveObject = _gameSceneManager.GetInteractiveItem(hit.collider.GetInstanceID());



                    // If this is the highest priority object so far then remember it

                    // if (hit.collider.GetInstanceID() > highestPriority)
                    //if (interactiveObject != null && interactiveObject.priority > highestPriority)
                    if (interactiveObject != null && interactiveObject.priority > highestPriority)
                    {
                        //     Debug.Log("GotCha!" + hit.collider.GetInstanceID().ToString());
                        priorityObject = interactiveObject;
                        highestPriority = interactiveObject.priority;
                    }
                }

                //  priorityObject = FindObjectOfType<InteractiveItem>();
                // If we found an object then display its text and process any possible activation
                if (priorityObject != null)
                {
                    if (_playerStats)
                        _playerStats.ShowMessageText(priorityObject.GetText());

                    // if (Input.GetKeyDown(KeyCode.F)) 
                    //if (Input.GetButtonDown("Use"))
                    if (Input.GetButtonDown("Use"))
                    {

                     //   priorityObject.Activate(this);
                    }
                }
            }
        }

        // --------------------------------------------------------------------------------------------
        // Name :   GetAudioRecording
        // Desc :   Returns a recording at the specified index
        // --------------------------------------------------------------------------------------------
        public InventoryItemAudio GetAudioRecording(int recordingIndex)
        {
            if (recordingIndex < 0 || recordingIndex >= _recordings.Count) return null;
            return _recordings[recordingIndex];
        }

        // --------------------------------------------------------------------------------------------
        // Name :   GetAudioRecordingCount
        // Desc :   Returns the number of Audio Logs in the list
        // --------------------------------------------------------------------------------------------
        public int GetAudioRecordingCount()
        {
            return _recordings.Count;
        }

        // --------------------------------------------------------------------------------------------
        // Name :   GetActiveAudioRecording
        // Desc :   Return the index of any Audio Recording currently playing.
        // --------------------------------------------------------------------------------------------
        public int GetActiveAudioRecording()
        {
            return _activeAudioRecordingIndex;
        }

        // --------------------------------------------------------------------------------------------
        // Name :   PlayAudioRecording
        // Desc :   Instructs audio player to play the Audio Log and sets the current active index
        // --------------------------------------------------------------------------------------------
        public bool PlayAudioRecording(int recordingIndex)
        {
            if (recordingIndex < 0 || recordingIndex >= _recordings.Count) return false;

            audioPlayer = InventoryAudioPlayer.instance;
            if (audioPlayer)
            {
                   Debug.Log("GotCha! added recording");
                audioPlayer.OnEndAudio.RemoveListener(StopAudioListener);
                audioPlayer.OnEndAudio.AddListener(StopAudioListener);

                audioPlayer.PlayAudio(_recordings[recordingIndex]);
                _activeAudioRecordingIndex = recordingIndex;

                
            }

            return true;
        }

        // --------------------------------------------------------------------------------------------
        // Name :   StopAudioListener
        // Desc :   Called by the AudioPlayer when the audio stops
        // ---------------------------------------------------------------------------------------------
        protected void StopAudioListener()
        {
            InventoryAudioPlayer audioPlayer = InventoryAudioPlayer.instance;
            if (audioPlayer)
                audioPlayer.OnEndAudio.RemoveListener(StopAudioListener);
            _activeAudioRecordingIndex = -1;
        }

        // -------------------------------------------------------------------------------------------
        // Name :   StopAudioRecording
        // Desc :   Instructs the Audio Player to stop playing it's audio. Triggers a manual stop of
        //          the audio player.
        // --------------------------------------------------------------------------------------------
        public  void StopAudioRecording()
        {
            InventoryAudioPlayer audioPlayer = InventoryAudioPlayer.instance;
            if (audioPlayer)
                audioPlayer.StopAudio();
        }

    }
}
