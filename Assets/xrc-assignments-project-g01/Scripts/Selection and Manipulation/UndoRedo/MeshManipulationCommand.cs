using UnityEngine;

namespace XRC.Assignments.Project.G01
{

    public class MeshManipulationCommand : UndoRedo.ICommand
    {
        private MeshFilter m_MeshFilter;
        private MeshCollider m_MeshCollider;
        private Vector3[] m_StartVertices;
        private Vector3[] m_NewVertices;

        public MeshManipulationCommand(MeshFilter finalMeshFilter, Vector3[] startVertices, MeshCollider meshCollider)
        {
            m_MeshCollider = meshCollider;
            m_MeshFilter = finalMeshFilter;
            m_StartVertices = startVertices;
            m_NewVertices = finalMeshFilter.mesh.vertices;
        }
        
        public void Execute()
        {
        }

        public void Undo()
        {
            m_MeshFilter.mesh.vertices = m_StartVertices;
            m_MeshFilter.mesh.RecalculateBounds();
            if (m_MeshCollider != null)
            {
                m_MeshCollider.sharedMesh = null;
                m_MeshCollider.sharedMesh = m_MeshFilter.mesh;
            }
            
        }

        public void Redo()
        {
            m_MeshFilter.mesh.vertices = m_NewVertices;
            m_MeshFilter.mesh.RecalculateBounds();
            if (m_MeshCollider != null)
            {
                m_MeshCollider.sharedMesh = null;
                m_MeshCollider.sharedMesh = m_MeshFilter.mesh;
            }
            
        }

        public void ForgetAction()
        {
        }
    }
}
