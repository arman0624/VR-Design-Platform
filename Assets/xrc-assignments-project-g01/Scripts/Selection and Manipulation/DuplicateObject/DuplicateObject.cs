using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRC.Assignments.Project.G01
{
    public class DuplicateObject : MonoBehaviour
    {
        [SerializeField] private XRDirectInteractor m_Interactor;
        [SerializeField] private Transform finalParent;
        [SerializeField] private Material finalMaterial;

        /// <summary>
        /// Goes through all selected objects and duplicates them
        /// </summary>
        public void OnDuplicateObject()
        {
            List<IXRInteractable> targets = new List<IXRInteractable>(m_Interactor.interactablesSelected);
            foreach (IXRInteractable target in targets)
            {
                if (target is XRBaseInteractable xrInteractable)
                {
                    // copy
                    XRBaseInteractable copy = Instantiate(xrInteractable, xrInteractable.transform.position, xrInteractable.transform.rotation);
                    copy.transform.parent = finalParent;
                    
                    // Remove tint
                    CustomInteractableTint tint = copy.gameObject.GetComponent<CustomInteractableTint>();
                    tint.Start();
                    tint.RemoveTint();
                    
                    // create command
                    DuplicateObjectCommand command = new DuplicateObjectCommand(copy.gameObject);
                    UndoRedo.Instance.AddCommand(command);
                }
            }
        }
    }
}