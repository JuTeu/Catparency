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
        [SerializeField] Renderer _renderer;
        EnemyProjectileMovement[] _movements;
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] ParticleSystem _particles;
        [SerializeField] Collider _collider;
        [SerializeField] MeshRenderer[] _projectileVisuals;

        public void Shoot(Quaternion rotation, EnemyProjectileMovement[] movements, float scale = 1f)
        {
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
            for (int i = 0; i < _movements.Length; i++)
            {
                float progress = 0f;
                while (progress < 1f)
                {
                    if (!_renderer.isVisible)
                    {
                        NoLongerInUse();
                        yield break;
                    }
                    progress += Time.fixedDeltaTime * Mathf.Max(0.01f, _movements[i].Duration);
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

        void OnCollisionEnter(Collision collision)
        {
            if (!IsInUse) return;
            StopAllCoroutines();
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

        void NoLongerInUse()
        {
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
