using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Components")]
    public Animator animator;

    [Header("Animation Settings")]
    public float animationSpeed = 1f;
    public bool useRootMotion = false;

    // �ִϸ��̼� �ؽð��� (���� ����ȭ)
    private int idleHash = Animator.StringToHash("Idle");
    private int walkHash = Animator.StringToHash("Walk");
    private int runHash = Animator.StringToHash("Run");
    private int jumpHash = Animator.StringToHash("Jump");
    private int attackHash = Animator.StringToHash("Attack");
    private int hitHash = Animator.StringToHash("Hit");
    private int deathHash = Animator.StringToHash("Death");
    private int drinkPotionHash = Animator.StringToHash("DrinkPotion");

    // ���� ����
    private bool isMoving = false;
    private bool isRunning = false;
    private bool isGrounded = true;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // �ִϸ��̼� �ӵ� ����
        animator.speed = animationSpeed;
    }

    // �̵� �ִϸ��̼� ����
    public void SetMovement(bool moving, bool running)
    {
        if (isDead || isAttacking) return;

        isMoving = moving;
        isRunning = running;

        // ��� �̵� �ִϸ��̼� �ʱ�ȭ
        animator.SetBool(walkHash, false);
        animator.SetBool(runHash, false);

        if (moving)
        {
            if (running)
                animator.SetBool(runHash, true);
            else
                animator.SetBool(walkHash, true);
        }
    }

    // ���� �ִϸ��̼�
    public void TriggerJump()
    {
        if (isDead || isAttacking) return;

        isGrounded = false;
        animator.SetBool(jumpHash, true);
        animator.SetTrigger(jumpHash); // ���� Ʈ���ŵ� ����
    }

    // ���� ó��
    public void OnLanding()
    {
        isGrounded = true;
        animator.SetBool(jumpHash, false);
    }

    // ���� �ִϸ��̼�
    public void TriggerAttack()
    {
        if (isDead || !isGrounded) return;

        isAttacking = true;
        animator.SetTrigger(attackHash);

        // ���� �߿��� �̵� �ִϸ��̼� ����
        animator.SetBool(walkHash, false);
        animator.SetBool(runHash, false);
    }

    // �ǰ� �ִϸ��̼�
    public void TriggerHit()
    {
        if (isDead) return;

        animator.SetTrigger(hitHash);
    }

    // ��� �ִϸ��̼�
    public void TriggerDeath()
    {
        isDead = true;

        // ��� �ٸ� �ִϸ��̼� ����
        animator.SetBool(walkHash, false);
        animator.SetBool(runHash, false);
        animator.SetBool(jumpHash, false);

        animator.SetTrigger(deathHash);
    }

    // ���� ��� �ִϸ��̼�
    public void TriggerDrinkPotion()
    {
        if (isDead || isAttacking) return;

        animator.SetTrigger(drinkPotionHash);
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼����
    public void OnAttackStart()
    {
        isAttacking = true;
    }

    public void OnAttackHit()
    {
        // ������ �����ϴ� ���� - PlayerController���� ������ ó��
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.ProcessAttackHit();
        }
    }

    public void OnAttackEnd()
    {
        isAttacking = false;

        // �̵� ���¿��ٸ� �̵� �ִϸ��̼� ����
        if (isMoving)
        {
            SetMovement(isMoving, isRunning);
        }
    }

    public void OnHitEnd()
    {
        // �ǰ� �ִϸ��̼� ����
    }

    public void OnDeathEnd()
    {
        // ��� �ִϸ��̼� �Ϸ�
        GameManager.Instance?.GameOver();
    }

    public void OnPotionDrinkEnd()
    {
        // ���� �ִϸ��̼� �Ϸ�
    }

    // �ܺο��� �ִϸ��̼� ���� Ȯ�ο�
    public bool IsAttacking => isAttacking;
    public bool IsDead => isDead;
    public bool IsGrounded => isGrounded;

    // �ִϸ��̼� �ӵ� ���� ����
    public void SetAnimationSpeed(float speed)
    {
        animationSpeed = speed;
        if (animator != null)
            animator.speed = speed;
    }

    public void UpdateMovementBlend(float horizontal, bool isRunning)
    {
        float targetSpeed = 0f;

        if (Mathf.Abs(horizontal) > 0.1f)
        {
            targetSpeed = isRunning ? 1f : 0.5f;
        }

        // �ε巯�� �ӵ� ��ȯ
        float currentSpeed = animator.GetFloat("Speed");
        float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        animator.SetFloat("Speed", newSpeed);
    }
}