using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{

    public class QuestUIItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _progress;

        private QuestStatus _status;
        public void Setup(QuestStatus status)
        {
            this._status = status;
            _title.text = status.GetQuest().GetTitle();
            _progress.text = status.GetCompletedObjectives() + "/" + status.GetQuest().GetObjectivesCount();
        }
        public QuestStatus GetQuest()
        {
            return _status;
        }
    }
}
