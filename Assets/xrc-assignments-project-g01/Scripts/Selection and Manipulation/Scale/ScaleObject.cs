
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace XRC.Assignments.Project.G01
    /// <Summary>
    /// This component contains the main logic for scaling the object per axis.
    /// For example, to make a unit cube into a pole-like shape, user is able to
    /// decrease scale of x and z, while maintaining or increasing scale of y.
    /// While a user grabs an object on the right controller, they can enable scaling
    /// per axes using button on left. Widgets appear in three dimensions on object.
    /// These can be dragged individually to change the scale. 
    /// </Summary>
{
    public class ScaleObject : MonoBehaviour
    {
        // The interactor responsible for selecting an object
        [SerializeField] 
        public XRDirectInteractor m_Interactor;
        
        // Scale Axes Layer Masks
        [SerializeField] 
        private string layerMaskName = "ScaleAxes";
        private InteractionLayerMask targetLayerMask;
        private InteractionLayerMask previousLayerMask;
        
        // Arrows
        [SerializeField] 
        [Tooltip("The scale axes forward arrow")]
        public GameObject m_ForwardArrow;
        private XRGrabInteractable m_ForwardArrowInteractable;
        public GameObject ForwardArrow => m_ForwardArrow;
        
        [SerializeField] 
        [Tooltip("The scale axes up arrow")]
        public GameObject m_UpArrow;
        private XRGrabInteractable m_UpArrowInteractable;
        public GameObject UpArrow => m_UpArrow;
        
        [SerializeField] 
        [Tooltip("The scale axes right arrow")]
        public GameObject m_RightArrow;
        private XRGrabInteractable m_RightArrowInteractable;
        public GameObject RightArrow => m_RightArrow;
        
        // Object Mesh
        private MeshFilter m_SelectedObject;
        
        // Scale factors
        private float forwardFactor;
        private float rightFactor;
        private float upFactor;
        
        // Arrow start positions
        private Vector3 forwardStartPosition;
        private Vector3 rightStartPosition;
        private Vector3 upStartPosition;
        
        // Position that interactor is clamped to
        private Vector3 forwardClampPosition;
        private Vector3 rightClampPosition;
        private Vector3 upClampPosition;

        // Planes representing arrow boundary
        private UnityEngine.Plane m_XPlane;
        private UnityEngine.Plane m_YPlane;
        private UnityEngine.Plane m_ZPlane;
        
        private float minScale = 1e-5f;
        
        // Undo Redo vectors
        private Vector3 m_StartScale;
        private Vector3 m_NewScale;
        
        /// <summary>
        /// Upon Awake, initialize Sphere Select and Interactor components,
        /// add arrow Grab Interactable components and listeners
        /// </summary>
        private void Start()
        {
            m_ForwardArrow.SetActive(false);
            m_RightArrow.SetActive(false);
            m_UpArrow.SetActive(false);
            
            targetLayerMask = InteractionLayerMask.GetMask(layerMaskName);
            
            m_SelectedObject = GetComponent<MeshFilter>();

            if (m_Interactor == null)
            {
                m_Interactor = GetComponent<XRDirectInteractor>();
            }
            
            // Set arrow Grab Interactable components, add listeners for enter and exited
            m_ForwardArrowInteractable = m_ForwardArrow.GetComponent<XRGrabInteractable>();
            m_UpArrowInteractable = m_UpArrow.GetComponent<XRGrabInteractable>();
            m_RightArrowInteractable = m_RightArrow.GetComponent<XRGrabInteractable>();
            
            m_ForwardArrowInteractable.selectEntered.AddListener(ForwardEntered);
            m_UpArrowInteractable.selectEntered.AddListener(UpEntered);
            m_RightArrowInteractable.selectEntered.AddListener(RightEntered);
            
            m_ForwardArrowInteractable.selectExited.AddListener(ForwardExited);
            m_UpArrowInteractable.selectExited.AddListener(UpExited);
            m_RightArrowInteractable.selectExited.AddListener(RightExited);
        }
        
        private void ForwardEntered(SelectEnterEventArgs arg0)
        {
            Vector3 arrowCurrentPosition = m_ForwardArrow.transform.position;
            Vector3 interactorPosition = arg0.interactableObject.GetAttachTransform(arg0.interactorObject).position;
            m_ZPlane = CreateClampPlane(GetForwardPointingVector(), arrowCurrentPosition, forwardClampPosition, interactorPosition);
        }

        private void UpEntered(SelectEnterEventArgs arg0)
        {
            Vector3 arrowCurrentPosition = m_UpArrow.transform.position;
            Vector3 interactorPosition = arg0.interactableObject.GetAttachTransform(arg0.interactorObject).position;
            m_YPlane = CreateClampPlane(GetUpPointingVector(), arrowCurrentPosition, upClampPosition, interactorPosition);
        }

        private void RightEntered(SelectEnterEventArgs arg0)
        {
            Vector3 arrowCurrentPosition = m_RightArrow.transform.position;
            Vector3 interactorPosition = arg0.interactableObject.GetAttachTransform(arg0.interactorObject).position;
            m_XPlane = CreateClampPlane(GetRightPointingVector(), arrowCurrentPosition, rightClampPosition, interactorPosition);
        }

        private void ForwardExited(SelectExitEventArgs arg0)
        {
            m_ForwardArrowInteractable.trackPosition = true;
        }

        private void UpExited(SelectExitEventArgs arg0)
        {
            m_UpArrowInteractable.trackPosition = true;
        }

        private void RightExited(SelectExitEventArgs arg0)
        {
            m_RightArrowInteractable.trackPosition = true;
        }
        
        /// <summary>
        /// Creates a plane to clamp arrows to a minimum position
        /// </summary>
        /// <param name="arrowNormal"></param>
        /// <param name="arrowCurrentPosition"></param>
        /// <param name="arrowClampPosition"></param>
        /// <param name="interactorPosition"></param>
        /// <returns></returns>
        private UnityEngine.Plane CreateClampPlane(Vector3 arrowNormal, Vector3 arrowCurrentPosition, Vector3 arrowClampPosition, Vector3 interactorPosition)
        {
            Vector3 objectPosition = m_SelectedObject.transform.position;
            Vector3 normal = arrowNormal;
            
            // Create the interactor vector
            Vector3 interactorVector = interactorPosition - objectPosition;
            
            // Obtain the interactor position, projected onto the normal
            Vector3 projectedInteractorPosition = Vector3.Project(interactorVector, normal) + objectPosition;
            float distanceWithArrow = Vector3.Distance(arrowCurrentPosition, projectedInteractorPosition);
            
            // check for distance flip, occurs when selecting behind the arrow
            Vector3 interactorVectorWithCurrentArrow = interactorPosition - arrowCurrentPosition;
            interactorVectorWithCurrentArrow.Normalize();
            float distanceFlip = Vector3.Dot(normal, interactorVectorWithCurrentArrow) < 0 ? -1.0f : 1.0f;
            
            Vector3 planePoint = arrowClampPosition + normal * distanceWithArrow * distanceFlip;
            return new UnityEngine.Plane(normal, planePoint);
        }

        /// <summary>
        /// The vector in which the right arrow is pointing, normalized
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRightPointingVector()
        {
            return m_RightArrow.transform.right;
        }
        
        /// <summary>
        /// The vector in which the up arrow is pointing, normalized
        /// </summary>
        /// <returns></returns>
        private Vector3 GetUpPointingVector()
        {
            return m_UpArrow.transform.up;
        }
        
        /// <summary>
        /// The vector in which the forward arrow is pointing, normalized
        /// </summary>
        /// <returns></returns>
        private Vector3 GetForwardPointingVector()
        {
            return m_ForwardArrow.transform.forward * -1.0f;
        }
        
        /// <summary>
        /// Set up the object grabbed for scaling, grabbing the forward, right and up positions
        /// Places new objects at the mesh bounds of interactable mesh, according to the forward,
        /// right and up directions.
        /// </summary>
        public void StartScaleObject(MeshFilter interactableMesh)
        {
            // Make arrows visible
            m_ForwardArrow.SetActive(true);
            m_RightArrow.SetActive(true);
            m_UpArrow.SetActive(true);
            
            // Change interaction layer
            previousLayerMask = m_Interactor.interactionLayers;
            m_Interactor.interactionLayers = targetLayerMask;
            
            // Set mesh
            m_SelectedObject = interactableMesh;
            Bounds bounds = m_SelectedObject.mesh.bounds;
            
            // Calculate arrow positions at the mesh bounds
            //Vector3 localForward = new Vector3(0, 0,bounds.center.z - bounds.extents.z); 
            //Vector3 localRight = new Vector3(bounds.center.x + bounds.extents.x, 0, 0);
            //Vector3 localUp = new Vector3(0, bounds.center.y + bounds.extents.y, 0);

            // Set arrow positions
            Vector3 forwardArrowPosition;
            Vector3 rightArrowPosition;
            Vector3 upArrowPosition;
            (forwardArrowPosition, rightArrowPosition, upArrowPosition) = GetGlobalArrowPositions(bounds);
            m_ForwardArrow.transform.position = forwardArrowPosition;
            m_RightArrow.transform.position = rightArrowPosition;
            m_UpArrow.transform.position = upArrowPosition;
            
            // Set arrow rotation based on object rotation
            m_ForwardArrow.transform.rotation = m_SelectedObject.transform.rotation;
            m_RightArrow.transform.rotation = m_SelectedObject.transform.rotation;
            m_UpArrow.transform.rotation = m_SelectedObject.transform.rotation;
            
            // Calculate factors for local scale over arrow prefab distance 
            forwardFactor = m_SelectedObject.transform.localScale.z / 
                            Vector3.Distance(m_ForwardArrow.transform.position, m_SelectedObject.transform.position);
            rightFactor = m_SelectedObject.transform.localScale.x /
                          Vector3.Distance(m_RightArrow.transform.position, m_SelectedObject.transform.position);
            upFactor = m_SelectedObject.transform.localScale.y / 
                       Vector3.Distance(m_UpArrow.transform.position, m_SelectedObject.transform.position);

            // initial positions
            forwardStartPosition = m_ForwardArrow.transform.position;
            rightStartPosition = m_RightArrow.transform.position;
            upStartPosition = m_UpArrow.transform.position;

            // Get Clamp
            m_StartScale = m_SelectedObject.transform.localScale;
            m_SelectedObject.transform.localScale = Vector3.one * minScale;
            
            (forwardArrowPosition, rightArrowPosition, upArrowPosition) = GetGlobalArrowPositions(bounds);

            // Set arrow positions
            forwardClampPosition = forwardArrowPosition;
            rightClampPosition = rightArrowPosition;
            upClampPosition = upArrowPosition;
            
            m_SelectedObject.transform.localScale = m_StartScale;
        }

        /// <summary>
        /// Returns the arrow position, in global space
        /// </summary>
        /// <param name="bounds">The bounds used to compute the arrow positions</param>
        /// <returns>forwardArrowPosition, rightArrowPosition, upArrowPosition</returns>
        private (Vector3, Vector3, Vector3) GetGlobalArrowPositions(Bounds bounds)
        {
            Vector3 localForward = new Vector3(0, 0,bounds.center.z - bounds.extents.z); 
            Vector3 localRight = new Vector3(bounds.center.x + bounds.extents.x, 0, 0);
            Vector3 localUp = new Vector3(0, bounds.center.y + bounds.extents.y, 0);
            
            Vector3 forwardArrowPosition = m_SelectedObject.transform.TransformPoint(localForward);
            Vector3 rightArrowPosition = m_SelectedObject.transform.TransformPoint(localRight);
            Vector3 upArrowPosition = m_SelectedObject.transform.TransformPoint(localUp);

            return (forwardArrowPosition, rightArrowPosition, upArrowPosition);
        }
        
        /// <summary>
        /// Returns the first scale interactable selected. When there are no valid objects selected, returns.
        /// Compute the object scale to match the change in distance of the arrow prefab in the given axes from
        /// the original position.
        /// </summary>
        /// <returns>IXRInteractable</returns>
        private void Update()
        {
            if (!m_ForwardArrow.activeSelf)
            {
                return;                
            }

            if (!m_SelectedObject)
            {
                return;
            }
                        
            // Get distance vector between arrow and object 
            float deltaRight = Vector3.Distance(m_RightArrow.transform.position, m_SelectedObject.transform.position); 
            float deltaUp = Vector3.Distance(m_UpArrow.transform.position, m_SelectedObject.transform.position); 
            float deltaForward = Vector3.Distance(m_ForwardArrow.transform.position, m_SelectedObject.transform.position);
            
            // Determine direction of arrow movement
            float xSign = GetArrowSign(rightStartPosition, m_RightArrow.transform.position);
            float ySign = GetArrowSign(upStartPosition, m_UpArrow.transform.position);
            float zSign = GetArrowSign(forwardStartPosition, m_ForwardArrow.transform.position);

            // Ensure scale of object does not reach 0
            float scaleX = Mathf.Max(minScale, rightFactor * deltaRight * xSign);
            float scaleY = Mathf.Max(minScale, upFactor * deltaUp * ySign);
            float scaleZ = Mathf.Max(minScale, forwardFactor * deltaForward * zSign);

            m_SelectedObject.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            
            // Clamp arrow and turn track position to false
            ClampArrow(m_RightArrowInteractable, m_XPlane, rightClampPosition);
            ClampArrow(m_UpArrowInteractable, m_YPlane, upClampPosition);
            ClampArrow(m_ForwardArrowInteractable, m_ZPlane, forwardClampPosition);
        }
        
        /// <summary>
        /// Gets the sign of the scale factor used for an arrow
        /// </summary>
        /// <param name="arrowStartPosition"></param>
        /// <param name="arrowCurrentPosition"></param>
        /// <returns></returns>
        private float GetArrowSign(Vector3 arrowStartPosition, Vector3 arrowCurrentPosition)
        {
            Vector3 objectPosition = m_SelectedObject.transform.position;
            Vector3 startVector = arrowStartPosition - objectPosition;
            Vector3 currentVector = arrowCurrentPosition - objectPosition;
            float dotResult = Vector3.Dot(startVector.normalized, currentVector.normalized);
            return dotResult < 0 ? -1.0f : 1.0f;
        }
        
        /// <summary>
        /// Clamps the arrow, stopping position track if reaching beyond the plane boundary
        /// Set the transform position to the clamp position parameter
        /// </summary>
        /// <param name="interactable"></param>
        /// <param name="plane"></param>
        /// <param name="clampPosition"></param>
        private void ClampArrow(XRGrabInteractable interactable, UnityEngine.Plane plane, Vector3 clampPosition)
        {
            if (interactable.isSelected)
            {
                // If within the plane boundary do nothing, else stop track position and clamp interactable
                if (plane.GetSide(m_Interactor.GetAttachTransform(interactable).position))
                {
                    interactable.trackPosition = true;
                }
                else
                {
                    interactable.trackPosition = false;
                    interactable.transform.position = clampPosition;
                }
            }
        }

        /// <summary>
        /// Handling of the end of the Scale selection
        /// </summary>
        public void StopScaleObject()
        {
            m_NewScale = m_SelectedObject.transform.localScale;
            
            // Remove the scale axes arrows
            m_ForwardArrow.SetActive(false);
            m_RightArrow.SetActive(false);
            m_UpArrow.SetActive(false);
            
            // Make object movable with sphere select again
            m_Interactor.interactionLayers = previousLayerMask;
            
            // Implement Undo/Redo capabilities
            ScaleObjectCommand command = new ScaleObjectCommand(m_SelectedObject, m_StartScale, m_NewScale);
            UndoRedo.Instance.AddCommand(command);
        }
        
    }
        
}
