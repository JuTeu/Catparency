using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Catparency
{
    public class EnemyProjectile : MonoBehaviour
    {
        public EnemyProjectilePool Pool;
        EnemyProjectileMovement[] _movements;
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] ParticleSystem _particles;
        [SerializeField] Collider _collider;
        [SerializeField] MeshRenderer[] _projectileVisuals;

        public void Shoot(Vector3 position, Quaternion rotation, EnemyProjectileMovement[] movements, float scale, bool IsGhost)
        {
            //Debug.Log(position);
            _rigidbody.position = position;
            _collider.enabled = true;
            transform.localScale = Vector3.one * scale;
            _rigidbody.rotation = rotation;
            _movements = movements;
            Pool.AvailableEnemyProjectiles.Remove(this);
            IsInUse = true;
            StartCoroutine(Move());
        }

        IEnumerator Move()
        {
            yield return null;
            foreach (var visual in _projectileVisuals)
            {
                visual.enabled = true;
            }
            for (int i = 0; i < _movements.Length; i++)
            {
                float progress = 0f;
                while (progress < 1f)
                {
                    if (_rigidbody.position.sqrMagnitude > 1000)
                    {
                        NoLongerInUse();
                        yield break;
                    }
                    progress += Time.fixedDeltaTime / Mathf.Max(0.01f, _movements[i].Duration);
                    Vector3 direction = Vector3.zero;
                    switch (_movements[i].ProjectilePathType)
                    {
                        case ProjectilePathType.GoStraight: direction = _rigidbody.rotation * _movements[i].DirectionOrOrbitPoint * _movements[i].Speed * Time.fixedDeltaTime; break;
                        case ProjectilePathType.FollowPlayer: direction = _movements[i].Speed * Time.fixedDeltaTime * (PlayerController.PlayerTransform.position - _rigidbody.position).normalized; break;
                        case ProjectilePathType.OrbitPoint: direction = _movements[i].Speed * Time.fixedDeltaTime * Vector2.Perpendicular((_movements[i].DirectionOrOrbitPoint - _rigidbody.position).normalized); break;
                    } //_rigidbody.rotation * _movements[i].DirectionOrOrbitPoint * _movements[i].Speed * Time.fixedDeltaTime;
                    _rigidbody.MovePosition(_rigidbody.position + direction);
                    yield return new WaitForFixedUpdate();
                }
            }
            NoLongerInUse();
        }

        bool IsInUse;

        void OnTriggerEnter(Collider other)
        {
            if (!IsInUse) return;
            StopAllCoroutines();
            StartCoroutine(Hit());
        }

        IEnumerator Hit()
        {
            _particles.Play();
            foreach (var visual in _projectileVisuals)
            {
                visual.enabled = false;
            }
            _collider.enabled = false;
            yield return new WaitForSeconds(1f);
            NoLongerInUse();
        }

        void NoLongerInUse()
        {
            foreach (var visual in _projectileVisuals)
            {
                visual.enabled = false;
            }
            IsInUse = false;
            _collider.enabled = false;
            Pool.AvailableEnemyProjectiles.Add(this);
        }
    }

    public enum ProjectilePathType
    {
        GoStraight,
        FollowPlayer,
        OrbitPoint
    }

    [Serializable]
    public struct EnemyProjectileMovement
    {
        public float Duration, Speed;
        public ProjectilePathType ProjectilePathType;
        public Vector3 DirectionOrOrbitPoint;
    }
}
