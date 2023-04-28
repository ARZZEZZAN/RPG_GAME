using RPG.Core;
using UnityEngine;

namespace RPG.Inventories
{
   
    [CreateAssetMenu(menuName = ("GameDevTV/GameDevTV.UI.InventorySystem/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        // CONFIG DATA
        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] private Condition _equipCondition;

        // PUBLIC
        public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
        {
            if(equipLocation != allowedEquipLocation) return false;

            return _equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
        }
        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }
    }
}