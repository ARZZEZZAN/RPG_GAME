using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{

    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] _fighters;
        [SerializeField] private bool _activateOnStart = false; 

        private void Start()
        {
            Activate(_activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in _fighters)
            {
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if(target != null)
                {
                    target.enabled = shouldActivate;
                }
                fighter.enabled = shouldActivate;
            }
        }
    }
}