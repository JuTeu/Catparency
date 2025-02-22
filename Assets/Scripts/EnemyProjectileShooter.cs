using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Catparency
{
    public class EnemyProjectileShooter : MonoBehaviour
    {
        [SerializeField] ProjectileParameters[] _projectiles;
        [SerializeField] float _angle, _angleRotationSpeed, _shootDelay, _roundDelay, _degreesBetweenProjectiles;
        [SerializeField] int _projectileCount, _rounds;

        void Start()
        {
            Shoot();    
        }

        public void Shoot()
        {
            IEnumerator IEShoot()
            {
                for (int i = 0; i < _rounds; i++)
                {
                    yield return new WaitForSeconds(_roundDelay);
                    for (int j = 0; j < _projectileCount; j++)
                    {
                        yield return new WaitForSeconds(_shootDelay);
                    }
                }
                
            }
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
            public EnemyProjectileMovement[] _projectileMovements;
            public float Size;
            public bool IsGhost;
        }
    }
}
