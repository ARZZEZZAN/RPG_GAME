using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shop
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] private ItemCategory _category = ItemCategory.None;
        private Button _button;
        private RPG.Shop.Shop _currentShop;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SelectFilter);
        }
        public void SetShop(RPG.Shop.Shop currentShop)
        {
            _currentShop = currentShop;
        }
        public void RefreshUI()
        {
            _button.interactable = _currentShop.GetFiler() != _category;
        }
        private void SelectFilter()
        {
            _currentShop.SelectFilter(_category);
        }
    }
}