
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class UIRayInteractorManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private XRRayInteractor m_XRRayInteractor;
        private XRInteractorLineVisual m_XRInteractorLineVisual;
        private LineRenderer m_LineRenderer;
        
        // The intial length of the ray interactor
        private float m_InitialMaxRaycastDistance;
        
        // The base color of the ray interactor
        private Color m_BaseColor;
        
        [SerializeField]
        [Tooltip("The gradient for the selected color")]
        private Color m_HoveredColor;
        
        [SerializeField]
        [Tooltip("The XR Origin")]
        private Transform m_XROrigin;

        void Awake()
        {
            m_XRRayInteractor = GetComponent<XRRayInteractor>();
            m_XRInteractorLineVisual = GetComponent<XRInteractorLineVisual>();
            m_LineRenderer = GetComponent<LineRenderer>();

            m_InitialMaxRaycastDistance = m_XRRayInteractor.maxRaycastDistance;
            m_BaseColor = m_LineRenderer.material.color;
        }

        private void OnDestroy()
        {
            m_LineRenderer.material.color = m_BaseColor;
        }

        /// <summary>
        /// Enable the ray interactor
        /// </summary>
        /// <param name="enable">True to enable the ray interactor</param>
        public void EnableRayInteractor(bool enable)
        {
            m_XRRayInteractor.maxRaycastDistance = m_InitialMaxRaycastDistance * m_XROrigin.localScale.x;
            m_XRRayInteractor.enabled = enable;
            m_XRInteractorLineVisual.enabled = enable;
        }
        
        public void OnHoverEntered()
        {
            m_LineRenderer.material.color = m_HoveredColor;
        }

        public void OnHoverExited()
        {
            m_LineRenderer.material.color = m_BaseColor;
        }

    }
}
