using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private CharacterController characterController;

    private float horizontal, vertical; //hướng di chuyển

    [SerializeField]
    private float speed = 100f; //tốc độ di chuyển

    private Vector3 movement; //tọa độ của player

    [SerializeField]
    private Animator animator;

    //kiểm tra xem thử PLayer có thực hiện hành động đánh hay không?
    private bool isAttack = false;

    // Trạng thái của nhân vật
    [SerializeField]
    public Damezone dame;

    // Audio components
    [SerializeField]
    private AudioSource audioSource; // Reference to AudioSource
    [SerializeField]
    private AudioClip attackSound; // Reference to Attack Sound clip
    public enum CharacterState
    {
        Normal, Attack

    }

    // Trạng thái hiện tại của Player 

    public CharacterState currentState;
    void Start()
    {
        // Ensure AudioSource is attached
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (isAttack == false)
        {
            isAttack = Input.GetMouseButtonDown(0);
        }
    }
    void FixedUpdate()
    {
        switch (currentState)
        {
            case CharacterState.Normal:
                Calculate();
                break;
            case CharacterState.Attack:
                break;
        }

        Calculate();
        characterController.Move(movement);
    }
    void Calculate()
    {
        if (isAttack == true)
        {
            ChangeState(CharacterState.Attack);
            animator.SetFloat("run", 0);
            isAttack = false;
            return;
        }

        movement.Set(horizontal, 0, vertical);
        movement.Normalize();

        movement = Quaternion.Euler(0, -45, 0) * movement;
        movement *= speed * Time.deltaTime;

        animator.SetFloat("run", movement.magnitude);//magnitude giá trị độ dài của vector

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
    // Hàm thay đổi trạng thái hiện tại của PL
    private void ChangeState(CharacterState newState)
    {
        // clear cache 
        isAttack = false;

        // thoát khỏi state hiện tại 
        switch (currentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attack:
                break;
        }
        // chuyển qua state mới
        switch (newState)
        {
            case CharacterState.Normal:
                //...
                break;
            case CharacterState.Attack:
                animator.SetTrigger("attack");
                //Play attack soud
                if (audioSource != null)
                {
                    audioSource.Play();
                }
                break;
        }

        currentState = newState;
    }
    private void OnDisable()
    {
        horizontal = 0;
        vertical = 0;
        isAttack = false;
    }
    public void EndAttack()
    {
        ChangeState(CharacterState.Normal);
    }
    public void BeginDame()
    {
        dame.beginDame();
    }
    public void EndDame()
    {
        dame.endDame();
    }
}