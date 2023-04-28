using System.Collections;
using System.Collections.Generic;
using RPG.SceneManagement;
using UnityEngine;
using TMPro;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private SavingWrapper _savingWrapper;
        [SerializeField] private TMP_InputField _newGameInputField;
        private void Start()
        {
            _savingWrapper = FindObjectOfType<SavingWrapper>();
        }
        
        public void ContinueGame()
        {
            _savingWrapper.ContinueGame();
        }
        public void NewGame()
        {
            _savingWrapper.NewGame(_newGameInputField.text);
        }
    }
}