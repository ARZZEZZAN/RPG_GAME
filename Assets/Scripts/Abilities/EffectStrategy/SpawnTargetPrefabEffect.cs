using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "Abilities/Effects/Spawn Target Prefab", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] private Transform _prefabToInstance;
        [SerializeField] private float _destroyToDelay = -1;
        public override void StartEffect(AbilityData data, Action finished)
        {

            data.StartCoroutine(Effect(data, finished));

        }
        private IEnumerator Effect(AbilityData data, Action finished)
        {
            Transform instance = Instantiate(_prefabToInstance);
            instance.position = data.GetTargetedPoint();
            if(_destroyToDelay > 0)
            {
                yield return new WaitForSeconds(_destroyToDelay);
                Destroy(instance.gameObject);
            }
            finished();
           
        }
    }
}