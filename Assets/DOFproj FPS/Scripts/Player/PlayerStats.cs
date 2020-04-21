/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DOFprojFPS;
using UnityEngine.PostProcessing;

namespace DOFprojFPS
{
    /// <summary>
    /// It's very simple class so I think that there is no need in deep explanation of it
    /// When health less than or equal to zero -> Death() || if respawn -> Respawn()
    /// Death and Respawn methods works in same way but in oposite direction
    /// </summary>

    public enum ScreenFadeType2 { FadeIn, FadeOut }

    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        public bool isGod = false;
        [Tooltip("Player's health")]
        public int health = 100;
        [Tooltip("UI element to draw health as number")]
        public Text healthUIText;
        [Tooltip("UI element to draw health as slider")]
        public Slider healthUISlider;

        public Slider hungerSlider;
        public Text hungerUIText;

        public Slider thirstSlider;
        public Text thirstUIText;

        [Header("Damage effect")]
        [Tooltip("UI Image with fullscreen hit fx")]
        public Image damageScreenFX;
        [Tooltip("UI Image color to change on hit")]
        public Color damageScreenColor;
        [Tooltip("UI Image fade speed after hit")]
        public float damageScreenFadeSpeed = 1.4f;

        [Header("Consume stats")]
        public bool useConsumeSystem = true;
        
        public int hydratation = 100;
        public float hydratationSubstractionRate = 3f;
        public int thirstDamage = 1;
        [HideInInspector]
        public float hydratationTimer;

        public int satiety = 100;
        public float satietySubstractionRate = 5f;
        public int hungerDamage = 1;
        [HideInInspector]
        public float satietyTimer;

        [SerializeField] private Image _screenFade = null;
        [SerializeField] private Text _messagesDisplay = null;
        [SerializeField] private Text _missionText = null;
        [SerializeField] private float _missionTextDisplayTime = 3.0f;
        [SerializeField] private Camera _camera = null;

        [SerializeField] private GameObject _flashLight = null;
        [SerializeField] private bool _flashlightOnAtStart = true;

        private Color damageScreenColor_temp;

        public static bool isPlayerDead = false;

        [HideInInspector]
        public Vector3 playerPosition;
        [HideInInspector]
        public Quaternion playerRotation;

        private GameObject playerBody;

        private Collider _collider = null;
        private GameSceneManager _gameSceneManager = null;
        private int _interactiveMask = 0;

        private InputManager input;

        #region utility objects
        private Rigidbody playerRigidbody;
        private FPSController controller;
        private CapsuleCollider playerCollider;
        private Sway sway;

        private Transform cameraHolder;

        private WeaponManager weaponManager;
        //Don't create any rigidbody here
        private Rigidbody rigidbody_temp;

        // Internals
        bool _inUse = false;
        float _currentFadeLevel = 1.0f;
        IEnumerator _coroutine = null;
        #endregion

        public PostProcessingProfile ppProfile;
        public bool changeValue;


        private void Start()
        {
            if (_screenFade)
            {
                Color color = _screenFade.color;
                color.a = _currentFadeLevel;
                _screenFade.color = color;
            }

            if (_missionText)
            {
                Invoke("HideMissionText", _missionTextDisplayTime);
            }

            //   _collider = GetComponent<Collider>();
            _collider = this.gameObject.transform.GetChild(2).GetChild(0).GetComponent<Collider>();
            _gameSceneManager = GameSceneManager.instance;
            _interactiveMask = 1 << LayerMask.NameToLayer("Interactive");
            cameraHolder = GameObject.Find("Camera Holder").GetComponent<Transform>();

            isPlayerDead = false;

            playerRigidbody = GetComponent<Rigidbody>();
            controller = GetComponent<FPSController>();
            // playerCollider = GetComponent<CapsuleCollider>();
            playerCollider = this.gameObject.transform.GetChild(2).GetChild(0).GetComponent<CapsuleCollider>();

            weaponManager = FindObjectOfType<WeaponManager>();
            sway = FindObjectOfType<Sway>();
            input = FindObjectOfType<InputManager>();

            if (!InputManager.useMobileInput)
            playerBody = FindObjectOfType<Body>().gameObject;


          
          
            
            //if (_gameSceneManager != null)
            //{
            //    PlayerInfo info = new PlayerInfo();
            //    info.camera = _camera;
            //    info.playerStats = this;
            //    info.collider = _collider;
            //   _gameSceneManager.RegisterPlayerInfo(_collider.GetInstanceID(), info);
            //}
            if (_flashLight)
                _flashLight.SetActive(_flashlightOnAtStart); ppProfile.vignette.enabled = _flashlightOnAtStart;



        }

        void Update()
        {
            if (isPlayerDead)
            {
                weaponManager.gameObject.SetActive(false);

                if (cameraHolder.transform.eulerAngles.x >= 90 || cameraHolder.transform.eulerAngles.x <= -90 || cameraHolder.transform.eulerAngles.z >= 90 || cameraHolder.transform.eulerAngles.z <= -90)
                {
                    if (rigidbody_temp)
                        rigidbody_temp.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }

            if (health == 0 && !isPlayerDead && !isGod)
            {
                PlayerDeath();
            }

            if (health < 0)
            {
                health = 0;
            }

            if(health >  100)
            {
                health = 100;
            }

           // HitObjectPriority();
            WritePlayerTransform();
            ConsumableManager(useConsumeSystem);
            DrawHealthStats();

            if (_inUse == false)
                DrawPlayerStats();

            if (Input.GetKeyDown(input.Flashlight))
            {
                if (_flashLight)
                    _flashLight.SetActive(!_flashLight.activeSelf); ppProfile.vignette.enabled = _flashLight.activeSelf;

                // ChangeDepthOfFieldAtRuntime(changeValue);

            }
           
        }

        public void DeactivateSpotLight(bool _vBool)
        {
            _flashLight.SetActive(_vBool); ppProfile.vignette.enabled = _vBool;
        }
        void ChangeDepthOfFieldAtRuntime(float val)
        {
            //copy current "depth of field" settings from the profile into a temporary variable
            DepthOfFieldModel.Settings deaptoffieldSettings = ppProfile.depthOfField.settings;
            deaptoffieldSettings.aperture += Time.deltaTime * val;
            //set the "depth of field" settings in the actual profile to the temp settings with the changed value
            ppProfile.depthOfField.settings = deaptoffieldSettings;
        }
        public void DoLevelComplete()
        {
            if (controller)
                controller.freezeMovement = true;

              Fade(4.0f, ScreenFadeType2.FadeOut);
              ShowMissionText("Mission Completed");
               //_playerStats.Invalidate(this);                             //actually this is for Health & Stamina , we dont need this anymore.
            

              Invoke("GameOver", 4.0f);

        }

        void GameOver()
        {
            // Show the cursor again
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (ApplicationManager.instance)
                ApplicationManager.instance.LoadMainMenu();
        }
        public void DoDeath()
        {
            if (controller)
                controller.freezeMovement = true;

            Fade(4.0f, ScreenFadeType2.FadeOut);
             ShowMissionText("Mission Failed");
            // _playerHUD.Invalidate(this);

            if (_flashLight)
                _flashLight.SetActive(!_flashLight.activeSelf);

            Invoke("GameOver", 4.0f);
        }
        public void Fade(float seconds, ScreenFadeType2 direction)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            float targetFade = 0.0f; ;

            switch (direction)
            {
                case ScreenFadeType2.FadeIn:
                    targetFade = 0.0f;
                    break;

                case ScreenFadeType2.FadeOut:
                    targetFade = 1.0f;
                    break;
            }

            _coroutine = FadeInternal(seconds, targetFade);
            StartCoroutine(_coroutine);
        }
        IEnumerator FadeInternal(float seconds, float targetFade)
        {
            if (!_screenFade) yield break;

            float timer = 0;
            float srcFade = _currentFadeLevel;
            Color oldColor = _screenFade.color;
            if (seconds < 0.1f) seconds = 0.1f;

            while (timer < seconds)
            {
                timer += Time.deltaTime;
                _currentFadeLevel = Mathf.Lerp(srcFade, targetFade, timer / seconds);
                oldColor.a = _currentFadeLevel;
                _screenFade.color = oldColor;
                yield return null;
            }

            oldColor.a = _currentFadeLevel = targetFade;
            _screenFade.color = oldColor;
        }
        public void ShowMissionText(string text)
        {
            if (_missionText)
            {
                _missionText.text = text;
                _missionText.gameObject.SetActive(true);
            }
        }
        public void ShowMessageText(string text)
        {
            if (_messagesDisplay)
            {
                _messagesDisplay.text = text;
                if (text.Length > 0)
                {
                    _inUse = true;
                }
                else { _inUse = false; }
               // _messagesDisplay.gameObject.SetActive(true);
            }
        }
        public void HideMissionText()
        {
            if (_missionText)
            {
                _missionText.gameObject.SetActive(false);
            }
        }
        public void ConsumableManager(bool useSystem)
        {
            if (!useSystem)
                return;

            if (Time.time > satietyTimer + satietySubstractionRate)
            {
                if (satiety <= 0)
                {
                    satiety = 0;
                    health -= hungerDamage;
                }

                satiety -= 1;
                satietyTimer = Time.time;
                
            }

            if (Time.time > hydratationTimer + hydratationSubstractionRate)
            {
                if (hydratation <= 0)
                {
                    hydratation = 0;
                    health -= thirstDamage;
                }
                hydratation -= 1;
                hydratationTimer = Time.time;
            }

            if(hydratation > 100)
            {
                hydratation = 100;
            }
            if(satiety > 100)
            {
                satiety = 100;
            }
        }

        public void DrawPlayerStats()
        {
            if(_messagesDisplay != null)
                _messagesDisplay.text = string.Format("--- Player statistic ---\n\n\n - Health: {0}\n\n - Hydratation: {1}\n\n - Satiety: {2}\n\n", health, hydratation, satiety);
        }

        public void ApplyDamage(int damage)
        {
            if (damage > 0)
            {
                health -= damage;
                damageScreenFX.color = damageScreenColor;
                damageScreenColor_temp = damageScreenColor;
                StartCoroutine(HitFX());
            }
        }

        public void AddSatiety(int points)
        {
            satiety += points;
        }

        public void AddHydratation(int points)
        {
            hydratation += points;
        }

        public void AddHealth(int hp)
        {
            health += hp;
        }

        private bool _gatePassKeyCode = false;
        public bool gatePass
        {
            get { return _gatePassKeyCode; }
            set { _gatePassKeyCode = value; }
        }


        IEnumerator HitFX()
        {
            while (damageScreenFX.color.a > 0)
            {
                damageScreenColor_temp = new Color(damageScreenColor_temp.r, damageScreenColor_temp.g, damageScreenColor_temp.b, damageScreenColor_temp.a -= damageScreenFadeSpeed * Time.deltaTime);
                damageScreenFX.color = damageScreenColor_temp;

                yield return new WaitForEndOfFrame();
            }
        }

        public void PlayerDeath()
        {
            if (!isPlayerDead)
            {
                DoDeath();

                var cameraTarget = GameObject.FindGameObjectWithTag("MainCamera").tag = "Untagged";

                sway.enabled = false;
                var leanController = FindObjectOfType<Lean>().enabled = false;
                controller.enabled = false;
                playerCollider.enabled = false;
                playerRigidbody.isKinematic = true;
                
                if(playerBody != null && !InputManager.useMobileInput)
                {
                    playerBody.SetActive(false);
                }

                if (!rigidbody_temp)
                {
                    rigidbody_temp = cameraHolder.gameObject.AddComponent<Rigidbody>();
                    rigidbody_temp.mass = 20;
                    cameraHolder.gameObject.AddComponent<SphereCollider>().radius = 0.3f;
                }
                else
                {
                    cameraHolder.GetComponent<SphereCollider>().enabled = true;
                }


                cameraHolder.transform.parent = null;

                rigidbody_temp.isKinematic = false;
                rigidbody_temp.AddTorque(cameraHolder.transform.forward * 10, ForceMode.Impulse);

                rigidbody_temp.constraints = RigidbodyConstraints.FreezeRotation;

                weaponManager.HideWeaponOnDeath();
                
                controller.lockCursor = false;

                isPlayerDead = true;

              //  if (_coroutine != null) StopCoroutine(_coroutine);

               // Destroy(this);
            }
            else
                return;
        }

        void WritePlayerTransform()
        {
            playerPosition = gameObject.transform.position;
            playerRotation = gameObject.transform.rotation;
        }

        void DrawHealthStats()
        {
            if (healthUIText != null)
                healthUIText.text = health.ToString();

            if (healthUISlider != null)
                healthUISlider.value = health;

            if (hungerUIText != null)
                hungerUIText.text = satiety.ToString();

            if (hungerSlider != null) hungerSlider.value = satiety;

            if (thirstUIText != null)
                thirstUIText.text = hydratation.ToString();

            if (thirstSlider != null) thirstSlider.value = hydratation;


        }
    }

}