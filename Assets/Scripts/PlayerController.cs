using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] TextMeshProUGUI _continuesText, _healthText;
        [SerializeField] Renderer _planarShiftToGhost, _planarShiftToNormal;
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
            _inputs.Player.Jump.started += Shift;
            _playerProjectiles = new PlayerProjectile[100];
            for (int i = 0; i < 100; i++)
            {
                _playerProjectiles[i] = Instantiate(_bullet).GetComponent<PlayerProjectile>();
            }
        }

        bool _shiftInProgress, _playerIsGhost;
        void Shift(InputAction.CallbackContext context)
        {
            if (_shiftInProgress) return;
            _shiftInProgress = true;
            _playerIsGhost = !_playerIsGhost;
            _planarShiftToGhost.gameObject.SetActive(false);
            _planarShiftToNormal.gameObject.SetActive(false);
            gameObject.layer = _playerIsGhost ? 11 : 10;
            StartCoroutine(IEShift());
        }

        IEnumerator IEShift()
        {
            float progress = 0;
            Renderer plane = _playerIsGhost ? _planarShiftToGhost : _planarShiftToNormal;
            plane.gameObject.SetActive(true);
            MaterialPropertyBlock mpb = new();
            plane.GetPropertyBlock(mpb);
            while (progress < 1f)
            {
                yield return null;
                progress += Time.deltaTime * 3f;
                float value = 1f - Mathf.Min(progress, 1f);
                value *= value;
                mpb.SetFloat("_Progress", value);
                plane.SetPropertyBlock(mpb);
            }
            _shiftInProgress = false;
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
            Vector2 moveInput = _inputs.Player.Move.ReadValue<Vector2>();
            //_rigidbody.rotation = Quaternion.Euler(moveInput.y * 30f, moveInput.x * 30f, 0);

            _rigidbody.Move(_rigidbody.position + 5f * Time.fixedDeltaTime * (Vector3)moveInput, Quaternion.Euler(moveInput.y * 30f, moveInput.x * -30f, 0));
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
            if (_invulnerability > 0f || !_canBeControlled) return;
            _health--;
            _healthText.text = $"HP: {_health}";
            _animator.SetTrigger("IsHit");
            if (_health > 0)
            {
                _invulnerability = 2f;
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
            
            _continues--;
            _continuesText.text = $"Lives left:\n{_continues}";
            _continuesText.ForceMeshUpdate();

            if (_continues < 0)
            {
                _gameOverCat.GetComponent<PlayerDefeat>().IsGameOver = true;
            }
            _gameOverCat.transform.position = new Vector3(transform.position.x, transform.position.y, _gameOverCat.transform.position.z);
            _gameOverCat.SetActive(true);
            
            if (_continues >= 0)
            {
                yield return new WaitForSeconds(4f);
                foreach (var objects in _objectsToDisable)
                {
                    objects.SetActive(true);
                }
                _gameOverCat.SetActive(false);
                _invulnerability = 3f;
                transform.position = Vector3.down * 2f;
                _health = 3;
                _healthText.text = $"HP: {_health}";
                _canBeControlled = true;
            }
            else
            {
                yield return new WaitForSeconds(2f);
                // Game over
                LevelManager.GameOver();
            }
        }
    }
}
