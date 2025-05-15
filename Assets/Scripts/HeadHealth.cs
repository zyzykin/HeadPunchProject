using UnityEngine;
using DG.Tweening;

public class HeadHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Effect References")]
    [SerializeField] private ParticleSystem hitParticleSystem;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private Transform model;
    [SerializeField] private SoftBody softBody;

    private HealthSystem _healthSystem;
    private Vector3 _initialModelPosition;
    private bool isDead = false;

    private const float HitOffset = 0.05f;
    private const float HitDuration = 0.1f;
    private const float ReturnDuration = 0.15f;
    private const float DeathRotationAngle = 90f;
    private const float DeathRotationDuration = 0.5f;

    private void Awake()
    {
        _healthSystem = new HealthSystem(maxHealth);
        _initialModelPosition = model.localPosition;
        if (softBody == null)
            softBody = GetComponent<SoftBody>();
    }

    public void Hit(float damageAmount, Vector3 hitPoint, Vector3 hitNormal)
    {
        _healthSystem.TakeDamage(damageAmount);

        if (_healthSystem.CurrentHealth <= 0f)
        {
            if (!isDead)
            {
                isDead = true;
                HandleDeathEffect();
            }
            return;
        }

        HandleHitEffects(hitPoint, hitNormal);
    }

    private void SpawnHitParticle(Vector3 point, Vector3 normal)
    {
        if (hitParticleSystem == null) return;

        hitParticleSystem.gameObject.SetActive(true);
        hitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        hitParticleSystem.transform.SetPositionAndRotation(point, Quaternion.LookRotation(normal));
        hitParticleSystem.transform.localScale = Vector3.one * Random.Range(0.1f, 0.2f);
        hitParticleSystem.Play();
    }

    private void ApplyHitReaction(Vector3 point)
    {
        model.DOKill();
        model.localPosition = _initialModelPosition;

        Vector3 localPoint = model.InverseTransformPoint(point);
        Vector3 direction = localPoint.x < 0f ? Vector3.right : Vector3.left;
        Vector3 targetPosition = _initialModelPosition + direction * HitOffset;

        DOTween.Sequence()
            .Append(model.DOLocalMove(targetPosition, HitDuration))
            .Append(model.DOLocalMove(_initialModelPosition, ReturnDuration));
    }

    private void HandleHitEffects(Vector3 point, Vector3 normal)
    {
        if (softBody != null)
            softBody.AddForce(point);

        ApplyHitReaction(point);
        SpawnHitParticle(point, normal);
    }

    private void HandleDeathEffect()
    {
        if (deathParticleSystem == null) return;

        deathParticleSystem.gameObject.SetActive(true);
        deathParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        deathParticleSystem.Play();

        model.DOKill();
        model.DOLocalRotate(new Vector3(DeathRotationAngle, 0f, 0f), DeathRotationDuration, RotateMode.LocalAxisAdd);
    }
}