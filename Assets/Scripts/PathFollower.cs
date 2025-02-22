using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Catparency
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] bool _flipByX, _flipByY;
        [SerializeField] Vector3 _offset;
        [SerializeField] Movement[] _movements;
        Rigidbody _rigidbody;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        IEnumerator Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            int loopPoint = int.MaxValue;
            for (int i = 0; i < _movements.Length; i++)
            {
                _movements[i].Event.Invoke();
                Vector3 targetPosition = _movements[i].IsRelativeMovement ?
                                         _rigidbody.position + _movements[i].MovementVectorOrPosition :
                                         _movements[i].MovementVectorOrPosition;
                targetPosition += _offset;
                targetPosition = new Vector3(_flipByX ? -targetPosition.x : targetPosition.x,
                                             _flipByY ? -targetPosition.y : targetPosition.y,
                                             targetPosition.z);
                Vector3 startPosition = _rigidbody.position;
                float progress = 0;
                while (progress < 1f)
                {
                    progress += Time.fixedDeltaTime / Mathf.Max(0.01f, _movements[i].Duration);
                    _rigidbody.MovePosition(new(Ease(startPosition.x, targetPosition.x, progress, _movements[i].XEasing),
                                                Ease(startPosition.y, targetPosition.y, progress, _movements[i].YEasing),
                                                Ease(startPosition.z, targetPosition.z, progress, _movements[i].ZEasing)));
                    yield return new WaitForFixedUpdate();
                }
                _rigidbody.position = targetPosition;
                if (_movements[i].LoopPoint) loopPoint = i;

                if (i == _movements.Length - 1) i = loopPoint - 1;
            }
        }

        float Ease(float start, float end, float progress, EasingMode easingMode)
        {
            progress = Mathf.Clamp01(progress);
            float adjustedProgress = easingMode switch
            {
                EasingMode.EaseInSine => Easing.InSine(progress),
                EasingMode.EaseOutSine => Easing.OutSine(progress),
                EasingMode.EaseInOutSine => Easing.InOutSine(progress),
                EasingMode.EaseInCubic => Easing.InCubic(progress),
                EasingMode.EaseOutCubic => Easing.OutCubic(progress),
                EasingMode.EaseInOutCubic => Easing.InOutCubic(progress),
                EasingMode.EaseInCirc => Easing.InCirc(progress),
                EasingMode.EaseOutCirc => Easing.OutCirc(progress),
                EasingMode.EaseInOutCirc => Easing.InOutCirc(progress),
                EasingMode.EaseInElastic => Easing.InElastic(progress),
                EasingMode.EaseOutElastic => Easing.OutElastic(progress),
                EasingMode.EaseInOutElastic => Easing.InOutElastic(progress),
                EasingMode.EaseInBack => Easing.InBack(progress),
                EasingMode.EaseOutBack => Easing.OutBack(progress),
                EasingMode.EaseInOutBack => Easing.InOutBack(progress),
                EasingMode.EaseInBounce => Easing.InBounce(progress),
                EasingMode.EaseOutBounce => Easing.OutBounce(progress),
                EasingMode.EaseInOutBounce => Easing.InOutBounce(progress),
                _ => progress,
            };
            return Mathf.Lerp(start, end, adjustedProgress);
        }
    }

    [Serializable]
    public struct Movement
    {
        public bool LoopPoint, IsRelativeMovement;
        public float Duration;
        public EasingMode XEasing, YEasing, ZEasing;
        public Vector3 MovementVectorOrPosition;
        public UnityEvent Event;
    }
}
