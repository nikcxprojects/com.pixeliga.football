using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] SpriteRenderer myRend;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(force * Vector2.up, ForceMode2D.Impulse);
        }
    }

    public void SetAlive(bool IsAlive)
    {
        rigid.isKinematic = !IsAlive;
        gameObject.SetActive(IsAlive);
        transform.position = Vector3.zero;
    }

    public void SetSprite(Sprite icon)
    {
        myRend.sprite = icon;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Manager.Instance.TakeDamage();
    }
}
