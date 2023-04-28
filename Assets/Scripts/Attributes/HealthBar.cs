using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health _health = null;
        [SerializeField] private RectTransform _foreground = null;
        [SerializeField] private Canvas _rootCanvas = null;

        private void Update()
        {
            if(Mathf.Approximately(_health.GetFraction(), 0f) || Mathf.Approximately(_health.GetFraction(), 1f))
            {
                _rootCanvas.enabled = false;
                return;
            }

            _rootCanvas.enabled = true;
            _foreground.localScale = new Vector3(_health.GetFraction(), 1f, 1f);
        }
    }
}