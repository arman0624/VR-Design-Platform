
using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class ColorObjectCommand : UndoRedo.ICommand
    {
        private MeshRenderer m_MeshRenderer;
        private Color m_StartColor;
        private Color m_NewColor;
        
        public ColorObjectCommand(MeshRenderer meshRenderer, Color startColor, Color newColor)
        {
            m_MeshRenderer = meshRenderer;
            m_StartColor = startColor;
            m_NewColor = newColor;
        }
        
        public void Execute()
        {
        }

        public void Undo()
        {
            m_MeshRenderer.material.color = m_StartColor;
        }

        public void Redo()
        {
            m_MeshRenderer.material.color = m_NewColor;
        }

        public void ForgetAction()
        {
        }
    }
}
