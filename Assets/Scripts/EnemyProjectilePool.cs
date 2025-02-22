using System.Collections.Generic;
using UnityEngine;

namespace Catparency
{
    public class EnemyProjectilePool : MonoBehaviour
    {
        static EnemyProjectilePool _instance;
        public List<EnemyProjectile> AvailableEnemyProjectiles { get; set; }
        [SerializeField] GameObject _enemyProjectileBase;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _instance = this;
            AvailableEnemyProjectiles = new();
            for (int i = 0; i < 1000; i++)
            {
                EnemyProjectile newProjectile = Instantiate(_enemyProjectileBase).GetComponent<EnemyProjectile>();
                newProjectile.Pool = this;
                AvailableEnemyProjectiles.Add(newProjectile);
            }
        }

        public static EnemyProjectile GetProjectile()
        {
            if (_instance.AvailableEnemyProjectiles.Count == 0)
            {
                EnemyProjectile newProjectile = Instantiate(_instance._enemyProjectileBase).GetComponent<EnemyProjectile>();
                newProjectile.Pool = _instance;
                _instance.AvailableEnemyProjectiles.Add(newProjectile);
            }
            return _instance.AvailableEnemyProjectiles[0];
        }
    }
}
