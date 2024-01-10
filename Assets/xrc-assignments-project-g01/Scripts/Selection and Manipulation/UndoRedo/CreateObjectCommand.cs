using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class CreateObjectCommand : UndoRedo.ICommand
    {
        private GameObject m_CreatedObject;
        
        public CreateObjectCommand(GameObject createdObject)
        {
            m_CreatedObject = createdObject;
        }
        
        public void Execute()
        {
        }

        public void Undo()
        {
            m_CreatedObject.SetActive(false);
        }

        public void Redo()
        {
            m_CreatedObject.SetActive(true);
        }

        public void ForgetAction()
        {
            if (!m_CreatedObject.activeSelf)
            {
                Object.Destroy(m_CreatedObject);
            }
        }
    }
}
