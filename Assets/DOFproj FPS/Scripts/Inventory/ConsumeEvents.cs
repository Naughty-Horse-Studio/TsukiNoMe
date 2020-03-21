/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class automaticly creates listener for item use event.
/// When we will consume some item we will use reciver method from that script
/// </summary>

namespace DOFprojFPS {

    public class ConsumeEvents : MonoBehaviour
    {
        public enum ConsumableEvents { addHealth, addSatiety, addHydratation, passCode }

        [Header("What reference should I add?")]
        public ConsumableEvents m_Event;

        public int pointsToAdd;

        private PlayerStats playerStats;
        private Item item;

        UnityAction addHealth, addSatiety, addHydratation, passCode;

        private void OnEnable()
        {
            playerStats = FindObjectOfType<PlayerStats>();
            item = GetComponent<Item>();

            addHealth += AddHealth;
            addHydratation += AddHydratation;
            addSatiety += AddSatiety;
            passCode += ActivatePasscode;

            switch (m_Event)
            {
                case ConsumableEvents.addHealth:
                    item.onUseEvent.AddListener(addHealth);
                    break;
                case ConsumableEvents.addHydratation:
                    item.onUseEvent.AddListener(addHydratation);
                    break;
                case ConsumableEvents.addSatiety:
                    item.onUseEvent.AddListener(addSatiety);
                    break;
                case ConsumableEvents.passCode:
                    item.onUseEvent.AddListener(passCode);
                    break;
            }
        }

        public void AddHealth()
        {
            playerStats.AddHealth(pointsToAdd);
        }

        public void AddSatiety()
        {
            playerStats.AddSatiety(pointsToAdd);
        }

        public void AddHydratation()
        {
            playerStats.AddHydratation(pointsToAdd);
        }

        public void ActivatePasscode()
        {
            playerStats.gatePass = true;
        }

    }
}
