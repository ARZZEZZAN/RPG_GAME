using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable, IPredicateEvaluator
    {
        [SerializeField] private TraitBonus[] _bonusConfig;

        [System.Serializable]
        public class TraitBonus 
        {
            public Trait trait;
            public Stat stat;
            public float additiveBonusPerPoint = 0;
            public float percentageBonusPerPoint = 0;
        }


        Dictionary<Trait, int> _assignedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> _stagedPoints = new Dictionary<Trait, int>();

        Dictionary<Stat, Dictionary<Trait, float>> _additiveBonusCache;
        Dictionary<Stat, Dictionary<Trait, float>> _percentageBonusCache;

        private void Awake()
        {
            _additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            _percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            foreach (var traitBonus in _bonusConfig)
            {
                if (!_additiveBonusCache.ContainsKey(traitBonus.stat))
                {
                    _additiveBonusCache[traitBonus.stat] = new Dictionary<Trait,float>();
                }
                if (!_percentageBonusCache.ContainsKey(traitBonus.stat))
                {
                    _percentageBonusCache[traitBonus.stat] = new Dictionary<Trait, float>();
                }
                _additiveBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.additiveBonusPerPoint;
                _percentageBonusCache[traitBonus.stat][traitBonus.trait] = traitBonus.percentageBonusPerPoint;
            }
        }
        public int GetProposedPoints(Trait trait)
        {
            return GetPoints(trait) + GetStagedPoints(trait);
        }
        public int GetPoints(Trait trait)
        {
            return _assignedPoints.ContainsKey(trait) ? _assignedPoints[trait] : 0;

        }
        public int GetStagedPoints(Trait trait)
        {
            return _stagedPoints.ContainsKey(trait) ? _stagedPoints[trait] : 0;

        }
        public void AssignPoints(Trait trait, int points)
        {
            if (!CanAssignPoints(trait, points)) return;
           _stagedPoints[trait] = GetStagedPoints(trait) + points;
        }
        public bool CanAssignPoints(Trait trait, int points)
        {
            if(GetStagedPoints(trait) + points < 0) return false;
            if(GetUnassignedPoints() < points) return false;
            return true;
        }
        public int GetUnassignedPoints()
        {
            return GetAssignablePoints() - GetTotalPreposedPoints();
        }

        public int GetTotalPreposedPoints()
        {
            int total = 0;
            foreach(int points in _assignedPoints.Values)
            {
                total += points;
            }
            foreach (int points in _stagedPoints.Values)
            {
                total += points;
            }
            return total;
        }

        public void Commit()
        {
            foreach (Trait trait in _stagedPoints.Keys)
            {
                _assignedPoints[trait] = GetProposedPoints(trait);
            }
            _stagedPoints.Clear();
        }
        public int GetAssignablePoints()
        {
            return (int)GetComponent<BaseStats>().GetStat(Stat.TotalTraitPoints);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (!_additiveBonusCache.ContainsKey(stat)) yield break;
            foreach (Trait trait in _additiveBonusCache[stat].Keys)
            {
                float bonus = _additiveBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (!_percentageBonusCache.ContainsKey(stat)) yield break;
            foreach (Trait trait in _percentageBonusCache[stat].Keys)
            {
                float bonus = _percentageBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public object CaptureState()
        {
            return _assignedPoints;
        }

        public void RestoreState(object state)
        {
            _assignedPoints = new Dictionary<Trait, int>((IDictionary<Trait, int>)state);
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "MinimumTrait")
            {
                if(Enum.TryParse<Trait>(parameters[0], out Trait trait))
                {
                    return GetPoints(trait) >= Int32.Parse(parameters[1]);
                }
            }
            return null;
        }
    }
}