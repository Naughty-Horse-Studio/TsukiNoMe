/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---

using UnityEngine;
using UnityEngine.EventSystems;

namespace DOFprojFPS.MobileInput
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [Header("Options")]
        [Range(0f, 2f)] public float handleLimit = 1f;

        public static Vector2 inputVector = Vector2.zero;

        [Header("Components")]
        public RectTransform background;
        public RectTransform handle;

        public float Horizontal { get { return inputVector.x; } }
        public float Vertical { get { return inputVector.y; } }
        public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

        Vector2 joystickPosition = Vector2.zero;
        private Camera cam = new Camera();

        void Start()
        {
            joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 direction = eventData.position - joystickPosition;
            inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
            handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
            InputManager.joystickInputVector = inputVector;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            InputManager.joystickInputVector = inputVector;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
