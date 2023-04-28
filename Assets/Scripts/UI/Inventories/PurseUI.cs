using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _balanceField;

        private Purse _playerPurse = null;

        private void Start()
        {
            _playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            if(_playerPurse != null)
            {
                _playerPurse.onChange += RefreshUI;
            }

            RefreshUI();
        }
        private void RefreshUI()
        {
            _balanceField.text = $"${_playerPurse.GetBalance():N2}";
        }
    }
}