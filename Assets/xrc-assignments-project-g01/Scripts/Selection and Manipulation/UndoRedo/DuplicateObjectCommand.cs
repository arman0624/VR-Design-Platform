
using UnityEngine;

namespace XRC.Assignments.Project.G01
{

    public class DuplicateObjectCommand : UndoRedo.ICommand
    {
        private GameObject m_DuplicatedObject;
        
        public DuplicateObjectCommand(GameObject duplicatedObject)
        {
            m_DuplicatedObject = duplicatedObject;
        }
        
        public void Execute()
        {
        }

        public void Undo()
        {
            m_DuplicatedObject.SetActive(false);
        }

        public void Redo()
        {
            m_DuplicatedObject.SetActive(true);
        }

        public void ForgetAction()
        {
            if (!m_DuplicatedObject.activeSelf)
            {
                Object.Destroy(m_DuplicatedObject);
            }
        }
    }
}
