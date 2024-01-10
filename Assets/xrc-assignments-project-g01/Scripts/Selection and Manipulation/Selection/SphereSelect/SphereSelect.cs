using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    /// <summary>
    /// This component contains the main logic for the Sphere Select interaction technique.
    /// It holds a reference to an interactor responsible for selecting the object of interest.
    /// The Sphere Select technique modifies the interactor's attach transform, as well as its collider,
    /// based on the SphereSelect component's property values.
    /// </summary>
    public class SphereSelect : MonoBehaviour
    {
        // The interactor responsible for selecting an object
        [SerializeField] private XRDirectInteractor m_Interactor;
        public XRDirectInteractor interactor => m_Interactor;
        
        // The maximum radius of the sphere.
        [SerializeField] private float m_MaxRadius;
        public float maxRadius => m_MaxRadius;
        
        // The minimum radius of the sphere.
        [SerializeField] private float m_MinRadius;
        public float minRadius => m_MinRadius;
        
        // The positional offset of the sphere, relative to the interactor's transform.
        [SerializeField] private Vector3 m_Offset;
        public Vector3 offset => m_Offset;

        
        // The radius of the sphere, clamped between minRadius and maxRadius.
        [SerializeField] private float m_Radius;
        
        // Defines whether the visual sphere select component is enabled or not.
        public bool IsVisualSphereSelectEnabled => m_IsVisualSphereSelectEnabled;
        private bool m_IsVisualSphereSelectEnabled = true;
        
        public float Radius
        {
            get => m_InteractorCollider.radius;
            set => m_InteractorCollider.radius = value;
        }
        
        // Reference to the interactor's sphere collider to update the radius value.
        private SphereCollider m_InteractorCollider;

        // Reference to the currently selected objects and their initial position
        private Dictionary<IXRSelectInteractable, (Vector3, Quaternion)> m_SelectedWorldPosition 
            = new Dictionary<IXRSelectInteractable, (Vector3, Quaternion)>();

        public bool UndoRedoEnabled = true;
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Initiates the reference to the sphere collider as well as the feedback component.
        /// Also set the offset of the sphere collider and initialize the sphere visualization state.
        /// </summary>
        public void Awake()
        {
            if (m_Interactor == null)
            {
                m_Interactor = GetComponent<XRDirectInteractor>();
            }

            m_Interactor.selectEntered.AddListener(SelectEnteredListener);
            m_Interactor.selectExited.AddListener(SelectExitedListener);
            m_InteractorCollider = m_Interactor.GetComponent<SphereCollider>();
            
            // Set the sphere collider's center to the offset.
            m_InteractorCollider.center = m_Offset;
            m_Interactor.attachTransform.localPosition = m_Offset;
            
            // Update the sphere collider's radius.
            m_InteractorCollider.radius = m_Radius;
        }

        private void OnDestroy()
        {
            m_Interactor.selectEntered.RemoveListener(SelectEnteredListener);
            m_Interactor.selectExited.RemoveListener(SelectExitedListener);
        }

        /// <summary>
        ///  Listener to the select entered event to select multiple objects at the same time
        /// </summary>
        /// <param name="args"></param>
        private void SelectEnteredListener(SelectEnterEventArgs args)
        {
            // Remember the first position
            Vector3 position = args.interactableObject.transform.position;
            Quaternion rotation = args.interactableObject.transform.rotation;
            m_SelectedWorldPosition.Add(args.interactableObject, (position, rotation));
            
            List<IXRInteractable> targets = new List<IXRInteractable>();
            m_Interactor.GetValidTargets(targets);
            foreach (var target in targets)
            {
                if (target is XRBaseInteractable interactable)
                {
                    // Make sure we are not selecting the same object twice
                    // Make sure we can select the interactable (Useful for layer masks)
                    bool isFocusPossible = args.manager.IsFocusPossible(args.interactorObject, interactable);
                    if (!interactable.IsSelected(args.interactorObject) && isFocusPossible)
                    {
                        args.manager.SelectEnter(args.interactorObject, interactable);
                    }
                }
            }
            
            // Disable the sphere UI
            m_IsVisualSphereSelectEnabled = m_SelectedWorldPosition.Count == 0;
        }

        /// <summary>
        /// Listener to the Select Exit event.
        /// Handles cancellation.
        /// </summary>
        /// <param name="args"></param>
        private void SelectExitedListener(SelectExitEventArgs args)
        {
            if (m_IsVisualSphereSelectEnabled)
            {
                return;
            }
            
            if (args.isCanceled)
            {
                foreach (var pair in m_SelectedWorldPosition)
                {
                    pair.Key.transform.position = pair.Value.Item1;
                    pair.Key.transform.rotation = pair.Value.Item2;
                }
            }
            else if (UndoRedoEnabled)
            {
                // Add the command
                List<Transform> selectedTransforms = new List<Transform>();
                List<Vector3> selectedStartPositions = new List<Vector3>();
                List<Quaternion> selectedStartRotations = new List<Quaternion>();
                foreach (var pair in m_SelectedWorldPosition)
                {
                    selectedTransforms.Add(pair.Key.transform);
                    selectedStartPositions.Add(pair.Value.Item1);
                    selectedStartRotations.Add(pair.Value.Item2);
                }
                SphereSelectCommand command = new SphereSelectCommand(selectedTransforms, selectedStartPositions, selectedStartRotations);
                UndoRedo.Instance.AddCommand(command);
            }

            m_SelectedWorldPosition.Clear();
            
            // Enable the UI
            m_IsVisualSphereSelectEnabled = m_SelectedWorldPosition.Count == 0;
        }

        /// <summary>
        /// Returns the first object selected.
        /// When there are no object selected, returns null.
        /// </summary>
        /// <returns>IXRInteractable</returns>
        public IXRInteractable GetFirstSelected()
        {
            List<IXRInteractable> targets = new List<IXRInteractable>(m_Interactor.interactablesSelected);
            if (targets.Count < 1)
            {
                return null;
            }

            return targets[0];
        }

        /// <summary>
        /// Cancels the current selection
        /// </summary>
        public void CancelSelect()
        {
            List<IXRInteractable> targets = new List<IXRInteractable>(m_Interactor.interactablesSelected);
            foreach (IXRInteractable target in targets)
            {
                if (target is XRBaseInteractable interactable)
                {
                    m_Interactor.interactionManager.SelectCancel((IXRSelectInteractor)m_Interactor, interactable);
                }
            }
        }
    }
}