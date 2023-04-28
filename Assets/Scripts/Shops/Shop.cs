using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shop
{

    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] private string _shopName;
        [Range(0, 100)] [SerializeField] private float _sellingPercent = 80f;
        [SerializeField] private float _maximumBarterDiscount = 80f;

        [SerializeField] private StockItemConfig[] _stockConfig; 

        [System.Serializable]
        public class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)] public float buyingDiscountPersent;
            public int _levelToUnlock;
        }
        private Dictionary<InventoryItem, int> _transaction = new Dictionary<InventoryItem, int>();
        private Dictionary<InventoryItem, int> _stockSold = new Dictionary<InventoryItem, int>();
        private Shopper _currentShopper;
        private bool _isBuyingMode = true;
        private ItemCategory _filter = ItemCategory.None;

        public event Action onChange;

        public void SetShopper(Shopper shopper)
        {
            _currentShopper = shopper;
        }
        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if(_filter == ItemCategory.None || item.GetCategory() == _filter)
                {
                    yield return shopItem;
                }
            }
        }
        public IEnumerable<ShopItem> GetAllItems()
        {
            int shopperLevel = GetShopperLevel();

            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();
            foreach (InventoryItem item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) continue;
             
                float price = prices[item];
                int quantityToTransaction = 0;
                _transaction.TryGetValue(item, out quantityToTransaction);
                int availability = availabilities[item];
                yield return new ShopItem(item, availability, price, quantityToTransaction);
            }

        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (_isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        _stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
                
            }
            
            return availabilities;
        }

        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach (var config in GetAvailableConfigs())
            {
                if (_isBuyingMode)
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();
                    }
                    prices[config.item] *= (1 - config.buyingDiscountPersent / 100);
                }
                else
                {
                    prices[config.item] = config.item.GetPrice() * (_sellingPercent / 100);
                }
              
            }
            return prices;
        }

        private float GetBarterDiscount()
        {
            BaseStats baseStats = _currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return (1 - Math.Min(discount, _maximumBarterDiscount)/100);
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (var config in _stockConfig)
            {
                if (config._levelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = _currentShopper.GetComponent<Inventory>();
            int total = 0;
            if (inventory == null) return 0;

            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }
        public void SelectFilter(ItemCategory category)
        {
            _filter = category;
            if(onChange != null)
            {
                onChange();
            }
        }
        public ItemCategory GetFiler()
        {
            return _filter;
        }
        public void SelectMode(bool isBuying)
        {
            _isBuyingMode = isBuying;
            if(onChange != null)
            {
                onChange();
            }
        }
        public bool IsBuyingMode()
        {
            return _isBuyingMode;
        }

        public string GetShopName()
        {
            return _shopName;
        }
        public bool CanTransact()
        {
            if (IsTransactEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }
        public void ConfirmTransaction()
        {
            Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = _currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantity();
                float price = shopItem.GetPrice();
                for (int i = 0; i < quantity; i++)
                {
                    if (IsBuyingMode())
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }

                    
                }
            }
            if(onChange != null)
            {
                onChange();
            }
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;
            AddToTransaction(item, -1);

            shopperInventory.RemoveFromSlot(slot, 1);
            if (!_stockSold.ContainsKey(item))
            {
                _stockSold[item] = 0;
            }
            _stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }


        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) return;
            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if (!_stockSold.ContainsKey(item))
                {
                    _stockSold[item] = 0;
                }
                _stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }
        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if(shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }
        public float TransactionTotal()
        {
            float total = 0;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantity();
            }
            return total;
        }
        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!_transaction.ContainsKey(item))
            {
                _transaction[item] = 0;
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if(_transaction[item] + quantity > availability)
            {
                _transaction[item] = availability;
            }
            else
            {
                _transaction[item] += quantity;

            }
            if (_transaction[item] <= 0)
            {
                _transaction.Remove(item);
            }
            if(onChange != null)
            {
                onChange();
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }
            return true;
        }
        public bool HasSufficientFunds()
        {
            if (!IsBuyingMode()) return true;
            Purse purse = _currentShopper.GetComponent<Purse>();
            if(purse == null) return false;
            return purse.GetBalance() >= TransactionTotal();
        }

        public bool IsTransactEmpty()
        {
            return _transaction.Count == 0;
        }
        public bool HasInventorySpace()
        {
            if(!IsBuyingMode()) return true;
            Inventory shopperInventory = _currentShopper.GetComponent<Inventory>();
            if(shopperInventory == null) return false;

            List<InventoryItem> flatItems = new List<InventoryItem>();

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantity();
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }
            return shopperInventory.HasSpaceForShop(flatItems);
        }
        private int GetShopperLevel()
        {
            BaseStats stats = _currentShopper.GetComponent<BaseStats>();
            if(stats == null) return 0;

            return stats.GetLevel();
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();
            foreach (var pair in _stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }
            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            _stockSold.Clear();
            foreach (var pair in saveObject)
            {
                _stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }
}