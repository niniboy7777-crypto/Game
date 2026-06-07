using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int score = 0;
    [SerializeField] private int targetsHit = 0;
    [SerializeField] private int totalTargets = 0;
    [SerializeField] private int throwsRemaining = 10;

    private BallThrower ballThrower;
    private UIManager uiManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        ballThrower = FindObjectOfType<BallThrower>();
        uiManager = FindObjectOfType<UIManager>();

        Target[] targets = FindObjectsOfType<Target>();
        totalTargets = targets.Length;

        UpdateUI();
    }

    public void OnTargetHit(int points)
    {
        score += points;
        targetsHit++;
        UpdateUI();
    }

    public void OnBallThrown()
    {
        throwsRemaining--;
        UpdateUI();

        if (throwsRemaining <= 0)
        {
            EndGame();
        }
    }

    public void OnBallReset()
    {
    }

    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
            uiManager.UpdateThrows(throwsRemaining);
            uiManager.UpdateTargets(targetsHit, totalTargets);
        }
    }

    private void EndGame()
    {
        Debug.Log($"Game Over! Final Score: {score}, Targets Hit: {targetsHit}/{totalTargets}");
        if (uiManager != null)
        {
            uiManager.ShowGameOver(score, targetsHit, totalTargets);
        }
    }

    public void ResetGame()
    {
        score = 0;
        targetsHit = 0;
        throwsRemaining = 10;

        Target[] targets = FindObjectsOfType<Target>();
        foreach (Target target in targets)
        {
            target.Reset();
        }

        UpdateUI();
    }

    public int Score => score;
    public int TargetsHit => targetsHit;
    public int TotalTargets => totalTargets;
    public int ThrowsRemaining => throwsRemaining;
}