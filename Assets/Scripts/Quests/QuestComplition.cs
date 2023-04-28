using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{

    public class QuestComplition : MonoBehaviour
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private string _objective;

        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.CompleteObjective(_quest, _objective);

        }
    }
}