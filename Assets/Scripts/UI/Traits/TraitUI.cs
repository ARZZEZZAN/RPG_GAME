using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Traits.UI
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _unassignedPointsText;
        [SerializeField] private Button _commitButton;
        private TraitStore _playerTraitStore = null;

        private void Start()
        {
            _playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            _commitButton.onClick.AddListener(() => _playerTraitStore.Commit());

        }
        private void Update()
        {
            _unassignedPointsText.text = _playerTraitStore.GetUnassignedPoints().ToString();
        }
    }
}