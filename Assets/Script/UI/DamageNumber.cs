using System;
using TheTD.DamageSystem;
using UnityEngine;

public abstract class DamageNumber : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeCurve;

    [SerializeField] private float criticalTextSizeMultiplier = 1.2f;
    [SerializeField] private float overtimeTextSizeMultiplier = 0.75f;
    [SerializeField] private Color criticalColor = Color.red;
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

    public virtual void InitDamageNumber(Damage damage, Vector3 position, Quaternion rotation, Transform parent, IOvertimeEffect overtimeEffect = null)
    {
        transform.position = position + Vector3.up * 0.5f;
        transform.rotation = rotation;
        transform.SetParent(parent);
        bool isOvertime = overtimeEffect != null;
        int damageValue = isOvertime ? overtimeEffect.TickDamage : damage.Value;
        CalculateElementScaleForDamage(damage.IsCritical, isOvertime);
        var textColor = isOvertime ? damage.DamageType.Color : damage.IsCritical ? criticalColor : damage.DamageType.Color;
        SetDamage(damageValue, textColor);
    }

    private void CalculateElementScaleForDamage(bool isCritical, bool isOvertime)
    {
        float sizeMultiplier = isOvertime ? 1.0f * overtimeTextSizeMultiplier : isCritical ? 1.0f * criticalTextSizeMultiplier : 1.0f;
        transform.localScale = Vector3.one * sizeMultiplier;
    }

    protected void LookAtCamera()
    {
        transform.LookAt(transform.position + Cam.transform.rotation * Vector3.forward, Cam.transform.rotation * Cam.transform.up);
    }

    public abstract void SetDamage(int amount, Color color);
    protected abstract void SetAlpha(float alpha);
}