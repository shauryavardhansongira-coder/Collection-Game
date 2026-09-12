using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private int score;

    [SerializeField] private TextMeshProUGUI scoreText;

    private GameObject[] collectibles;
    private GameObject[] counters;

    private bool isWalking;
    private Vector3 lastInteractDir;

    private void Start()
    {
        collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        counters = GameObject.FindGameObjectsWithTag("Counter");
    }
    private void Update()
    {
        HandleMovement();
        HandleCollecting();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
    public bool IsWalking()
    {
        return isWalking;
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
            Vector2 playerPos2D = new Vector2(transform.position.x, transform.position.z);

            if (Vector2.Distance(collectiblePos2D, playerPos2D) < collectDistance)
            {
                collectible.SetActive(false);

                score++;
                moveSpeed += 0.5f;

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
        Vector2 playerPos2D = new Vector2(transform.position.x, transform.position.z);

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
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance))
        {
            print(raycastHit.transform);
        }
        else
        {
            print("-");
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + new Vector3(0, playerHeight, 0), playerRadius, moveDir, moveDistance);

        if (canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        else
        {
            //Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;

            canMove = !Physics.CapsuleCast(transform.position, transform.position + new Vector3(0, playerHeight, 0), playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                transform.position += moveDirX * moveDistance;
            }
            else        //cannot move in only X, attempt only Z movement
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;

                canMove = !Physics.CapsuleCast(transform.position, transform.position + new Vector3(0, playerHeight, 0), playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    transform.position += moveDirZ * moveDistance;
                }
            }
        }

        isWalking = (moveDir != Vector3.zero);

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
