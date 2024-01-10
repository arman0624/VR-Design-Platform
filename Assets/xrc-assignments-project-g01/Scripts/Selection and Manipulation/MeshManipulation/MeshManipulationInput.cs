using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class MeshManipulationInput : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The manipulation action.")]
        private InputActionProperty m_ManipulateMeshAction;
        
        [SerializeField] 
        [Tooltip("The Sphere Select.")]
        private SphereSelect m_SphereSelect;
        
        /// <summary>
        /// Callback action to indicate the mesh manipulation has started
        /// </summary>
        public event Action OnStartMeshManipulation;
        
        /// <summary>
        /// Callback action to indicate the mesh manipulation has ended
        /// </summary>
        public event Action OnEndMeshManipulation;

        private MeshManipulation m_MeshManipulation;

        private bool m_IsManipulating = false;

        // Start is called before the first frame update
        void Start()
        {
            m_MeshManipulation = GetComponent<MeshManipulation>();

            m_ManipulateMeshAction.action.performed += OnMeshManipulatePerformed;
        }

        private void OnEnable()
        {
            m_ManipulateMeshAction.action.Enable();
        }

        private void OnDisable()
        {
            m_ManipulateMeshAction.action.Disable();
        }

        private void OnDestroy()
        {
            m_ManipulateMeshAction.action.performed -= OnMeshManipulatePerformed;
        }

        /// <summary>
        /// Callback to start the mesh manipulation.
        /// The button is pressed only once to enter the state, then pressed again to exit the state.
        /// </summary>
        /// <param name="obj"></param>
        private void OnMeshManipulatePerformed(InputAction.CallbackContext obj)
        {
            if (m_IsManipulating && m_SphereSelect.interactor.interactablesSelected.Count == 0)
            {
                m_MeshManipulation.ManipulationStopped();
                m_SphereSelect.UndoRedoEnabled = true;
                m_IsManipulating = false;
                OnEndMeshManipulation?.Invoke();
            }
            else
            {
                IXRInteractable interactable = m_SphereSelect.GetFirstSelected();
                if (interactable != null)
                {
                    MeshFilter mesh = interactable.transform.gameObject.GetComponent<MeshFilter>();
                    if (mesh != null)
                    {
                        m_SphereSelect.CancelSelect();
                        m_SphereSelect.UndoRedoEnabled = false;
                        m_MeshManipulation.ManipulationStarted(mesh);
                        m_IsManipulating = true;
                        OnStartMeshManipulation?.Invoke();
                    }
                }
            }
        }
    }
}