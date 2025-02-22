using System;
using UnityEngine;
using UnityEngine.Events;

namespace Catparency
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] int _maxHealth = 5;
        [SerializeField] UnityEvent _onOffScreenEvent, _onDamagedEvent;
        [SerializeField] HealthEvent[] _healthEvents;
        int _health;
        void Start()
        {
            _health = _maxHealth;   
        }

        void OnBecameInvisible()
        {
            _onOffScreenEvent.Invoke();
        }

        void OnTriggerEnter(Collider other)
        {
            _health--;
            _onDamagedEvent.Invoke();
            HealthEvent healthEvent = null;
            for (int i = 0; i < _healthEvents.Length; i++)
            {
                if (_healthEvents[i].Health >= _health && (healthEvent == null || _healthEvents[i].Health < healthEvent.Health))
                {
                    healthEvent = _healthEvents[i];
                }
            }

            if (healthEvent != null)
            {
                healthEvent.Event.Invoke();
            }
        }
    }

    [Serializable]
    class HealthEvent
    {
        public int Health;
        public UnityEvent Event;
    }
}
