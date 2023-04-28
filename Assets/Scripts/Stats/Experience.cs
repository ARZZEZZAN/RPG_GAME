using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;

namespace RPG.Stats
{

    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] private float _experiencePoints = 0f;

        public event Action onExperienceGained;
        private void Update()
        {
            if (Input.GetKey(KeyCode.O))
            {
                GainExperience(Time.deltaTime * 1000);
            }
        }
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            onExperienceGained();
        }
        public object CaptureState()
        {
            return _experiencePoints;
        }


        public void RestoreState(object state)
        {
           _experiencePoints = (float)state;
        }

        public float GetPoints()
        {
            return _experiencePoints;
        }
    }
}
