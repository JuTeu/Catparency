using System.Collections;
using UnityEngine;

namespace Catparency
{
    public class WhiteFlasher : MonoBehaviour
    {
        [SerializeField] MeshRenderer[] _renderers;
        Vector4[] _originalColors;
        MaterialPropertyBlock _materialPropertyBlock;
        bool _blinkingInProgress;
        float _blinkingTimer = 0f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _originalColors = new Vector4[_renderers.Length];
            _materialPropertyBlock = new();
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalColors[i] = _renderers[i].material.color;
                //renderer.GetPropertyBlock(_materialPropertyBlock);
                //_materialPropertyBlock.SetVector("_BaseColor", new Vector4(2, 0, 0, 1));
                //renderer.SetPropertyBlock(_materialPropertyBlock);
            }
        }

        public void StartBlinking(float time)
        {
            _blinkingTimer = Mathf.Max(time, _blinkingTimer);
            if (!_blinkingInProgress)
            {
                _blinkingInProgress = true;
                StartCoroutine(Blink());
            }
        }

        IEnumerator Blink()
        {
            bool even = false;
            while (_blinkingTimer > 0f)
            {
                _blinkingTimer -= 0.05f;
                even = !even;
                if (_blinkingTimer <= 0f)
                {
                    even = false;
                }
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].GetPropertyBlock(_materialPropertyBlock);
                    _materialPropertyBlock.SetVector("_BaseColor", even ? new Vector4(5, 5, 5, 1) : _originalColors[i]);
                    _renderers[i].SetPropertyBlock(_materialPropertyBlock);
                }
                yield return new WaitForSeconds(0.05f);
            }
            _blinkingInProgress = false;
        }
    }
}
