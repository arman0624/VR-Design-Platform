using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    /// <summary>
    /// This component renders a sphere based on the properties of the SphereSelect component, such as radius and offset.
    /// The component provides feedback based on the interactor's hover and select events by
    /// changing the color of the sphere to hoverColor, and by not rendering the sphere, respectively.
    /// This is done by adding listeners to SphereSelect's interactor hover and select events.
    /// When the interactor is neither hovering nor selecting, the sphere color is set to color.
    /// </summary>
    public class SphereSelectFeedback : MonoBehaviour
    {
        // Color of the sphere when the interactor is not hovering over any object.
        [SerializeField] private Color m_Color;
        
        // Color of the sphere when the interactor is hovering over an object.
        [SerializeField] private Color m_HoverColor;
        
        // Haptic feedback strength when valid selection occurs.
        [SerializeField] private float m_HapticFeedbackStrength = 0.1f;
        
        // Haptic feedback duration when valid selection occurs.
        [SerializeField] private float m_HapticFeedbackDuration = 0.025f;
        
        // Reference to the SphereSelect component.
        private SphereSelect m_SphereSelect;
        
        // Transform Visualizer is responsible for visualizing the sphere.
        private MeshRenderer m_SphereMeshRenderer;
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Initiate the reference to the SphereSelect component as well as the renderer and mesh filter.
        /// </summary>
        private void Awake()
        {
            m_SphereSelect = GetComponent<SphereSelect>();

            // Initialize the sphere mesh filter and renderer.
            m_SphereMeshRenderer = GetComponent<MeshRenderer>();
            transform.parent = m_SphereSelect.interactor.transform;
            transform.localPosition = m_SphereSelect.offset;
            
            // Set the sphere color to the default color.
            m_SphereMeshRenderer.material.color = m_Color;
            
            // Add a listener to the interactor's select event,
            // send a haptic feedback when a valid selection occurs.
            m_SphereSelect.interactor.selectEntered.AddListener(SendSphereSelectHapticFeedback);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Check if the interactor is hovering over any grabbable objects every frame to update the sphere color.
        /// </summary>
        private void Update()
        {
            // Check if the interactor is hovering over any object.
            bool isHovered = m_SphereSelect.interactor.interactablesHovered.Count > 0;

            // If the sphere select component is enabled, set the sphere color based on whether the interactor
            // is hovering over any object.
            if (m_SphereSelect.interactor.allowSelect && m_SphereSelect.IsVisualSphereSelectEnabled)
            {
                m_SphereMeshRenderer.enabled = true;
                // Based on whether there are any grabbable objects within the sphere, set the sphere color.
                m_SphereMeshRenderer.material.color = isHovered ? m_HoverColor : m_Color;
            }
            else // If the sphere select component is disabled, set the sphere color to the default color.
            {
                m_SphereMeshRenderer.enabled = false;
            }
        }
        
        /// <summary>
        /// Send haptic feedback to the interactor when a valid selection occurs.
        /// </summary>
        /// <param name="selectEnterEventArgs"></param>
        private void SendSphereSelectHapticFeedback(SelectEnterEventArgs selectEnterEventArgs)
        {
            m_SphereSelect.interactor.SendHapticImpulse(m_HapticFeedbackStrength, m_HapticFeedbackDuration);
        }
    }
}