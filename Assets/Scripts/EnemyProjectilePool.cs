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

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
