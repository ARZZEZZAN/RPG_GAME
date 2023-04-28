using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{

    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine _currentActiveFade = null;
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            
        }
        public void FadeOutImmediate()
        {
            if (_canvasGroup == null) 
            {
                _canvasGroup = GetComponent<CanvasGroup>(); 
            }
            _canvasGroup.alpha = 1.0f;
        }
     
        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);   
            
        } 
        public Coroutine FadeIn(float time)
        {
           return Fade(0f, time);
        }
        public Coroutine Fade(float target, float time)
        {
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }

            _currentActiveFade = StartCoroutine(FadeOutRoutine(target, time));
            return _currentActiveFade;
        }
        private IEnumerator FadeOutRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
                yield return null;
            }
        }
    }
}
