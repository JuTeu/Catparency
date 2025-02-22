using UnityEngine;
using TMPro;
using System.Collections;

namespace Catparency
{
    public class PlayerDefeat : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] TextMeshPro _faceText;
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
            yield return new WaitForSeconds(1f);
            _collided = true;
        }
    }
}
