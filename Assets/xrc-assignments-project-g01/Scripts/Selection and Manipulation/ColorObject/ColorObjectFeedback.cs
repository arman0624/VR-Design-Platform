using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class ColorFeedback : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The color wheel that will be visually affected.")]
        private Transform m_ColorWheelTransform;
        [SerializeField]
        [Tooltip("The color wheel indicator.")]
        private Transform m_ColorWheelIndicator;
        [SerializeField]
        [Tooltip("The color wheel that will be visually affected.")]
        private Material m_ColorWheelMaterial;
        private Vector3 m_ColorWheelStartHSVColor;
        
        [SerializeField] 
        [Tooltip("The material of the pointer that will match the selected color.")]
        private Material m_PointerMaterial;
        private Color m_StartPointerColor;
        
        // The logic component
        private ColorObject m_ColorObject;
        
        // The starting scale of the color wheel
        private float m_StartScaleZ;
        
        // The starting local middle point of wheel
        private float m_StartLocalPositionZ;

        private void Awake()
        {
            Color.RGBToHSV(m_ColorWheelMaterial.color, out float h, out float s, out float v);
            m_ColorWheelStartHSVColor = new Vector3(h, s, v);

            m_StartPointerColor = m_PointerMaterial.color;
            
            m_ColorObject = GetComponent<ColorObject>();
        }

        private void OnDestroy()
        {
            Color originalColor = Color.HSVToRGB(m_ColorWheelStartHSVColor.x, m_ColorWheelStartHSVColor.y, m_ColorWheelStartHSVColor.z);
            m_ColorWheelMaterial.color = originalColor;
            m_PointerMaterial.color = m_StartPointerColor;
        }


        // Start is called before the first frame update
        void Start()
        {
            m_StartScaleZ = m_ColorWheelTransform.localScale.z;
            m_StartLocalPositionZ = m_ColorWheelTransform.localPosition.z;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateColorWheel();
            UpdateColorWheelIndicator();
        }

        /// <summary>
        /// Updates the color wheel position as well as the pointer.
        /// </summary>
        void UpdateColorWheel()
        {
            UpdateColorWheelBrightness();
            
            float newScaleZ = m_ColorObject.HSV.z * m_StartScaleZ;
            float newLocalPositionZ = m_StartLocalPositionZ - m_StartScaleZ / 2 * (1.0f - m_ColorObject.HSV.z);

            Vector3 currentScale = m_ColorWheelTransform.localScale;
            currentScale.z = newScaleZ;
            m_ColorWheelTransform.localScale = currentScale;
            
            Vector3 currentLocalPosition = m_ColorWheelTransform.localPosition;
            currentLocalPosition.z = newLocalPositionZ;
            m_ColorWheelTransform.localPosition = currentLocalPosition;

            m_PointerMaterial.color = Color.HSVToRGB(m_ColorObject.HSV.x, m_ColorObject.HSV.y, m_ColorObject.HSV.z);
        }
        
        /// <summary>
        /// Updates the color wheel indicator.
        /// I.E.: The small circle indicating the selected color.
        /// </summary>
        private void UpdateColorWheelIndicator()
        {
            Vector3 localPosition = m_ColorWheelIndicator.transform.localPosition;
            float x = m_ColorObject.HSV.y * -0.5f * Mathf.Cos(m_ColorObject.HSV.x * 2.0f * Mathf.PI);
            float y = m_ColorObject.HSV.y * -0.5f * Mathf.Sin(m_ColorObject.HSV.x * 2.0f * Mathf.PI);
            localPosition.x = x;
            localPosition.y = y;
            m_ColorWheelIndicator.transform.localPosition = localPosition;
        }

        /// <summary>
        /// Updates the color wheel brightness.
        /// </summary>
        void UpdateColorWheelBrightness()
        {
            Vector3 newHSV = new Vector3(m_ColorWheelStartHSVColor.x, m_ColorWheelStartHSVColor.y, m_ColorWheelStartHSVColor.z);
            newHSV.z = m_ColorObject.HSV.z;
            m_ColorWheelMaterial.color = Color.HSVToRGB(newHSV.x, newHSV.y, newHSV.z);
        }
    }
}
