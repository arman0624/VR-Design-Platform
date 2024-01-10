using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class GrabMoveInput : MonoBehaviour
    {
        [SerializeField]
        [Tooltip(
            "The Input System Action that will be used to perform grab movement while held. Must be a Button Control.")]
        InputActionProperty m_GrabMoveAction =
            new InputActionProperty(new InputAction("Grab Move", type: InputActionType.Button));

        // The Grab Move Component we are bound to
        private GrabMove m_GrabMove;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void Awake()
        {
            m_GrabMove = GetComponent<GrabMove>();
            m_GrabMoveAction.action.started += OnGrabMoveAction;
            m_GrabMoveAction.action.canceled += OnGrabMoveCanceled;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnEnable()
        {
            m_GrabMoveAction.action.Enable();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDisable()
        {
            m_GrabMoveAction.action.Disable();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected void OnDestroy()
        {
            m_GrabMoveAction.action.started -= OnGrabMoveAction;
            m_GrabMoveAction.action.canceled -= OnGrabMoveCanceled;
        }

        private void OnGrabMoveAction(InputAction.CallbackContext obj)
        {
            m_GrabMove.IsMoving = true;
        }

        private void OnGrabMoveCanceled(InputAction.CallbackContext obj)
        {
            m_GrabMove.IsMoving = false;
        }

    }
}
