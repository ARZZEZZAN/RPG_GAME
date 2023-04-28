using System.Collections;
using System.Collections.Generic;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] private Transform _contentRoot;
        [SerializeField] private GameObject _buttonPref;

        private void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null) return;
            foreach (Transform child in _contentRoot)
            {
                Destroy(child.gameObject);
            }
            foreach(string save in savingWrapper.ListSaves())
            {
                if (save == null) return;
                GameObject buttonInstance = Instantiate(_buttonPref, _contentRoot);
                TMP_Text buttonText = buttonInstance.GetComponentInChildren<TMP_Text>();
                buttonText.text = save;
                Button button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => savingWrapper.LoadGame(save));
            }
            
        }
    }
}