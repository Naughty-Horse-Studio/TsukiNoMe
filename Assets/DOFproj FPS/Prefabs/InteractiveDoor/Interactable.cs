/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.UI;
using DOFprojFPS;
public class Interactable : MonoBehaviour, IInteractableTarget, IInteractableMessage
{
        [Tooltip("The ID of the Interactable, used by the Interact ability for filtering. A value of -1 indicates no ID.")]
        [SerializeField] protected int m_ID = -1;
        [SerializeField] private PlayerStats _playerStats = null;
        [Tooltip("The object(s) that the interaction is performend on. This component must implement the IInteractableTarget.")]
        [SerializeField] protected MonoBehaviour[] m_Targets;

        public int ID { get { return m_ID; } set { m_ID = value; } }
        public MonoBehaviour[] Targets { get { return m_Targets; } set { m_Targets = value; } }

        public IInteractableTarget[] m_InteractableTargets;
    //private AbilityIKTarget[] m_IKTargets;


    //public AbilityIKTarget[] IKTargets { get { return m_IKTargets; } }

    /// <summary>
    /// Initialize the default values.
    /// </summary>

    bool mIsinPerimeter;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
            mIsinPerimeter = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            mIsinPerimeter = false; _playerStats.ShowMessageText("");
    }


    private void Update()
    {
       if ((_playerStats != null)&&(mIsinPerimeter)) { _playerStats.ShowMessageText("Press 'Use' to open/close Door."); }
        //if (CanInteract(this.gameObject) && mIsinPerimeter)
        //{
        //   statplayerText.text = "Press 'Use' to open/close Door.";
            if (Input.GetKeyDown(KeyCode.F) && mIsinPerimeter)
            {
                if (CanInteract(this.gameObject))
                {
                    Interact(this.gameObject);
                }
            }
        //}
    }
    private void Awake()
        {
            if (m_Targets == null || m_Targets.Length == 0) {
                Debug.LogError("Error: An IInteractableTarget must be specified in the Targets field.");
                return;
            }

            m_InteractableTargets = new IInteractableTarget[m_Targets.Length];
            for (int i = 0; i < m_Targets.Length; ++i) {
                if (m_Targets[i] == null || !(m_Targets[i] is IInteractableTarget)) {
                    Debug.Log("Error: element " + i + " of the Targets array is null or does not subscribe to the IInteractableTarget iterface.");
                } else {
                    m_InteractableTargets[i] = m_Targets[i] as IInteractableTarget;
                }
            }


        _playerStats = FindObjectOfType<PlayerStats>();
        //m_IKTargets = GetComponentsInChildren<AbilityIKTarget>();
    }

        /// <summary>
        /// Determines if the character can interact with the InteractableTarget.
        /// </summary>
        /// <param name="character">The character that wants to interactact with the target.</param>
        /// <returns>True if the character can interact with the InteractableTarget</returns>
        public bool CanInteract(GameObject character)
        {
        for (int i = 0; i < m_InteractableTargets.Length; ++i)
        {
            if (m_InteractableTargets[i] == null || !m_InteractableTargets[i].CanInteract(character))
            {
                return false;
            }
        }

        return true;
        }

        /// <summary>
        /// Performs the interaction.
        /// </summary>
        /// <param name="character">The character that wants to interactact with the target.</param>
        public void Interact(GameObject character)
        {

            for (int i = 0; i < m_InteractableTargets.Length; ++i) {
                m_InteractableTargets[i].Interact(character);
            }
        }

        /// <summary>
        /// Returns the message that should be displayed when the object can be interacted with.
        /// </summary>
        /// <returns>The message that should be displayed when the object can be interacted with.</returns>
        public string AbilityMessage()
        {
            if (m_InteractableTargets != null) {
                for (int i = 0; i < m_InteractableTargets.Length; ++i) {
                    // Returns the message from the first IInteractableMessage object.
                    if (m_InteractableTargets[i] is IInteractableMessage) {
                        return (m_InteractableTargets[i] as IInteractableMessage).AbilityMessage();
                    }
                }
            }
            return string.Empty;
        }
    }
