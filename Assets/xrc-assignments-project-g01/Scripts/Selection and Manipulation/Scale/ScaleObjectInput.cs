using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
    /// <Summary>
    /// Scale object input specifically covers cases for when sphere select touches
    /// any interactable and the grip button on right controller is selected (Sphere select).
    /// The input action property for scale axes is the secondary button on left controller.  
    /// </Summary>
{
    public class ScaleObjectInput : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The Input System Action that will be used to enable scaling axes.")]
        private InputActionProperty m_ScaleAxesAction = 
            new InputActionProperty(new InputAction("Scale Axes", type: InputActionType.Button));

        private ScaleObject m_ScaleObject;

        [SerializeField] 
        [Tooltip("The Direct Interactor.")]
        private SphereSelect m_SphereSelect;

        /// <summary>
        /// Callback action to indicate the mesh manipulation has started
        /// </summary>
        public event Action OnStartScaleAxes;
        
        /// <summary>
        /// Callback action to indicate the mesh manipulation has ended
        /// </summary>
        public event Action OnEndScaleAxes;
        
        private bool m_isScalable = false;

        private void Start()
        {
            m_ScaleAxesAction.action.performed += ScaleObjectPerformed;
            
            m_ScaleObject = GetComponent<ScaleObject>();
        }
        
        /// <summary>
        /// Get input through Sphere Select as well as if Scale Axes secondary button is
        /// pressed while interactable is held in Sphere Select. If multiple interactables,
        /// grab the first one. 
        /// </summary>
        private void ScaleObjectPerformed(InputAction.CallbackContext obj)
        {
            if (m_isScalable && m_SphereSelect.interactor.interactablesSelected.Count == 0)
            {
                m_ScaleObject.StopScaleObject();
                m_isScalable = false;
                m_SphereSelect.UndoRedoEnabled = true;
                OnEndScaleAxes?.Invoke();
            }
            else
            {
                // Call get first selected in sphere select to grab first interactable in list
                IXRInteractable interactable = m_SphereSelect.GetFirstSelected();
            
                if (interactable != null)
                {
                    // set mesh to the interactable mesh filter and call start scale object function with it
                    MeshFilter mesh = interactable.transform.gameObject.GetComponent<MeshFilter>(); 
                    if (mesh != null)
                    {
                        m_SphereSelect.CancelSelect();
                        m_ScaleObject.StartScaleObject(mesh);
                        m_isScalable = true;
                        m_SphereSelect.UndoRedoEnabled = false;
                        OnStartScaleAxes?.Invoke();
                    }
                }
            }
        }
        
        private void OnDestroy()
        {
            m_ScaleAxesAction.action.performed -= ScaleObjectPerformed; 
        }
        
        private void OnEnable()
        {
            m_ScaleAxesAction.action.Enable();
        }

        private void OnDisable()
        {
            m_ScaleAxesAction.action.Disable();
        }
    }
    
}