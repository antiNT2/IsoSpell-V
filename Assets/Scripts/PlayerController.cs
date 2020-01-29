using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    GameObject ropeHingeAnchor;
    [SerializeField]
    public GameObject crosshair;
    SpriteRenderer crosshairSprite;
    [SerializeField]
    GameObject anchorProjectilePrefab;
    [SerializeField]
    PlayerInput playerInput;
    public LayerMask ropeCollisionLayerMask;
    PlayerMotor playerMotor;
    RopeSystem ropeSystem;
    Animator anim;
    PlayerInput thisPlayerInput;
    PlayerHealth playerHealth;

    public float movementAxis = 0; // 1 for right / -1 for left / 0 for nothing

    public float aimAngle;
    Vector2 rightStickPosition = new Vector2();

    private void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerHealth = GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
        ropeSystem = GetComponent<RopeSystem>();
        crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
        thisPlayerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (GameManager.instance.isInWeaponSelection)
            return;

        if (CanMove())
        {
            if (movementAxis == 1)
            {
                playerMotor.MoveRight();
            }
            else if (movementAxis == -1)
            {
                playerMotor.MoveLeft();
            }

        }
        if (movementAxis != 1 && movementAxis != -1)
        {
            anim.SetBool("walk", false);
        }

        if (thisPlayerInput.currentControlScheme == "Keyboard&Mouse")
            SetCrosshairPosition(true);
        else
            SetCrosshairPosition(false);

        anim.SetBool("slide", playerMotor.wallSliding);
    }

    #region Player Input Script methods
    void OnMove(InputValue value) //performed by player input script
    {
        movementAxis = value.Get<Vector2>().x;
    }

    void OnGrapplingHook()
    {
        if (CanMove())
            FireRope();
    }

    void OnJumpButton()
    {
        if (CanMove())
            playerMotor.Jump();
    }

    void OnRotateCrosshair(InputValue value)
    {
        //print("r: " + value.Get<Vector2>().ToString());
        if (value.Get<Vector2>() != Vector2.zero)
            rightStickPosition = value.Get<Vector2>();
    }

    #endregion

    void FireRope()
    {
        if (playerMotor.isSwinging)
        {
            ropeSystem.ResetRope();
            return;
        }

        var hit = Physics2D.Raycast(transform.position, GetAimDirection(), 99f, ropeCollisionLayerMask);
        if (hit.collider != null)
        {
            AnchorBehaviour anchorProj = Instantiate(anchorProjectilePrefab).GetComponent<AnchorBehaviour>();
            anchorProj.transform.position = transform.position;
            anchorProj.destination = hit.point;
            anchorProj.playerOwner = this.gameObject;
            anchorProj.speed = 100f;
            anchorProj.OnArriveAtDestination += () => ropeSystem.AttachRopeToPoint(anchorProj.transform.position);
            anchorProj.OnArriveAtDestination += () => playerMotor.Jump();
            anchorProj.OnArriveAtDestination += () => Destroy(anchorProj.gameObject);
        }
        Debug.DrawRay(this.transform.position, GetAimDirection());
    }

    void SetCrosshairPosition(bool isUsingMouse)
    {
        if (isUsingMouse)
        {
            var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
            var facingDirection = worldMousePosition - transform.position;
            aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }
        }
        else
        {
            aimAngle = Mathf.Acos(rightStickPosition.x);
            if (rightStickPosition.y < 0)
                aimAngle = -aimAngle;
        }

        //print("Aim Angle : " + aimAngle);
        if (!crosshairSprite.enabled)
        {
            crosshairSprite.enabled = true;
        }

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle);
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle);

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }

    public Vector2 GetAimDirection()
    {
        //float aimAngle = ropeSystem.aimAngle;

        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;

        return aimDirection;
    }

    public bool CanMove()
    {
        if(playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth.currentHealth <= 0 || GameManager.instance.isInWeaponSelection == true)
            return false;


        return true;
    }
}
