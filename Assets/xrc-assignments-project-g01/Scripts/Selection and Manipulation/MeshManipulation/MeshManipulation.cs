using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class MeshManipulation : MonoBehaviour
    {
        private MeshFilter m_ManipulatedMesh;
        
        [SerializeField]
        [Tooltip("The XR Origin.")]
        private Transform m_XROrigin;
        
        [SerializeField]
        [Tooltip("The direct interactor.")]
        private XRDirectInteractor m_XRDirectInteractor;
        
        [SerializeField] 
        [Tooltip("The interactable pregfab used for manipulation")]
        private GameObject m_EmptyInteractablePrefab;
        
        [SerializeField] 
        [Tooltip("The name of the layer to target")]
        private string m_LayerMaskName = "MeshManipulation";

        // Each vertex and their associated group
        private Dictionary<XRGrabInteractable, List<int>> m_VertexGroups = new Dictionary<XRGrabInteractable, List<int>>();
        private InteractionLayerMask m_TargetLayerMask;
        private InteractionLayerMask m_PreviousLayerMask;
        
        // The initial vertices
        private Vector3[] m_StartVertices;
        
        // The initial MeshCollider
        private MeshCollider m_MeshCollider = null;

        /// <summary>
        /// Get the appropriate target layer mask
        /// </summary>
        private void Start()
        {
            m_TargetLayerMask = InteractionLayerMask.GetMask(m_LayerMaskName);
        }

        // Update is called once per frame
        void Update()
        {
            if (m_VertexGroups.Count == 0)
            {
                return;
            }
            
            // Update vertices based on their interactor's location
            Vector3[] meshVertices = m_ManipulatedMesh.mesh.vertices;
            foreach (var pair in m_VertexGroups)
            {
                XRGrabInteractable interactable = pair.Key;
                List<int> affectedVertices = pair.Value;
                foreach (int vertexIndex in affectedVertices)
                {
                    meshVertices[vertexIndex] = m_ManipulatedMesh.transform.InverseTransformPoint(interactable.transform.position);
                }
            }
            m_ManipulatedMesh.mesh.vertices = meshVertices;
            m_ManipulatedMesh.mesh.RecalculateBounds();
        }

        /// <summary>
        /// Call this function when you need to start the manipulation.
        /// </summary>
        public void ManipulationStarted(MeshFilter meshFilter)
        {
            m_ManipulatedMesh = meshFilter;
            m_StartVertices = m_ManipulatedMesh.mesh.vertices;
            CreateControlPoints();
            SetLayerMask();
            SetMeshCollider();
        }

        /// <summary>
        /// Create the control points to manipulate the vertices
        /// </summary>
        private void CreateControlPoints()
        {
            // make sure we are starting fresh
            m_VertexGroups.Clear();
            
            // local space dictionary
            Dictionary<Vector3, List<int>> localVertexGroup = new Dictionary<Vector3, List<int>>();
            
            // add vertices
            Vector3[] vertices = m_ManipulatedMesh.mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                if (localVertexGroup.ContainsKey(vertex))
                {
                    localVertexGroup[vertex].Add(i);
                }
                else
                {
                    localVertexGroup[vertex] = new List<int>() {i};
                }
            }
            
            // Change how the data is saved
            foreach (var pair in localVertexGroup)
            {
                // convert vertices to world space
                Vector3 worldVertex = m_ManipulatedMesh.transform.TransformPoint(pair.Key);
                
                // Create an interactable at the vertex group position
                GameObject interactable = Instantiate(m_EmptyInteractablePrefab, worldVertex, Quaternion.identity);
                interactable.transform.SetParent(transform);
                interactable.transform.localScale *= m_XROrigin.localScale.x;
                
                XRGrabInteractable grabComponent = interactable.GetComponent<XRGrabInteractable>();
                m_VertexGroups[grabComponent] = pair.Value;
            }
        }

        /// <summary>
        /// Set the layer mask of the interactor so that only specific
        /// interactables can be selected.
        /// </summary>
        private void SetLayerMask()
        {
            // save previous
            m_PreviousLayerMask = m_XRDirectInteractor.interactionLayers;
            // set target
            m_XRDirectInteractor.interactionLayers = m_TargetLayerMask;
        }
        
        /// <summary>
        /// Finds and sets the mesh collider
        /// </summary>
        private void SetMeshCollider()
        {
            Collider[] colliders = m_ManipulatedMesh.gameObject.GetComponents<Collider>();
            List<Collider> collidersToDisable = new List<Collider>();

            bool foundMeshCollider = false;
            foreach (Collider individualCollider in colliders)
            {
                if (individualCollider is MeshCollider meshCollider)
                {
                    meshCollider.enabled = true;
                    m_MeshCollider = meshCollider;
                    m_MeshCollider.sharedMesh = null;
                    foundMeshCollider = true;
                }
                else
                {
                    collidersToDisable.Add(individualCollider);
                }
            }

            if (foundMeshCollider)
            {
                foreach (var individualCollider in collidersToDisable)
                {
                    individualCollider.enabled = false;
                }
            }
        }

        /// <summary>
        /// Call this function when you need to stop the manipulation.
        /// </summary>
        public void ManipulationStopped()
        {
            // Delete control points
            List<XRGrabInteractable> grabInteractables = new List<XRGrabInteractable>(m_VertexGroups.Keys);
            m_VertexGroups.Clear();
            foreach (XRGrabInteractable interactable in grabInteractables)
            {
                Destroy(interactable.gameObject);
            }
            
            // Apply the changes to the mesh collider
            if (m_MeshCollider != null)
            {
                m_MeshCollider.sharedMesh = null;
                m_MeshCollider.sharedMesh = m_ManipulatedMesh.mesh;
            }

            // restore layer mask
            m_XRDirectInteractor.interactionLayers = m_PreviousLayerMask;
            
            // Create the command
            MeshManipulationCommand command = new MeshManipulationCommand(m_ManipulatedMesh, m_StartVertices, m_MeshCollider);
            UndoRedo.Instance.AddCommand(command);
            
            m_MeshCollider = null;
        }
    }
}
