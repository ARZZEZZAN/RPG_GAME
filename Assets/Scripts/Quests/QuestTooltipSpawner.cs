using System.Collections;
using System.Collections.Generic;
using RPG.Core.UI.Tooltips;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{

    public class QuestTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestUIItem>().GetQuest();
            tooltip.GetComponent<QuestTooltipUI>().Setup(status);

        }
    }
}
