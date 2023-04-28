using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionSheduler : MonoBehaviour
    {
        private IAction _currentAction;
        public void StartAction(IAction action)
        {
            if (_currentAction == action) return;

            if(_currentAction != null)
            {
                _currentAction.Cancel();
            }
            _currentAction = action;
        }
        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
