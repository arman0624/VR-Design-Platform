using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class ColorObjectInput : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The Input System Action that will be used to perform Color Object.")]
        private InputActionProperty m_ColorObjectAction = 
            new InputActionProperty(new InputAction("Color Object", type: InputActionType.Button));
        
        [SerializeField]
        [Tooltip("The Input System Action that will be used to perform Color Object.")]
        private InputActionProperty m_ConfirmColorObjectAction = 
            new InputActionProperty(new InputAction("Confirm Color Object", type: InputActionType.Button));

        [SerializeField] 
        [Tooltip("The Direct Intertactor.")]
        private SphereSelect m_SphereSelect;
        
        /// <summary>
        /// Callback action to indicate the mesh manipulation has started
        /// </summary>
        public event Action OnStartColorObject;
        
        /// <summary>
        /// Callback action to indicate the mesh manipulation has ended
        /// </summary>
        public event Action OnEndColorObject;
        
        /// The logic component
        private ColorObject m_ColorObject;
        private bool m_CanSelect = false;

        private void Start()
        {
            
            m_ColorObjectAction.action.performed += ColorObjectPerformed;
            m_ColorObjectAction.action.canceled += ColorObjectCanceled;

            m_ConfirmColorObjectAction.action.performed += ColorObjectSelectionPerformed;

            m_ColorObject = GetComponent<ColorObject>();
        }

        private void OnDestroy()
        {
            m_ColorObjectAction.action.performed -= ColorObjectPerformed;
            m_ColorObjectAction.action.canceled -= ColorObjectCanceled;
            
            m_ConfirmColorObjectAction.action.performed -= ColorObjectSelectionPerformed;
        }

        private void OnEnable()
        {
            m_ColorObjectAction.action.Enable();
            m_ConfirmColorObjectAction.action.Enable();
        }

        private void OnDisable()
        {
            m_ColorObjectAction.action.Disable();
            m_ConfirmColorObjectAction.action.Disable();
        }

        /// <summary>
        /// Ony start coloring an object if an interactable with a mesh renderer is selected
        /// </summary>
        /// <param name="obj"></param>
        private void ColorObjectPerformed(InputAction.CallbackContext obj)
        {
            IXRInteractable interactable = m_SphereSelect.GetFirstSelected();
            if (interactable != null)
            {
                MeshRenderer mesh = interactable.transform.gameObject.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    m_SphereSelect.CancelSelect();
                    m_ColorObject.StartColorObject(mesh);
                    m_CanSelect = true;
                    OnStartColorObject?.Invoke();
                }
            }
        }

        /// <summary>
        /// Handling of stopping the color selection
        /// </summary>
        /// <param name="obj"></param>
        private void ColorObjectCanceled(InputAction.CallbackContext obj)
        {
            if (m_CanSelect)
            {
                m_ColorObject.StopColorObject();
                m_CanSelect = false;
                OnEndColorObject?.Invoke();
            }
        }

        /// <summary>
        /// Handling of the action of selecting a color on the color wheel
        /// </summary>
        /// <param name="obj"></param>
        private void ColorObjectSelectionPerformed(InputAction.CallbackContext obj)
        {
            if (m_CanSelect)
            {
                m_ColorObject.ApplySelectedColor();
            }
        }
    }
}
