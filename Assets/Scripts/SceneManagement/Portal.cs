using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Control;

namespace RPG.SceneManagement
{

    public class Portal : MonoBehaviour
    {
        public enum DestinationIdentifier 
        {

            A, B, C, D, E
        }


        [SerializeField] private int _sceneToLoad;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private DestinationIdentifier _destinationIdentifier;
        [SerializeField] private float _fadeOutTime = 1f;  
        [SerializeField] private float _fadeInTime = 2f;
        [SerializeField] private float _fadeWaitTime = 0.5f;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }
        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;   

            yield return fader.FadeOut(_fadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            savingWrapper.Load();   

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);


            savingWrapper.Save();


            yield return new WaitForSeconds(_fadeWaitTime);
            fader.FadeIn(_fadeInTime);

            newPlayerController.enabled = true;
            Destroy(gameObject);

        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.GetComponent<NavMeshAgent>().Warp(otherPortal._spawnPoint.position);
            player.transform.rotation = otherPortal._spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;

        }

        private Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal._destinationIdentifier != _destinationIdentifier) continue;

                return portal;
            }
            return null;
        }
    }
}
