using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Delay Composite Effect", menuName = "Abilities/Effects/Delay Composite", order = 0)]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] private float _delay = 0;
        [SerializeField] private EffectStrategy[] _effects;
        [SerializeField] private bool _abortIfCancelled = false;
        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffect(data, finished));
        }

        private IEnumerator DelayedEffect(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(_delay);
            if(_abortIfCancelled && data.IsCancelled()) yield break;
            foreach (var effect in _effects)
            {
                effect.StartEffect(data, finished);
            }
        }
    }
}
