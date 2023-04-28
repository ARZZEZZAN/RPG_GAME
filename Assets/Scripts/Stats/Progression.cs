using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] private ProgressionCharacterClass[] _characterClass = null;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            if (!_lookupTable[characterClass].ContainsKey(stat))
            {
                return 0;
            }


            if (!_lookupTable.ContainsKey(characterClass)) return 0;

            float[] levels = _lookupTable[characterClass][stat];

            if(levels.Length == 0) return 0;

            if (level > levels.Length)
            {
                return levels[levels.Length - 1];
            }
            return levels[level - 1];

        }
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = _lookupTable[characterClass][stat];

            return levels.Length;
        }
        
        private void BuildLookup()
        {
            if (_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacterClass in _characterClass)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionCharacterClass.Stats)
                {
                    statLookupTable[progressionStat.Stat] = progressionStat.Levels;
                }

                _lookupTable[progressionCharacterClass.Characterclass] = statLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass 
        {
            [SerializeField] private CharacterClass _characterClass;
            [SerializeField] private ProgressionStat[] _stats; 

            public CharacterClass Characterclass => _characterClass;
            public ProgressionStat[] Stats => _stats;

        }
        [System.Serializable]
        class ProgressionStat
        {
            [SerializeField] private Stat _stat;
            [SerializeField] private float[] _levels;
            public float[] Levels => _levels;
            public Stat Stat => _stat;
        }

    }

}
