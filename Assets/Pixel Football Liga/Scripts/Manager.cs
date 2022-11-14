using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;
    public static Manager Instance
    {
        get
        {
            if(!instance)
            {
                instance = FindObjectOfType<Manager>();
            }

            return instance;
        }
    }

    int scores;
    int idPlayer;
    int healthCount;
    bool gameStarted;

    const int maxLines = 5;

    const float initOffsetX = 4.0f;
    const float obstacleOffsetX = 6.0f;

    const string savekey = "leaders";

    [Space(10)]
    [SerializeField] float obstaclesSpeed;
    [SerializeField] Transform obstacles;

    [Space(10)]
    [SerializeField] Text timerText;
    [SerializeField] Text scoresText;

    [Space(10)]
    [SerializeField] GameObject menu;
    [SerializeField] GameObject game;
    [SerializeField] GameObject results;

    [Space(10)]
    [SerializeField] GameObject shop;
    [SerializeField] GameObject leaderboard;

    [Space(10)]
    [SerializeField] Transform healthBar;

    [Space(10)]
    [SerializeField] Sprite full;
    [SerializeField] Sprite empty;

    [Space(10)]
    [SerializeField] Text leadersText;

    [Space(10)]
    [SerializeField] Player player;

    [Space(10)]
    [SerializeField] Text playerNameText;
    [SerializeField] Image playerIcon;

    [Space(10)]
    [SerializeField] PlayerData[] playerDatas;

    [Space(10)]
    [SerializeField] Leaderboard_data leaderboard_Data;

    private void Start()
    {
        idPlayer = 0;
        SetPlayer(0);

        LoadResults();

        game.SetActive(false);
        results.SetActive(false);
        shop.SetActive(false);
        leaderboard.SetActive(false);
        menu.SetActive(true);

        player.SetAlive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(game.activeSelf && gameStarted)
            {
                gameStarted = false;
                ResetObstacles();

                player.SetAlive(false);
                game.SetActive(false);
                menu.SetActive(true);

                SaveResult();
                return;
            }
        }

        if(!gameStarted)
        {
            return;
        }

        UpdateObstaclePositions();
    }

    void ResetObstacles()
    {
        foreach(Transform t in obstacles)
        {
            t.localPosition = new Vector2(initOffsetX + (obstacleOffsetX * t.GetSiblingIndex()), GetRandY());
        }
    }

    void LoadResults()
    {
        leaderboard_Data = PlayerPrefs.HasKey(savekey) ? JsonUtility.FromJson<Leaderboard_data>(PlayerPrefs.GetString(savekey)) : new Leaderboard_data();
        var newlist = leaderboard_Data.result.OrderByDescending(i => i);
        leaderboard_Data.result = newlist.ToList();

        leadersText.text = string.Empty;
        for(int i = 0; i < maxLines; i++)
        {
            leadersText.text += string.Format("{0}.  {1}  pts", i + 1, leaderboard_Data.result[i]);
            if (i < maxLines - 1)
            {
                leadersText.text += "\n";
            }
        }
    }

    void SaveResult()
    {
        leaderboard_Data.result.Add(scores);

        var newlist = leaderboard_Data.result.OrderByDescending(i => i);
        leaderboard_Data.result = newlist.ToList();

        string data = JsonUtility.ToJson(leaderboard_Data);
        PlayerPrefs.SetString(savekey, data);
        PlayerPrefs.Save();
        
    }

    float GetRandY()
    {
        return Random.Range(-2.68f, 2.68f);
    }

    void UpdateObstaclePositions()
    {
        foreach(Transform t in obstacles)
        {
            t.Translate(obstaclesSpeed * Time.deltaTime * Vector2.left);
            if(t.position.x < -initOffsetX)
            {
                t.localPosition = new Vector2(obstacleOffsetX + GetLastObstacleSetPos().x, GetRandY());
                t.SetAsLastSibling();
            }
        }
    }

    Vector3 GetLastObstacleSetPos()
    {
        return obstacles.GetChild(obstacles.childCount - 1).localPosition;
    }

    public void StartGame()
    {
        scores = 0;
        UpdateScore(0);

        healthBar.gameObject.SetActive(false);
        scoresText.gameObject.SetActive(false);

        foreach(Transform t in healthBar)
        {
            t.GetComponent<Image>().sprite = full;
        }

        healthCount = healthBar.childCount;

        results.SetActive(false);
        menu.SetActive(false);
        game.SetActive(true);
        StartCoroutine(nameof(Timer));
    }

    public void OpenShop()
    {
        menu.SetActive(false);
        shop.SetActive(true);
    }

    public void OpenLeaderboard()
    {
        LoadResults();

        menu.SetActive(false);
        leaderboard.SetActive(true);
    }

    public void Back()
    {
        if(shop.activeSelf)
        {
            shop.SetActive(false);
            menu.SetActive(true);
            return;
        }
        else if(leaderboard.activeSelf)
        {
            leaderboard.SetActive(false);
            menu.SetActive(true);
            return;
        }
    }

    public void UpdateScore(int amount)
    {
        scores += amount;
        scoresText.text = scores.ToString();
    }

    public void SetPlayer(int dir)
    {
        idPlayer += dir;
        if(idPlayer < 0)
        {
            idPlayer = playerDatas.Length - 1;
        }
        else if(idPlayer > playerDatas.Length - 1)
        {
            idPlayer = 0;
        }

        playerNameText.text = playerDatas[idPlayer].playerName;
        playerIcon.sprite = playerDatas[idPlayer].playerSprite;

        playerIcon.SetNativeSize();
        player.SetSprite(playerDatas[idPlayer].playerSprite);
    }

    public void TakeDamage()
    {
        healthCount--;
        if(healthCount <= 0)
        {
            SaveResult();

            gameStarted = false;
            game.SetActive(false);
            results.SetActive(true);

            player.SetAlive(false);
            ResetObstacles();
        }

        healthBar.GetChild(healthCount).GetComponent<Image>().sprite = empty;
    }

    IEnumerator Timer()
    {
        timerText.gameObject.SetActive(true);
        timerText.text = "GET READY";
        yield return new WaitForSeconds(0.25f);

        int timerCount = 3;

        for(int i = timerCount; i > 0; i--)
        {
            timerText.text = i.ToString();
            yield return new WaitForSeconds(0.25f);
        }

        timerText.text = "GO!";
        yield return new WaitForSeconds(0.25F);
        timerText.gameObject.SetActive(false);

        player.SetAlive(true);
        ResetObstacles();

        healthBar.gameObject.SetActive(true);
        scoresText.gameObject.SetActive(true);

        gameStarted = true;
    }

    [System.Serializable]
    public class Leaderboard_data
    {
        public List<int> result;

        public Leaderboard_data()
        {
            result = new List<int> { 0, 0, 0, 0, 0 };
        }
    }
}
