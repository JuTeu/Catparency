using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Catparency
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] GameObject[] _essentials;
        [SerializeField] Image _fader;
        [SerializeField] GameObject _mainMenu, _gameOverMenu;
        static LevelManager _instance;
        public static bool AlreadyLoaded { get; private set; }
        bool _levelChangeInProgress = false;
        string _currentLevelName = "Main Menu";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            #if UNITY_EDITOR
                if (SceneManager.sceneCount > 1)
                {
                    _mainMenu.SetActive(false);
                }
            #endif
            if (AlreadyLoaded)
            {
                foreach (var essential in _essentials)
                {
                    Destroy(essential);
                }
                return;
            }
            AlreadyLoaded = true;
            _instance = this;
            foreach (var essential in _essentials)
            {
                DontDestroyOnLoad(essential);
            }
        }

        public void RetryLevel()
        {
            ChangeLevel(_currentLevelName);
        }

        public static void GameOver()
        {
            _instance._gameOverMenu.SetActive(true);
        }

        public void ChangeLevel(string levelName)
        {
            if (_levelChangeInProgress) return;
            _levelChangeInProgress = true;
            StartCoroutine(IEChangeLevel());
            IEnumerator IEChangeLevel()
            {
                float progress = 0f;
                while (progress < 1f)
                {
                    progress += Time.deltaTime * 3f;
                    _fader.color = new Color(0, 0, 0, progress);
                    yield return null;
                }
                _fader.color = new Color(0, 0, 0, 1);
                SceneManager.LoadScene(levelName);
                _currentLevelName = levelName;
                _mainMenu.SetActive(levelName == "Main Menu");
                _gameOverMenu.SetActive(false);
                progress = 0f;
                while (progress < 1f)
                {
                    progress += Time.deltaTime * 3f;
                    _fader.color = new Color(0, 0, 0, 1f - progress);
                    yield return null;
                }
                _fader.color = new Color(0, 0, 0, 0);
                _levelChangeInProgress = false;
            }
        }

        public void QuitGamePressed()
        {
            Application.Quit();
        }
    }
}