using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(dashKey)) StartCoroutine(Dash());
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetKeyDown(KeyCode.F)) Player.Health = -1;
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
        float speed = (Run * PlayerCharacteristics.RunSpeed) + PlayerCharacteristics.MovementSpeed;
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
        float vertical = (Run * Vertical) + Vertical;
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
            EnemyScript.isEnemyAttacked = true;
            Player.Health -= enm.EnemyDamage;
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