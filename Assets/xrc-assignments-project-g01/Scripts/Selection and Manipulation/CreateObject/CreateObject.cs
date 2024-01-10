using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRC.Assignments.Project.G01
{
    public class CreateObject : MonoBehaviour
    {
        [SerializeField] private List<GameObject> primitives = new List<GameObject>(); //Holds the shape prefabs
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        [SerializeField] private Transform finalParent; //The parent of the created object
        [SerializeField] private Material finalMaterial; //The material of the created object after scaling is complete
        private GameObject shapeObject; //The created object
        private Dictionary<ShapeEnum, int> shapes = new Dictionary<ShapeEnum, int>();
        public ShapeEnum currentShape;

        // Is true while the right trigger is still held, tells when the object is still being scaled
        private bool _isScaling; 

        // Used to get the boundary of the created object
        private MeshRenderer _cubeMeshRenderer;

        private void Start()
        {
            currentShape = ShapeEnum.Cube;
            shapes.Add(ShapeEnum.Cube, 0);
            shapes.Add(ShapeEnum.Capsule, 1);
            shapes.Add(ShapeEnum.Sphere, 2);
            shapes.Add(ShapeEnum.Cylinder, 3);
        }

        // Update is called once per frame
        void Update()
        {
            // While the right trigger is held down, the scale of the created object
            // continuously adjusts based on the distance between the controllers
            if (_isScaling && shapeObject != null)
            {
                Bounds limits = _cubeMeshRenderer.bounds;
                Vector3 center = (leftController.position + rightController.position) * 0.5f;
                shapeObject.transform.position = center;
                float distanceFromCenter = Vector3.Distance(center, rightController.position);
                float currentSize = limits.extents.x;
                if (currentSize > 0.0f)
                {
                    shapeObject.transform.localScale *= distanceFromCenter / currentSize;
                }
            }
        }

        /// <summary>
        /// Creates the object and scales it when the right trigger is initially held
        /// </summary>
        public void OnCreateObject()
        {
            Vector3 center = (leftController.position + rightController.position) * 0.5f;
            shapeObject = Instantiate(primitives[shapes[currentShape]], center, Quaternion.identity); 
            shapeObject.transform.parent = rightController.parent;
            shapeObject.transform.rotation = rightController.parent.transform.rotation;
            _isScaling = true;
            _cubeMeshRenderer = shapeObject.GetComponentInChildren<MeshRenderer>();
            Bounds limits = _cubeMeshRenderer.bounds;
            float distanceFromCenter = Vector3.Distance(center, rightController.position);
            float currentSize = limits.extents.x;
            if (currentSize > 0.0f)
            {
                shapeObject.transform.localScale *= distanceFromCenter / currentSize;
            }
        }

        /// <summary>
        /// Stops scaling and releases the created object upon the right trigger being released
        /// </summary>
        public void OnCreateObjectCanceled() 
        {
            _isScaling = false;
            shapeObject.transform.parent = finalParent;
            _cubeMeshRenderer.material = finalMaterial;
            CreateObjectCommand command = new CreateObjectCommand(shapeObject);
            UndoRedo.Instance.AddCommand(command);
        }

    }
}