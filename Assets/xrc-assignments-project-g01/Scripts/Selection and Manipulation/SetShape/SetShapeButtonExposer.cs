using System;
using UnityEngine;
using UnityEngine.UI;

namespace XRC.Assignments.Project.G01
{
    public class SetShapeButtonExposer : MonoBehaviour
    {
        public event Action<ShapeEnum> OnShapePressed;

        [SerializeField] private Button _cube;
        [SerializeField] private Button _capsule;
        [SerializeField] private Button _sphere;
        [SerializeField] private Button _cylinder;
        
    
        public void OnCubeSelect()
        {
            OnShapePressed?.Invoke(ShapeEnum.Cube);
            SetSelectedShape(ShapeEnum.Cube);
        }
        
        public void OnSphereSelect()
        {
            OnShapePressed?.Invoke(ShapeEnum.Sphere);
            SetSelectedShape(ShapeEnum.Sphere);
        }
        
        public void OnCapsuleSelect()
        {
            OnShapePressed?.Invoke(ShapeEnum.Capsule);
            SetSelectedShape(ShapeEnum.Capsule);
        }
        
        public void OnCylinderSelect()
        {
            OnShapePressed?.Invoke(ShapeEnum.Cylinder);
            SetSelectedShape(ShapeEnum.Cylinder);
        }

        public void SetSelectedShape(ShapeEnum shapeEnum)
        {
            switch (shapeEnum)
            {
                case ShapeEnum.Cube:
                    _cube.Select();
                    break;
                case ShapeEnum.Capsule:
                    _capsule.Select();
                    break;
                case ShapeEnum.Cylinder:
                    _cylinder.Select();
                    break;
                case ShapeEnum.Sphere:
                    _sphere.Select();
                    break;
                
            }
        }
    }
}