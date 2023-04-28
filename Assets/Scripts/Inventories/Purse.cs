using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        [SerializeField] private float _startingBalance = 400f;

        private float _balance = 0f;

        public event Action onChange;
        private void Awake()
        {
            _balance = _startingBalance;
        }
        public float GetBalance()
        {
            return _balance;
        }
        public void UpdateBalance(float amount)
        {
            _balance += amount;
            if(onChange != null)
            {
                onChange();
            }
        }

        public object CaptureState()
        {
            return _balance;
        }

        public void RestoreState(object state)
        {
            _balance = (float)state;
        }

        public int AddItems(InventoryItem item, int number)
        {
            if(item is CurrencyItem)
            {
                UpdateBalance(item.GetPrice() * number);
                return number;
            }
            return 0;
        }
    }
}