using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private GameObject PlayerFirst;

    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RotationSpeed;
    [SerializeField] private float EnemyDistance;
    public float EnemyDamage = 5f;
    public float EnemyHealth = 100f;

    private CharacterController EnemyController;
    private Animator EnemyAnimator;

    public static bool isEnemyAttacked = false;

    private void Start()
    {
        PlayerFirst = GameObject.FindGameObjectWithTag("Player");
        EnemyController = GetComponent<CharacterController>();
        EnemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, PlayerFirst.transform.position) <= EnemyDistance)
        {
            EnemyAnimator.SetTrigger("Attack");
            isEnemyAttacked = false;
        }
        else
        {
            EnemyAnimator.SetTrigger("Run");
            EnemyMoveController();
        }
        IsEnemyDied();
    }

    private void EnemyMoveController()
    {
        Vector3 lookDir = PlayerFirst.transform.position - transform.position;
        lookDir.y = 0;
        Vector3 trposition = MoveSpeed * Time.deltaTime * transform.forward;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), RotationSpeed * Time.deltaTime);
        EnemyController.Move(trposition);
    }

    private void IsEnemyDied()
    {
        if (EnemyHealth <= 0)
        {
            Destroy(gameObject);
            Player.PlayerKills += 1;
            Player.PlayerLevel += 1;
            Player.SkillPoints += 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sword")) EnemyHealth -= Player.Damage;
    }
}