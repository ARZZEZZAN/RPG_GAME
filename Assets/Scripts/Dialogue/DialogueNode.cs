using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{

    public class DialogueNode : ScriptableObject
    {
        [SerializeField] private bool isPlayerSpeaking = false;
        [SerializeField] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(0, 0, 200, 100);
        [SerializeField] private string _onExitAction;
        [SerializeField] private string _onEnterAction;
        [SerializeField] private Condition _condition;

        public Rect GetRect()
        {
            return rect;
        }
        public string GetText()
        {
            return text;
        }
        public void SetText(string text)
        {
            Undo.RecordObject(this, "Update Dialogue Text");
            if (text != this.text)
                this.text = text;
            EditorUtility.SetDirty(this);
        }
        public List<string> GetChildren()
        {
            return children;
        }
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }
        public string GetOnEnterAction()
        {
            return _onEnterAction;
        }
        public string GetOnExitAction()
        {
            return _onExitAction;
        }
        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return _condition.Check(evaluators);
        }
        public void AddChildren(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        public void RemoveChildren(string childID)
        {
            Undo.RecordObject(this, "Unlink Dialogue");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
#if UNITY_EDITOR
        public void SetPos(Vector2 newPos)
        {
            Undo.RecordObject(this, "Move dialogue node");
            rect.position = newPos;
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool IsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            this.isPlayerSpeaking = IsPlayerSpeaking;
        }


#endif
    }
}
