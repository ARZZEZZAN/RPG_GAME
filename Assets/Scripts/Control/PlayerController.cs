using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using System;
using RPG.Attributes;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Inventories;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour
    {   
        
        [System.Serializable]
        struct CursorMapping
        {
            [SerializeField] private CursorType _cursorType;
            internal CursorMode cursorMode;
            [SerializeField] private Texture2D _texture;
            [SerializeField] private Vector2 _hotspot;

            public CursorType CursorType => _cursorType;
            public Texture2D Texture => _texture;
            public Vector2 Hotspot => _hotspot;
        }

        [SerializeField] private CursorMapping[] _cursorMappings = null;
        [SerializeField] private float _maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float _raycastRadius = 1f;
        [SerializeField] private int _numberOfAbilities = 6;

        private RaycastHit _hit;
        private Health _health;
        private bool _isDragging = false;
        private ActionStore actionStore;


        private void Awake()
        {
            _health = GetComponent<Health>();
            actionStore = GetComponent<ActionStore>();
        }

        private void Update()
        {
            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            UseAbilities();
            if (InteractWithComponent())
                return;

            if (InteractWithUI())
                return;


            if (InteractWithMovement())
                return;

            
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _isDragging = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (_isDragging)
            {
                return true;
            }
            return false;
        }

        private void UseAbilities()
        {
            for (int i = 0; i < _numberOfAbilities; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    actionStore.Use(i, gameObject);
                }
            }
            
        }
        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();

                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }

            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }


        private bool InteractWithMovement()
        {

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                   
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            
            return false;
        }
        private bool RaycastNavMesh(out Vector3 target)
        {

            target = new Vector3();

            bool hasHit = Physics.Raycast(GetMouseRay(), out _hit);
            if(!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(_hit.point, out navMeshHit, _maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh) return false;
            target = navMeshHit.position;

           
            return true;
        }


        private void SetCursor(CursorType cursorType)
        {
            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.Texture, cursorMapping.Hotspot, cursorMapping.cursorMode);
        }
        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach (CursorMapping mapping in _cursorMappings)
            {
                if(mapping.CursorType == cursorType)
                {
                    return mapping;
                }
            }
            return _cursorMappings[0];
        }

        public Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
