using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] private Transform PlayerCamera;
    [SerializeField] private MovementCharacteristics PlayerCharacteristics;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashStamina;
    [SerializeField] private float dashTime;
    [SerializeField] private KeyCode dashKey;

    private float Vertical, Run;

    private readonly string VerticalKey = "Vertical";
    private readonly string RunKey = "Run";
    private readonly string JumpKey = "Jump";

    private readonly float DistanceOffsetCamera = 5f;

    private CharacterController PlayerController;
    private Animator PlayerAnimator;

    private Vector3 PlayerDirection;
    private Quaternion PlayerLook;

    private Vector3 TargetRotate => PlayerCamera.forward * DistanceOffsetCamera;

    private readonly EnemyScript enm = new();

    private bool Idle => Vertical == 0;

    private void Start()
    {
        PlayerController = GetComponent<CharacterController>();
        Cursor.visible = PlayerCharacteristics.VisibleCursor;
        PlayerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        Rotate();
        PlayerDied();
        Regeneration();
        Stamina();
        if (Input.GetKeyDown(dashKey)) StartCoroutine(Dash());
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.F)) Player.Health = -1;
    }


    private void Regeneration()
    {
        if (Player.Health <= Math.Ceiling(Player.FULL_HP) && Time.time - Player.TimeWhenTakedDamage >= Player.HealthRegenerationTimeout)
        {
            if (Player.Health + Player.HealthRegeneration >= Math.Ceiling(Player.FULL_HP))
            {
                Player.Health += Player.FULL_HP - Player.Health;
            }
            else
            {
                Player.Health += Player.HealthRegeneration;
            }
        }
    }

    private void Stamina()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) )
        {
            Player.TimeWhenRun = (int)Time.time;
        }
        if (Input.GetKey(KeyCode.LeftShift) && CanRun())
        {
            if (Time.time - Player.TimeWhenRun >= 1f)
            {
                print("yes");
                Player.Stamina -= 9;
                Player.TimeWhenRun = (int)Time.time;
                Player.TimeAfterRun = (int)Time.time;
            }
        }
        else
        {
            if (Player.Stamina <= Math.Ceiling(Player.FULL_STAMINA) && Time.time - Player.TimeAfterRun >= 10)
            {
                if (Player.Stamina + Player.StaminaRegeneration >= Math.Ceiling(Player.FULL_STAMINA))
                {
                    Player.Stamina += Player.FULL_STAMINA - Player.Stamina;
                }
                else
                {
                    Player.Stamina += Player.StaminaRegeneration;
                }
            }
        }
    }

    private bool CanRun()
    {
        if (Player.Stamina - 9 >= 0)
        {
            return true;
        }
        return false;
    }

    private void Movement()
    {
        if (PlayerController.isGrounded)
        {
            Vertical = Input.GetAxis(VerticalKey);
            Run = Input.GetAxis(RunKey);

            PlayerDirection = transform.TransformDirection(0, 0, Vertical).normalized;
            PlayAnimation();
            Jump();
        }
        PlayerDirection.y -= PlayerCharacteristics.Gravity * Time.deltaTime;

        float speed;
        if (CanRun())
        {
            speed = (Run * PlayerCharacteristics.RunSpeed) + PlayerCharacteristics.MovementSpeed;
        }
        else
        {
            speed = PlayerCharacteristics.MovementSpeed;
        }
        Vector3 dir = speed * Time.deltaTime * PlayerDirection;
        dir.y = PlayerDirection.y;
        _ = PlayerController.Move(dir);
    }

    private void Rotate()
    {
        if (Idle) return;
        Vector3 target = TargetRotate;
        target.y = 0;
        PlayerLook = Quaternion.LookRotation(target);
        float speed = PlayerCharacteristics.AngularSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, PlayerLook, speed);
    }

    private void Jump()
    {
        if (Input.GetButtonDown(JumpKey))
        {
            PlayerAnimator.SetTrigger(JumpKey);
            PlayerDirection.y += PlayerCharacteristics.Jump;
        }
    }

    private void PlayAnimation()
    {
        float vertical;
        if (CanRun())
        {
            vertical = (Run * Vertical) + Vertical;
        }
        else
        {
            vertical = Vertical;
        }
        PlayerAnimator.SetFloat("Vertical", vertical);
    }

    private IEnumerator Dash()
    {
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            Vector3 dir = dashSpeed * Time.deltaTime * PlayerDirection;
            _ = PlayerController.Move(dir);
            yield return null;
        }
    }

    private void PlayerDied()
    {
        if (Player.Health <= 0)
        { 
            Player.isDied = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && !EnemyScript.isEnemyAttacked)
        {
            print("yes!!!");
            EnemyScript.isEnemyAttacked = true;
            Player.Health -= enm.EnemyDamage;
            Player.TimeWhenTakedDamage = (int)Time.time;
        }
        if (other.gameObject.CompareTag("SpawnArea"))
        {
            gameObject.GetComponent<EnemyController>().enabled = true;
        }
    }

    private void Attack()
    {
        PlayerAnimator.SetTrigger("Attack");
    }
}