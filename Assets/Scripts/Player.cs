using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public Projectile laserPrefab;
    public System.Action killed;
    public bool laserActive { get; private set; }

    private void Update()
    {
        Vector3 position = transform.position;

        //el jugador se desplaza hacia la derecha cuando se pulsa la A o la flecha derecha
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= speed * Time.deltaTime;
        }
        //el jugador se desplaza hacia la izquierda cuando se pulsa la D o la flecha izquierda
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            position.x += speed * Time.deltaTime;
        }

        //asigna los limites del jugador por los que se puede desplazar en la pantalla
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        //desplaza al jugador
        transform.position = position;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        //comprueba que solo haya un laser activo al mismo tiempo
        if (!laserActive)
        {
            laserActive = true;

            Projectile laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.destroyed += OnLaserDestroyed;
        }
    }

    private void OnLaserDestroyed(Projectile laser)
    {
        laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            if (killed != null)
            {
                killed.Invoke();
            }
        }
    }

}