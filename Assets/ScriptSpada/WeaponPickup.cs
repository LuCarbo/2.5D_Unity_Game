using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Weapon Settings")]
    [Tooltip("Drag the Animator Override Controller for this weapon here")]
    public AnimatorOverrideController swordOverrideController;

    // Assuming your 2.5D game uses 2D physics for sprite collisions. 
    // If you are using 3D colliders, change this to OnTriggerEnter(Collider other)
    private void OnTriggerEnter(Collider collision)
    {
        // Check if the object that touched the sword has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            // Try to find the PlayerWeaponManager script on the player
            PlayerWeaponManager playerWeaponManager = collision.GetComponent<PlayerWeaponManager>();

            if (playerWeaponManager != null)
            {
                // Tell the player to equip this weapon's override controller
                playerWeaponManager.EquipWeapon(swordOverrideController);

                // Destroy the sword item on the ground so it can't be picked up twice
                Destroy(gameObject);
            }
        }
    }
}