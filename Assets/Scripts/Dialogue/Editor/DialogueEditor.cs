using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{

    public class DialogueEditor : EditorWindow
    {
        private Dialogue _selectedDialogue = null;
        private GUIStyle _nodeStyle;
        private GUIStyle _playerNodeStyle;
        private DialogueNode _draggingNode = null;  
        private Vector2 _draggingOffset;
        private DialogueNode _creatingNode = null; 
        private DialogueNode _deletingNode = null;
        private DialogueNode _linkingParentNode = null;
        private Vector2 _scrollPos;
        private bool _draggingCanvas = false;
        private Vector2 _draggingCanvasOffset;

        private const float _canvasSize = 4000;
        private const float _BGSize = 50;

       [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
           GetWindow(typeof(DialogueEditor), false, "DialogueEditor");
        }
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line) 
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            return false;
        }
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _nodeStyle.border = new RectOffset(12, 12, 12, 12);


            _playerNodeStyle = new GUIStyle();
            _playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            _playerNodeStyle.normal.textColor = Color.white;
            _playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            _playerNodeStyle.border = new RectOffset(12, 12, 12, 12); 
        }
        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }
        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                _selectedDialogue = newDialogue;
                Repaint();

            }
        }
        private void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
            else
            {
                ProcessEvents();

                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                Rect canvas = GUILayoutUtility.GetRect(_canvasSize, _canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, _canvasSize / _BGSize, _canvasSize / _BGSize); 
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }
                EditorGUILayout.EndScrollView();
                if(_creatingNode != null)
                {
                    _selectedDialogue.CreateNode(_creatingNode);
                    _creatingNode = null;
                }
                if(_deletingNode != null)
                {
                    _selectedDialogue.DeleteNode(_deletingNode);
                    _deletingNode = null;
                }

            }
        }


        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && _draggingNode == null)
            {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPos);
                if(_draggingNode != null)
                {
                    _draggingOffset = _draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                }
                else
                {
                    _draggingCanvas = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPos;
                    Selection.activeObject = _selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingNode != null)
            {
                _draggingNode.SetPos(Event.current.mousePosition + _draggingOffset);
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _scrollPos = _draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && _draggingNode != null)
            {
                _draggingNode = null;
           
            }
            else if (Event.current.type == EventType.MouseDrag && _draggingCanvas)
            {
                _draggingCanvas = false;
            }
        }

         
        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = _nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = _playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);

            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                _deletingNode = node;
            }
            DrawLinkButton(node);

            if (GUILayout.Button("+"))
            {
                _creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButton(DialogueNode node)
        {
            if (_linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    _linkingParentNode = node;
                }
            }
            else if(_linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    _linkingParentNode = null;
                }
            }
            else if (_linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    _linkingParentNode.RemoveChildren(node.name);
                    _linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    _linkingParentNode.AddChildren(node.name);
                    _linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (DialogueNode childNode in _selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPos = childNode.GetRect().center;
                Vector3 controlPointOffset = new Vector2(100, 0);
                Handles.DrawBezier(startPos, endPos, startPos + controlPointOffset, endPos - controlPointOffset, Color.white, null, 4f);
            }
        }
        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;  
            foreach (DialogueNode node in _selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}
