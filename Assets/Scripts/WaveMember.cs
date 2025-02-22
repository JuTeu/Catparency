using UnityEngine;
using UnityEngine.Events;

namespace Catparency
{
    public class WaveMember : MonoBehaviour
    {
        WaveManager _waveManager;
        bool _dealtWith;
        [SerializeField] UnityEvent _onStart; 
        
        public void StartMe(WaveManager waveManager)
        {
            _dealtWith = false;
            _waveManager = waveManager;
        }

        public void DealtWith()
        {
            if (_dealtWith) return;
            _dealtWith = true;
            _waveManager.WaveMemberDealtWith();
        }
    }
}
