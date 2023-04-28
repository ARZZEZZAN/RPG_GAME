using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Abilities
{
    public class AbilityData : IAction
    {
        private GameObject _user;
        private IEnumerable<GameObject> _targets;
        private Vector3 _targetedPoint;
        private bool _cancelled;

        public AbilityData(GameObject user)
        {
            _user = user;
        }

        public void SetTargets(IEnumerable<GameObject> targets)
        {
            _targets = targets;
        }
        public void SetTargetedPoint(Vector3 targetedPoint)
        {
            _targetedPoint = targetedPoint;
        }
        public Vector3 GetTargetedPoint()
        {
            return _targetedPoint;
        }
        public IEnumerable<GameObject> GetTargets()
        {
            return _targets;
        }

        public GameObject GetUser()
        {
            return _user;
        }
        public void StartCoroutine(IEnumerator coroutine)
        {
            _user.GetComponent<MonoBehaviour>().StartCoroutine(coroutine);
        }

        public void Cancel()
        {
            _cancelled = true;
        }
        public bool IsCancelled()
        {
            return _cancelled;
        }
    }
}