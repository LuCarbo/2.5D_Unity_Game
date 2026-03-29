using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;

    // Optional: Keep track of the base controller in case you want to disarm later
    private RuntimeAnimatorController baseController;

    void Start()
    {
        // Grab the animator component on the player
        animator = GetComponent<Animator>();

        // Save the default (unarmed) controller
        if (animator != null)
        {
            baseController = animator.runtimeAnimatorController;
        }
    }

    // This function will be called by the item when the player picks it up
    public void EquipWeapon(AnimatorOverrideController newWeaponController)
    {
        if (animator != null && newWeaponController != null)
        {
            // Swap the current animator controller with the override controller!
            animator.runtimeAnimatorController = newWeaponController;
            Debug.Log("Equipped new weapon animations!");
        }
        else
        {
            Debug.LogWarning("Missing Animator or Override Controller.");
        }
    }

    // Call this if the player drops the sword to go back to punching
    public void UnequipWeapon()
    {
        if (animator != null && baseController != null)
        {
            animator.runtimeAnimatorController = baseController;
            Debug.Log("Returned to unarmed animations.");
        }
    }
}