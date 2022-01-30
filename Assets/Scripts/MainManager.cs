using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BlastX");
    }
}
