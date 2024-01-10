using System;
using UnityEngine;

namespace XRC.Assignments.Project.G01
{
    public class TriggerExposer : MonoBehaviour
    {

        public event Action<Collider> TriggerEnterAction;
        public event Action<Collider> TriggerExitAction;
        public event Action<Collider> TriggerStayAction;

        protected void OnTriggerEnter(Collider other)
        {
            TriggerEnterAction?.Invoke(other);
        }

        protected void OnTriggerExit(Collider other)
        {
            TriggerExitAction?.Invoke(other);
        }

        protected void OnTriggerStay(Collider other)
        {
            TriggerStayAction?.Invoke(other);
        }
    }
}
