using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    List<GameObject> _ballsList = new List<GameObject>();

    public GameObject ballPrefab;
    public GameObject playerPrefab;
    public Text scoreText;
    public Text ballsText;
    public Text levelText;
    public Text highscoreText;
    public Text highscoreTextGameOver;
    public Text scoreTextGameOver;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;

    public GameObject[] levels;

    public int _ballsInPlay = 0;

    public static GameManager Instance {  get; private set; }

    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER}
    State _state;
    GameObject _currentBall;
    GameObject _extraBalls;
    GameObject _currentLevel;
    bool _isSwitchingState;

    //Properties för spelet
    private int _score;

    public int Score
    {
        get { return _score; }
        set { _score = value; scoreText.text = "SCORE: " + _score; }
    }

    private int _level;

    public int Level
    {
        get { return _level; }
        set { _level = value; levelText.text = "LEVEL: " + (_level + 1); }
    }

    private int _balls;

    public int Balls
    {
        get { return _balls; }
        set { _balls = value; ballsText.text = "BALLS: " + _balls;  }
    }




    public void PlayClicked()
    {
        SwitchState(State.INIT);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchState(State.MENU);
        Instance = this;
    }

    public void SwitchState(State newState, float delay = 0)
    {
        StartCoroutine(SwitchDelay(newState, delay));
    }

    IEnumerator SwitchDelay(State newState, float delay)
    {
        _isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        _state = newState;
        BeginState(newState);
        _isSwitchingState= false;
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MENU:
                Cursor.visible = true;
                highscoreText.text = "HIGHSCORE: " + PlayerPrefs.GetInt("highscore");
                panelMenu.SetActive(true);
                break;
            case State.INIT:
                Cursor.visible = false;
                panelPlay.SetActive(true);
                Score = 0;
                Level = 0;
                Balls = 3;
                if (_currentLevel != null)
                {
                    Destroy(_currentLevel);
                }
                Instantiate(playerPrefab);
                SwitchState(State.LOADLEVEL);
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                Destroy(_currentBall);
                foreach (GameObject ball in _ballsList)
                {
                    Destroy(ball);
                    //_ballsList.Remove(ball);
                }
                Destroy(_currentLevel);
                Balls++;
                _ballsInPlay = 0;
                Level++;
                panelLevelCompleted.SetActive(true);
                SwitchState(State.LOADLEVEL, 2f);
                break;
            case State.LOADLEVEL:
                if (Level >= levels.Length)
                {
                    SwitchState(State.GAMEOVER);
                }
                else
                {
                    _currentLevel = Instantiate(levels[Level]);
                    SwitchState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                if (Score > PlayerPrefs.GetInt("highscore"))
                {
                    PlayerPrefs.SetInt("highscore", Score);
                }
                scoreTextGameOver.text = "SCORE: " + Score;
                highscoreTextGameOver.text = "HIGHSCORE: " + PlayerPrefs.GetInt("highscore");
                panelGameOver.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case State.MENU:
                break;
            case State.INIT:
                break;
            case State.PLAY:
                if (_currentBall == null)
                {
                    if (Balls > 0 && _ballsInPlay == 0)
                    {
                        _currentBall = Instantiate(ballPrefab);
                        _ballsInPlay++;
                        Balls--;
                    }
                    else if (Balls == 0 && _ballsInPlay == 0)
                    {
                        SwitchState(State.GAMEOVER);
                    }

                }

                if (_currentLevel != null && _currentLevel.transform.childCount == 0 && !_isSwitchingState)
                {
                    SwitchState(State.LEVELCOMPLETED);
                }
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                if (Input.anyKeyDown)
                {
                    SwitchState(State.MENU);
                }
                break;
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case State.MENU:
                panelMenu.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                panelLevelCompleted.SetActive(false);
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                break;
        }
        
    }
    
    public void AddBall()
    {
        if (_ballsInPlay >= 10)
        {
            return;
        }
        _extraBalls = Instantiate(ballPrefab);
        _ballsList.Add(_extraBalls);
        _ballsInPlay++;
    }
}