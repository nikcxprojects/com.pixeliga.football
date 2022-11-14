using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Material mat;

    private void Update()
    {
        mat.mainTextureOffset = speed * Time.time * Vector2.right;
    }
}