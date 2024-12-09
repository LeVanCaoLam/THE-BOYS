using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float groundDistance = 0.2f;
    private Vector3 velocity;
    private bool isGrounded;

    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    AudioSource attackSource;
    [SerializeField]
    AudioSource jumpSource;

    [SerializeField]
    private CharacterController characterController1;

    [SerializeField]
    private float horizontal, vertical;

    [SerializeField]
    private float speed = 4f;

    [SerializeField]
    private float turnSmoothTime = 0.2f;

    private float rotationSpeed;

    [SerializeField]
    Animator animator1;

    [SerializeField]
    GameObject triggerAttack;

    // biến kiểm tra player có thực hiện đánh ko
    private bool isAttack;
    // kiểm tra player có bị thương ko
    private bool isHurt = false;

    // biến trạng thái của nhân vật
    public enum CharacterState
    {
        Normal, Attack
    }

    // trạng thái hiện tại của player 
    public CharacterState currentState;

    private Vector3 moveMent;

    // Start is called before the first frame update
    void Start()
    {
        characterController1 = GetComponent<CharacterController>();
        animator1 = GetComponent<Animator>();
        triggerAttack.SetActive(false);
    }

    public void SetHurtState(bool hurt)
    {
        isHurt = hurt;
        if (hurt)
        {
            StopAllActions(); // Ngừng mọi hành động
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ko làm gì nếu bị thương
        if (isHurt) return;

        // Gọi hàm nhảy
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator1.SetBool("isGrounded", true);
        }

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (currentState == CharacterState.Attack)
        {
            horizontal = 0;
            vertical = 0;
            return;
        }

        // kiểm tra có thể đánh ko
        if (!isAttack)
        {
            isAttack = Input.GetMouseButtonDown(0);
        }

        // Kiểm tra có thể đánh không
        if (!isAttack && currentState == CharacterState.Normal)
        {
            // Lấy GameSession
            GameSession gameSession = FindFirstObjectByType<GameSession>();

            // Nếu nhấn chuột và kiểm tra MP
            if (Input.GetMouseButtonDown(0))
            {
                speed = 0f;
                if (gameSession != null && gameSession.CurrentMP > 0)
                {
                    isAttack = true;
                }
                else
                {
                    // Không đủ MP
                    Debug.Log("Không đủ MP để tấn công!");
                }
            }
        }

        JumpHigh();
    }
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                CaculatedMove();
                break;
            case CharacterState.Attack:
                break;
        }

        if (currentState == CharacterState.Normal)
        {
            CaculatedMove();
            characterController1.Move(moveMent);
        }
        else
        {
            moveMent = Vector3.zero;
            characterController1.Move(moveMent);
        }

        // xử lý phần mid_air cho player
        animator1.SetFloat("Jump_Air", velocity.y);
    }

    void JumpHigh()
    {
        // Kiểm tra nếu đang ở trên mặt đất và nhấn phím nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Lấy GameSession
            GameSession gameSession1 = FindFirstObjectByType<GameSession>();

            // Kiểm tra MP đủ để nhảy
            if (gameSession1 != null && gameSession1.CurrentMP > 0)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                animator1.SetTrigger("Jump");

                // Trừ MP sau khi nhảy
                gameSession1.UseMP(5f);

                animator1.SetBool("isGrounded", false);
                jumpSource.PlayOneShot(jumpSource.clip);
                CaculatedMove();
            }
            else
            {
                // Thông báo không đủ MP để nhảy
                Debug.Log("Không đủ MP để nhảy!");
            }
        }

        // Áp dụng trọng lực khi nhân vật không ở trên mặt đất
        velocity.y += gravity * Time.deltaTime;
        characterController1.Move(velocity * Time.deltaTime);
    }

    void CaculatedMove()
    {
        // Lấy GameSession
        GameSession gameSession = FindFirstObjectByType<GameSession>();

        if (isAttack)
        {
            if (gameSession != null)
            {
                if (gameSession.CurrentMP <= 0)
                {
                    // Có thể thêm âm thanh hoặc hiệu ứng báo không đủ MP
                    Debug.Log("Không đủ MP để tấn công!");

                    // Reset trạng thái tấn công
                    isAttack = false;
                    return;
                }

                // Thử bắt đầu tấn công (trừ 20 MP)
                if (gameSession.StartAttack(10f))
                {
                    ChangeState(CharacterState.Attack);
                    animator1.SetFloat("Run", 0); // Ngưng lại movement để tấn công
                    return; // Kết thúc hẳn
                }
            }
        }

        // di chuyển
        Transform cameraTransform = Camera.main.transform;

        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg +
                                                cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                                                  ref rotationSpeed, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            characterController1.Move(moveDirection.normalized * speed * Time.deltaTime);

            // Cập nhật animation chạy
            animator1.SetFloat("Run", moveDirection.magnitude);
        }
        else
        {
            animator1.SetFloat("Run", 0f);
        }
    }

    // Hàm thay đổi trạng thái hiện tại sang trạng thái mong muốn
    private void ChangeState(CharacterState newState)
    {
        // clear cache - dọn dẹp các state cũ (luôn luôn dọn dẹp mỗi khi sang state khác)
        isAttack = false;

        // thoát khỏi state (trạng thái) hiện tại
        switch (currentState)
        {
            case CharacterState.Normal:
                horizontal = 0;
                vertical = 0;
                moveMent = Vector3.zero;
                break;
            case CharacterState.Attack:
                break;
        }

        // chuyển sang state mới
        switch (newState)
        {
            case CharacterState.Normal:
                break;

            case CharacterState.Attack:
                animator1.SetTrigger("Atk1");
                attackSource.PlayOneShot(attackSource.clip);
                StartCoroutine(TriggerAttackCoroutine());
                break;
        }

        // cập nhật state mới thành state hiện tại
        currentState = newState;
    }

    private void OnDisable() // Hàm kiểm tra khoá di chuyển
    {
        horizontal = 0;
        vertical = 0;
        isAttack = false;
    }

    public void EndAttack() // Kết thúc state tấn công, quay về state normal
    {
        ChangeState(CharacterState.Normal);

        // Lấy component GameSession
        GameSession gameSession = FindFirstObjectByType<GameSession>();

        // Trừ MP khi tấn công thành công
        if (gameSession != null)
        {
            gameSession.EndAttack();
        }

        // Chuyển về trạng thái normal
        ChangeState(CharacterState.Normal);
    }

    // Coroutine để điều khiển việc bật tắt trigger
    private IEnumerator TriggerAttackCoroutine()
    {
        // Đợi 1 giây
        yield return new WaitForSeconds(0.2f);

        // Bật trigger object
        triggerAttack.SetActive(true);

        // Đợi 0.5 giây
        yield return new WaitForSeconds(0.15f);

        // Tắt trigger object
        triggerAttack.SetActive(false);
    }
    public void EnableControll()
    {
        isHurt = false; // Cho phép điều khiển lại
    }

    public void StopAllActions()
    {
        StopAllCoroutines(); // Dừng tất cả coroutine
        horizontal = 0;
        vertical = 0;
        isAttack = false;
        currentState = CharacterState.Normal;
    }

}
