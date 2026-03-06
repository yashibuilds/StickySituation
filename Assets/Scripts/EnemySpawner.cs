using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Timing")]
    public float initialDelay = 10f;
    public Vector2 nerdInterval = new Vector2(16f, 24f);
    public Vector2 jockInterval = new Vector2(22f, 32f);
    public Vector2 crushInterval = new Vector2(26f, 38f);

    [Header("Spawn Limits")]
    public int maxActiveNerds = 3;
    public int maxActiveJocks = 2;
    public int maxActiveCrushes = 1;

    [Header("Initial Setup")]
    public int initialExtraEnemies = 2;

    [Header("Spawn Spacing")]
    public float minXSpacingFromAnyEnemy = 5.5f;
    public float minAheadOfPlayer = 5f;
    public float maxAheadOfPlayer = 16f;
    public float edgePadding = 1.5f;
    public int maxSpawnAttempts = 20;

    private GameManager gm;
    private PlayerMovement player;
    private NerdEnemy nerdTemplate;
    private JockEnemy jockTemplate;
    private CrushEnemy crushTemplate;
    private float minSpawnX;
    private float maxSpawnX;
    private bool initialized;

    IEnumerator Start()
    {
        // Wait one frame so all scene objects are alive.
        yield return null;
        InitializeSpawner();
        if (!initialized) yield break;

        SpawnInitialExtraEnemies();

        StartCoroutine(SpawnLoopNerd());
        StartCoroutine(SpawnLoopJock());
        StartCoroutine(SpawnLoopCrush());
    }

    private void InitializeSpawner()
    {
        gm = FindFirstObjectByType<GameManager>();
        player = FindFirstObjectByType<PlayerMovement>();
        nerdTemplate = GetLargestScaleTemplate<NerdEnemy>();
        jockTemplate = GetLargestScaleTemplate<JockEnemy>();
        crushTemplate = GetLargestScaleTemplate<CrushEnemy>();

        if (player == null || (nerdTemplate == null && jockTemplate == null && crushTemplate == null))
        {
            return;
        }

        CalculateSpawnBounds();
        initialized = true;
    }

    private void CalculateSpawnBounds()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        Collider2D[] all = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);

        bool foundAny = false;
        float minX = 0f;
        float maxX = 0f;

        for (int i = 0; i < all.Length; i++)
        {
            Collider2D col = all[i];
            if (col == null || col.gameObject.layer != groundLayer) continue;

            Bounds b = col.bounds;
            if (!foundAny)
            {
                minX = b.min.x;
                maxX = b.max.x;
                foundAny = true;
            }
            else
            {
                if (b.min.x < minX) minX = b.min.x;
                if (b.max.x > maxX) maxX = b.max.x;
            }
        }

        if (!foundAny)
        {
            // Fallback window if ground layer setup changes.
            minX = player.transform.position.x - 10f;
            maxX = player.transform.position.x + 30f;
        }

        minSpawnX = minX + edgePadding;
        maxSpawnX = maxX - edgePadding;
    }

    private IEnumerator SpawnLoopNerd()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            if (CanSpawn() && CountActiveNerds() < maxActiveNerds)
            {
                TrySpawn(nerdTemplate != null ? nerdTemplate.gameObject : null);
            }
            yield return new WaitForSeconds(Random.Range(nerdInterval.x, nerdInterval.y));
        }
    }

    private void SpawnInitialExtraEnemies()
    {
        int spawned = 0;
        int safety = 0;
        while (spawned < initialExtraEnemies && safety < initialExtraEnemies * 10)
        {
            if (TrySpawnAnyType())
            {
                spawned++;
            }
            safety++;
        }
    }

    private bool TrySpawnAnyType()
    {
        List<GameObject> candidates = new List<GameObject>();

        if (nerdTemplate != null && CountActiveNerds() < maxActiveNerds)
        {
            candidates.Add(nerdTemplate.gameObject);
        }
        if (jockTemplate != null && CountActiveJocks() < maxActiveJocks)
        {
            candidates.Add(jockTemplate.gameObject);
        }
        if (crushTemplate != null && CountActiveCrushes() < maxActiveCrushes)
        {
            candidates.Add(crushTemplate.gameObject);
        }

        if (candidates.Count == 0) return false;

        // Randomize type choice so the two extra enemies are not always the same.
        int index = Random.Range(0, candidates.Count);
        return TrySpawn(candidates[index]);
    }

    private IEnumerator SpawnLoopJock()
    {
        yield return new WaitForSeconds(initialDelay + 3f);
        while (true)
        {
            if (CanSpawn() && CountActiveJocks() < maxActiveJocks)
            {
                TrySpawn(jockTemplate != null ? jockTemplate.gameObject : null);
            }
            yield return new WaitForSeconds(Random.Range(jockInterval.x, jockInterval.y));
        }
    }

    private IEnumerator SpawnLoopCrush()
    {
        yield return new WaitForSeconds(initialDelay + 6f);
        while (true)
        {
            if (CanSpawn() && CountActiveCrushes() < maxActiveCrushes)
            {
                TrySpawn(crushTemplate != null ? crushTemplate.gameObject : null);
            }
            yield return new WaitForSeconds(Random.Range(crushInterval.x, crushInterval.y));
        }
    }

    private bool CanSpawn()
    {
        return initialized && player != null && gm != null && !gm.gameOver;
    }

    private bool TrySpawn(GameObject template)
    {
        if (template == null) return false;

        float playerX = player.transform.position.x;
        float desiredMin = Mathf.Max(minSpawnX, playerX + minAheadOfPlayer);
        float desiredMax = Mathf.Min(maxSpawnX, playerX + maxAheadOfPlayer);

        bool hasForwardWindow = desiredMax > desiredMin;
        float fallbackMin = minSpawnX;
        float fallbackMax = maxSpawnX;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            float x = hasForwardWindow
                ? Random.Range(desiredMin, desiredMax)
                : Random.Range(fallbackMin, fallbackMax);

            if (Mathf.Abs(x - playerX) < minAheadOfPlayer) continue;
            if (IsTooCloseToOtherEnemies(x)) continue;

            Vector3 pos = template.transform.position;
            Vector3 spawnPos = new Vector3(x, pos.y, pos.z);
            GameObject spawned = Instantiate(template, spawnPos, template.transform.rotation);
            spawned.transform.localScale = template.transform.lossyScale;

            NerdEnemy spawnedNerd = spawned.GetComponent<NerdEnemy>();
            if (spawnedNerd != null)
            {
                spawnedNerd.ResetForSpawn();
            }

            CrushEnemy spawnedCrush = spawned.GetComponent<CrushEnemy>();
            if (spawnedCrush != null)
            {
                spawnedCrush.ResetForSpawn();
            }
            return true;
        }

        return false;
    }

    private T GetLargestScaleTemplate<T>() where T : MonoBehaviour
    {
        T[] found = FindObjectsByType<T>(FindObjectsSortMode.None);
        if (found == null || found.Length == 0) return null;

        T best = found[0];
        float bestScore = ScaleScore(found[0].transform.localScale);

        for (int i = 1; i < found.Length; i++)
        {
            float score = ScaleScore(found[i].transform.localScale);
            if (score > bestScore)
            {
                best = found[i];
                bestScore = score;
            }
        }

        return best;
    }

    private float ScaleScore(Vector3 scale)
    {
        return Mathf.Abs(scale.x * scale.y * scale.z);
    }

    private bool IsTooCloseToOtherEnemies(float x)
    {
        NerdEnemy[] nerds = FindObjectsByType<NerdEnemy>(FindObjectsSortMode.None);
        for (int i = 0; i < nerds.Length; i++)
        {
            if (Mathf.Abs(nerds[i].transform.position.x - x) < minXSpacingFromAnyEnemy) return true;
        }

        JockEnemy[] jocks = FindObjectsByType<JockEnemy>(FindObjectsSortMode.None);
        for (int i = 0; i < jocks.Length; i++)
        {
            if (Mathf.Abs(jocks[i].transform.position.x - x) < minXSpacingFromAnyEnemy) return true;
        }

        CrushEnemy[] crushes = FindObjectsByType<CrushEnemy>(FindObjectsSortMode.None);
        for (int i = 0; i < crushes.Length; i++)
        {
            if (Mathf.Abs(crushes[i].transform.position.x - x) < minXSpacingFromAnyEnemy) return true;
        }

        return false;
    }

    private int CountActiveNerds()
    {
        return FindObjectsByType<NerdEnemy>(FindObjectsSortMode.None).Length;
    }

    private int CountActiveJocks()
    {
        return FindObjectsByType<JockEnemy>(FindObjectsSortMode.None).Length;
    }

    private int CountActiveCrushes()
    {
        return FindObjectsByType<CrushEnemy>(FindObjectsSortMode.None).Length;
    }
}
