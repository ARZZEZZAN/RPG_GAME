using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance;
        [SerializeField] private float _suspicionTime;
        [SerializeField] private float _aggroCooldownTime;
        [SerializeField] private PatrolPath _path;
        [SerializeField] private float _waypointTolerance;
        [SerializeField] private float _moveTimeTolerance;
        [SerializeField] private float _patrolSpeedFraction;
        [SerializeField] private float _shoutDistance = 6f;
        private bool _hasBeenAggroedRecently = false;

        private GameObject _player;
        private Fighter _fighter;
        private Health _health;
        private Mover _mover;

        private Vector3 _quardLocation;

        private float _timeSinceLastSawPlayer;
        private float _timeAtWaypoint;
        private float _timeSinceAggravated;

        private int _currentWaypointIndex;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _quardLocation = transform.position;

        }

        public void Reset()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.Warp(_quardLocation);
            _timeSinceLastSawPlayer = Mathf.Infinity;
            _timeAtWaypoint = Mathf.Infinity;
            _timeSinceAggravated = Mathf.Infinity;
            _currentWaypointIndex = 0;

        }

        private void Update()
        {
            if (_health.IsDead()) return;
            if (IsAggravated() && _fighter.CanAttack(_player))
            {
               
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }
        public void AggroAllies()
        {
            if (_hasBeenAggroedRecently == true) { return;}

            if (_hasBeenAggroedRecently == false)
            {
                _timeSinceAggravated = 0f;
                _timeSinceLastSawPlayer = 0f;
                _hasBeenAggroedRecently = true;
            }
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeAtWaypoint += Time.deltaTime;
            _timeSinceAggravated += Time.deltaTime;
            if (_timeSinceAggravated >= _aggroCooldownTime && _timeSinceLastSawPlayer >= _suspicionTime)
            {
                _hasBeenAggroedRecently = false;
            }

        }

        private void PatrolBehaviour()
        {
            Vector3 nextPos = _quardLocation;
            if (_path != null)
            {
                if (AtWaypoint())
                {
                    _timeAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPos = GetCurrentWaypoint();
            }

            if(_timeAtWaypoint > _moveTimeTolerance)
            {
                _mover.StartMoveAction(nextPos, _patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _path.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _path.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
          
            GetComponent<ActionSheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if(ai == null) continue;
               
                ai.AggroAllies();
            
            }
        }

        private bool IsAggravated()
        {
           float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
           return distanceToPlayer < _chaseDistance || _timeSinceAggravated < _aggroCooldownTime;    
        }
        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}
