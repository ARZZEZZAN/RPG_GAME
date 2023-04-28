using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _regenerationPercen = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        private bool _wasDeadLastFrame = false;

        public UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent: UnityEvent<float>
        {

        }

        private float _health = -1f;

        private void Awake()
        {
            if(_health < 0f)
            {
                _health = GetMaxHealthPoints();

            }
        }
        public bool IsDead()
        {
            return _health <= 0;
        }
        private void OnEnable()
        {
            GetComponent<BaseStats>().OnLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().OnLevelUp -= RegenerateHealth;
        }


        public void TakeDamage(GameObject instigator, float damage)
        {
            _health = Mathf.Max(_health - damage, 0);
            if(IsDead())
            {
                onDie?.Invoke();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage?.Invoke(damage);
            }
            UpdateState();
        }

        public void Heal(float healthToRestore)
        {
            _health = Mathf.Min(_health + healthToRestore, GetMaxHealthPoints());
            UpdateState();
        }

        public float GetHealthPoints()
        {
            return _health;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        public float GetPercentage()
        {
            return GetFraction() * 100;
        }
        public float GetFraction()
        {
            return _health / GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        private void UpdateState()
        {
            Animator animator = GetComponent<Animator>();
            if (!_wasDeadLastFrame && IsDead())
            {
                animator.SetTrigger("Die");
                GetComponent<ActionSheduler>().CancelCurrentAction();
            }

            if (_wasDeadLastFrame && !IsDead())
            {
                animator.Rebind();
            }

            _wasDeadLastFrame = IsDead();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (_regenerationPercen / 100);
            _health = Mathf.Max(_health, regenHealthPoints);
        }


        public object CaptureState()
        {
            return _health;
        }

        public void RestoreState(object state)
        {
            _health = (float) state;
            UpdateState();
        }
    }

}

