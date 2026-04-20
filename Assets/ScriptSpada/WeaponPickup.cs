using UnityEngine;
using UnityEngine.Events;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    public AnimatorOverrideController swordOverrideController;

    [Header("Eventi")]
    public UnityEvent onSpadaRaccolta;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerWeaponManager playerWeaponManager = collision.GetComponent<PlayerWeaponManager>();
            if (playerWeaponManager != null)
            {
                playerWeaponManager.EquipWeapon(swordOverrideController);
                onSpadaRaccolta?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}