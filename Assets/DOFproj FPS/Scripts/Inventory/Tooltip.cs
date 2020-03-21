/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DOFprojFPS
{

    public class Tooltip : MonoBehaviour
    {
        public Text tooltip;
        public Text header;
        [HideInInspector]
        public RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            rectTransform.position = Input.mousePosition;
        }

        public void UpdatePos()
        {
            rectTransform.position = Input.mousePosition;
        }

        public void GenerateContent(Item item)
        {
            string tooltip;
            string header;

            if (item != null)
            {
                header = item.title;
                tooltip = item.description;
            }
            else
            {
                header = string.Empty;
                tooltip = string.Empty;
            }

            this.header.text = header;
            this.tooltip.text = tooltip;
        }
    }
}
