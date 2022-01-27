using UnityEngine;

public class Bunker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (
            other.gameObject.layer == LayerMask.NameToLayer("Invader")
            || other.gameObject.layer == LayerMask.NameToLayer("Missile")
        )
        {
            this.gameObject.SetActive(false);
        }
    }

    void Start() { }

    void Update() { }
}
