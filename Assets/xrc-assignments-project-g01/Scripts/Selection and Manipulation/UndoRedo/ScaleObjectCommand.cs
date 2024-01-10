
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace XRC.Assignments.Project.G01
{

    public class ScaleObjectCommand : UndoRedo.ICommand
    {
        private MeshFilter m_MeshFilter;
        private UnityEngine.Vector3 m_StartScale;
        private UnityEngine.Vector3 m_NewScale;
        
        public ScaleObjectCommand(MeshFilter meshFilter, UnityEngine.Vector3 startScale, UnityEngine.Vector3 newScale)
        {
            m_MeshFilter = meshFilter;
            m_StartScale = startScale;
            m_NewScale = newScale;
        }
        public void Execute()
        {
        }

        public void Undo()
        {
            m_MeshFilter.transform.localScale = m_StartScale;
        }

        public void Redo()
        {
            m_MeshFilter.transform.localScale = m_NewScale;
        }

        public void ForgetAction()
        {
        }
    }
}
