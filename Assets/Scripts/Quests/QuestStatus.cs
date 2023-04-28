using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus 
    { 

        private Quest _quest;
        private List<string> _completedObjectives = new List<string>();

        [System.Serializable]
        private class QuestStatusRecord
        {
            public string _questName;
            public List<string> _completedObjectives;
        }

        public QuestStatus(Quest quest)
        {
            this._quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            _quest = Quest.GetByName(state._questName);
            _completedObjectives = state._completedObjectives;
        }

        public Quest GetQuest()
        {
            return _quest;
        }

        public bool IsComplete()
        {
            foreach (var objective in _quest.GetObjectives())
            {
                if (!_completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetCompletedObjectives()
        {
            return _completedObjectives.Count;
        }
        public bool IsObjectiveComplete(string objective)
        {
            return _completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if (_quest.HasObjective(objective))
            {
                _completedObjectives.Add(objective);
            }
            
        }

        public object CaptureState()
        {
            QuestStatusRecord state = new QuestStatusRecord();
            state._questName = _quest.name;
            state._completedObjectives = _completedObjectives;
            return state;
        }
    }
}