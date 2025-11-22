using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, ProjectActions.IOverworldActions
{
    [HideInInspector] public Locomotion locomotion;
    [HideInInspector] public PlayerAnimations anim;
    [HideInInspector] public PlayerState playerState;
    [HideInInspector] public PlayerCombat combat;
    [HideInInspector] public ProjectActions input;
    [HideInInspector] public LockOn lockOn;
    [HideInInspector] public CameraController cameraController;

    [HideInInspector] public bool isDead { get; private set; }
    private void OnEnable()
    {
        locomotion = GetComponentInChildren<Locomotion>();
        anim = GetComponent<PlayerAnimations>();
        playerState = GetComponent<PlayerState>();
        combat = GetComponentInChildren<PlayerCombat>();
        lockOn = GetComponentInChildren<LockOn>();
        cameraController = FindAnyObjectByType<CameraController>();
        if (cameraController == null)
        {
            Debug.Log("camera controller missing");
        }

        input = new ProjectActions();
        input.Enable();
        input.Overworld.SetCallbacks(this);
    }

    private void OnDisable()
    {
        input.Disable();
        input.Overworld.RemoveCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        locomotion.SetDirection(context.canceled ? Vector2.zero : dir);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        locomotion.SetJumpInput(context.ReadValueAsButton());
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        playerState.TryDropWeapon(transform.forward);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && locomotion.IsGrounded())
        {
            combat.Attack();
        }
    }

    public void OnLockOn(InputAction.CallbackContext context) => lockOn.HandleLockOnInput();

    public void die()
    {
        isDead = true;
        playerState.Die();
        anim.deathAnimation();
    }

    private void OnTriggerEnter(Collider other)
    {
        Ipickups pickup = other.GetComponent<Ipickups>();
        if (pickup != null) pickup.Pickup(this);
    }

    public void LoadPlayerData(PlayerData data)
    {
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = data.Transform.Position.ToVector3();
        transform.eulerAngles = data.Transform.Rotation.ToVector3();
        transform.localScale = data.Transform.Scale.ToVector3();

        if (cc != null) cc.enabled = true;

        GameManager.Instance.PlayerHealth = data.Health;

        Weapon weapon = FindAnyObjectByType<Weapon>();

        if (weapon == null)
        {
            Debug.LogWarning("Weapon not found in scene.");
            return;
        }

        if (data.IsWeaponEquipped)
        {
            //Equip weapon and sync playerState
            weapon.Equip(GetComponent<Collider>(), combat.WeaponAttachPoint);
            playerState.Equipped = true;
            playerState.weapon = weapon;
            combat.SetWeapon(weapon);
        }
        else
        {
            if (data.WeaponPosition != null)
            {
                weapon.transform.position = data.WeaponPosition.ToVector3();
            }
            if (data.WeaponRotation != null)
            {
                weapon.transform.eulerAngles = data.WeaponRotation.ToVector3();
            }
        }

        Debug.Log("Loaded player data after spawn.");
    }
}