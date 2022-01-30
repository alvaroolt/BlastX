using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;

    public GameObject gameOverUI;
    public Text scoreText;
    public Text livesText;

    public int score { get; private set; }
    public int lives { get; private set; }

    private void Awake()
    {
        //asignacion de variables
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();
    }

    private void Start()
    {
        //actualiza la puntuación y las vidas
        player.killed += OnPlayerKilled;
        mysteryShip.killed += OnMysteryShipKilled;
        invaders.killed += OnInvaderKilled;

        NewGame();
    }

    private void Update()
    {
        //si el jugador se queda sin vidas, reinicia la partida
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }
    }

    //nuevo juego
    private void NewGame()
    {
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    //si mueren todos los invasores, se crea otra ronda
    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    //reaparicion del jugador
    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    //se acaba la partida, oculta los elementos del juego y muestra el menu de Game Over
    private void GameOver()
    {
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
        mysteryShip.gameObject.SetActive(false);
        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].gameObject.SetActive(false);
        }
    }

    //asigna la puntuacion
    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    //asigna las vidas
    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = lives.ToString();
    }

    //algoritmo del jugador al ser impactado por un misil
    private void OnPlayerKilled()
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0)
        {
            Invoke(nameof(NewRound), 1f);
        }
        else
        {
            GameOver();
        }
    }

    //asigna los puntos de los invasores eliminados y genera nuevas rondas si todos son eliminados
    private void OnInvaderKilled(Invader invader)
    {
        SetScore(score + invader.score);

        if (invaders.AmountKilled == invaders.TotalAmount)
        {
            NewRound();
        }
    }

    //asigna los puntos de las naves misteriosas
    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

}