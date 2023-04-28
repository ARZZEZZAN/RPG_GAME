using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Mana : MonoBehaviour, ISaveable
    {

        private float _mana;

        private void Awake()
        {
            if (_mana < 0f)
            {
                _mana = GetMaxManaPoints();

            }
        }

        private void Update()
        {
            if(_mana < GetMaxManaPoints())
            {
                _mana += GetManaRegenRate() * Time.deltaTime;
                if(_mana > GetMaxManaPoints())
                {
                    _mana = GetManaRegenRate();
                }
            }
        }
        public float GetMaxManaPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Mana);
        
        }
        public float GetManaRegenRate()
        {
            return GetComponent<BaseStats>().GetStat(Stat.ManaRegenRate);
        }

        public float GetMana()
        {
            return _mana;
        }
        public bool UseMana(float manaToUse)
        {
            if (manaToUse > _mana) return false;
            _mana -= manaToUse;
            return true;
        }

        public object CaptureState()
        {
            return _mana;
        }

        public void RestoreState(object state)
        {
            _mana = (float)state;
        }
    }
}