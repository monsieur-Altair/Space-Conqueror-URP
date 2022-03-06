using System;
using System.Collections;
using UnityEngine;

namespace Control
{
    public class Zone : MonoBehaviour
    {
        private SphereCollider _zone;
        public delegate void TriggerFunction(Collider other);

        private TriggerFunction _triggerAction;
        
        public void Start()
        {
            _zone = GetComponent<SphereCollider>();
            if (_zone == null)
                throw new MyException("cannot get zone collider");
        }

        public void SetTriggerFunction(TriggerFunction function)
        {
            _triggerAction = function;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            _triggerAction(other);
        }
    }
    
}