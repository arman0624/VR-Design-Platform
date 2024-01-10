using System.Collections.Generic;
using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class GrabMove : MonoBehaviour
    {
        [SerializeField] [Tooltip("The environment that will appear unaffected by the Grab Move to the user")]
        private List<Transform> m_Environment = new List<Transform>();

        [SerializeField]
        [Tooltip("The left controller Transform that will drive grab movement with its local position.")]
        private Transform m_LeftControllerTransform;

        [SerializeField]
        [Tooltip("The right controller Transform that will drive grab movement with its local position.")]
        private Transform m_RightControllerTransform;

        [SerializeField] [Tooltip("The XR Origin object to provide access control to.")]
        private Transform m_XROrigin;

        [SerializeField] [Tooltip("The minimum scale allowed by Grab Move.")]
        private float m_MinimumScale = 0.02f;

        [SerializeField] [Tooltip("TThe maximum scale allowed by Grab Move.")]
        private float m_MaximumScale = 10.0f;

        public Vector3 LeftControllerPosition => m_LeftControllerTransform.position;

        public Vector3 RightControllerPosition => m_RightControllerTransform.position;

        // Whether or not we are currently performing the Grab Move action
        private bool m_IsMoving = false;

        public bool IsMoving
        {
            get => m_IsMoving;
            set
            {
                m_IsMoving = value;
                if (m_IsMoving)
                {
                    SaveForNextFrame();
                }
            }
        }

        public float OriginScale => m_XROrigin.localScale.x;

        // The information kept from the last frame
        private Vector3 m_LastLeftControllerPosition = Vector3.zero;
        private Vector3 m_LastRightControllerPosition = Vector3.zero;
        private Vector3 m_LastLeftControllerForward = Vector3.zero;
        private Vector3 m_LastRightControllerForward = Vector3.zero;

        // Start is called before the first frame update
        protected void Start()
        {
            foreach (Transform environment in m_Environment)
            {
                environment.parent = m_XROrigin;
            }
        }

        // Update is called once per frame
        protected void Update()
        {
            if (!m_IsMoving)
            {
                return;
            }

            ScaleOrigin();
            RotateOrigin();
            MoveOrigin();
            SaveForNextFrame();
        }

        /// <summary>
        /// Scale the origin
        /// </summary>
        private void ScaleOrigin()
        {
            float currentDistance =
                Vector3.Distance(m_LeftControllerTransform.position, m_RightControllerTransform.position);
            float lastDistance = Vector3.Distance(m_LastLeftControllerPosition, m_LastRightControllerPosition);
            float targetScale = m_XROrigin.localScale.x * lastDistance / currentDistance;
            targetScale = Mathf.Clamp(targetScale, 1.0f / m_MaximumScale, 1.0f / m_MinimumScale);
            m_XROrigin.localScale = Vector3.one * targetScale;
        }

        /// <summary>
        /// Rotate the origin according to the difference in position between controllers
        /// as well as according to the difference between their forward vector
        /// </summary>
        private void RotateOrigin()
        {
            // Rotation caused by a change in position
            Vector3 leftToRightDirection = m_RightControllerTransform.position - m_LeftControllerTransform.position;
            Vector3 lastLeftToRightDirection = m_LastRightControllerPosition - m_LastLeftControllerPosition;
            Vector3 rotationAxis = Vector3.Cross(leftToRightDirection.normalized, lastLeftToRightDirection.normalized);
            rotationAxis.Normalize();
            float rotationAngle = Vector3.SignedAngle(leftToRightDirection, lastLeftToRightDirection, rotationAxis);
            
            // Apply the rotation:
            Vector3 pivot = (m_RightControllerTransform.position + m_LeftControllerTransform.position) * 0.5f;
            m_XROrigin.RotateAround(pivot, rotationAxis, rotationAngle);
            
            // Rotation caused by a change in orientation
            Vector3 leftToRight = (m_RightControllerTransform.position - m_LeftControllerTransform.position).normalized;
            Vector3 currentAverageForward = (m_LeftControllerTransform.forward + m_RightControllerTransform.forward) * 0.5f;
            Vector3 projectedCurrentForward = Vector3.ProjectOnPlane(currentAverageForward, leftToRight);

            Vector3 lastLeftToRight = (m_LastRightControllerPosition - m_LastLeftControllerPosition).normalized;
            Vector3 lastAverageForward = (m_LastLeftControllerForward + m_LastRightControllerForward) * 0.5f;
            Vector3 projectedLastForward = Vector3.ProjectOnPlane(lastAverageForward, lastLeftToRight);

            Vector3 axisForward = Vector3.Cross(projectedCurrentForward, projectedLastForward).normalized;
            float angleForward = Vector3.SignedAngle(projectedCurrentForward, projectedLastForward, axisForward);

            // Apply the rotation:
            pivot = (m_RightControllerTransform.position + m_LeftControllerTransform.position) * 0.5f;
            m_XROrigin.RotateAround(pivot, axisForward, angleForward);
        }

        /// <summary>
        /// Scaling affects the position of the controllers, therefore we must get the position from the transform
        /// during this part
        /// </summary>
        private void MoveOrigin()
        {
            Vector3 currentMiddle = (m_LeftControllerTransform.position + m_RightControllerTransform.position) * 0.5f;
            Vector3 lastMiddle = (m_LastRightControllerPosition + m_LastLeftControllerPosition) * 0.5f;
            Vector3 move = lastMiddle - currentMiddle;
            m_XROrigin.position += move;
        }

        /// <summary>
        /// Saves the relevant information for the next frame.
        /// </summary>
        private void SaveForNextFrame()
        {
            m_LastLeftControllerPosition = m_LeftControllerTransform.position;
            m_LastRightControllerPosition = m_RightControllerTransform.position;
            m_LastLeftControllerForward = m_LeftControllerTransform.forward;
            m_LastRightControllerForward = m_RightControllerTransform.forward;
        }
    }
}
