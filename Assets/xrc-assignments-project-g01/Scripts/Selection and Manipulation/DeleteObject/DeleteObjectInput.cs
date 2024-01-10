using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class DeleteObjectInput : MonoBehaviour
    {
        [SerializeField] 
        private InputActionProperty deleteSelectedObjectAction = new InputActionProperty(new InputAction("Delete Selected Object", type: InputActionType.Button)); //Tracks the b input
        private DeleteObject selectedDeleteObject;
        
        /// <summary>
        /// Callback action to indicate the deletion has been completed
        /// </summary>
        public event Action OnDeleteObjectCompleted;
        
        private void Awake()
        {
            selectedDeleteObject = GetComponent<DeleteObject>();
            deleteSelectedObjectAction.action.performed += OnDeleteObject; 
        }
        
        private void OnEnable()
        {
            deleteSelectedObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            deleteSelectedObjectAction.action.Disable();
        }

        private void OnDestroy()
        {
            deleteSelectedObjectAction.action.performed -= OnDeleteObject; 
        }

        private void OnDeleteObject(InputAction.CallbackContext context) 
        {
            selectedDeleteObject.OnDeleteObject();
            OnDeleteObjectCompleted?.Invoke();
        }
    }
}