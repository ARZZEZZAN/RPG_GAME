using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speedProj = 1f;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private bool _isHoming = true;
        [SerializeField] private float _maxLifeTime = 10f;
        [SerializeField] private GameObject[] _destroyOnHit = null;
        [SerializeField] private float _lifeAfterImpact = 2f;
        [SerializeField] private UnityEvent onHit;

        private GameObject _instigator;

        private Health _target;
        private Vector3 _targetPoint;
        private float _damage;
        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }
        private void Update()
        {
            if(_target != null &&_isHoming && !_target.IsDead())
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * _speedProj * Time.deltaTime);
        }
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }
        public void SetTarget(Vector3 targetPoint, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }
        public void SetTarget(GameObject instigator, float damage, Health target=null, Vector3 targetPoint = default)
        {
            this._target = target;
            this._targetPoint = targetPoint;
            this._damage = damage;
            this._instigator = instigator;

            Destroy(gameObject, _maxLifeTime);
        }
        private Vector3 GetAimLocation()
        {
            if(_target == null)
            {
                return _targetPoint;
            }
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if(targetCapsule == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (_target != null && health != _target) return;
            if (health == null || health.IsDead()) return;
            if (other.gameObject == _instigator) return;
            health.TakeDamage(_instigator, _damage);

            _speedProj = 0f;

            onHit?.Invoke();
            if(_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in _destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }

}
