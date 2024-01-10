using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class DeleteObject : MonoBehaviour
    {
        [SerializeField] private XRDirectInteractor m_Interactor;
        [SerializeField] private SphereSelect sphereSel;

        /// <summary>
        /// Goes through all selected objects and deletes them
        /// </summary>
        public void OnDeleteObject()
        {
            List<IXRInteractable> targets = new List<IXRInteractable>(m_Interactor.interactablesSelected);
            sphereSel.CancelSelect();
            foreach (IXRInteractable target in targets)
            {
                if (target is XRBaseInteractable xrInteractable)
                {
                    DeleteObjectCommand command = new DeleteObjectCommand(xrInteractable.gameObject);
                    UndoRedo.Instance.AddCommand(command);
                }
            }
        }
    }
}