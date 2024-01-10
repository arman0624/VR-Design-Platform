using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
    /// <Summary>
    /// Scale object feedback provides changes to the transform of the object if input criteria is met.
    /// sphere select overlapping axis widget and and grip button held down. This also includes changing
    /// the shape of the mesh that holds the axis widget. After the grip button is released, feedback
    /// of transform object scale ends.  
    /// </Summary>
{
    public class ScaleObjectFeedback : MonoBehaviour
    {
        // Forward arrow interactable, color list and mesh renderer list of prefab objects
        private XRGrabInteractable m_ForwardArrowInteractable;
        private List<Color> m_ForwardArrowInitialColors = new List<Color>();
        private List<MeshRenderer> m_ForwardArrowMeshRenderers;
        
        // Right arrow interactable, color list and mesh renderer list of prefab objects
        private XRGrabInteractable m_RightArrowInteractable;
        private List<Color> m_RightArrowInitialColors = new List<Color>();
        private List<MeshRenderer> m_RightArrowMeshRenderers;
        
        // Up arrow interactable, color list and mesh renderer list of prefab objects
        private XRGrabInteractable m_UpArrowInteractable;
        private List<Color> m_UpArrowInitialColors = new List<Color>();
        private List<MeshRenderer> m_UpArrowMeshRenderers;

        private ScaleObject m_ScaleObject;
        
        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// Initiate the reference to the Scale Object component, XR grab interactables,
        /// arrow prefab mesh renderers list and color list.
        /// </summary>
        private void Awake()
        {
            m_ScaleObject = GetComponent<ScaleObject>();
            
            // Initialize the XR Grab Interactable components
            m_ForwardArrowInteractable = m_ScaleObject.ForwardArrow.GetComponent<XRGrabInteractable>();
            m_RightArrowInteractable = m_ScaleObject.RightArrow.GetComponent<XRGrabInteractable>();
            m_UpArrowInteractable = m_ScaleObject.UpArrow.GetComponent<XRGrabInteractable>();
            
            // Initialize the Mesh Renderer List for children in prefabs
            m_ForwardArrowMeshRenderers = new List<MeshRenderer>(m_ForwardArrowInteractable.GetComponentsInChildren<MeshRenderer>());
            m_RightArrowMeshRenderers = new List<MeshRenderer>(m_RightArrowInteractable.GetComponentsInChildren<MeshRenderer>());
            m_UpArrowMeshRenderers = new List<MeshRenderer>(m_UpArrowInteractable.GetComponentsInChildren<MeshRenderer>());
            
            // Call helper function to set initial mesh renderer color
            SetInitialColors(m_ForwardArrowMeshRenderers, m_ForwardArrowInitialColors);
            SetInitialColors(m_RightArrowMeshRenderers, m_RightArrowInitialColors);
            SetInitialColors(m_UpArrowMeshRenderers, m_UpArrowInitialColors);
        }

        /// <summary>
        /// Helper function that iterates through the mesh renderers in the arrow prefab
        /// and adds the color to the list of colors for that arrow.
        /// </summary>
        private void SetInitialColors(List<MeshRenderer> meshRendererList, List<Color> colorList)
        {
            foreach (var meshRenderer in meshRendererList)
            {
                colorList.Add(meshRenderer.material.color);
            }
        }
        
        /// <summary>
        /// At every frame in Update, check for if the arrow prefabs are hovered.  
        /// If interactable is hovered, then change transparency to fully opaque and assign to arrow material.
        /// </summary>
        private void Update()
        {
            CheckForHover(m_ForwardArrowMeshRenderers, m_ForwardArrowInitialColors, m_ForwardArrowInteractable);
            CheckForHover(m_RightArrowMeshRenderers, m_RightArrowInitialColors, m_RightArrowInteractable);
            CheckForHover(m_UpArrowMeshRenderers, m_UpArrowInitialColors, m_UpArrowInteractable);
        }

        /// <summary>
        /// Checks if an interactable is hovered and change color accordingly
        /// </summary>
        /// <param name="meshRendererList"></param>
        /// <param name="colorList"></param>
        /// <param name="grabInteractable"></param>
        private void CheckForHover(List<MeshRenderer> meshRendererList, List<Color> colorList, XRGrabInteractable grabInteractable)
        {
            for (int i = 0; i < meshRendererList.Count; i++)
            {
                if (grabInteractable.isHovered || grabInteractable.isSelected)
                {
                    Color colorHover = colorList[i];
                    colorHover.a = 1.0f;
                    meshRendererList[i].material.color = colorHover;
                }
                else
                {
                    meshRendererList[i].material.color = colorList[i];
                }
            }
        }
        
    }
    
}