using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

namespace Catparency
{
    public class PlayerDefeat : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] TextMeshPro _faceText;
        [SerializeField] TextMeshProUGUI _continuesText;
        [SerializeField] Image _continuesGradient;
        Vector3 _animationPos;
        public bool _isGameOver = false;
        bool _collided = false;
        void Awake()
        {
            _animationPos = transform.position;
        }

        void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _collided && !_isGameOver ? _animationPos + Vector3.down * 8f : _animationPos, Time.deltaTime * 12f);
        }

        void OnEnable()
        {
            _collided = false;
            _animator.SetTrigger("IsDefeat");
            StartCoroutine(ChangeFace());
        }

        IEnumerator ChangeFace() 
        {
            _faceText.text = ">w<";
            yield return new WaitForSeconds(1.8f);
            _faceText.text = "-.-\"";
            float progress = 0f;
            if (!_isGameOver)
                while (progress < 1f)
                {
                    yield return null;
                    progress += Time.deltaTime;
                    float alpha = Mathf.Min(progress, 1f);
                    Color continuesColor = _continuesText.color;
                    continuesColor.a = alpha;
                    _continuesText.color = continuesColor;
                    Color gradientColor = _continuesGradient.color;
                    gradientColor.a = alpha;
                    _continuesGradient.color = gradientColor;
                }
            else
                yield return new WaitForSeconds(1f);
            _collided = true;
            progress = 0f;
            if (!_isGameOver)
            while (progress < 1f)
            {
                yield return null;
                progress += Time.deltaTime;
                float alpha = 1f - Mathf.Min(progress, 1f);
                Color continuesColor = _continuesText.color;
                continuesColor.a = alpha;
                _continuesText.color = continuesColor;
                Color gradientColor = _continuesGradient.color;
                gradientColor.a = alpha;
                _continuesGradient.color = gradientColor;
            }
        }
    }
}
