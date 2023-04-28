using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.Inventories;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _timeBetweenAttacks;

        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeapon = null;
        [SerializeField] private float _autoAttackRange = 4.5f;
       

        private float _timeLastAttack;

        private WeaponConfig _currentWeaponConfig;
        private Weapon _currentWeapon;

        private Health _target;
        private Equipment _equipment;
        private void Awake()
        {
            _currentWeaponConfig = _defaultWeapon;
            _equipment = GetComponent<Equipment>();
            if (_equipment)
            {
                _equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }
        private void Start()
        {
            if(_currentWeaponConfig == null)
            {
                EquipWeapon(_defaultWeapon);
            }
            _currentWeapon = SetupDefaultWeapon();
           
        }

        private void Update()
        {
            _timeLastAttack += Time.deltaTime;
    
            if (_target == null) return;
            if (_target.IsDead())
            {
                _target = FindNewTargetInRange();
                if (_target == null) return;
            }

            if (!GetIsInRange(_target.transform))
            {
                GetComponent<Mover>().StartMoveAction(_target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
               
            }

        }
        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon = AttachWeapon(weapon);
        }
        private void UpdateWeapon()
        {
            var weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if(weapon == null)
            {
                EquipWeapon(_defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }

        }
        public Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransform, _leftHandTransform, animator);  

        }

        public Health GetTarget()
        {
            
            return _target;

        }

        public Transform GetTransformHand(bool isRightHand)
        {
            if (isRightHand)
            {
                return _rightHandTransform;
            }
            else
            {
                return _leftHandTransform;
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (_timeLastAttack >= _timeBetweenAttacks)
            {
                TriggerAttack();
                _timeLastAttack = 0;
            }
        }
        private Health FindNewTargetInRange()
        {
            Health bestTarget = null;
            float bestTargetDistance = Mathf.Infinity;
            foreach(var target in FindAllTargetsInRange())
            {
                float targetDistance = Vector3.Distance(transform.position, target.transform.position);
                if(targetDistance < bestTargetDistance)
                {
                    bestTarget = target;
                    bestTargetDistance = targetDistance;
                }
            }
            return bestTarget;  
        }

        private IEnumerable<Health> FindAllTargetsInRange()
        {
            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, _autoAttackRange, Vector3.up);

            foreach (var hit in raycastHits)
            {
                Health health = hit.transform.GetComponent<Health>();
                if (health == null) continue;
                if (health.IsDead()) continue;
                if (health.gameObject == gameObject) continue;
                yield return health;
            }
        }
        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        //Animation Event
        private void Hit()
        {
            if(_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            float defence = _target.GetComponent<BaseStats>().GetStat(Stat.Defence);
            damage /= 1 + defence / damage;

            if (_currentWeaponConfig != null)
            {
                _currentWeapon.OnHit();
            }
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target, gameObject, damage);
            }
            else
            {
               
                _target.TakeDamage(gameObject, damage);

            }

        }
        private void Shoot()
        {
            Hit();
        }
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < _currentWeaponConfig.GetRange();

        }
        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform))
            {
                return false;
            }
            Health targetTest = combatTarget.GetComponent<Health>();
            return targetTest != null && !targetTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionSheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }
        public void Cancel()
        {
            StopAttack();
            _target = null;
            GetComponent<Mover>().Cancel(); 
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }

    }
    
}
