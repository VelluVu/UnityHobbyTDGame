using UnityEngine;

public abstract class DamageNumber : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeCurve;

    private float age;

    private Camera _cam;
    public Camera Cam { get => _cam = _cam != null ? _cam : Camera.main; }

    protected virtual void Update()
    {
        age += Time.deltaTime;
        if (age > duration)
        {
            age = 0;
            gameObject.SetActive(false);
            return;
        }
        LookAtCamera();
        float lerpValue = age / fadeDuration;
        float alpha = fadeCurve.Evaluate(lerpValue);
        SetAlpha(alpha);
    }

    public virtual void InitDamageNumber(int amount, Color textColor, Vector3 position, Quaternion rotation, Transform parent, bool isCritical = false, bool isOvertime = false)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.SetParent(parent);
        float sizeMultiplier = isCritical ? 1.2f : 1.0f;
        sizeMultiplier = isOvertime ? sizeMultiplier * 0.75f : sizeMultiplier;
        transform.localScale = Vector3.one * sizeMultiplier;
        SetDamage(amount, textColor);
    }

    protected void LookAtCamera()
    {
        transform.LookAt(transform.position + Cam.transform.rotation * Vector3.forward, Cam.transform.rotation * Cam.transform.up);
    }

    public abstract void SetDamage(int amount, Color color);
    protected abstract void SetAlpha(float alpha);
}