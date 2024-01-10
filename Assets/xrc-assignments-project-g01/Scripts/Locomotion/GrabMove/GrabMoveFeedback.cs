using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class GrabMoveFeedback : MonoBehaviour
    {
        [SerializeField] [Tooltip("The Color of the pivot used for Grab Move.")]
        private Color m_PivotColor = Color.red;

        [SerializeField] [Tooltip("The Color of the line between both controllers during Grab Move.")]
        private Color m_LineColor = Color.grey;

        [SerializeField] [Tooltip("The width of the line between both controllers during Grab Move.")]
        private float m_LineWidth = 0.002f;

        [SerializeField] [Tooltip("The text gameobject.")]
        private GameObject m_PercentageText;

        [SerializeField] [Tooltip("The camera for the text to follow.")]
        private Transform m_CameraTransform = null;
        
        [SerializeField]
        [Tooltip("The left controller interactor.")]
        private XRBaseControllerInteractor m_LeftControllerInteractor;

        [SerializeField]
        [Tooltip("The right controller interactor.")]
        private XRBaseControllerInteractor m_RightControllerInteractor;

        [SerializeField]
        private float m_HapticFeedbackStrength = 0.1f;
        [SerializeField]
        private float m_HapticFeedbackDuration = 0.025f;
        

        // The Grab Move Component we are bound to
        private GrabMove m_GrabMove;

        // The TextMesh we will put the current scale in
        private TextMesh m_TextMesh = null;

        // The line between both controllers
        private LineRenderer m_LineRenderer = null;
        private GameObject m_LineObject = null;

        // The pivot displayed
        private GameObject m_PivotObject = null;
        private Vector3 m_PivotScale = Vector3.one * 0.025f;
        
        // The last displayed scale
        private float m_LastHapticScale = 100.0f;

        void Awake()
        {
            m_GrabMove = GetComponent<GrabMove>();
            (m_LineRenderer, m_LineObject) = BuildVisualScale();
            m_TextMesh = m_PercentageText.GetComponent<TextMesh>();
            m_TextMesh.color = m_LineColor;
            m_PivotObject = BuildPivotObject();
            SetChildrenActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_GrabMove.IsMoving)
            {
                SetChildrenActive(false);
                return;
            }

            SetChildrenActive(true);

            Vector3 leftControllerPosition = m_GrabMove.LeftControllerPosition;
            Vector3 rightControllerPosition = m_GrabMove.RightControllerPosition;
            Vector3 middle = (leftControllerPosition + rightControllerPosition) / 2.0f;

            UpdatePivot(middle);
            UpdateLine(leftControllerPosition, rightControllerPosition);
            UpdateText(middle);
        }

        protected void OnDestroy()
        {
            Destroy(m_LineObject);
            Destroy(m_PivotObject);
        }

        /// <summary>
        /// Update the pivot position
        /// </summary>
        /// <param name="middle">Where the pivot should be placed</param>
        private void UpdatePivot(Vector3 middle)
        {
            m_PivotObject.transform.position = middle;
        }

        /// <summary>
        /// Update the line position
        /// </summary>
        /// <param name="start">The start of the line</param>
        /// <param name="end">The end of the line</param>
        private void UpdateLine(Vector3 start, Vector3 end)
        {
            m_LineRenderer.widthMultiplier = m_LineWidth * m_GrabMove.OriginScale;
            m_LineRenderer.SetPosition(0, start);
            m_LineRenderer.SetPosition(1, end);
        }

        /// <summary>
        /// Update the text placement and rotation
        /// </summary>
        /// <param name="middle">Where the text should be placed</param>
        private void UpdateText(Vector3 middle)
        {
            m_PercentageText.transform.rotation = m_CameraTransform.rotation;
            m_PercentageText.transform.position = middle;
            m_PercentageText.transform.localPosition += new Vector3(0, m_PivotScale.y, 0);

            float displayedScale = float.PositiveInfinity;
            if (m_GrabMove.OriginScale != 0.0f)
            {
                displayedScale = 100 / m_GrabMove.OriginScale;
            }

            m_TextMesh.text = displayedScale.ToString("F1") + "%";

            // send haptic feedback
            float last = Mathf.Floor(m_LastHapticScale);
            float current = Mathf.Floor(displayedScale);
            if (!Mathf.Approximately(last, current))
            {
                m_RightControllerInteractor.SendHapticImpulse(m_HapticFeedbackStrength, m_HapticFeedbackDuration);
                m_LeftControllerInteractor.SendHapticImpulse(m_HapticFeedbackStrength, m_HapticFeedbackDuration);
                m_LastHapticScale = displayedScale;
            }

           
        }

        /// <summary>
        /// Builds the line that will sit between controllers
        /// </summary>
        /// <returns>A LineRenderer and the created GameObject</returns>
        private (LineRenderer, GameObject) BuildVisualScale()
        {
            const string childName = "ScaleRenderer";
            GameObject childObject = new GameObject(childName);
            childObject.transform.parent = gameObject.transform;

            LineRenderer line = childObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));

            line.name = childName;
            line.positionCount = 2;
            line.widthMultiplier = m_LineWidth;
            line.startColor = m_LineColor;
            line.endColor = m_LineColor;

            return (line, childObject);
        }

        /// <summary>
        /// Build the pivot
        /// </summary>
        /// <returns>The pivot GameObject</returns>
        private GameObject BuildPivotObject()
        {
            const string pivotName = "pîvot";
            GameObject pivot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pivot.transform.localScale = m_PivotScale;
            pivot.transform.parent = gameObject.transform;

            MeshRenderer pivotRenderer = pivot.GetComponent<MeshRenderer>();
            pivotRenderer.material = new Material(Shader.Find("Unlit/Color"));
            pivotRenderer.material.color = m_PivotColor;
            pivot.name = pivotName;

            return pivot;
        }

        /// <summary>
        /// Changes the activity of visual elements
        /// </summary>
        /// <param name="active">True if the children should be active</param>
        private void SetChildrenActive(bool active)
        {
            m_LineObject.SetActive(active);
            m_PivotObject.SetActive(active);
            m_PercentageText.SetActive(active);
        }
    }
}
