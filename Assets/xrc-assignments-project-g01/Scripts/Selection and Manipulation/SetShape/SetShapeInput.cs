using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class SetShapeInput: MonoBehaviour
    {
        [SerializeField] private InputActionProperty setShapeObjectAction = new InputActionProperty(new InputAction("Select Object Shape", type: InputActionType.Button)); //Tracks the menu input
        private SetShape m_SetShape;
        
        /// <summary>
        /// Callback action to indicate the shape selection has begun
        /// </summary>
        public event Action OnStartSelectShape;
        
        /// <summary>
        /// Callback action to indicate the shape selection has ended
        /// </summary>
        public event Action OnEndSelectShape;
        private void Awake()
        {
            //m_CreateObject = GetComponent<CreateObject>();
            m_SetShape = GetComponent<SetShape>();
            setShapeObjectAction.action.performed += OnSetObjectShape; 
        }
        
        private void OnEnable()
        {
            setShapeObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            setShapeObjectAction.action.Disable();
        }

        private void OnDestroy()
        {
            setShapeObjectAction.action.performed -= OnSetObjectShape; 
        }
        
        /// <summary>
        /// This function is called when the left controller primary button is pressed. If the menu
        /// is already open, it closes it. If it's not open, it opens it
        /// </summary>
        /// <param name="context"></param>
        private void OnSetObjectShape(InputAction.CallbackContext context) 
        {
            if(!m_SetShape.menuIsOpen)
            {
                m_SetShape.OpenSetObjectShapeMenu();
                OnStartSelectShape?.Invoke();
            }
            else
            {
                m_SetShape.CloseSetObjectShapeMenu();
                OnEndSelectShape?.Invoke();
            }
            
        }
    }
}