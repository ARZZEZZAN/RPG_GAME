using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Project/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Objective> _objectives = new List<Objective>();
        [SerializeField] private List<Reward> _rewards = new List<Reward>();

        [System.Serializable]
        public class Reward    
        {
            [Min(1)]
            public int number;
            public InventoryItem item;
        }
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
            public bool _usesCondition = false;
            public Condition _completionCondition;
        }

        public string GetTitle()
        {
            return name;
        }
        public int GetObjectivesCount()
        {
            return _objectives.Count;
        }
        public IEnumerable<Objective> GetObjectives()
        {
            return _objectives;
        }
        public IEnumerable<Reward> GetRewards()
        {
            return _rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            foreach (var objective in _objectives)
            {
                if(objective.reference == objectiveRef)
                {
                    return true;
                }
                
            }
            return false;
        }
        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}
