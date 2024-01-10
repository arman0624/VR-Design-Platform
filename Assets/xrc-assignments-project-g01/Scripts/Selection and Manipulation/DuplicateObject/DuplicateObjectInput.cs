using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class DuplicateObjectInput : MonoBehaviour
    {
        [SerializeField] private InputActionProperty duplicateSelectedObjectAction = new InputActionProperty(new InputAction("Duplicate Selected Object", type: InputActionType.Button)); //Tracks the trigger input
        private DuplicateObject selectedDuplicateObject;
        private void Awake()
        {
            selectedDuplicateObject = GetComponent<DuplicateObject>();
            duplicateSelectedObjectAction.action.performed += OnDuplicateObject; 
        }

        private void OnEnable()
        {
            duplicateSelectedObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            duplicateSelectedObjectAction.action.Disable();
        }

        private void OnDestroy()
        {
            duplicateSelectedObjectAction.action.performed -= OnDuplicateObject; 
        }

        private void OnDuplicateObject(InputAction.CallbackContext context) 
        {
            selectedDuplicateObject.OnDuplicateObject();
        }
    }
}