using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class HelpAndDocumentation: MonoBehaviour
    {
        [SerializeField] private Transform leftController;
        [SerializeField] private GameObject m_HelpAndDocumentationMenu;
        [SerializeField] private UIRayInteractorManager uiRay;
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private Canvas m_MenuCanvas;
        public bool menuIsOpen;

        public void Start()
        {
            m_MenuCanvas.worldCamera = m_MainCamera;
            menuIsOpen = false;
        }

        /// <summary>
        /// This function opens the help menu and enables the use of the UI ray interactor to use the
        /// scrollbar. It also sets the menu to be slightly right of the left controller and to follow
        /// the left controller. The flag tracking menu open status is also set to indicate it's open
        /// </summary>
        public void OpenHelpAndDocumentationMenu()
        {
            uiRay.EnableRayInteractor(true);
            menuIsOpen = true;
            Vector3 menuOffset = leftController.right * 0.1f * leftController.lossyScale.x;
            m_HelpAndDocumentationMenu.transform.rotation = Quaternion.identity;
            m_HelpAndDocumentationMenu.transform.position = leftController.position;
            m_HelpAndDocumentationMenu.transform.Translate(menuOffset, Space.Self);
            
            m_HelpAndDocumentationMenu.transform.LookAt(m_MainCamera.transform);
            Vector3 eulerRotation = m_HelpAndDocumentationMenu.transform.localRotation.eulerAngles;
            // Remove local rotation along z and flip
            m_HelpAndDocumentationMenu.transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(0, 180, eulerRotation.z));
            
            m_HelpAndDocumentationMenu.transform.parent = leftController;
            m_HelpAndDocumentationMenu.SetActive(true);
        }

        /// <summary>
        /// This function closes the menu, deactivates the UI ray interactor, stops the menu from following
        /// the controller anymore by making its parent stationary, and updates the flag to show the menu
        /// is closed
        /// </summary>
        public void CloseHelpAndDocumentationMenu()
        {
            uiRay.EnableRayInteractor(false);
            m_HelpAndDocumentationMenu.SetActive(false);
            m_HelpAndDocumentationMenu.transform.parent = transform;
            menuIsOpen = false;
        }
    }
}