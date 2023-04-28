using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        private PlayerConversant _playerConversant = null;
        [SerializeField] private string _action;
        [SerializeField] private UnityEvent onTrigger;
        public void Trigger(string actionToTrigger)
        {
            if(actionToTrigger == _action)
            {
                onTrigger.Invoke();
            }
        }
    }
}