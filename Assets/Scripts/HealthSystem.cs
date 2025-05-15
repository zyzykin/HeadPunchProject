using UnityEngine;

public class HealthSystem
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    public HealthSystem(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }
}