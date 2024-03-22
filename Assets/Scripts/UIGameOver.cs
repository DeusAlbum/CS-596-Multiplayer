using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    private Canvas _gameOverCanvas;

    //Get canvas for game over
    private void Start()
    {
        _gameOverCanvas = GetComponent<Canvas>();
    }

    //Make GameOverEvent happen
    private void OnEnable()
    {
        PlayerController.GameOverEvent += GameOver;
    }

    //Disable gameover
    private void OnDisable()
    {
        PlayerController.GameOverEvent -= GameOver;
    }

    //Enable game over canvas
    private void GameOver()
    {
        _gameOverCanvas.enabled = true;
    }
}
