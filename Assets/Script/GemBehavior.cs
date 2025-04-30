using UnityEngine;

public class GemBehavior : MonoBehaviour
{
    public Vector2 boxSize = new Vector2(1f, 1.5f);
    public LayerMask stoneLayer;
    public float flySpeed = 2f;
    private bool isFlying = false;
    private string gemName;

    void Start()
    {
        gemName = gameObject.name.Replace("(Clone)", "").Trim();
    }
    void Update()
    {
        if (!IsGemSupported())
        {
            if (!isFlying)
                isFlying = true;
        }

        if (isFlying)
            FlyUp();
    }

    bool IsGemSupported()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, stoneLayer);
        return colliders.Length > 0;
    }

    void FlyUp()
    {
        transform.position += Vector3.up * flySpeed * Time.deltaTime;

        if (transform.position.y > 15f)
        {
            GemCountUI.Instance.OnGemDestroyed(gemName);
            Destroy(gameObject); 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0f));
    }
}
