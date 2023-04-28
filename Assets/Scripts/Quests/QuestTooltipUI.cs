using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using RPG.Quests;
using System;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Transform _objectiveContainer;
        [SerializeField] private GameObject _objectivePrefab;
        [SerializeField] private GameObject _objectiveIncompletePrefab;
        [SerializeField] private TextMeshProUGUI _rewardText;
        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            _title.text = status.GetQuest().GetTitle();
            foreach (Transform item in _objectiveContainer)
            {
                Destroy(item.gameObject);
            }
            foreach (var objective in status.GetQuest().GetObjectives())
            {
                GameObject prefab = _objectiveIncompletePrefab;
                if(status.IsObjectiveComplete(objective.reference))
                {
                    prefab = _objectivePrefab;
                }
                else
                {
                    prefab = _objectiveIncompletePrefab;
                }
                GameObject objectiveInstance = Instantiate(prefab, _objectiveContainer);
                TextMeshProUGUI objectiveText = objectiveInstance.GetComponentInChildren<TextMeshProUGUI>();
                objectiveText.text = objective.description;
            }
            _rewardText.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            string rewardText = "";
            foreach(var reward in quest.GetRewards())
            {
                if(rewardText != "")
                {
                    rewardText += ", ";
                }
                if(reward.number > 1)
                {
                    rewardText += reward.number + " ";
                }
                rewardText += reward.item.GetDisplayName();
            }
            if(rewardText == "")
            {
                rewardText = "No reward";
            }
            rewardText = ".";
            return rewardText;  
        }
    }
}
