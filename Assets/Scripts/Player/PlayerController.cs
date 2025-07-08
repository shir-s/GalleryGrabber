using UnityEngine;
using Spine.Unity;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GameObject frontModel;
    public GameObject backModel;
    public GameObject sideModel;

    private SkeletonAnimation frontAnim;
    private SkeletonAnimation backAnim;
    private SkeletonAnimation sideAnim;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.down; // default facing front
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (frontModel != null) frontAnim = frontModel.GetComponent<SkeletonAnimation>();
        if (backModel != null) backAnim = backModel.GetComponent<SkeletonAnimation>();
        if (sideModel != null) sideAnim = sideModel.GetComponent<SkeletonAnimation>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimationsAndDirection();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(moveX, moveY);

        if (input != Vector2.zero)
            lastMoveDirection = input.normalized;

        moveDirection = input.normalized;
    }

    void HandleAnimationsAndDirection()
    {
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;
        Vector2 dir = isMoving ? moveDirection : lastMoveDirection;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            ShowOnlyModel(sideModel);
            // sideModel.transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
            // sideModel.transform.localScale = new Vector3(moveDirection.x < 0 ? 1 : -1, 1, 1);
            sideModel.transform.localScale = new Vector3(dir.x < 0 ? 1 : -1, 1, 1);
            SetAnimation(sideAnim, isMoving ? "walki" : "idle");
        }
        else
        {
            if (dir.y > 0)
            {
                ShowOnlyModel(backModel);
                SetAnimation(backAnim, isMoving ? "walki" : "idle");
            }
            else
            {
                ShowOnlyModel(frontModel);
                SetAnimation(frontAnim, isMoving ? "walking" : "idle");
            }
        }
    }
    
    void ShowOnlyModel(GameObject modelToShow)
    {
        if (frontModel != null)
        {
            bool active = (modelToShow == frontModel);
            frontModel.SetActive(active);
            SetColliderActive(frontModel, active);
        }
        if (backModel != null)
        {
            bool active = (modelToShow == backModel);
            backModel.SetActive(active);
            SetColliderActive(backModel, active);
        }
        if (sideModel != null)
        {
            bool active = (modelToShow == sideModel);
            sideModel.SetActive(active);
            SetColliderActive(sideModel, active);
        }
    }

    void SetColliderActive(GameObject model, bool active)
    {
        var collider = model.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = active;
    }


    void SetAnimation(SkeletonAnimation anim, string animationName)
    {
        if (anim != null)
        {
            var current = anim.AnimationName;
            if (current != animationName)
            {
                anim.state.SetAnimation(0, animationName, true);
            }
        }
    }
}