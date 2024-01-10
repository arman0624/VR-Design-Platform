using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace XRC.Assignments.Project.G01
{
    public class SetShape: MonoBehaviour
    {
        [SerializeField] private Transform leftController;
        [SerializeField] private GameObject m_SetShapeMenu;
        [SerializeField] private UIRayInteractorManager uiRay;
        [SerializeField] private Camera m_MainCamera;
        [SerializeField] private Canvas m_MenuCanvas;
        [SerializeField] private CreateObject m_CreateObject;
        public bool menuIsOpen;
        private SetShapeButtonExposer m_exposer;

        public void Start()
        {
            m_exposer = m_MenuCanvas.GetComponent<SetShapeButtonExposer>();
            m_exposer.OnShapePressed += OnShapeSelectFeedback;
            m_exposer.SetSelectedShape(m_CreateObject.currentShape);
            m_MenuCanvas.worldCamera = m_MainCamera;
            menuIsOpen = false;
        }

        private void OnDestroy()
        {
            m_exposer.OnShapePressed -= OnShapeSelectFeedback;
        }

        private void OnShapeSelectFeedback(ShapeEnum selectedShape)
        {
            m_CreateObject.currentShape = selectedShape;
        }

        /// <summary>
        /// This function opens the set shape menu and enables the use of the UI ray interactor to select
        /// menu options. It also sets the menu to be slightly right of the left controller and to follow
        /// the left controller. The flag tracking menu open status is also set to indicate it's open
        /// </summary>
        public void OpenSetObjectShapeMenu()
        {
            uiRay.EnableRayInteractor(true);
            menuIsOpen = true;
            Vector3 menuOffset = leftController.right * 0.08f * leftController.lossyScale.x;
            m_SetShapeMenu.transform.rotation = Quaternion.identity;
            m_SetShapeMenu.transform.position = leftController.position;
            m_SetShapeMenu.transform.Translate(menuOffset, Space.Self);
            
            m_SetShapeMenu.transform.LookAt(m_MainCamera.transform);
            Vector3 eulerRotation = m_SetShapeMenu.transform.localRotation.eulerAngles;
            // Remove local rotation along z and flip
            m_SetShapeMenu.transform.localRotation *= Quaternion.Inverse(Quaternion.Euler(0, 180, eulerRotation.z));
            
            m_SetShapeMenu.transform.parent = leftController;
            m_exposer.SetSelectedShape(m_CreateObject.currentShape);
            m_SetShapeMenu.SetActive(true);
        }

        /// <summary>
        /// This function closes the menu, deactivates the UI ray interactor, stops the menu from following
        /// the controller anymore by making its parent stationary, and updates the flag to show the menu
        /// is closed
        /// </summary>
        public void CloseSetObjectShapeMenu()
        {
            uiRay.EnableRayInteractor(false);
            m_SetShapeMenu.SetActive(false);
            m_SetShapeMenu.transform.parent = transform;
            menuIsOpen = false;
        }
    }
}