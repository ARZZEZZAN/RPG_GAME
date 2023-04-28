using System;
using UnityEngine;
using RPG.Attributes;
using RPG.Inventories;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem
    {
        [SerializeField] private AnimatorOverrideController _overrideController = null;
        [SerializeField] private Weapon _weapon = null;
        [SerializeField] private float _weaponDamage = 0;  
        [SerializeField] private float _percentageBonus = 0;
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private bool _isRightHanded = true;
        [SerializeField] private Projectile _projectile = null;

        const string _weaponName = "Weapon";

        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestoyOldWeapon(rightHandTransform, leftHandTransform);

            Weapon weapon = null;


            if (_weapon != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);
                weapon = Instantiate(_weapon, handTransform);
                weapon.gameObject.name = _weaponName;
            }
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_overrideController != null)
            {
                animator.runtimeAnimatorController = _overrideController;
            }
            else if (overrideController != null)
            { 
                
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

        private void DestoyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(_weaponName);
            if(oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(_weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handTransform;
            if (_isRightHanded) handTransform = rightHandTransform;
            else handTransform = leftHandTransform;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }
        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectile = Instantiate(_projectile, GetTransform(rightHandTransform, leftHandTransform).position, Quaternion.identity);
            projectile.SetTarget(target, instigator, calculatedDamage);
        }
        public float GetDamage()
        {
            return _weaponDamage;
        } 
        public float GetPercentageBonus()
        {
            return _percentageBonus;
        }
        public float GetRange()
        {
            return _weaponRange;
        }
    }
}