using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using XRC.Assignments.Project.G01;

public class VisualizeControllerState : MonoBehaviour
{
    [SerializeField] [Tooltip("Right hand grip button action")]
    private InputActionProperty m_RightHandGripAction;
    
    [SerializeField] [Tooltip("Right hand trigger button action")]
    private InputActionProperty m_RightHandTriggerAction;
    
    [SerializeField] [Tooltip("Left hand grip button action")]
    private InputActionProperty m_LeftHandGripAction;
    
    [SerializeField] [Tooltip("Left hand trigger button action")]
    private InputActionProperty m_LeftHandTriggerAction;
    
    [SerializeField] [Tooltip("Right hand joystick action")]
    private InputActionProperty m_RightHandJoystickAction;
    
    [SerializeField] [Tooltip("Left hand joystick action")]
    private InputActionProperty m_LeftHandJoystickAction;
    
    [SerializeField] [Tooltip("Grab move action")]
    private InputActionProperty m_GrabMoveAction;
    
    [SerializeField] [Tooltip("Right hand primary button")]
    private InputActionProperty m_RightHandPrimaryButtonAction;
    
    [SerializeField] [Tooltip("Right hand secondary button")]
    private InputActionProperty m_RightHandSecondaryButtonAction;
    
    [SerializeField] [Tooltip("Left hand primary button")]
    private InputActionProperty m_LeftHandPrimaryButtonAction;
    
    [SerializeField] [Tooltip("Left hand secondary button")]
    private InputActionProperty m_LeftHandSecondaryButtonAction;
    
    [SerializeField] [Tooltip("Left controller")]
    private GameObject m_LeftController;
    
    [SerializeField] [Tooltip("Right controller")]
    private GameObject m_RightController;
    
    [SerializeField] [Tooltip("Right interactor")]
    private XRDirectInteractor m_RightInteractor;
    
    [SerializeField] [Tooltip("Right hand grip button material")]
    private Material m_RightHandGripMaterial;
    
    [SerializeField] [Tooltip("Right hand trigger button material")]
    private Material m_RightHandTriggerMaterial;
    
    [SerializeField] [Tooltip("Left hand grip button material")]
    private Material m_LeftHandGripMaterial;
    
    [SerializeField] [Tooltip("Left hand trigger button material")]
    private Material m_LeftHandTriggerMaterial;
    
    [SerializeField] [Tooltip("Undo icon material")]
    private Material m_UndoMaterial;
    
    [SerializeField] [Tooltip("Redo icon material")]
    private Material m_RedoMaterial;
    
    [SerializeField] [Tooltip("Delete icon material")]
    private Material m_DeleteMaterial;
    
    [SerializeField] [Tooltip("Colorpicker icon material")]
    private Material m_ColorPickerMaterial;
    
    [SerializeField] [Tooltip("Mesh manipulation icon material")]
    private Material m_MeshManipulationMaterial;
    
    [SerializeField] [Tooltip("Exit manipulation icon material")]
    private Material m_ExitMaterial;
    
    [SerializeField] [Tooltip("Select shape menu icon material")]
    private Material m_SelectShapeMenuMaterial;
    
    [SerializeField] [Tooltip("Help and documentation icon material")]
    private Material m_HelpAndDocumentationMaterial;
    
    [SerializeField] [Tooltip("Scale by axis icon material")]
    private Material m_ScaleByAxisMaterial;

    // The default colors for buttons and joysticks.
    private Color m_GripMaterialColor = new Color(0.145098f, 0.6980392f, 0.9607843f, 255);
    private Color m_TriggerMaterialColor = new Color(0.1529411f, 0.1529411f, 0.9607843f, 255);
    private Color m_JoystickMaterialColor = new Color(0.63f, 0.63f, 0.63f, 255);
    
    // The default colors for buttons
    private Color m_PrimaryButtonColor = new Color(0.145098f, 0.4392157f, 0.9607843f, 255);
    private Color m_SecondaryButtonColor = new Color(0.145098f, 0.9568627f, 0.9607843f, 255);
    
    // Reference to the joystick game objects.
    private GameObject m_RightHandJoystick;
    private GameObject m_LeftHandJoystick;
    
    // Reference to the button game objects.
    private GameObject m_RightHandPrimaryButton;
    private GameObject m_RightHandSecondaryButton;
    private GameObject m_LeftHandPrimaryButton;
    private GameObject m_LeftHandSecondaryButton;
    
    // Reference to the button UI game objects.
    private GameObject m_RightHandPrimaryButtonUI;
    private GameObject m_RightHandSecondaryButtonUI;
    private GameObject m_LeftHandPrimaryButtonUI;
    private GameObject m_LeftHandSecondaryButtonUI;
    private GameObject m_RightHandJoystickUI;
    
    
    /// <summary>
    /// Register all the input actions.
    /// </summary>
    private void Awake()
    {
        m_RightHandGripAction.action.performed += OnRightHandGripPressed;
        m_RightHandGripAction.action.canceled += OnRightHandGripPressed;
        
        m_RightHandTriggerAction.action.performed += OnRightHandTriggerPressed;
        m_RightHandTriggerAction.action.canceled += OnRightHandTriggerPressed;
        
        m_LeftHandGripAction.action.performed += OnLeftHandGripPressed;
        m_LeftHandGripAction.action.canceled += OnLeftHandGripPressed;
        
        m_LeftHandTriggerAction.action.performed += OnLeftHandTriggerPressed;
        m_LeftHandTriggerAction.action.canceled += OnLeftHandTriggerPressed;
        
        m_RightHandJoystickAction.action.performed += OnRightHandJoystickMoved;
        m_RightHandJoystickAction.action.canceled += OnRightHandJoystickMoved;
        
        m_LeftHandJoystickAction.action.performed += OnLeftHandJoystickMoved;
        m_LeftHandJoystickAction.action.canceled += OnLeftHandJoystickMoved;
        
        m_RightHandPrimaryButtonAction.action.performed += OnRightHandPrimaryButtonPressed;
        m_RightHandPrimaryButtonAction.action.canceled += OnRightHandPrimaryButtonPressed;
        
        m_RightHandSecondaryButtonAction.action.performed += OnRightHandSecondaryButtonPressed;
        m_RightHandSecondaryButtonAction.action.canceled += OnRightHandSecondaryButtonPressed;
        
        m_LeftHandPrimaryButtonAction.action.performed += OnLeftHandPrimaryButtonPressed;
        m_LeftHandPrimaryButtonAction.action.canceled += OnLeftHandPrimaryButtonPressed;
        
        m_LeftHandSecondaryButtonAction.action.performed += OnLeftHandSecondaryButtonPressed;
        m_LeftHandSecondaryButtonAction.action.canceled += OnLeftHandSecondaryButtonPressed;
    }

    /// <summary>
    /// Initialize the different components of the controllers
    /// </summary>
    private void Start()
    {
        // Set up the joystick rotation.
        m_RightHandJoystick = GameObject.Find("RightHandThumbStick");
        if (m_RightHandJoystick != null)
        {
            m_RightHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        
        m_LeftHandJoystick = GameObject.Find("LeftHandThumbStick");
        if (m_LeftHandJoystick != null)
        {
            m_LeftHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        m_RightHandPrimaryButton = GameObject.Find("RightHandPrimaryButton");
        m_RightHandSecondaryButton = GameObject.Find("RightHandSecondaryButton");
        m_RightHandPrimaryButtonUI = GameObject.Find("RightHandPrimaryButtonUI");
        m_RightHandSecondaryButtonUI = GameObject.Find("RightHandSecondaryButtonUI");
        m_LeftHandPrimaryButton = GameObject.Find("LeftHandPrimaryButton");
        m_LeftHandSecondaryButton = GameObject.Find("LeftHandSecondaryButton");
        m_LeftHandPrimaryButtonUI = GameObject.Find("LeftHandPrimaryButtonUI");
        m_LeftHandSecondaryButtonUI = GameObject.Find("LeftHandSecondaryButtonUI");
        m_RightHandJoystickUI = GameObject.Find("RightHandJoystickUI");
    }
    
    /// <summary>
    /// In case the components of the controller is not initialized in the Start() method,
    /// update the instances of controller components here.
    /// </summary>
    private void Update()
    {
        // Update controller joystick instance if not initialized.
        // This is a workaround for the issue that the joystick instance is not initialized in the Start() method.
        if (m_RightHandJoystick == null)
        {
            m_RightHandJoystick = GameObject.Find("RightHandThumbStick");
            m_RightHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (m_LeftHandJoystick == null)
        {
            m_LeftHandJoystick = GameObject.Find("LeftHandThumbStick");
            m_LeftHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        if (m_RightHandPrimaryButton == null)
        {
            m_RightHandPrimaryButton = GameObject.Find("RightHandPrimaryButton");
        }
        
        if (m_RightHandSecondaryButton == null)
        {
            m_RightHandSecondaryButton = GameObject.Find("RightHandSecondaryButton");
        }
        
        if (m_RightHandPrimaryButtonUI == null)
        {
            m_RightHandPrimaryButtonUI = GameObject.Find("RightHandPrimaryButtonUI");
        }
        
        if (m_RightHandSecondaryButtonUI == null)
        {
            m_RightHandSecondaryButtonUI = GameObject.Find("RightHandSecondaryButtonUI");
        }
        
        if (m_LeftHandPrimaryButton == null)
        {
            m_LeftHandPrimaryButton = GameObject.Find("LeftHandPrimaryButton");
        }
        
        if (m_LeftHandSecondaryButton == null)
        {
            m_LeftHandSecondaryButton = GameObject.Find("LeftHandSecondaryButton");
        }
        
        if (m_LeftHandPrimaryButtonUI == null)
        {
            m_LeftHandPrimaryButtonUI = GameObject.Find("LeftHandPrimaryButtonUI");
        }
        
        if (m_LeftHandSecondaryButtonUI == null)
        {
            m_LeftHandSecondaryButtonUI = GameObject.Find("LeftHandSecondaryButtonUI");
        }
        
        if (m_RightHandJoystickUI == null)
        {
            m_RightHandJoystickUI = GameObject.Find("RightHandJoystickUI");
        }
    }

    private void OnEnable()
    {
        m_RightHandGripAction.action.Enable();
        m_RightHandTriggerAction.action.Enable();
        m_RightHandJoystickAction.action.Enable();
        m_LeftHandGripAction.action.Enable();
        m_LeftHandTriggerAction.action.Enable();
        m_LeftHandJoystickAction.action.Enable();
        m_RightHandPrimaryButtonAction.action.Enable();
        m_RightHandSecondaryButtonAction.action.Enable();
        m_LeftHandPrimaryButtonAction.action.Enable();
        m_LeftHandSecondaryButtonAction.action.Enable();
        m_GrabMoveAction.action.Enable();
    }
    
    private void OnDisable()
    {
        m_RightHandGripAction.action.Disable();
        m_RightHandTriggerAction.action.Disable();
        m_RightHandJoystickAction.action.Disable();
        m_LeftHandGripAction.action.Disable();
        m_LeftHandTriggerAction.action.Disable();
        m_LeftHandJoystickAction.action.Disable();
        m_RightHandPrimaryButtonAction.action.Disable();
        m_RightHandSecondaryButtonAction.action.Disable();
        m_LeftHandPrimaryButtonAction.action.Disable();
        m_LeftHandSecondaryButtonAction.action.Disable();
        m_GrabMoveAction.action.Disable();
    }
    
    /// <summary>
    /// Highlight the right hand grip button when it is pressed.
    /// Change the displayed icon based on whether there is any object selected.
    /// </summary>
    /// <param name="context"></param>
    private void OnRightHandGripPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Change the material of the right hand grip button
            m_RightHandGripMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            // Restore to the default color
            m_RightHandGripMaterial.color = m_GripMaterialColor;
        }
    }
    
    /// <summary>
    /// Highlight the right hand trigger button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnRightHandTriggerPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Change the material of the right hand trigger button
            m_RightHandTriggerMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            // Restore to the default color
            m_RightHandTriggerMaterial.color = m_TriggerMaterialColor;
        }
    }
    
    /// <summary>
    /// Highlight the left hand grip button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftHandGripPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Change the material of the left hand grip button
            m_LeftHandGripMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            // Restore to the default color
            m_LeftHandGripMaterial.color = m_GripMaterialColor;
        }
    }
    
    /// <summary>
    /// Highlight the left hand trigger button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftHandTriggerPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Change the material of the left hand trigger button
            m_LeftHandTriggerMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            // Restore to the default color
            m_LeftHandTriggerMaterial.color = m_TriggerMaterialColor;
        }
    }
    
    /// <summary>
    /// Highlight the right hand joystick when it is moved.
    /// Move the joystick accordingly.
    /// </summary>
    /// <param name="context"></param>
    private void OnRightHandJoystickMoved(InputAction.CallbackContext context)
    {
        Vector2 joystickDelta = context.ReadValue<Vector2>();
        
        // Move the joystick accordingly.
        // Calculate the rotation on x and z axis.
        float xRotation = joystickDelta.y * 30.0f;
        float zRotation = joystickDelta.x * 30.0f;
        
        // Change the Joystick rotation based on the joystick delta.
        if (context.performed)
        {
            m_RightHandJoystick.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
            m_RightHandJoystick.transform.localRotation = Quaternion.Euler(-xRotation, 0, zRotation);
        }
        else if (context.canceled)
        {
           m_RightHandJoystick.GetComponent<MeshRenderer>().sharedMaterial.color = m_JoystickMaterialColor;
           m_RightHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    /// <summary>
    /// Highlight the left hand joystick when it is moved.
    /// Move the joystick accordingly.
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftHandJoystickMoved(InputAction.CallbackContext context)
    {
        Vector2 joystickDelta = context.ReadValue<Vector2>();
        
        // Move the joystick accordingly.
        // Calculate the rotation on x and z axis.
        float xRotation = joystickDelta.y * 30.0f;
        float zRotation = joystickDelta.x * 30.0f;
        
        // Change the Joystick rotation based on the joystick delta.
        if (context.performed)
        {
            m_LeftHandJoystick.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
            m_LeftHandJoystick.transform.localRotation = Quaternion.Euler(-xRotation, 0, zRotation);
        }
        else if (context.canceled)
        {
            m_LeftHandJoystick.GetComponent<MeshRenderer>().sharedMaterial.color = m_JoystickMaterialColor;
            m_LeftHandJoystick.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    /// <summary>
    /// Highlight the right hand primary button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnRightHandPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_RightHandPrimaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            m_RightHandPrimaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = m_PrimaryButtonColor;
        }
    }
    
    /// <summary>
    /// Highlight the right hand secondary button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnRightHandSecondaryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_RightHandSecondaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            m_RightHandSecondaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = m_SecondaryButtonColor;
        }
    }
    
    /// <summary>
    /// Highlight the left hand primary button when it is pressed.
    /// Change the icon based on the state of mesh manipulation.
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftHandPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_LeftHandPrimaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            m_LeftHandPrimaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = m_PrimaryButtonColor;
        }
    }
    
    /// <summary>
    /// Highlight the left hand secondary button when it is pressed.
    /// </summary>
    /// <param name="context"></param>
    private void OnLeftHandSecondaryButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            m_LeftHandSecondaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        }
        else if (context.canceled)
        {
            m_LeftHandSecondaryButton.GetComponent<MeshRenderer>().sharedMaterial.color = m_SecondaryButtonColor;
        }
    }

    /// <summary>
    /// Update the controller visualization based on the input action type.
    /// </summary>
    /// <param name="inputActionType"></param>
    public void UpdateControllerVisualization(InputManager.InputActionType inputActionType)
    {
        switch (inputActionType)
        {
            // Valid grabbable object is selected.
            case (InputManager.InputActionType.SelectionMade):
                m_RightHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ColorPickerMaterial;
                m_RightHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_DeleteMaterial;
                m_LeftHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_MeshManipulationMaterial;
                m_LeftHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ScaleByAxisMaterial;
                m_RightHandJoystickUI.SetActive(false);
                break;
            // Mesh manipulation is performing.
            case (InputManager.InputActionType.ManipulateMesh):
                m_LeftHandSecondaryButtonUI.SetActive(false);
                m_LeftHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ExitMaterial;
                HideRightHandButtonUI();
                ShowRightHandJoyStickUI();
                break;
            // Scale by axis is performing.
            case (InputManager.InputActionType.ScaleAxes):
                m_LeftHandPrimaryButtonUI.SetActive(false);
                m_LeftHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ExitMaterial;
                HideRightHandButtonUI();
                ShowRightHandJoyStickUI();
                break;
            // Color picker is performing.
            case (InputManager.InputActionType.ColorObject):
                m_RightHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ColorPickerMaterial;
                m_RightHandSecondaryButtonUI.SetActive(false);
                HideLeftHandButtonUI();
                HideRightHandJoyStickUI();
                break;
            // Select shape menu is performing.
            case (InputManager.InputActionType.SetShapeObject):
                m_LeftHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ExitMaterial;
                m_LeftHandSecondaryButtonUI.SetActive(false);
                HideRightHandButtonUI();
                HideRightHandJoyStickUI();
                break;
            // Help and documentation menu is performing.
            case (InputManager.InputActionType.HelpAndDocumentation):
                m_LeftHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_ExitMaterial;
                m_LeftHandPrimaryButtonUI.SetActive(false);
                HideRightHandButtonUI();
                HideRightHandJoyStickUI();
                break;
            // Grab move or create object is performing.
            case (InputManager.InputActionType.GrabMove):
            case (InputManager.InputActionType.CreateObject):
                HideRightHandButtonUI();
                HideRightHandJoyStickUI();
                HideLeftHandButtonUI();
                break;
            // Default or starting state.
            default:
            case (InputManager.InputActionType.AllActions):
                m_RightHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_UndoMaterial;
                m_RightHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_RedoMaterial;
                m_LeftHandPrimaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_SelectShapeMenuMaterial;
                m_LeftHandSecondaryButtonUI.GetComponent<MeshRenderer>().sharedMaterial = m_HelpAndDocumentationMaterial;
                ShowLeftHandButtonUI();
                ShowRightHandButtonUI();
                ShowRightHandJoyStickUI();
                break;
        }
    }
    
    private void HideRightHandButtonUI()
    {
        m_RightHandPrimaryButtonUI.SetActive(false);
        m_RightHandSecondaryButtonUI.SetActive(false);
    }
    
    private void HideRightHandJoyStickUI()
    {
        m_RightHandJoystickUI.SetActive(false);
    }
    
    private void HideLeftHandButtonUI()
    {
        m_LeftHandPrimaryButtonUI.SetActive(false);
        m_LeftHandSecondaryButtonUI.SetActive(false);
    }
    
    private void ShowRightHandButtonUI()
    {
        m_RightHandPrimaryButtonUI.SetActive(true);
        m_RightHandSecondaryButtonUI.SetActive(true);
    }
    
    private void ShowRightHandJoyStickUI()
    {
        m_RightHandJoystickUI.SetActive(true);
    }

    private void ShowLeftHandButtonUI()
    {
        m_LeftHandPrimaryButtonUI.SetActive(true);
        m_LeftHandSecondaryButtonUI.SetActive(true);
    }
}
