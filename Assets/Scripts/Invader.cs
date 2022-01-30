using UnityEngine;

//El atributo RequireComponent agrega automáticamente los componentes necesarios como dependencias.
[RequireComponent(typeof(SpriteRenderer))]
public class Invader : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 0.1f;
    public int animationFrame { get; private set; }
    public int score = 10;
    public System.Action<Invader> killed;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationSprites[0];
    }

    //desactivado
    private void AnimateSprite()
    {
        animationFrame++;
        // si sale del array, vuelve al estado inicial
        if (animationFrame >= animationSprites.Length)
        {
            // animationFrame = 0;
            // return;
        }
        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    //si el invasor es alcanzado por un laser, se invoca a killed
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            // InvokeRepeating(nameof(AnimateSprite), 0, animationTime);
            killed?.Invoke(this);
        }
    }

}