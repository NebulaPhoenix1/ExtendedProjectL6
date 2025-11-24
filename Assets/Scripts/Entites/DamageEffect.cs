using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private GameObject damageEffectPrefab;
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private float effectDuration = 2.0f;

    public void PlayDamageEffect()
    {
        if(damageEffectPrefab != null && effectSpawnPoint != null)
        {
            GameObject effect = Instantiate(damageEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);
            Destroy(effect, effectDuration);
        }
        else
        {
            Debug.LogWarning("DamageEffect: Prefab or Spawn Point is not assigned.");
        }
    }
}
