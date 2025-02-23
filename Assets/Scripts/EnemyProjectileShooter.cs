using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Catparency
{
    public class EnemyProjectileShooter : MonoBehaviour
    {
        [SerializeField] ProjectileParameters[] _projectiles;
        [SerializeField] float _distanceFromMiddle, _angle, _angleRotationSpeed, _shootDelay, _roundDelay, _degreesBetweenProjectiles, _spacingBetweenProjectiles;
        [SerializeField] int _projectileCount, _rounds;

        /*void Start()
        {
            Shoot();    
        }*/

        void OnDrawGizmosSelected()
        {
            //Gizmos.
            //Gizmos.DrawRay(transform.position, Vector3.up * 10);
            float angle = _angle;
            Quaternion rotation;
            if (_projectiles.Length > 0)
            for (int i = 0; i < _projectileCount; i++)
            {
                rotation = Quaternion.Euler(0, 0, angle);
                ProjectileParameters parameters = _projectiles[i % _projectiles.Length];
                Vector3 offset = Vector3.zero;
                Vector3 direction = Vector3.down;
                if (parameters.ProjectileMovements.Length > 0)
                {
                    direction = rotation * parameters.ProjectileMovements[0].DirectionOrOrbitPoint;
                    Vector3 rowness = Vector3.right * (_spacingBetweenProjectiles * i);
                    offset = rotation * (Vector3.down * _distanceFromMiddle + rowness);
                    direction *= parameters.ProjectileMovements[0].Speed;
                }
                Gizmos.DrawRay(transform.position + offset, direction);
                angle += _degreesBetweenProjectiles;
            }
        }

        public void Shoot()
        {
            IEnumerator IEShoot()
            {
                float accumulatedAngle = 0f;
                float lastCheckedTime = Time.time;
                for (int i = 0; i < _rounds; i++)
                {
                    accumulatedAngle += Time.time - lastCheckedTime;
                    lastCheckedTime = Time.time;
                    /*for (int j = 0; j < _projectileCount; j++)
                    {
                        yield return new WaitForSeconds(_shootDelay);
                    }*/
                    float angle = _angle + accumulatedAngle * _angleRotationSpeed;
                    Quaternion rotation;
                    //Vector3 offset;
                    if (_projectiles.Length == 0) throw new Exception($"Add some projectiles to {gameObject}");
                    for (int j = 0; j < _projectileCount; j++)
                    {
                        rotation = Quaternion.Euler(0, 0, angle);
                        ProjectileParameters parameters = _projectiles[j % _projectiles.Length];
                        //Vector3 offset = Vector3.zero;
                        Vector3 rowness = Vector3.right * (_spacingBetweenProjectiles * j);
                        Vector3 offset = rotation * (Vector3.down * _distanceFromMiddle + rowness);
                        //Vector3 direction = Vector3.down;
                        /*if (parameters.ProjectileMovements.Length > 0)
                        {
                            //direction = rotation * parameters.ProjectileMovements[0].DirectionOrOrbitPoint;
                            Vector3 rowness = Vector3.right * (_spacingBetweenProjectiles * j);
                            offset = Vector3.down * _distanceFromMiddle + rowness;
                            direction *= parameters.ProjectileMovements[0].Speed;
                        }*/
                        //Gizmos.DrawRay(transform.position + offset, direction);
                        EnemyProjectilePool.GetProjectile().Shoot(transform.position + offset, rotation, parameters.ProjectileMovements, parameters.Size, parameters.IsGhost);
                        angle += _degreesBetweenProjectiles;
                        yield return new WaitForSeconds(_shootDelay);
                    }
                    yield return new WaitForSeconds(_roundDelay);
                }
                
            }
            StartCoroutine(IEShoot());
            /*EnemyProjectilePool.GetProjectile().Shoot(transform.position, Quaternion.identity, new EnemyProjectileMovement[]
            {
                new EnemyProjectileMovement()
                {
                    Duration = 5,
                    Speed = 1,
                    DirectionOrOrbitPoint = Vector3.left,
                    ProjectilePathType = ProjectilePathType.GoStraight
                }
            });*/
        }

        [Serializable]
        struct ProjectileParameters
        {
            public EnemyProjectileMovement[] ProjectileMovements;
            public float Size;
            public bool IsGhost;
        }
    }
}
