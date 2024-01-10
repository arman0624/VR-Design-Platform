using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class UndoRedoInput : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The undo action.")]
        private InputActionProperty m_UndoAction;

        [SerializeField]
        [Tooltip("The redo action.")]
        private InputActionProperty m_RedoAction;

        // Start is called before the first frame update
        private void Start()
        {
            m_UndoAction.action.performed += UndoPerformed;
            m_RedoAction.action.performed += RedoPerformed;
        }

        private void OnDestroy()
        {
            m_UndoAction.action.performed -= UndoPerformed;
            m_RedoAction.action.performed -= RedoPerformed;
        }

        private void OnEnable()
        {
            m_UndoAction.action.Enable();
            m_RedoAction.action.Enable();
        }
        
        private void OnDisable()
        {
            m_UndoAction.action.Disable();
            m_RedoAction.action.Disable();
        }

        private void UndoPerformed(InputAction.CallbackContext obj)
        {
            UndoRedo.Instance.Undo();
        }

        private void RedoPerformed(InputAction.CallbackContext obj)
        {
            UndoRedo.Instance.Redo();
        }
    }
}
