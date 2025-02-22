using System.Collections;
using UnityEngine;

namespace Catparency
{
    public class PlayerProjectile : MonoBehaviour
    {
        public bool IsInUse { get; private set; }
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] ParticleSystem _particles;
        [SerializeField] Collider _collider;
        [SerializeField] MeshRenderer[] _projectileVisuals;

        // Update is called once per frame
        void FixedUpdate()
        {
            _rigidbody.linearVelocity = IsInUse ? Vector3.up * 20f : Vector3.zero;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!IsInUse) return;
            StartCoroutine(Hit());
        }

        IEnumerator Hit()
        {
            foreach (var visual in _projectileVisuals)
            {
                visual.enabled = false;
            }
            _collider.enabled = false;
            _particles.Play();
            yield return new WaitForSeconds(1f);
            IsInUse = false;
        }

        public void Shoot(Vector3 position)
        {
            _collider.enabled = true;
            _rigidbody.position = position;
            foreach (var visual in _projectileVisuals)
            {
                visual.enabled = true;
            }
            IsInUse = true;
        }
    }
}