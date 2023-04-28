using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "My Ability", menuName = "Abilities/Ability", order = 0)]
    public class Ability : ActionItem
    {
        [SerializeField] private TargetingStrategy _targetingStrategy;
        [SerializeField] private FilterStrategy[] _filterStrategies;
        [SerializeField] private EffectStrategy[] _effectStrategies;
        [SerializeField] private float _cooldownTime;
        [SerializeField] private float _manaCost;

        public override void Use(GameObject user)
        {
            Mana mana = user.GetComponent<Mana>();
            if (mana.GetMana() < _manaCost) return;


            CooldownStore cooldownStore = user.GetComponent<CooldownStore>();
            if(cooldownStore.GetCooldownTimeRemaining(this) > 0)
            {
                return;
            }

            AbilityData data = new AbilityData(user);

            ActionSheduler actionSheduler = user.GetComponent<ActionSheduler>();
            actionSheduler.StartAction(data);

            _targetingStrategy.StartTargeting(data, () => 
            {
                TargetAquired(data);
            });
        }
        private void TargetAquired(AbilityData data)
        {
            if (data.IsCancelled()) return;

            Mana mana = data.GetUser().GetComponent<Mana>();
            if(!mana.UseMana(_manaCost)) return;

            CooldownStore cooldownStore = data.GetUser().GetComponent<CooldownStore>();
            cooldownStore.StartCooldown(this, _cooldownTime);


            foreach (var filterStrategy in _filterStrategies)
            {
                data.SetTargets(filterStrategy.Filter(data.GetTargets()));
            }
            foreach (var effect in _effectStrategies)
            {
                effect.StartEffect(data, EffectFinished);
            }
           
        }
        private void EffectFinished()
        {

        }
    }
}