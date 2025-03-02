using TMPro;
using UnityEngine;

namespace Catparency
{
    public class ScoreCounter : MonoBehaviour
    {
        static ScoreCounter _instance;
        [SerializeField] TextMeshProUGUI _scoreText;
        double _shownValue = 0;
        public static int Score { get { return _score; } set { _score = Mathf.Clamp(value, 0, 500_000_000); }}
        static int _score = 0;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Score = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (_shownValue < Score)
            {
                _shownValue += (double)Score * Time.deltaTime;
            }
            if (_shownValue > Score) _shownValue = Score;

            _scoreText.text = $"Score:\n<mspace=21>{(int)_shownValue}";
        }
    }
}
