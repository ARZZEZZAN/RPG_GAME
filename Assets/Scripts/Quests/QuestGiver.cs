using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{

    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest _quest;
        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.AddQuest(_quest);
        }
    }
}