using UnityEngine;

public class RadialGravity : MonoBehaviour
{

    public float gravityStrength = 10f;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 direction = Vector2.zero - (Vector2)transform.position;
        rb.AddForce(direction.normalized * gravityStrength);
    }
}
