using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    /// <summary>
    /// This component handles the user input, and passes the corresponding values to the SphereSelect component.
    /// </summary>
    public class SphereSelectInput : MonoBehaviour
    {
        // The scale factor applied to the user input when determining the radius value.
        [SerializeField] [Tooltip("Input sensitivity for changing the radius of the sphere")]
        private float m_InputSensitivity;
        
        [SerializeField] [Tooltip("Use the joystick to change the radius of the sphere")]
        private InputActionProperty m_ChangeRadiusAction;
        
        // A reference to the SphereSelect component.
        private SphereSelect m_SphereSelect;
        
        // The delta value of the joystick input.
        private Vector2 m_JoyStickDelta = Vector2.zero;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnEnable()
        {
            m_ChangeRadiusAction.action.Enable();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDisable()
        {
            m_ChangeRadiusAction.action.Disable();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Initiate the reference to the SphereSelect component and the SphereCollider.
        /// Set the event listeners for the input actions.
        /// </summary>
        private void Awake()
        {
            m_SphereSelect = GetComponent<SphereSelect>();
            
            m_ChangeRadiusAction.action.performed += OnRadiusChanged;
            m_ChangeRadiusAction.action.canceled += OnRadiusChanged;
        }
        
        private void Start()
        {
            gameObject.transform.localScale = m_SphereSelect.Radius * 2.0f * Vector3.one;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Calculate the new radius value based on the joystick input.
        /// Apply the new scale to the sphere and update the sphere collider's radius.
        /// </summary>
        private void Update()
        {
            // Calculate the new radius scale
            float newRadius = CalculateNewSphereRadius(Time.deltaTime);
            
            // Update the actual radius
            m_SphereSelect.Radius = newRadius;
            gameObject.transform.localScale = newRadius * 2.0f * Vector3.one;
        }
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Clear the event listeners.
        /// </summary>
        private void OnDestroy()
        {
            m_ChangeRadiusAction.action.performed -= OnRadiusChanged;
            m_ChangeRadiusAction.action.canceled -= OnRadiusChanged;
        }

        /// <summary>
        /// This method is called when the user moves the joystick.
        /// The delta value of the joystick is used to update the radius value.
        /// </summary>
        /// <param name="ctx"></param>
        private void OnRadiusChanged(InputAction.CallbackContext ctx)
        {
            // Get the value for joystick input
            m_JoyStickDelta = ctx.ReadValue<Vector2>();
        }
        
        /// <summary>
        /// Calculate the new radius of the sphere based on the current radius, the scale speed, and the delta time.
        /// We then apply this radius to the radius of the sphere collider.
        /// </summary>
        /// <param name="deltaTime">The interval in seconds from the last frame to the current one</param>
        /// <returns></returns>
        private float CalculateNewSphereRadius(float deltaTime)
        {
            float scaleSpeed = m_JoyStickDelta.y * m_InputSensitivity;
            float newRadius = m_SphereSelect.Radius + deltaTime * scaleSpeed * m_SphereSelect.Radius;
            newRadius = Mathf.Clamp(newRadius, m_SphereSelect.minRadius, m_SphereSelect.maxRadius);
            return newRadius;
        }
    }
}