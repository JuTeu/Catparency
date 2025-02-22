using UnityEngine;

namespace Catparency
{
    public class AnimationTester : MonoBehaviour
    {
        [SerializeField] Animator animator;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K)) animator.SetTrigger("IsHit");
        }
    }
}
