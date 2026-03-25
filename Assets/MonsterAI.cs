using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public float moveSpeed = 3f;              // Base speed
    public float speedIncreaseRate = 0.2f;    // How fast it increases
    public float maxMoveSpeed = 6f;           // Hard cap so game stays fair
    public GameObject gameOverUI;
    private Transform player;

    private AudioSource monsterSound;
    public float soundRange = 6f;
    public AudioClip heartBeatSound;
    private AudioSource heartBeatSource;      // second sound source
    public float heartBeatRange = 3f;         // distance for heartbeat
    public float maxVolume = 1f;
    public float minHeartPitch = 1f;
    public float maxHeartPitch = 2f;
    public float visionRange = 10f;
    public LayerMask obstacleLayer;

    public float patrolRadius = 3f;
    public float patrolPauseTime = 1f;
    public float patrolPointReachDistance = 0.2f;

    Vector3 lastKnownPlayerPosition;
    bool hasLastKnownPosition = false;
    Vector3 patrolCenter;
    Vector3 currentPatrolTarget;
    bool hasPatrolTarget = false;
    float patrolPauseTimer = 0f;

    float playerLookupTimer = 0f;
    const float playerLookupInterval = 0.5f;

    void Start()
    {
        TryFindPlayer();

        patrolCenter = transform.position;
        ChooseNewPatrolTarget();

        monsterSound = GetComponent<AudioSource>();
        if (monsterSound != null)
        {
            monsterSound.loop = true;
            monsterSound.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("MonsterAI requires an AudioSource component for monsterSound.");
        }

        if (heartBeatSound != null)
        {
            heartBeatSource = gameObject.AddComponent<AudioSource>();
            heartBeatSource.volume = 0.8f;
            heartBeatSource.clip = heartBeatSound;
            heartBeatSource.loop = true;
            heartBeatSource.playOnAwake = false;
        }
        else
        {
            Debug.LogWarning("MonsterAI heartBeatSound is not assigned.");
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            playerLookupTimer -= Time.fixedDeltaTime;
            if (playerLookupTimer <= 0f)
            {
                TryFindPlayer();
                playerLookupTimer = playerLookupInterval;
            }

            Patrol();
            return;
        }

        // Increase speed gradually
        moveSpeed += speedIncreaseRate * Time.fixedDeltaTime;
        moveSpeed = Mathf.Min(moveSpeed, maxMoveSpeed);

        Vector2 direction = player.position - transform.position;
        float distanceToPlayer = direction.magnitude;
        bool canSeePlayer = CanSeePlayer(direction, distanceToPlayer);

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            hasLastKnownPosition = true;
            MoveTowards(player.position);
        }
        else if (hasLastKnownPosition)
        {
            MoveTowards(lastKnownPlayerPosition);

            float distanceToLast = Vector2.Distance(transform.position, lastKnownPlayerPosition);
            if (distanceToLast < 0.3f)
            {
                hasLastKnownPosition = false;
                patrolPauseTimer = patrolPauseTime;
            }
        }
        else
        {
            Patrol();
        }

        UpdateAudio(distanceToPlayer);
    }

    bool CanSeePlayer(Vector2 direction, float distanceToPlayer)
    {
        if (distanceToPlayer > visionRange)
        {
            return false;
        }

        int detectionMask = obstacleLayer | (1 << player.gameObject.layer);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distanceToPlayer, detectionMask);

        if (hit.collider == null)
        {
            return false;
        }

        return hit.collider.CompareTag("Player");
    }

    void TryFindPlayer()
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
    }

    void Patrol()
    {
        if (patrolPauseTimer > 0f)
        {
            patrolPauseTimer -= Time.fixedDeltaTime;
            return;
        }

        if (!hasPatrolTarget || Vector2.Distance(transform.position, currentPatrolTarget) <= patrolPointReachDistance)
        {
            ChooseNewPatrolTarget();
            patrolPauseTimer = patrolPauseTime;
            return;
        }

        MoveTowards(currentPatrolTarget);
    }

    void ChooseNewPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        currentPatrolTarget = patrolCenter + new Vector3(randomOffset.x, randomOffset.y, 0f);
        hasPatrolTarget = true;
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector2 moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)moveDirection * moveSpeed * Time.fixedDeltaTime;
    }

    void UpdateAudio(float distance)
    {
        if (monsterSound != null && distance < soundRange)
        {
            if (!monsterSound.isPlaying)
                monsterSound.Play();

            float volume = 1 - (distance / soundRange);
            monsterSound.volume = volume * maxVolume;
        }
        else if (monsterSound != null)
        {
            monsterSound.Stop();
        }

        if (heartBeatSource != null && distance < heartBeatRange)
        {
            if (!heartBeatSource.isPlaying)
                heartBeatSource.Play();

            float t = 1 - (distance / heartBeatRange);
            heartBeatSource.pitch = Mathf.Lerp(minHeartPitch, maxHeartPitch, t);
        }
        else if (heartBeatSource != null && heartBeatSource.isPlaying)
        {
            heartBeatSource.Stop();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0f;

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
        }
    }
}