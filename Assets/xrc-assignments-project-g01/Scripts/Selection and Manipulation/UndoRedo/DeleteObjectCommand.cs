using UnityEngine;

namespace XRC.Assignments.Project.G01
{

    public class DeleteObjectCommand : UndoRedo.ICommand
    {
        private GameObject m_DeletedObject;
        
        public DeleteObjectCommand(GameObject deletedObject)
        {
            m_DeletedObject = deletedObject;
        }
        
        public void Execute()
        {
            m_DeletedObject.SetActive(false);
        }

        public void Undo()
        {
            m_DeletedObject.SetActive(true);
        }

        public void Redo()
        {
            m_DeletedObject.SetActive(false);
        }

        public void ForgetAction()
        {
            if (!m_DeletedObject.activeSelf)
            {
                Object.Destroy(m_DeletedObject);
            }
        }
    }
}
