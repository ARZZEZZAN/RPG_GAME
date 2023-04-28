using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using RPG.Attributes;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private Transform _respawnLocation;
        [SerializeField] private float _respawnDelay = 2;
        [SerializeField] private float _fadeTime = 0.2f;
        [SerializeField] private float _playerHealPercentage = 20;
        [SerializeField] private float _enemyHealPercentage = 20;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.onDie.AddListener(Respawn);
        }
        private void Start()
        {
            if (_health.IsDead())
            {
                Respawn();
            }
        }
        private void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }
        private IEnumerator RespawnRoutine()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            yield return new WaitForSeconds(_respawnDelay);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOut(_fadeTime);
            RespawnPlayer();
            ResetEnemies();
            savingWrapper.Save();
            fader.FadeIn(_fadeTime);
        }

        private void ResetEnemies()
        {
            foreach (AIController enemyController in FindObjectsOfType<AIController>())
            {
                Health health = enemyController.GetComponent<Health>();
                if (health && !health.IsDead())
                {
                    enemyController.Reset();
                    health.Heal(health.GetMaxHealthPoints() * _enemyHealPercentage / 100);
                }
            }
        }

        private void RespawnPlayer()
        {
            Vector3 posDelta = _respawnLocation.position - transform.position;
            GetComponent<NavMeshAgent>().Warp(_respawnLocation.position);
            _health.Heal(_health.GetMaxHealthPoints() * _playerHealPercentage / 100);
            ICinemachineCamera activeVirtualCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;
            if(activeVirtualCamera.Follow == transform)
            {
                activeVirtualCamera.OnTargetObjectWarped(transform, posDelta);
            }
        }
    }
}