using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class HelpAndDocumentationInput: MonoBehaviour
    {
        [SerializeField] private InputActionProperty HelpAndDocumentationObjectAction = new InputActionProperty(new InputAction("Open Help Menu", type: InputActionType.Button)); //Tracks the menu input
        private HelpAndDocumentation m_HelpAndDocumentation;
        
        /// <summary>
        /// Callback action to indicate the help menu has opened
        /// </summary>
        public event Action OnHelpAndDocumentationOpen;
        
        /// <summary>
        /// Callback action to indicate the help menu has closed
        /// </summary>
        public event Action OnHelpAndDocumentationClose;
        private void Awake()
        {
            //m_CreateObject = GetComponent<CreateObject>();
            m_HelpAndDocumentation = GetComponent<HelpAndDocumentation>();
            HelpAndDocumentationObjectAction.action.performed += OnHelpAndDocumentation; 
        }
        
        private void OnEnable()
        {
            HelpAndDocumentationObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            HelpAndDocumentationObjectAction.action.Disable();
        }

        private void OnDestroy()
        {
            HelpAndDocumentationObjectAction.action.performed -= OnHelpAndDocumentation; 
        }
        
        /// <summary>
        /// This function is called when the left controller thumbstick button is pressed. If the menu
        /// is already open, it closes it. If it's not open, it opens it
        /// </summary>
        /// <param name="context"></param>
        private void OnHelpAndDocumentation(InputAction.CallbackContext context) 
        {
            if(!m_HelpAndDocumentation.menuIsOpen)
            {
                m_HelpAndDocumentation.OpenHelpAndDocumentationMenu();
                OnHelpAndDocumentationOpen?.Invoke();
            }
            else
            {
                m_HelpAndDocumentation.CloseHelpAndDocumentationMenu();
                OnHelpAndDocumentationClose?.Invoke();
            }
            
        }
    }
}