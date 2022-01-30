using UnityEngine;

//El atributo RequireComponent agrega automáticamente los componentes necesarios como dependencias.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bunker : MonoBehaviour
{
    //zona de impacto y rotura del bunker
    public Texture2D splat;
    public Texture2D originalTexture { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public new BoxCollider2D collider { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        originalTexture = spriteRenderer.sprite.texture;

        ResetBunker();
    }

    public void ResetBunker()
    {
        //cada bunker necesita la instancia del sprite ya que se modificará
        CopyTexture(originalTexture);

        gameObject.SetActive(true);
    }

    private void CopyTexture(Texture2D source)
    {
        Texture2D copy = new Texture2D(source.width, source.height, source.format, false);
        copy.filterMode = source.filterMode;
        copy.anisoLevel = source.anisoLevel;
        copy.wrapMode = source.wrapMode;
        copy.SetPixels(source.GetPixels());
        copy.Apply();

        Sprite sprite = Sprite.Create(copy, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);
        spriteRenderer.sprite = sprite;
    }

    public bool CheckPoint(Vector3 hitPoint, out int px, out int py)
    {
        //transforma el punto del espacio en espacio local
        Vector3 localPoint = transform.InverseTransformPoint(hitPoint);

        //desplaza el punto a la esquina del objeto en lugar del centro, para usarlo como coordenadas
        localPoint.x += collider.size.x / 2;
        localPoint.y += collider.size.y / 2;

        Texture2D texture = spriteRenderer.sprite.texture;

        //transforma el punto local a coordenadas
        px = (int)((localPoint.x / collider.size.x) * texture.width);
        py = (int)((localPoint.y / collider.size.y) * texture.height);

        //devuelve true si el pixel no esta vacio
        return texture.GetPixel(px, py).a != 0f;
    }

    public bool Splat(Vector3 hitPoint)
    {
        int px;
        int py;

        //entra en el if si el punto toca un pixel no vacio
        if (!CheckPoint(hitPoint, out px, out py))
        {
            return false;
        }

        Texture2D texture = spriteRenderer.sprite.texture;

        //desplaza el punto a la mitad del tamaño del splat, para que el splat "golpee" en el centro del golpe
        px -= splat.width / 2;
        py -= splat.height / 2;

        int startX = px;

        //recorre todas las coordenadas de la textura splat para sobreponerla a la del bunker
        for (int y = 0; y < splat.height; y++)
        {
            px = startX;

            for (int x = 0; x < splat.width; x++)
            {
                //multiplica el pixel del splat con el del bunker para que parezca que se ha destruido
                Color pixel = texture.GetPixel(px, py);
                pixel.a *= splat.GetPixel(x, y).a;
                texture.SetPixel(px, py, pixel);
                px++;
            }

            py++;
        }

        texture.Apply();

        return true;
    }

    public bool CheckCollision(BoxCollider2D other, Vector3 hitPoint)
    {
        Vector2 offset = other.size / 2;

        // Check the hit point and each edge of the colliding object to see if
        // it splats with the bunker for more accurate collision detection
        return Splat(hitPoint) ||
               Splat(hitPoint + (Vector3.down * offset.y)) ||
               Splat(hitPoint + (Vector3.up * offset.y)) ||
               Splat(hitPoint + (Vector3.left * offset.x)) ||
               Splat(hitPoint + (Vector3.right * offset.x));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            gameObject.SetActive(false);
        }
    }

}