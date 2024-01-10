
using System.Collections.Generic;
using UnityEngine;

namespace XRC.Assignments.Project.G01
{

    public class SphereSelectCommand : UndoRedo.ICommand
    {
        private List<Transform> m_MovedObjects = new List<Transform>();
        private List<Vector3> m_InitialPositions = new List<Vector3>();
        private List<Quaternion> m_InitialRotations = new List<Quaternion>();
        private List<Vector3> m_FinalPositions = new List<Vector3>();
        private List<Quaternion> m_FinalRotations = new List<Quaternion>();
        
        public SphereSelectCommand(IEnumerable<Transform> movedObjects, 
            IEnumerable<Vector3> initialPositions, 
            IEnumerable<Quaternion> initialRotations)
        {
            foreach (Transform movedObject in movedObjects)
            {
                m_MovedObjects.Add(movedObject);
                m_FinalPositions.Add(movedObject.position);
                m_FinalRotations.Add(movedObject.rotation);
            }
            
            foreach (Vector3 initialPosition in initialPositions)
            {
                m_InitialPositions.Add(initialPosition);
            }
            
            foreach (Quaternion initialRotation in initialRotations)
            {
                m_InitialRotations.Add(initialRotation);
            }
        }
        
        public void Execute()
        {
        }

        public void Undo()
        {
            for (int i = 0; i < m_MovedObjects.Count; i++)
            {
                m_MovedObjects[i].position = m_InitialPositions[i];
                m_MovedObjects[i].rotation = m_InitialRotations[i];
            }
        }

        public void Redo()
        {
            for (int i = 0; i < m_MovedObjects.Count; i++)
            {
                m_MovedObjects[i].position = m_FinalPositions[i];
                m_MovedObjects[i].rotation = m_FinalRotations[i];
            }
        }

        public void ForgetAction()
        {
        }
    }
}
