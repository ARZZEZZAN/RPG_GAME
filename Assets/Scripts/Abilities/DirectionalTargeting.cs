using System;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "Abilities/Targeting/Directional", order = 0)]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _groundOffset = 1;
        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            RaycastHit raycastHit;
            Ray ray = playerController.GetMouseRay();
            if (Physics.Raycast(ray, out raycastHit, 1000, _layerMask))
            {
                data.SetTargetedPoint(raycastHit.point + ray.direction * _groundOffset/ray.direction.y);
            }
            finished();
        }
    }
}