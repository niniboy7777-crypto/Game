using UnityEngine;

public class BallThrower : MonoBehaviour
{
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private float touchStartTime;
    private float touchEndTime;

    private Rigidbody rb;
    private bool isThrown = false;

    [Header("Throw Settings")]
    [Tooltip("How much force is applied based on swipe speed.")]
    public float throwForceMultiplier = 0.05f;
    [Tooltip("Adds an upward lift to the ball so it arcs nicely.")]
    public float upwardForceMultiplier = 1.5f; 

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (isThrown) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                touchStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEndPos = touch.position;
                touchEndTime = Time.time;
                
                if (touchEndPos.y > touchStartPos.y)
                {
                    ThrowBall();
                }
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Input.mousePosition;
            touchStartTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            touchEndPos = Input.mousePosition;
            touchEndTime = Time.time;

            if (touchEndPos.y > touchStartPos.y)
            {
                ThrowBall();
            }
        }
    }

    void ThrowBall()
    {
        isThrown = true;
        rb.useGravity = true;

        float swipeDuration = touchEndTime - touchStartTime;
        float swipeDistY = touchEndPos.y - touchStartPos.y;
        float swipeDistX = touchEndPos.x - touchStartPos.x;

        if (swipeDuration <= 0) swipeDuration = 0.1f;

        float forceZ = (swipeDistY / swipeDuration) * throwForceMultiplier;
        float forceX = (swipeDistX / swipeDuration) * throwForceMultiplier;
        float forceY = forceZ * upwardForceMultiplier;

        forceZ = Mathf.Clamp(forceZ, 5f, 25f);
        forceX = Mathf.Clamp(forceX, -10f, 10f);
        forceY = Mathf.Clamp(forceY, 5f, 20f);

        Vector3 throwVector = new Vector3(forceX, forceY, forceZ);
        rb.AddForce(throwVector, ForceMode.Impulse);

        GameManager.Instance.OnBallThrown();
        Invoke("ResetBall", 5.0f);
    }

    void ResetBall()
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        isThrown = false;

        GameManager.Instance.OnBallReset();
    }

    public bool IsThrown => isThrown;
}