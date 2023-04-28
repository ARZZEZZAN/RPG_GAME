using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{

    public class ManaDisplay : MonoBehaviour
    {
        private Mana _mana;

        private void Awake()
        {
            _mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }
        private void Update()
        {

            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", _mana.GetMana(), _mana.GetMaxManaPoints());
        }
    }
}
