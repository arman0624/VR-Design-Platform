using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class ColorObject : MonoBehaviour
    {
        private MeshRenderer m_SelectedObject;
        
        [Header("Main fields")]
        [SerializeField] 
        [Tooltip("The color menu object")]
        private GameObject m_ColorMenu;
        
        [SerializeField]
        [Tooltip("The Right controller.")]
        private Transform m_RightController;
        
        [SerializeField] 
        [Tooltip("The camera for the display to follow.")]
        private Camera m_MainCamera;
        
        // Color wheel
        [Header("Color Wheel")]
        [SerializeField] 
        private GameObject m_ColorWheelWrapperObject;
        private Collider m_ColorWheelWrapperCollider;
        [SerializeField] 
        private Transform m_ColorWheelWrapperRadius;
        [SerializeField] 
        private Transform m_ColorWheelWrapperHalfHeight;
        [SerializeField] 
        private MeshRenderer m_ColorPreview;
        
        // Pointer
        [Header("Pointer")]
        [SerializeField] 
        [Tooltip("The pointer for selecting colors on the color wheel.")]
        private GameObject m_Pointer;
        private TriggerExposer m_PointerTrigger;
        private Collider m_PointerCollider;
        
        private bool m_IsSelectingColor = false;

        // Color
        private Vector3 m_KeptHSV = Vector3.one;
        private Vector3 m_HSV = Vector3.one;
        public Vector3 HSV => m_HSV;
        private Color m_StartColor = Color.white;
        

        /// <summary>
        /// Initialize components and callbacks
        /// </summary>
        private void Start()
        {
            m_ColorWheelWrapperCollider = m_ColorWheelWrapperObject.GetComponent<Collider>();
            
            m_PointerTrigger = m_Pointer.GetComponentInChildren<TriggerExposer>();
            m_PointerCollider = m_Pointer.GetComponentInChildren<Collider>();
            
            m_PointerTrigger.TriggerEnterAction += OnPointerEnterTrigger;
            m_PointerTrigger.TriggerExitAction += OnPointerExitTrigger;
        }

        /// <summary>
        /// Remove callbacks
        /// </summary>
        private void OnDestroy()
        {
            m_PointerTrigger.TriggerEnterAction -= OnPointerEnterTrigger;
            m_PointerTrigger.TriggerEnterAction -= OnPointerExitTrigger;
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_ColorMenu.activeSelf)
            {
                return;
            }
            
            if (m_IsSelectingColor)
            {
                Vector3 menuFacingDirection = m_ColorMenu.transform.forward;
                (float pointerHeight, Vector3 pointerPositionOnMenu) = GetPointerInformationOnMenu(menuFacingDirection);
                GetHSV(pointerHeight, pointerPositionOnMenu, menuFacingDirection);
                SetInteractableColor(HSV);
            }
            else
            {
                m_HSV = m_KeptHSV;
                SetInteractableColor(m_KeptHSV);
            }
        }

        /// <summary>
        /// Sets the color of the interactable
        /// </summary>
        /// <param name="incomingHSV">The HSV component used for colaring</param>
        private void SetInteractableColor(Vector3 incomingHSV)
        {
            Color newColor = Color.HSVToRGB(incomingHSV.x, incomingHSV.y, incomingHSV.z);
            m_SelectedObject.material.color = newColor;
        }

        /// <summary>
        /// Computes the HSV values based on the pointer position, relative to the center
        /// of the color wheel
        /// Note: This will only work on a flat screen
        /// Note2: We are assuming we are facing the forward vector of the menu
        /// </summary>
        private void GetHSV(float pointerHeight, Vector3 pointerPositionOnMenu, Vector3 menuFacingDirection)
        {
            // Get the center of the color wheel
            Vector3 colorWheelCenter = m_ColorWheelWrapperObject.transform.position;
            
            // Get the radius of the color wheel
            float allowedRadius = Vector3.Distance(m_ColorWheelWrapperRadius.position, colorWheelCenter);
            
            // get the color wheel position on the menu
            Vector3 colorWheelPositionOnMenu = Vector3.ProjectOnPlane( colorWheelCenter, menuFacingDirection);
            
            // The position of pure red is the reference
            Vector3 zeroAnglePositionOnMenu = allowedRadius * m_ColorWheelWrapperObject.transform.right + colorWheelCenter;
            zeroAnglePositionOnMenu = Vector3.ProjectOnPlane( zeroAnglePositionOnMenu, menuFacingDirection);
            
            // Get the Hue
            Vector3 zeroVector = (pointerPositionOnMenu - colorWheelPositionOnMenu).normalized;
            Vector3 positionVector = (zeroAnglePositionOnMenu - colorWheelPositionOnMenu).normalized;
            float hue = Vector3.SignedAngle(zeroVector, positionVector, -1.0f * menuFacingDirection);
            hue = (hue + 180.0f) / 360.0f;
            
            // Get the saturation
            float distanceFromCenter = Vector3.Distance(pointerPositionOnMenu, colorWheelPositionOnMenu);
            float saturation = Mathf.Clamp(distanceFromCenter / allowedRadius, 0, 1.0f);
            
            // Get the value
            // The bounds should be unchanging. Make sure to get the right coordinate
            float maxHeight = 2.0f * Vector3.Distance(m_ColorWheelWrapperHalfHeight.position, colorWheelCenter);
            float value = Mathf.Clamp(pointerHeight / maxHeight, 0.0f, 1.0f);
            
            m_HSV.x = hue;
            m_HSV.y = saturation;
            m_HSV.z = value;
        }

        /// <summary>
        /// Computes the pointer information on the menu
        /// Note: This only works on flat screens
        /// </summary>
        /// <param name="menuFacingDirection">The normal vector for the facing direction of the menu</param>
        /// <returns>The pointer height relative to the menu, the relative pointer position on the menu</returns>
        private (float, Vector3) GetPointerInformationOnMenu(Vector3 menuFacingDirection)
        {
            // Assume we are facing the forward vector of the menu
            Vector3 pointerPosition = m_PointerCollider.bounds.center;
            
            // Get the pointer position on the menu
            Vector3 pointerPositionOnMenu = Vector3.ProjectOnPlane(pointerPosition, menuFacingDirection);

            // Add the menu depth to the pointer
            Vector3 menuDepth = Vector3.Project(m_ColorMenu.transform.position, menuFacingDirection);
            Vector3 absolutePointerPositionOnMenu = pointerPositionOnMenu + menuDepth;
            float pointerHeight = Vector3.Distance(pointerPosition, absolutePointerPositionOnMenu);
            
            // Check if we are behind the menu
            Vector3 heightVector = (pointerPosition - absolutePointerPositionOnMenu).normalized;
            if (Vector3.Dot(heightVector, menuFacingDirection) < 0.0f)
            {
                pointerHeight *= -1.0f;
            }

            return (pointerHeight, pointerPositionOnMenu);
        }

        /// <summary>
        /// Helper function to change the state from selecting a color in the color
        /// wheel to interacting with UI elements
        /// </summary>
        /// <param name="isSelectingColor"></param>
        private void SetIsSelectingColor(bool isSelectingColor)
        {
            m_IsSelectingColor = isSelectingColor;
        }
        
        /// <summary>
        /// Callback for obtaining information about the color wheel
        /// entering the collider of the pointer
        /// </summary>
        /// <param name="other"></param>
        private void OnPointerEnterTrigger(Collider other)
        {
            if (!m_ColorMenu.activeSelf)
            {
                return;
            }
            
            if (other == m_ColorWheelWrapperCollider)
            {
                SetIsSelectingColor(true);
            }
        }
        
        /// <summary>
        /// Callback for obtaining information about the color wheel
        /// exiting the collider of the pointer
        /// </summary>
        /// <param name="other"></param>
        private void OnPointerExitTrigger(Collider other)
        {
            if (!m_ColorMenu.activeSelf)
            {
                return;
            }
            
            if (other == m_ColorWheelWrapperCollider)
            {
                SetIsSelectingColor(false);
            }
        }

        /// <summary>
        /// Handling of the start of the color selection
        /// </summary>
        /// <param name="interactableMesh">The mesh we are interacting with</param>
        public void StartColorObject(MeshRenderer interactableMesh)
        {
            // save the initial color
            m_StartColor = interactableMesh.material.color;
            
            // Set the target
            m_SelectedObject = interactableMesh;
            Color.RGBToHSV(m_SelectedObject.material.color, out float hue, out float saturation, out float value);
            m_HSV = new Vector3(hue, saturation, value);
            ApplySelectedColor(true);
            
            // Setup the menu
            Vector3 menuOffset = m_RightController.forward * 0.2f * m_RightController.lossyScale.x;
            m_ColorMenu.transform.rotation = Quaternion.identity;
            m_ColorMenu.transform.position = m_RightController.position;
            m_ColorMenu.transform.Translate(menuOffset, Space.Self);
            m_ColorMenu.transform.LookAt(m_MainCamera.transform);
            Vector3 eulerRotation = m_ColorMenu.transform.localRotation.eulerAngles;
            // Remove local rotation along z
            m_ColorMenu.transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(0, 0, eulerRotation.z));
            m_ColorMenu.SetActive(true);

            // Setup the pointer
            Vector3 pointerOffset = m_RightController.forward * -0.02f * m_RightController.lossyScale.x;
            m_Pointer.transform.rotation = Quaternion.identity;
            m_Pointer.transform.position = m_RightController.position;
            m_Pointer.transform.Translate(pointerOffset, Space.Self);
            m_Pointer.transform.rotation = m_RightController.rotation;
            m_Pointer.transform.SetParent(m_RightController);
            m_Pointer.SetActive(true);

            // Set the current state
            SetIsSelectingColor(false);
        }

        /// <summary>
        /// Handling of the end of the color selection
        /// </summary>
        public void StopColorObject()
        {
            // Set the color
            Color newColor = Color.HSVToRGB(m_KeptHSV.x, m_KeptHSV.y, m_KeptHSV.z);
            m_SelectedObject.material.color = newColor;
            
            // Create the command
            ColorObjectCommand command = new ColorObjectCommand(m_SelectedObject, m_StartColor, newColor);
            UndoRedo.Instance.AddCommand(command);
            
            // Remove the menu
            m_ColorMenu.SetActive(false);
            m_Pointer.SetActive(false);
            m_Pointer.transform.SetParent(transform);

            SetIsSelectingColor(true);
        }

        /// <summary>
        /// When called, the selected color will be saved and applied to the current object
        /// </summary>
        /// <param name="force">Force color application</param>
        public void ApplySelectedColor(bool force = false)
        {
            if (m_IsSelectingColor || force)
            {
                m_KeptHSV = HSV;
                Color newColor = Color.HSVToRGB(m_KeptHSV.x, m_KeptHSV.y, m_KeptHSV.z);
                m_ColorPreview.material.color = newColor;
            }
        }
    }
}
