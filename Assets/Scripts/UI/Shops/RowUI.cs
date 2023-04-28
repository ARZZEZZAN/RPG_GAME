using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{

    public class RowUI : MonoBehaviour
    {
        [SerializeField] private Image _iconField;
        [SerializeField] private TextMeshProUGUI _nameField;
        [SerializeField] private TextMeshProUGUI _availabilityField;
        [SerializeField] private TextMeshProUGUI _priceField;
        [SerializeField] private TextMeshProUGUI _quantityField;

        private RPG.Shop.Shop _currentShop = null;
        private ShopItem _item = null;
        public void Setup(RPG.Shop.Shop _currentShop, ShopItem item)
        {
            this._currentShop = _currentShop;
            this._item = item;  
            _iconField.sprite = item.GetIcon();
            _nameField.text = item.GetName();
            _availabilityField.text = $"{item.GetAvailability()}";
            _priceField.text = $"${item.GetPrice():N2}";
            _quantityField.text = $"{item.GetQuantity()}";
        }
        public void Add()
        {
            _currentShop.AddToTransaction(_item.GetInventoryItem(), 1);
        }
        public void Remove()
        {
            _currentShop.AddToTransaction(_item.GetInventoryItem(), -1);

        }
    }
}
