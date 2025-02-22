using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Catparency
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _waveTitle;
        [SerializeField] Animator _waveTitleAnimator;
        [SerializeField] Wave[] _waves;
        int _waveMembersDealtWith = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            for (int i = 0; i < _waves.Length; i++)
            {
                yield return new WaitForSeconds(_waves[i].Delay);
                _waveTitle.text = _waves[i].Title;
                _waveTitleAnimator.Play("Show Title");
                _waveMembersDealtWith = 0;
                foreach (var waveMember in _waves[i].WaveMembers)
                {
                    waveMember.StartMe(this);
                }
                yield return new WaitWhile(() => _waveMembersDealtWith < _waves[i].WaveMembers.Length);

            }
        }

        public void WaveMemberDealtWith()
        {
            _waveMembersDealtWith++;
        }
    }

    [Serializable]
    public struct Wave
    {
        public float Delay;
        [TextArea] public string Title;
        public WaveMember[] WaveMembers;
    }
}
