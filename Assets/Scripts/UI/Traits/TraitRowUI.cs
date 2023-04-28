using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Traits.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] private Trait _trait;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Button _minusButton;
        [SerializeField] private Button _plusButton;

        private TraitStore _playerTraitStore = null;

        private void Start()
        {
            _playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();

            _minusButton.onClick.AddListener(() => Allocate(-1));
            _plusButton.onClick.AddListener(() => Allocate(+1));
        }
        private void Update()
        {
            _minusButton.interactable = _playerTraitStore.CanAssignPoints(_trait, -1);
            _plusButton.interactable = _playerTraitStore.CanAssignPoints(_trait, +1);

            _valueText.text = _playerTraitStore.GetProposedPoints(_trait).ToString();
        }
        public void Allocate(int points)
        {
            _playerTraitStore.AssignPoints(_trait, points);
        }
    }
}