using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Delayed Click Targeting", menuName = "Abilities/Targeting/Delayed Click", order = 0)]
    public class DelayedClickTargeting : TargetingStrategy
    {
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 cursorHotspot;
        [SerializeField] private float areaAffectRadius;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private Transform _targetingPrefab;

        private Transform _targetingPrefabInstance = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController playerController = data.GetUser().GetComponent<PlayerController>();
            playerController.StartCoroutine(Targeting(data, playerController, finished));

        }
        private IEnumerator Targeting(AbilityData data, PlayerController playerController, Action finished)
        {
            playerController.enabled = false;
            if (_targetingPrefabInstance == null)
            {
                _targetingPrefabInstance = Instantiate(_targetingPrefab);
            }
            else
            {
                _targetingPrefabInstance.gameObject.SetActive(true);
            }
            _targetingPrefabInstance.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);
            while (!data.IsCancelled())
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
                RaycastHit raycastHit;

                if (Physics.Raycast(playerController.GetMouseRay(), out raycastHit, 1000, _layerMask))
                {
                    _targetingPrefabInstance.position = raycastHit.point;
                    if (Input.GetMouseButtonUp(0))
                    {
                        data.SetTargetedPoint(raycastHit.point);
                        data.SetTargets(GetGameObjectsInRadius(playerController, raycastHit.point));
                        break;
                    }
                }
                yield return null;
            }
            _targetingPrefabInstance.gameObject.SetActive(false);
            playerController.enabled = true;
            finished();
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(PlayerController playerController, Vector3 point)
        {

            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);
            foreach (var hit in hits)
            {
                yield return hit.collider.gameObject;
            }

        }
    }
}


