using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    public GameObject player;

    [SerializeField]
    private CharacterController characterController1;

    [SerializeField]
    private float horizontal, vertical;

    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    Animator animator1;

    [SerializeField]
    GameObject triggerAttack;

    // biến kiểm tra player có thực hiện đánh ko
    private bool isAttack;

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
        // Ẩn con trỏ chuột
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        characterController1 = GetComponent<CharacterController>();
        animator1 = GetComponent<Animator>();
        triggerAttack.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
    }
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                Caculated();
                break;
            case CharacterState.Attack:
                break;
        }

        if (currentState == CharacterState.Normal)
        {
            Caculated();
            characterController1.Move(moveMent);
        }
        else
        {
            moveMent = Vector3.zero;
            characterController1.Move(moveMent);
        }

    }
    void Caculated()
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

            //ChangeState(CharacterState.Attack);
            //animator1.SetFloat("Run", 0); // Ngưng lại movement để tấn công
            //return; // Kết thúc hẳn
        }

        //moveMent.Set(horizontal, 0, vertical);
        //moveMent.Normalize();

        //moveMent = Quaternion.Euler(0, -45, 0) * moveMent;
        //moveMent *= speed * Time.deltaTime;

        //animator1.SetFloat("run", moveMent.magnitude);

        //if (moveMent != Vector3.zero)
        //{
        //    transform.rotation = Quaternion.LookRotation(moveMent);
        //}

        // Lấy hướng của camera
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Loại bỏ thành phần Y để di chuyển trên mặt phẳng
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Tính toán vector di chuyển dựa trên input và hướng camera
        moveMent = cameraForward * vertical + cameraRight * horizontal;
        moveMent.Normalize();

        // Áp dụng tốc độ
        moveMent *= speed * Time.deltaTime;

        // Cập nhật animator
        animator1.SetFloat("Run", moveMent.magnitude);

        // Xoay nhân vật theo hướng di chuyển (như Elden Ring)
        if (moveMent != Vector3.zero)
        {
            // Sử dụng Slerp để xoay mượt mà
            Quaternion targetRotation = Quaternion.LookRotation(moveMent);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        yield return new WaitForSeconds(0.1f);

        // Bật trigger object
        triggerAttack.SetActive(true);

        // Đợi 0.5 giây
        yield return new WaitForSeconds(0.5f);

        // Tắt trigger object
        triggerAttack.SetActive(false);
    }
}
