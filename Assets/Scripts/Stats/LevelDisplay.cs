using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{

    public class LevelDisplay: MonoBehaviour
    {
        private BaseStats _baseStats = null;

        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }
        private void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", _baseStats.GetLevel().ToString());
        }
    }
}
