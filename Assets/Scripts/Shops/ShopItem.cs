using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Shop
{
    public class ShopItem : MonoBehaviour
    {
      
        private InventoryItem item;
        private int availability;
        private float price;
        private int quantityInTransaction;
        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;   
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }
        public int GetAvailability()
        {
            return availability;
        }
        public float GetPrice()
        {
            return price;
        }
        public int GetQuantity()
        {
            return quantityInTransaction;
        }

        public InventoryItem GetInventoryItem()
        {
            return item;
        }
    }
}