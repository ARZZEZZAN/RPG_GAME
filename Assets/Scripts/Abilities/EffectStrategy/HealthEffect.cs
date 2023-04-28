using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Health Effect", menuName = "Abilities/Effects/Health", order = 0)]
    public class HealthEffect : EffectStrategy
    {
        [SerializeField] private float _healthChange;
        public override void StartEffect(AbilityData data, Action finished)
        {
            foreach(var target in data.GetTargets())
            {
                var health = target.GetComponent<Health>();
                if (health)
                {
                    if(_healthChange < 0)
                    {
                        health.TakeDamage(data.GetUser(), -_healthChange);
                    }
                    else
                    {
                        health.Heal(_healthChange);
                    }
                    
                }
            }
            finished();
        }
    }
}