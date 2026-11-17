using UnityEngine;
using UnityEngine.Events;

public class SimpleCombatHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public UnityEvent onDefeated;

    private bool defeated;

    private void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 1f, maxHealth);
    }

    public void ApplyDamage(float damage)
    {
        if (defeated || damage <= 0f) return;
        currentHealth = Mathf.Max(0f, currentHealth - damage);
        if (currentHealth <= 0f)
        {
            defeated = true;
            onDefeated?.Invoke();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (defeated || amount <= 0f) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }
}
