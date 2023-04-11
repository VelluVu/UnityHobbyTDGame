using System.Collections;
using UnityEngine;

[System.Serializable]
public class MoveWithAnimationCurve
{
    public AnimationCurve curve;
    public float moveLength = 3f;
    public float lerpTime = 1f;
    private Transform _moveTarget;
    private Vector3 _startPosition;
    private float _timer = 0f;

    public void StartMove(Vector3 direction, MonoBehaviour target)
    {
        _moveTarget = target.transform;
        _startPosition = _moveTarget.position;
        target.StopAllCoroutines();
        target.StartCoroutine(Move(direction));   
    }

    private IEnumerator Move(Vector3 direction)
    {
        _timer = 0f;
        while(_timer <= lerpTime)
        {
            _timer += Time.deltaTime;
            float lerpRatio = _timer / lerpTime;
            float curveValue = curve.Evaluate(lerpRatio);
            _moveTarget.position = Vector3.Lerp(_startPosition, _startPosition + direction * curveValue, lerpRatio);
            yield return null;
        }
    }
}
