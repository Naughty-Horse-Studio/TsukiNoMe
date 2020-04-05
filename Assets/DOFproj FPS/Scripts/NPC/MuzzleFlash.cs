/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.

/// ---------------------------------------------

using UnityEngine;


    /// <summary>
    /// Shows an object which slowly fades out with time. Can optionally attach a light to the GameObject and that light will be faded as well.
    /// </summary>
    public class MuzzleFlash : MonoBehaviour
    {
        [Tooltip("The name of the shader tint color property.")]
        [SerializeField] protected string m_TintColorPropertyName = "_TintColor";

        [Tooltip("The alpha value to initialize the muzzle flash material to.")]
        [Range(0, 1)] [SerializeField] protected float m_StartAlpha = 0.5f;
        [Tooltip("The minimum fade speed - the larger the value the quicker the muzzle flash will fade.")]
        [SerializeField] protected float m_MinFadeSpeed = 3;
        [Tooltip("The maximum fade speed - the larger the value the quicker the muzzle flash will fade.")]
        [SerializeField] protected float m_MaxFadeSpeed = 4;

        private GameObject m_GameObject;
#if FIRST_PERSON_CONTROLLER && THIRD_PERSON_CONTROLLER
        private Transform m_Transform;
#endif
        private Material m_Material;
        private Light m_Light;
        private ParticleSystem m_Particles;

        private int m_TintColorPropertyID;
        private Color m_Color;
        private float m_StartLightIntensity;
        private float m_FadeSpeed;
        private float m_TimeScale = 1;

        private GameObject m_Character;

        private bool m_Pooled;
        private int m_StartLayer;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_GameObject = gameObject;

            m_TintColorPropertyID = Shader.PropertyToID(m_TintColorPropertyName);

            var renderer = GetComponent<Renderer>();
            if (renderer != null) {
                m_Material = renderer.sharedMaterial;
            }
            m_Light = GetComponent<Light>();
            m_Particles = GetComponent<ParticleSystem>();
            // If a light exists set the start light intensity. Every time the muzzle flash is enabed the light intensity will be reset to its starting value.
            if (m_Light != null) {
                m_StartLightIntensity = m_Light.intensity;
            }
            m_StartLayer = m_GameObject.layer;
        }

        /// <summary>
        /// The muzzle flash has been enabled.
        /// </summary>
        private void OnEnable()
        {
            m_Color.a = 0;
            if (m_Material != null) {
                m_Material.SetColor(m_TintColorPropertyID, m_Color);
            }
            if (m_Light != null) {
                m_Light.intensity = 0;
            }
        }

        /// <summary>
        /// A weapon has been fired and the muzzle flash needs to show. Set the starting alpha value and light intensity if the light exists.
        /// </summary>
        /// <param name="item">The item that the muzzle flash is attached to.</param>
        /// <param name="itemActionID">The ID which corresponds to the ItemAction that spawned the muzzle flash.</param>
        /// <param name="pooled">Is the muzzle flash pooled?</param>
        /// <param name="characterLocomotion">The character that the muzzle flash is attached to.</param>
        //public void Show(Item item, int itemActionID, bool pooled, UltimateCharacterLocomotion characterLocomotion)
        //{
        //    // The muzzle flash may be inactive if the object isn't pooled.
        //    if (!m_Pooled) {
        //        m_GameObject.SetActive(true);
        //    }

        //    if (m_Character == null && characterLocomotion != null) {
        //        m_Character = characterLocomotion.gameObject;

        //        EventHandler.RegisterEvent<float>(m_Character, "OnCharacterChangeTimeScale", OnChangeTimeScale);

        //    }


        //    m_Pooled = pooled;
        //    if (characterLocomotion != null) {
        //        m_TimeScale = characterLocomotion.TimeScale;
        //        m_GameObject.layer = characterLocomotion.FirstPersonPerspective ? LayerManager.Overlay : m_StartLayer;
        //    } else {
        //        m_TimeScale = 1;
        //        m_GameObject.layer = m_StartLayer;
        //    }

        //    m_Color = Color.white;
        //    m_Color.a = m_StartAlpha;
        //    if (m_Material != null) {
        //        m_Material.SetColor(m_TintColorPropertyID, m_Color);
        //    }
        //    m_FadeSpeed = Random.Range(m_MinFadeSpeed, m_MaxFadeSpeed);
        //    if (m_Light != null) {
        //        m_Light.intensity = m_StartLightIntensity;
        //    }
        //    if (m_Particles != null) {
        //        m_Particles.Play(true);
        //    }
        //    // The muzzle flash may be inactive if the object isn't pooled.
        //    if (!m_Pooled) {
        //        m_GameObject.SetActive(true);
        //    }
        //}
        
        /// <summary>
        /// Decrease the alpha value of the muzzle flash to give it a fading effect. As soon as the alpha value reaches zero place the muzzle flash back in
        /// the object pool. If a light exists decrease the intensity of the light as well.
        /// </summary>
        private void Update()
        {
            if (m_Color.a > 0) {
                m_Color.a = Mathf.Max(m_Color.a - (m_FadeSpeed * Time.deltaTime * m_TimeScale), 0);
                if (m_Material != null) {
                    m_Material.SetColor(m_TintColorPropertyID, m_Color);
                }
                // Keep the light intensity synchronized with the alpha channel's value.
                if (m_Light != null) {
                    m_Light.intensity = m_StartLightIntensity * (m_Color.a / m_StartAlpha);
                }
            } else {
                if (m_Pooled) {
                  //  ObjectPool.Destroy(m_GameObject);
                } else {
                    m_GameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// The character's local timescale has changed.
        /// </summary>
        /// <param name="timeScale">The new timescale.</param>
        private void OnChangeTimeScale(float timeScale)
        {
            m_TimeScale = timeScale;
        }



        /// <summary>
        /// The object has been disabled.
        /// </summary>
        private void OnDisable()
        {
            if (m_Pooled && m_Character != null) {
            //    EventHandler.UnregisterEvent<float>(m_Character, "OnCharacterChangeTimeScale", OnChangeTimeScale);

                m_Character = null;
            }
        }
    }