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
        [SerializeField] GameObject _mainMenu, _gameOverMenu, _pauseMenu;
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

        public static void Pause()
        {
            if (Time.timeScale < 0.1f)
            {
                _instance._pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                _instance._pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        public static void ReturnToMainMenu() => _instance.ChangeLevel("Main Menu");

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
                _pauseMenu.SetActive(false);
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