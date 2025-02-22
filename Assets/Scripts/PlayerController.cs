using System.Collections;
using UnityEngine;

namespace Catparency
{
    public class PlayerController : MonoBehaviour
    {
        public static Transform PlayerTransform;
        [SerializeField] GameObject _bullet;
        [SerializeField] Renderer[] _playerMeshes;
        [SerializeField] Animator _animator;
        [SerializeField] GameObject _gameOverCat;
        [SerializeField] GameObject[] _objectsToDisable;
        PlayerProjectile[] _playerProjectiles;
        Rigidbody _rigidbody;
        InputSystem_Actions _inputs;
        float _shootCooldown = 0f, _invulnerability = 0f;
        int _health = 3, _continues = 3;
        bool _isVisible = true, _canBeControlled = true;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _gameOverCat.SetActive(false);
            #if UNITY_EDITOR
            IEnumerator LoadMainMenu()
            {
                yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main Menu", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Main Menu");
            }
            if (!LevelManager.AlreadyLoaded)
            {
                StartCoroutine(LoadMainMenu());
            }
            #endif
            PlayerTransform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _inputs = new();
            _inputs.Player.Enable();
            _playerProjectiles = new PlayerProjectile[100];
            for (int i = 0; i < 100; i++)
            {
                _playerProjectiles[i] = Instantiate(_bullet).GetComponent<PlayerProjectile>();
            }
        }

        void OnDisable()
        {
            _inputs.Player.Disable();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_canBeControlled) return;
            _shootCooldown -= Time.fixedDeltaTime;
            _rigidbody.Move(_rigidbody.position + 5f * Time.fixedDeltaTime * (Vector3)_inputs.Player.Move.ReadValue<Vector2>(), Quaternion.identity);
            _rigidbody.position = new Vector3(Mathf.Clamp(_rigidbody.position.x, -10f, 10f), 
                                            Mathf.Clamp(_rigidbody.position.y, -5f, 5f));

            if (_invulnerability > 0f)
            {
                _invulnerability -= Time.fixedDeltaTime;

                _isVisible = !_isVisible;
                foreach (var renderer in _playerMeshes)
                {
                    renderer.enabled = _isVisible || _invulnerability <= 0f;
                }
            }

            if (_inputs.Player.Attack.inProgress && _shootCooldown < 0f)
            {
                _shootCooldown = 0.05f;
                //Debug.Log("Schüt");
                for (int i = 0; i < _playerProjectiles.Length; i++)
                {
                    if (!_playerProjectiles[i].IsInUse)
                    {
                        _playerProjectiles[i].Shoot(transform.position);
                        break;
                    }
                }
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (_invulnerability > 0f) return;
            _health--;
            _animator.SetTrigger("IsHit");
            if (_health > 0)
            {
                // Invoke("Freeze", 0.1f);
            }
            else
            {
                StartCoroutine(Die());
            }
        }

        void Freeze()
        {
            System.Threading.Thread.Sleep(100);
            _invulnerability = 2f;
        }

        IEnumerator Die()
        {
            _canBeControlled = false;
            Debug.Log("autsis");
            foreach (var objects in _objectsToDisable)
            {
                objects.SetActive(false);
            }
            _gameOverCat.transform.position = new Vector3(transform.position.x, transform.position.y, _gameOverCat.transform.position.z);
            _gameOverCat.SetActive(true);
            yield return new WaitForSeconds(4f);
            _continues--;
            if (_continues >= 0)
            {
                foreach (var objects in _objectsToDisable)
                {
                    objects.SetActive(true);
                }
                _gameOverCat.SetActive(false);
                _invulnerability = 3f;
                transform.position = Vector3.down * 2f;
                _health = 3;
                _canBeControlled = true;
            }
            else
            {
                // Game over
                LevelManager.GameOver();
            }
        }
    }
}
