using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Abilities
{
    public class CooldownStore : MonoBehaviour
    {
        Dictionary<InventoryItem, float> _cooldowns = new Dictionary<InventoryItem, float>();
        Dictionary<InventoryItem, float> _initialCooldowns = new Dictionary<InventoryItem, float>();
        private void Update()
        {
            var keys = new List<InventoryItem>(_cooldowns.Keys);
            foreach (InventoryItem ability in keys)
            {
                _cooldowns[ability] -= Time.deltaTime;
                if(_cooldowns[ability] < 0)
                {
                    _cooldowns.Remove(ability);
                    _initialCooldowns.Remove(ability);
                }
            }
        }
        public void StartCooldown(InventoryItem ability, float cooldownTime)
        {
            _cooldowns[ability] = cooldownTime;
            _initialCooldowns[ability] = cooldownTime;

        }
        public float GetCooldownTimeRemaining(InventoryItem ability)
        {
            if (!_cooldowns.ContainsKey(ability))
            {
                return 0;
            }
            return _cooldowns[ability];
        }

        public float GetFractionRemain(InventoryItem ability)
        {
            if (ability == null)
            {
                return 0;
            }

            if (!_cooldowns.ContainsKey(ability))
            {
                return 0;
            }

            return _cooldowns[ability] / _initialCooldowns[ability];

        }
    }
}
