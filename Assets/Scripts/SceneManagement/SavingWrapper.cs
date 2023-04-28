using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        private const string _currentSaveKey = "currentSaveName";
        [SerializeField] private float _fadeInTime = 0.3f;
        [SerializeField] private float _fadeOutTime = 0.3f;
        [SerializeField] private int _firstFieldBuildIndex = 1;
        [SerializeField] private int _menuBuildIndex = 0;

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(_currentSaveKey)) return;
            if (!GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave())) return;
            StartCoroutine(LoadLastScene());
        }
        public void NewGame(string saveFile)
        {
            if (!String.IsNullOrEmpty(saveFile))
            {
                SetCurrentSaveFile(saveFile);
                StartCoroutine(LoadFirstScene());
            }
        }
        public void LoadGame(string saveFile)
        {
            SetCurrentSaveFile(saveFile);
            StartCoroutine(LoadLastScene());
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuScene());
        }

        private void SetCurrentSaveFile(string saveFile)
        {
            PlayerPrefs.SetString(_currentSaveKey, saveFile);
        }
        private string GetCurrentSave()
        {
            return PlayerPrefs.GetString(_currentSaveKey);
        }

        private IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeOutTime);
            yield return SceneManager.LoadSceneAsync(_firstFieldBuildIndex);
            yield return fader.FadeIn(_fadeInTime);
        }
        private IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeOutTime);
            yield return SceneManager.LoadSceneAsync(_menuBuildIndex);
            yield return fader.FadeIn(_fadeInTime);
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(_fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            yield return fader.FadeIn(_fadeInTime);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Save();

            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(LoadLastScene());

            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }
        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }
        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
    }
}
