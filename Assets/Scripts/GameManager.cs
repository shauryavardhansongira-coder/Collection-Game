using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    static int highscore;
    [SerializeField] private int score;
    private int lives = 3;

    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI scoreText;

    private GameObject[] collectibles;
    private GameObject[] counters;
    private GameObject[] bombs;
    private GameObject[] allSpawnables;

    private void Start()
    {
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        bombs = GameObject.FindGameObjectsWithTag("Bomb");

        List<GameObject> spawnablesList = new List<GameObject>();
        spawnablesList.AddRange(collectibles);
        spawnablesList.AddRange(bombs);
        allSpawnables = spawnablesList.ToArray();

        counters = GameObject.FindGameObjectsWithTag("Counter");

        scoreText.text = "Highscore: " + highscore + "\nScore: " + score + "\nLives: " + lives;
    }

    private void Update()
    {
        HandleCollecting();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void HandleCollecting()
    {
        float collectDistance = .8f;

        Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);

        foreach (GameObject collectible in collectibles)
        {
            if (!collectible.activeSelf)
            {
                continue;
            }

            Vector2 collectiblePos2D = new Vector2(collectible.transform.position.x, collectible.transform.position.z);

            if (Vector2.Distance(collectiblePos2D, playerPos2D) < collectDistance)
            {
                collectible.SetActive(false);

                score++;
                player.IncreaseSpeed(0.5f);

                scoreText.text = "Highscore: " + highscore + "\nScore: " + score + "\nLives: " + lives;

                collectible.transform.position = GetRespawnPosition(collectible);
                collectible.SetActive(true);
            }
        }

        foreach (GameObject bomb in bombs)
        {
            if (!bomb.activeSelf)
            {
                continue;
            }

            Vector2 bombPos2D = new Vector2(bomb.transform.position.x, bomb.transform.position.z);

            if (Vector2.Distance(bombPos2D, playerPos2D) < collectDistance)
            {
                bomb.SetActive(false);

                lives--;
                player.IncreaseSpeed(-2f);

                scoreText.text = "Highscore: " + highscore + "\nScore: " + score + "\nLives: " + lives;

                bomb.transform.position = GetRespawnPosition(bomb);
                bomb.SetActive(true);
                if (lives == 0)
                    die();
            }
        }
    }

    private Vector3 GetRespawnPosition(GameObject currentSpawnable)
    {
        Vector2 currentSpawnablePos2D = new Vector2(currentSpawnable.transform.position.x, currentSpawnable.transform.position.z);
        Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);

        Vector2 newPosition = currentSpawnablePos2D;

        for (int i = 0; i < 50; i++)
        {
            bool eligible = true;

            newPosition = new Vector2(Random.Range(-7f, 7f), Random.Range(-5f, 5.5f));
            if (Vector2.Distance(newPosition, currentSpawnablePos2D) > 2)
            {
                if (Vector2.Distance(newPosition, playerPos2D) > 2)
                {
                    foreach (GameObject spawnable in allSpawnables)
                    {
                        if (!spawnable.activeSelf)
                            continue;

                        Vector2 spawnablePos2D = new Vector2(spawnable.transform.position.x, spawnable.transform.position.z);

                        if (Vector2.Distance(newPosition, spawnablePos2D) < 1)
                        {
                            eligible = false;
                            break;
                        }
                    }
                    if (eligible)
                    {
                        foreach (GameObject counter in counters)
                        {
                            if (!counter.activeSelf)
                                continue;

                            Vector2 counterPos2D = new Vector2(counter.transform.position.x, counter.transform.position.z);

                            if (Vector2.Distance(newPosition, counterPos2D) < 1.5)
                            {
                                eligible = false;
                                break;
                            }
                        }
                    }
                    if (eligible)
                        break;
                }
            }
        }
        return new Vector3(newPosition.x, 1f, newPosition.y);
    }

    private void die()
    {
        if (score > highscore)
        {
            highscore = score;
        }
        SceneManager.LoadScene(0);
    }
}
