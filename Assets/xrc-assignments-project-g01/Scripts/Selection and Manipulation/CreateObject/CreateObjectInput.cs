using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class CreateObjectInput: MonoBehaviour
    {
        [SerializeField] private InputActionProperty createObjectAction = new InputActionProperty(new InputAction("Create Object", type: InputActionType.Button)); //Tracks the right trigger input
        private CreateObject m_CreateObject;
        private void Awake()
        {
            m_CreateObject = GetComponent<CreateObject>();
            createObjectAction.action.performed += OnCreateObject; 
            createObjectAction.action.canceled += OnCreateObjectCanceled;
        }
        
        private void OnEnable()
        {
            createObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            createObjectAction.action.Disable();
        }

        private void OnDestroy()
        {
            createObjectAction.action.performed -= OnCreateObject; 
            createObjectAction.action.canceled -= OnCreateObjectCanceled;
        }

        private void OnCreateObject(InputAction.CallbackContext context) 
        {
            m_CreateObject.OnCreateObject();
        }
        
        private void OnCreateObjectCanceled(InputAction.CallbackContext context) 
        {
            m_CreateObject.OnCreateObjectCanceled();
        }
        
    }
}