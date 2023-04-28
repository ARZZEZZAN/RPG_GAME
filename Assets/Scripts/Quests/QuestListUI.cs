using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using RPG.UI.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{

    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestUIItem _questPrefab;
        private QuestList questList;
        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestUIItem uiInstance = Instantiate<QuestUIItem>(_questPrefab, transform);
                uiInstance.Setup(status);
            }
        }
    }
}
