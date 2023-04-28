using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Shop;
using RPG.UI.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shop
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _shopName;
        [SerializeField] private Transform _listRoot;
        [SerializeField] private RowUI _rowPrefab;
        [SerializeField] private TextMeshProUGUI _totalField;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _switchButton;

        private Shopper _shopper = null;
        private RPG.Shop.Shop _currentShop = null;
        private Color _originalTotalFieldColor;
        private void Start()
        {
            _originalTotalFieldColor = _totalField.color;
            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (_shopper == null) return;

            _shopper.activeShopChanged += ShopChanged;
            _confirmButton.onClick.AddListener(ConfirmTransaction);
            _switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        private void ShopChanged()
        {
            if(_currentShop != null)
            {
                _currentShop.onChange -= RefreshUI;
            }
            _currentShop = _shopper.GetActiveShop();
            gameObject.SetActive(_currentShop != null);

            foreach (FilterButtonUI filterButton in GetComponentsInChildren<FilterButtonUI>())
            {
                filterButton.SetShop(_currentShop);
            }

            if (_currentShop == null) return;
            _shopName.text = _currentShop.GetShopName();


            _currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in _listRoot)
            {
                Destroy(child.gameObject);
            }
            foreach (ShopItem item in _currentShop.GetFilteredItems())
            {
                RowUI row = Instantiate<RowUI>(_rowPrefab, _listRoot);
                row.Setup(_currentShop, item);
            }
            _totalField.text = $"Total: ${_currentShop.TransactionTotal():N2}";
            if (!_currentShop.HasSufficientFunds())
            {
                _totalField.color = Color.red;
            }
            else
            {
                _totalField.color = _originalTotalFieldColor;
            }
            _confirmButton.interactable = _currentShop.CanTransact();

            TextMeshProUGUI switchButtonText = _switchButton.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI _confirmButtonText = _confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (_currentShop.IsBuyingMode())
            {
                switchButtonText.text = "Switch To Selling";
                _confirmButtonText.text = "Buy";

            }
            else
            {
                switchButtonText.text = "Switch To Buying";
                _confirmButtonText.text = "Sell";
            }

            foreach (FilterButtonUI filterButton in GetComponentsInChildren<FilterButtonUI>())
            {
                filterButton.RefreshUI();
            }
        }

        public void Close()
        {
            _shopper.SetActiveShop(null);
        }
        public void ConfirmTransaction()
        {
            _currentShop.ConfirmTransaction();
        }
        public void SwitchMode()
        {
            _currentShop.SelectMode(!_currentShop.IsBuyingMode());
        }
    }
}