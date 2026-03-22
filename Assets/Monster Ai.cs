using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public float moveSpeed = 3f;              // Base speed
    public float speedIncreaseRate = 0.2f;    // How fast it increases
    public GameObject gameOverUI;
    private Transform player;

     private AudioSource monsterSound;
     public float soundRange = 6f;
     public AudioClip heartBeatSound; 
     private AudioSource heartBeatSource;  // second sound source
    public float heartBeatRange = 3f;    // distance for heartbeat
    public float maxVolume = 1f; 
    public float minHeartPitch = 1f;
    public float maxHeartPitch = 2f;
    public float visionRange = 10f;
    public LayerMask obstacleLayer;
    Vector3 lastKnownPlayerPosition;
    bool hasLastKnownPosition = false;
    
            // max monster sound volume
         void Start()
    {
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
        {
            player = foundPlayer.transform;
        }
        monsterSound = GetComponent<AudioSource>();
       monsterSound.loop = true;
    monsterSound.playOnAwake = false;

    heartBeatSource = gameObject.AddComponent<AudioSource>();
    heartBeatSource.volume = 0.8f;
    heartBeatSource.clip = heartBeatSound;
    heartBeatSource.loop = true;
    heartBeatSource.playOnAwake = false;

    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Increase speed gradually
        moveSpeed += speedIncreaseRate * Time.fixedDeltaTime;

        // Move toward player
        Vector2 direction = player.position - transform.position;
        float distanceToPlayer = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, visionRange, obstacleLayer);

        bool canSeePlayer = false;

         if (hit.collider != null && hit.collider.CompareTag("Player") && distanceToPlayer <= visionRange)
          {
            canSeePlayer = true;
          }
     if (canSeePlayer)
{
    lastKnownPlayerPosition = player.position;
    hasLastKnownPosition = true;

    Vector2 moveDirection = direction.normalized;
    transform.position += (Vector3)moveDirection * moveSpeed * Time.fixedDeltaTime;
}
else if (hasLastKnownPosition)
{
    Vector2 targetDirection = ((Vector2)lastKnownPlayerPosition - (Vector2)transform.position).normalized;

    transform.position += (Vector3)targetDirection * moveSpeed * Time.fixedDeltaTime;

    float distanceToLast = Vector2.Distance(transform.position, lastKnownPlayerPosition);

    if (distanceToLast < 0.3f)
    {
        hasLastKnownPosition = false;
    }
}
         
        float distance = Vector2.Distance(transform.position, player.position);

if (distance < soundRange)
{
    if (!monsterSound.isPlaying)
        monsterSound.Play();

    float volume = 1 - (distance / soundRange);
    monsterSound.volume = volume * maxVolume;
}
else
{
    monsterSound.Stop();
}

if (distance < heartBeatRange)
{
    if (!heartBeatSource.isPlaying)
        heartBeatSource.Play();

    float t = 1 - (distance / heartBeatRange);
    heartBeatSource.pitch = Mathf.Lerp(minHeartPitch, maxHeartPitch, t);
}
else
{
    if (heartBeatSource.isPlaying)
    {
        heartBeatSource.Stop();
    }
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