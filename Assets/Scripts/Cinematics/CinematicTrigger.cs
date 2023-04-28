using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{

    public class CinematicTrigger : MonoBehaviour
    {
        private bool _isTriggered;
        private void Awake()
        {
            _isTriggered = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (_isTriggered) return;
            if(other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                _isTriggered = true;

            }
        }
    }
}
