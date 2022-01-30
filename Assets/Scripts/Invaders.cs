using UnityEngine;

public class Invaders : MonoBehaviour
{
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
    public Vector3 direction { get; private set; } = Vector3.right;
    public Vector3 initialPosition { get; private set; }
    public System.Action<Invader> killed;

    public int AmountKilled { get; private set; }
    public int AmountAlive => TotalAmount - AmountKilled;
    public int TotalAmount => rows * columns;
    public float PercentKilled => (float)AmountKilled / (float)TotalAmount;

    [Header("Grid")]
    public int rows = 5; //filas de invasores
    public int columns = 5; //columnas de invasores

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;

    private void Awake()
    {
        initialPosition = transform.position;

        // Grid de invasores
        for (int i = 0; i < rows; i++)
        {
            float width = 2f * (columns - 1);
            float height = 2f * (rows - 1);

            Vector2 centerOffset = new Vector2(-width * 1f, -height * 1f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2.5f * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                // Crea al invasor y lo emparenta al transform
                Invader invader = Instantiate(prefabs[i], transform);
                invader.killed += OnInvaderKilled;

                // Calcula la posición del invasor en la fila
                Vector3 position = rowPosition;
                position.x += 4f * j;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }

    private void MissileAttack()
    {
        int amountAlive = AmountAlive;

        // Comprueba que no spawneen misiles si no hay invasores vivos
        if (amountAlive == 0)
        {
            return;
        }

        foreach (Transform invader in transform)
        {
            // impide que invasores muertos disparen misiles
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            // Posibilidad aleatoria de generar un misil en función de cuántos invasores hay vivos
            // (cuanto más invasores estén vivos, menor será la posibilidad)
            if (Random.value < (1f / (float)amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void Update()
    {
        // Evalua la velocidad de los invasores según cuántos hayan muerto
        float speed = this.speed.Evaluate(PercentKilled);
        transform.position += direction * speed * Time.deltaTime;

        // transforma el viewport a coordenadas para comprobar cuando los invasores alcanzan el límite de la pantalla
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // los invasores avanzan una fila al alcanzar el limite de la pantalla
        foreach (Transform invader in transform)
        {
            // omite a los invasores muertos
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            // si tocan el borde, avanzan una fila
            if ((direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.5f)) || (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.5f)))
            {
                AdvanceRow();
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        // cambia la direccion de los invasores
        direction = new Vector3(-direction.x, 0f, 0f);

        // mueve a todos los invasores una fila mas abajo
        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;
    }

    //desactiva al invasor y suma 1 a la cuenta de invasores eliminados
    private void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);
        AmountKilled++;
        killed(invader);
    }

    //resetea el spawn de invasores
    public void ResetInvaders()
    {
        AmountKilled = 0;
        direction = Vector3.right;
        transform.position = initialPosition;

        foreach (Transform invader in transform)
        {
            invader.gameObject.SetActive(true);
        }
    }

}