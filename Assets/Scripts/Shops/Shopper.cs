using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shop
{
    public class Shopper : MonoBehaviour
    {
        private Shop _activeShop = null;

        public event Action activeShopChanged;
        public void SetActiveShop(Shop shop)
        {
            if(_activeShop != null)
            {
                _activeShop.SetShopper(null);
            }
            _activeShop = shop;
            if (_activeShop != null)
            {
                _activeShop.SetShopper(this);
            }
            if (activeShopChanged != null)
            {
                activeShopChanged();
            }
        }

        public Shop GetActiveShop()
        {
            return _activeShop;
        }
    }
}