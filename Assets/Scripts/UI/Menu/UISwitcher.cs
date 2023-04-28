using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class UISwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject _entryPoint;
        private void Start()
        {
            ToSwitch(_entryPoint);
        }
        public void ToSwitch(GameObject ToDisplay)
        {
            if (ToDisplay.transform.parent != transform) return;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(child.gameObject == ToDisplay);
            }
        }
        
    }
}