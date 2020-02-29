using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    [SerializeField]
    Image grapplingHookChargeDisplay;
    public LayerMask ropeCollisionLayerMask;
    PlayerMotor playerMotor;
    RopeSystem ropeSystem;
    Animator anim;
    PlayerInput thisPlayerInput;
    PlayerHealth playerHealth;
    bool isUsingAutoAim;
    GameObject statutsIndicatorPrefab;

    bool hasShotGrapplingHook;

    public float movementAxis = 0; // 1 for right / -1 for left / 0 for nothing

    /// <summary>
    /// Player aim angle in radians
    /// </summary>
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
        statutsIndicatorPrefab = playerHealth.damageDisplayPrefab;
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

        if (Time.timeScale != 0)
        {
            if (thisPlayerInput.currentControlScheme == "Keyboard&Mouse")
                SetCrosshairPosition(true);
            else
                SetCrosshairPosition(false);
        }

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
        {
            playerMotor.Jump();
        }
    }

    void OnRotateCrosshair(InputValue value)
    {
        //print("r: " + value.Get<Vector2>().ToString());
        if (value.Get<Vector2>() != Vector2.zero)
            rightStickPosition = value.Get<Vector2>();
    }

    void OnPause()
    {
        if (GameManager.instance.isInWeaponSelection == false)
            PauseMenu.instance.Pause();
    }

    void OnAutoAimToggle()
    {
        ToggleAutoAim();
    }

    #endregion

    void FireRope()
    {
        if (playerMotor.isSwinging)
        {
            ropeSystem.ResetRope();
            return;
        }

        if (grapplingHookChargeDisplay.fillAmount < 0.95f || hasShotGrapplingHook == true)
            return;

        var hit = Physics2D.Raycast(transform.position, GetAimDirection(), 99f, ropeCollisionLayerMask);
        if (hit.collider != null)
        {
            AnchorBehaviour anchorProj = Instantiate(anchorProjectilePrefab).GetComponent<AnchorBehaviour>();
            anchorProj.transform.position = transform.position;
            anchorProj.destination = hit.point;
            anchorProj.playerOwner = this.gameObject;
            anchorProj.speed = 100f;
            hasShotGrapplingHook = true;
            anchorProj.OnArriveAtDestination += () => ropeSystem.AttachRopeToPoint(anchorProj.transform.position, anchorProj.transform.rotation);
            anchorProj.OnArriveAtDestination += () => playerMotor.Jump();
            anchorProj.OnArriveAtDestination += () => Destroy(anchorProj.gameObject);
            anchorProj.OnArriveAtDestination += () => hasShotGrapplingHook = false;
        }
        Debug.DrawRay(this.transform.position, GetAimDirection());
        StartCoroutine(ReloadGrapplingHook());
    }

    void SetCrosshairPosition(bool isUsingMouse)
    {
        if (isUsingAutoAim == false)
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
        }
        else
        {
            Vector3 targetPos = CustomFunctions.GetClosestPlayerPosition(this.transform.position);

            if (Spawner.instance.isInZombieMode)
                targetPos = CustomFunctions.GetClosestZombiePosition(this.transform.position);

            aimAngle = GetAutoAimAngle(targetPos);
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
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth.currentHealth <= 0 || GameManager.instance.isInWeaponSelection == true || playerHealth.currentParryState != PlayerHealth.ParryState.None || Time.timeScale == 0f)
            return false;


        return true;
    }

    void ToggleAutoAim()
    {
        isUsingAutoAim = !isUsingAutoAim;

        if (isUsingAutoAim)
        {
            crosshairSprite.color = Color.red;
        }
        else
        {
            crosshairSprite.color = Color.white;
        }
        SpawnAutoAimStatutsIndicator();
    }

    Vector2 GetAutoAimDirection(Vector2 targetPosition)
    {
        return (Vector2)(targetPosition - (Vector2)this.transform.position).normalized;
    }

    float GetAutoAimAngle(Vector3 targetPos)
    {
        float angle = Vector2.Angle(GetAutoAimDirection(targetPos), Vector2.left) + 180f;
        if (transform.position.y < targetPos.y)
        {
            angle = -angle;
        }

        return angle * Mathf.Deg2Rad;
    }

    void SpawnAutoAimStatutsIndicator()
    {
        GameObject indicator = Instantiate(statutsIndicatorPrefab);
        indicator.transform.position = this.transform.position;

        string message = "Auto Aim : ON";
        if (isUsingAutoAim == false)
            message = "Auto Aim : OFF";
        indicator.GetComponentInChildren<TextMeshProUGUI>().text = message;

        Destroy(indicator, 0.3f);
    }

    IEnumerator ReloadGrapplingHook()
    {
        yield return new WaitForSeconds(0.5f);
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = 0.01f / 0.5f; //The amount of change to apply.
        grapplingHookChargeDisplay.fillAmount = 0;
        while (progress < 1f)
        {
            grapplingHookChargeDisplay.fillAmount = progress;
            progress += increment;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
