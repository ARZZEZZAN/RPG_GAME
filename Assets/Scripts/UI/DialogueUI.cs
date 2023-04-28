using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _AIText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _AIResponse;
        [SerializeField] private Transform _choiceRoot;
        [SerializeField] private GameObject _choicePrefab;
        [SerializeField] private TextMeshProUGUI _conversantName;
        [SerializeField] public Button _quitButton;

        private PlayerConversant _playerConversant;

          
        private void Start()
        {
            _playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            _playerConversant.onConversationUpdated += UpdateUI;
            _nextButton.onClick.AddListener(() =>
            {
                _playerConversant.Next();
            }); 
            _quitButton.onClick.AddListener(() =>
            {
                _playerConversant.Quit();
            });
            UpdateUI();
        }
        private void UpdateUI()
        {
            gameObject.SetActive(_playerConversant.IsActive());
            if (!_playerConversant.IsActive())
            {
                return;
            }
            _conversantName.text = _playerConversant.GetCurrentConversantName();
            _AIResponse.SetActive(!_playerConversant.IsChoosing());
            _choiceRoot.gameObject.SetActive(_playerConversant.IsChoosing());
            if (_playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                _AIText.text = _playerConversant.GetText();
                _nextButton.gameObject.SetActive(_playerConversant.HasNext());
            }
            
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in _choiceRoot)
            {
                Destroy(item.gameObject);   
            }
            foreach (DialogueNode choice in _playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(_choicePrefab, _choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    _playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}