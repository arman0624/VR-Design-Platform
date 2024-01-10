using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] 
        [Tooltip("The right interactor")]
        private XRDirectInteractor m_RightInteractor;
        
        [SerializeField] 
        [Tooltip("The sphere select component")]
        private SphereSelect m_SphereSelectScript;
        
        [SerializeField] 
        [Tooltip("The sphere select change radius action")]
        private InputActionProperty m_SphereSelectChangeRadiusAction;

        [SerializeField] 
        [Tooltip("The grab move action")]
        private InputActionProperty m_GrabMoveAction;
        
        [SerializeField] 
        [Tooltip("The create object action")]
        private InputActionProperty m_CreateObjectAction;
        
        [SerializeField] 
        [Tooltip("The color object action")]
        private InputActionProperty m_ColorObjectAction;
        
        [SerializeField] 
        [Tooltip("The color object input script")]
        private ColorObjectInput m_ColorObjectInputScript;
        
        [SerializeField] 
        [Tooltip("The delete object action")]
        private InputActionProperty m_DeleteObjectAction;
        
        [SerializeField] 
        [Tooltip("The delete object input script")]
        private DeleteObjectInput m_DeleteObjectInputScript;
        
        [SerializeField] 
        [Tooltip("The duplicate object action")]
        private InputActionProperty m_DuplicateObjectAction;
        
        [SerializeField] 
        [Tooltip("The mesh manipulation action")]
        private InputActionProperty m_ManipulateMeshAction;
        
        [SerializeField] 
        [Tooltip("The mesh manipulation input script")]
        private MeshManipulationInput m_MeshManipulationInputScript;
        
        [SerializeField] 
        [Tooltip("The scale axes action")]
        private InputActionProperty m_ScaleAxesAction;
        
        [SerializeField] 
        [Tooltip("The scale axes input script")]
        private ScaleObjectInput m_ScaleAxesInputScript;
        
        [SerializeField] 
        [Tooltip("The undo action")]
        private InputActionProperty m_UndoAction;
        
        [SerializeField] 
        [Tooltip("The redo action")]
        private InputActionProperty m_RedoAction;
        
        [SerializeField] 
        [Tooltip("The select object action")]
        private InputActionProperty m_SetShapeObjectAction;

        [SerializeField] 
        [Tooltip("The select object input script")]
        private SetShapeInput m_SetShapeObjectInputScript;
        
        [SerializeField] 
        [Tooltip("The help and documentation action")]
        private InputActionProperty m_HelpAndDocumentationAction;
        
        [SerializeField] 
        [Tooltip("The help and documentation input script")]
        private HelpAndDocumentationInput m_HelpAndDocumentationInputScript;
        
        [SerializeField]
        [Tooltip("Visualization controller script")]
        private VisualizeControllerState m_VisualizaeControllerStateScript;
        
        // Define the input action type for controller visualization
        public enum InputActionType
        {
            SelectionMade,
            SphereSelectChangeRadius,
            GrabMove,
            CreateObject,
            ColorObject,
            DeleteObject,
            DuplicateObject,
            ManipulateMesh,
            Undo,
            Redo,
            SetShapeObject,
            HelpAndDocumentation,
            ScaleAxes,
            NoAction,
            AllActions
        }
        
        private void Start()
        {
            m_GrabMoveAction.action.performed += OnGrabMovePerformed;
            m_GrabMoveAction.action.canceled += OnStandaloneActionCanceled;

            m_CreateObjectAction.action.performed += OnCreateObjectPerformed;
            m_CreateObjectAction.action.canceled += OnStandaloneActionCanceled;
            
            m_ColorObjectInputScript.OnStartColorObject += OnStartColorObject;
            m_ColorObjectInputScript.OnEndColorObject += OnEndColorObject;

            m_DeleteObjectInputScript.OnDeleteObjectCompleted += OnDeleteObjectCompleted;

            m_MeshManipulationInputScript.OnStartMeshManipulation += OnStartMeshManipulation;
            m_MeshManipulationInputScript.OnEndMeshManipulation += OnEndMeshManipulation;

            m_ScaleAxesInputScript.OnStartScaleAxes += OnStartScaleAxes;
            m_ScaleAxesInputScript.OnEndScaleAxes += OnEndScaleAxes;

            m_SetShapeObjectInputScript.OnStartSelectShape += OnStartSelectShape;
            m_SetShapeObjectInputScript.OnEndSelectShape += OnEndSelectShape;
            
            m_HelpAndDocumentationInputScript.OnHelpAndDocumentationOpen += OnHelpAndDocumentationOpen;
            m_HelpAndDocumentationInputScript.OnHelpAndDocumentationClose += OnHelpAndDocumentationClose;
            
            m_SphereSelectScript.interactor.selectEntered.AddListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.AddListener(OnSphereSelectExited);
        }

        private void OnDestroy()
        {
            m_GrabMoveAction.action.performed -= OnGrabMovePerformed;
            m_GrabMoveAction.action.canceled -= OnStandaloneActionCanceled;
            
            m_CreateObjectAction.action.performed -= OnCreateObjectPerformed;
            m_CreateObjectAction.action.canceled -= OnStandaloneActionCanceled;
            
            m_ColorObjectInputScript.OnStartColorObject -= OnStartColorObject;
            m_ColorObjectInputScript.OnEndColorObject -= OnEndColorObject;

            m_DeleteObjectInputScript.OnDeleteObjectCompleted -= OnDeleteObjectCompleted;
            
            m_MeshManipulationInputScript.OnStartMeshManipulation -= OnStartMeshManipulation;
            m_MeshManipulationInputScript.OnEndMeshManipulation -= OnEndMeshManipulation;
            
            m_ScaleAxesInputScript.OnStartScaleAxes -= OnStartScaleAxes;
            m_ScaleAxesInputScript.OnEndScaleAxes -= OnEndScaleAxes;
            
            m_SetShapeObjectInputScript.OnStartSelectShape -= OnStartSelectShape;
            m_SetShapeObjectInputScript.OnEndSelectShape -= OnEndSelectShape;
            
            m_HelpAndDocumentationInputScript.OnHelpAndDocumentationOpen -= OnHelpAndDocumentationOpen;
            m_HelpAndDocumentationInputScript.OnHelpAndDocumentationClose -= OnHelpAndDocumentationClose;
            
            m_SphereSelectScript.interactor.selectEntered.RemoveListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.RemoveListener(OnSphereSelectExited);
        }

        private void OnEnable()
        {
            HashSet<InputAction> allActions = GetAllActions();
            EnableActions(allActions);
        }

        private void OnDisable()
        {
            HashSet<InputAction> allActions = GetAllActions();
            DisableActions(allActions);
        }

        /// <summary>
        /// Enables actions
        /// </summary>
        /// <param name="actions">The actions to enable</param>
        private void EnableActions(IEnumerable<InputAction> actions)
        {
            foreach (InputAction inputAction in actions)
            {
                inputAction.Enable();
            }
        }

        /// <summary>
        /// Disabless actions
        /// Note: When disabling actions, invoking dispose on the action might cause issues
        /// </summary>
        /// <param name="actions">The actions to disable</param>
        private void DisableActions(IEnumerable<InputAction> actions)
        {
            foreach (InputAction inputAction in actions)
            {
                inputAction.Disable();
            }
        }

        /// <summary>
        /// Returns all actions that needs to be managed by the input manager
        /// </summary>
        /// <returns></returns>
        private HashSet<InputAction> GetAllActions()
        {
            return new HashSet<InputAction>
            {
                m_SphereSelectChangeRadiusAction.action,
                m_GrabMoveAction.action,
                m_CreateObjectAction.action,
                m_ColorObjectAction.action,
                m_DeleteObjectAction.action,
                m_DuplicateObjectAction.action,
                m_ManipulateMeshAction.action,
                m_ScaleAxesAction.action,
                m_UndoAction.action,
                m_RedoAction.action,
                m_SetShapeObjectAction.action,
                m_HelpAndDocumentationAction.action
            };
        }
        
        /// <summary>
        /// Returns all actions that depend on the sphere select
        /// </summary>
        /// <returns></returns>
        private HashSet<InputAction> GetAllSphereSelectDependentActions()
        {
            return new HashSet<InputAction>
            {
                m_ColorObjectAction.action,
                m_DeleteObjectAction.action,
                m_DuplicateObjectAction.action,
                m_ManipulateMeshAction.action,
                m_ScaleAxesAction.action
            };
        }
        
        /// <summary>
        /// Enable or disable the sphere select component
        /// </summary>
        private void EnableMultipleSelection()
        {
            m_RightInteractor.allowSelect = true;
            m_RightInteractor.allowHover = true;
        }
        
        private void DisableMultipleSelection()
        {
            m_RightInteractor.allowSelect = false;
            m_RightInteractor.allowHover = false;
        }

        /// <summary>
        /// Set the starting state, where no object is selected
        /// </summary>
        private void SetToStartingState()
        {
            HashSet<InputAction> allActions = GetAllActions();
            HashSet<InputAction> allSphereSelectDependentActions = GetAllSphereSelectDependentActions();
            allActions.ExceptWith(allSphereSelectDependentActions);
                
            EnableActions(allActions);
            DisableActions(allSphereSelectDependentActions);
            EnableMultipleSelection();
            
            // Initialize the controller visualization to starting state
            UpdateControlVisualization(InputActionType.AllActions);
        }
        
        /// <summary>
        /// Restore to initial state
        /// </summary>
        /// <param name="obj"></param>
        private void OnStandaloneActionCanceled(InputAction.CallbackContext obj)
        {
            SetToStartingState();
        }
        
        /// <summary>
        /// When the create object is performed, disable all actions except itself.
        /// Update the controller visualization for CreateObject
        /// </summary>
        /// <param name="obj"></param>
        private void OnCreateObjectPerformed(InputAction.CallbackContext obj)
        {
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(obj.action);
            DisableActions(allActions);
            DisableMultipleSelection();
            
            // Update the controller visualization for CreateObject
            UpdateControlVisualization(InputActionType.CreateObject);
        }
        
        /// <summary>
        /// When the grab move is performed, disable all actions except itself.
        /// Update the controller visualization for GrabMove
        /// </summary>
        /// <param name="obj"></param>
        private void OnGrabMovePerformed(InputAction.CallbackContext obj)
        {
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(obj.action);
            DisableActions(allActions);
            DisableMultipleSelection();
            
            // Update the controller visualization for GrabMove
            UpdateControlVisualization(InputActionType.GrabMove);
        }
        
        private void OnSphereSelectEntered(SelectEnterEventArgs args)
        {
            HashSet<InputAction> allActions = GetAllActions();
            HashSet<InputAction> allSphereSelectDependentActions = GetAllSphereSelectDependentActions();
            allActions.ExceptWith(allSphereSelectDependentActions);
            
            DisableActions(allActions);
            EnableActions(allSphereSelectDependentActions);
            
            // Update the controller visualization for valid sphere select
            m_VisualizaeControllerStateScript.UpdateControllerVisualization(InputActionType.SelectionMade);
        }
        
        private void OnSphereSelectExited(SelectExitEventArgs args)
        {
            if (!args.isCanceled)
            {
                SetToStartingState();
            }
        }
        
        private void OnStartColorObject()
        {
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(m_ColorObjectAction.action);
            DisableActions(allActions);
            DisableMultipleSelection();
            
            // Update the controller visualization for ColorObject
            UpdateControlVisualization(InputActionType.ColorObject);
        }

        
        private void OnEndColorObject()
        {
            SetToStartingState();
        }
        
        /// <summary>
        /// When the delete object is completed, return to the original state,
        /// where nothing is selected
        /// </summary>
        private void OnDeleteObjectCompleted()
        {
            SetToStartingState();
        }
        
        /// <summary>
        /// When manipulating the mesh, use the available sphere select logic.
        /// However, do not listen to its callbacks.
        /// Disables everything except itself and the sphere select logic.
        /// </summary>
        private void OnStartMeshManipulation()
        {
            // prevent listening to m_SphereSelectScript and changing inputs
            m_SphereSelectScript.interactor.selectEntered.RemoveListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.RemoveListener(OnSphereSelectExited);
            
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(m_ManipulateMeshAction.action);
            allActions.Remove(m_SphereSelectChangeRadiusAction.action);
            DisableActions(allActions);
            
            // Update the controller visualization for ManipulateMesh
            UpdateControlVisualization(InputActionType.ManipulateMesh);
            
            // Enable sphere select
            EnableMultipleSelection();
            EnableActions(new []{m_SphereSelectChangeRadiusAction.action});
        }

        private void OnEndMeshManipulation()
        {
            // Enable everything back
            m_SphereSelectScript.interactor.selectEntered.AddListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.AddListener(OnSphereSelectExited);

            SetToStartingState();
        }

        /// <summary>
        /// Similar to mesh manipulation, use the existing sphere select logic
        /// without listening to its callbacks.
        /// Disables everything except itself and the sphere select logic.
        /// </summary>
        private void OnStartScaleAxes()
        {
            // prevent listening to m_SphereSelectScript and changing inputs
            m_SphereSelectScript.interactor.selectEntered.RemoveListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.RemoveListener(OnSphereSelectExited);
            
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(m_ScaleAxesAction.action);
            allActions.Remove(m_SphereSelectChangeRadiusAction.action);
            DisableActions(allActions);
            
            // Enable sphere select
            EnableMultipleSelection();
            EnableActions(new []{m_SphereSelectChangeRadiusAction.action});
            
            // Update the controller visualization for ScaleAxes
            UpdateControlVisualization(InputActionType.ScaleAxes);
        }

        private void OnEndScaleAxes()
        {
            // Enable everything back
            m_SphereSelectScript.interactor.selectEntered.AddListener(OnSphereSelectEntered);
            m_SphereSelectScript.interactor.selectExited.AddListener(OnSphereSelectExited);

            SetToStartingState();
        }

        private void OnStartSelectShape()
        {
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(m_SetShapeObjectAction.action);
            DisableActions(allActions);
            DisableMultipleSelection();
            
            // Update the controller visualization for SetShapeObject
            UpdateControlVisualization(InputActionType.SetShapeObject);
        }

        private void OnEndSelectShape()
        {
            SetToStartingState();
        }
        
        
        private void OnHelpAndDocumentationOpen()
        {
            HashSet<InputAction> allActions = GetAllActions();
            allActions.Remove(m_HelpAndDocumentationAction.action);
            DisableActions(allActions);
            DisableMultipleSelection();
            
            // Update the controller visualization for HelpAndDocumentation
            UpdateControlVisualization(InputActionType.HelpAndDocumentation);
        }

        private void OnHelpAndDocumentationClose()
        {
            SetToStartingState();
        }
        
        private void UpdateControlVisualization(InputActionType currentInputActionType)
        {
            m_VisualizaeControllerStateScript.UpdateControllerVisualization(currentInputActionType);
        }
    }
}
