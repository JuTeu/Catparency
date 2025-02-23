using UnityEngine;

namespace Catparency
{
    public class CatFollower : MonoBehaviour
    {
        [SerializeField] Transform _cat;
        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = new Vector3(_cat.transform.position.x, _cat.transform.position.y, transform.position.z);
        }
    }
}
