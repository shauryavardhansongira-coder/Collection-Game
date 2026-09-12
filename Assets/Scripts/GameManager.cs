using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;

    [SerializeField] private Player player;

    [SerializeField] private TextMeshProUGUI scoreText;

    private GameObject[] collectibles;
    private GameObject[] counters;

    private void Start()
    {
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        counters = GameObject.FindGameObjectsWithTag("Counter");
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

        foreach (GameObject collectible in collectibles)
        {
            if (!collectible.activeSelf)
            {
                continue;
            }

            Vector2 collectiblePos2D = new Vector2(collectible.transform.position.x, collectible.transform.position.z);
            Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);

            if (Vector2.Distance(collectiblePos2D, playerPos2D) < collectDistance)
            {
                collectible.SetActive(false);

                score++;
                player.IncreaseSpeed(0.5f);

                scoreText.text = "Collected: " + score + "/10";
                if (score == 10)
                {
                    scoreText.text = "Collected: " + score + "/10\nR to Reload";
                }
                collectible.transform.position = GetRespawnPosition(collectible);
                collectible.SetActive(true);
            }
        }
    }

    private Vector3 GetRespawnPosition(GameObject currentCollectible)
    {
        Vector2 currentCollectiblePos2D = new Vector2(currentCollectible.transform.position.x, currentCollectible.transform.position.z);
        Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);

        Vector2 newPosition = currentCollectiblePos2D;

        for (int i = 0; i < 50; i++)
        {
            bool eligible = true;

            newPosition = new Vector2(Random.Range(-7f, 7f), Random.Range(-5f, 5.5f));
            if (Vector2.Distance(newPosition, currentCollectiblePos2D) > 2)
            {
                if (Vector2.Distance(newPosition, playerPos2D) > 2)
                {
                    foreach (GameObject collectible in collectibles)
                    {
                        if (!collectible.activeSelf)
                            continue;

                        Vector2 collectiblePos2D = new Vector2(collectible.transform.position.x, collectible.transform.position.z);

                        if (Vector2.Distance(newPosition, collectiblePos2D) < 1)
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
}
