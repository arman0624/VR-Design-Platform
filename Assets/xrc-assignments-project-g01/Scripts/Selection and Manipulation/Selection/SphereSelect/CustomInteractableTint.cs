using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(MeshRenderer))]
public class CustomInteractableTint : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Tint color for interactable.")]
    private Color m_TintColor = Color.yellow;
    private Color m_originalTintColor = Color.black;
    
    [SerializeField]
    [Tooltip("Tint color property name.")]
    private string m_TintPropertyName = "_EmissionColor";
    private int m_TintPropertyID;

    public int TintPropertyID => m_TintPropertyID;

    private XRGrabInteractable m_XRGrabInteractable;
    private MeshRenderer m_MeshRenderer;
    
    
    // Start is called before the first frame update
    public void Start()
    {
        m_XRGrabInteractable = GetComponent<XRGrabInteractable>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_TintPropertyID = Shader.PropertyToID(m_TintPropertyName);
        
        m_XRGrabInteractable.hoverEntered.AddListener(HoverEnteredListener);
        m_XRGrabInteractable.hoverExited.AddListener(HoverExitedListener);
        
        m_XRGrabInteractable.selectEntered.AddListener(SelectEnteredListener);
        m_XRGrabInteractable.selectExited.AddListener(SelectExitedListener);
    }

    private void OnDestroy()
    {
        m_XRGrabInteractable.hoverEntered.RemoveListener(HoverEnteredListener);
        m_XRGrabInteractable.hoverExited.RemoveListener(HoverExitedListener);
        
        m_XRGrabInteractable.selectEntered.RemoveListener(SelectEnteredListener);
        m_XRGrabInteractable.selectExited.RemoveListener(SelectExitedListener);
    }

    private void HoverEnteredListener(HoverEnterEventArgs args)
    {
        ApplyTint();
    }

    private void HoverExitedListener(HoverExitEventArgs args)
    {
        RemoveTint();
    }
    
    private void SelectEnteredListener(SelectEnterEventArgs args)
    {
        RemoveTint();
    }
    
    private void SelectExitedListener(SelectExitEventArgs args)
    {
        if (args.interactableObject is XRBaseInteractable interactable)
        {
            if (interactable.isHovered)
            {
                ApplyTint();
            }
        }
    }

    /// <summary>
    /// Apply the tint on an object
    /// </summary>
    private void ApplyTint()
    {
        if (m_MeshRenderer.material.HasProperty(m_TintPropertyID))
        {
            m_MeshRenderer.material.SetColor(m_TintPropertyID, m_TintColor);
        }
    }

    /// <summary>
    /// Remove the tint on an object
    /// </summary>
    public void RemoveTint()
    {
        if (m_MeshRenderer.material.HasProperty(m_TintPropertyID))
        {
            m_MeshRenderer.material.SetColor(m_TintPropertyID, m_originalTintColor);
        }
    }
}
