using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI remainEnemyText;
    [SerializeField] private TextMeshProUGUI remainMissileText;
    [SerializeField] private UIManager _uiManager;

    private int currentScore = 0;
    private int enemiesRemaining;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "점수: " + currentScore;
        enemiesRemaining = GameObject.Find("Enemy").gameObject.transform.childCount;
        remainEnemyText.text = "남은 적: " + enemiesRemaining ;
        remainMissileText.text = "남은 미사일 수 : " + transform.gameObject.GetComponent<Tank>().missiles.Count ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = "Score: " + currentScore;
    }

    public void DecreaseRemainEnemy()
    {
        enemiesRemaining-- ;
        remainEnemyText.text = "Remain Enemies: " + enemiesRemaining;
        if (enemiesRemaining == 0)
        {
            StartCoroutine(WaitForFinish(2));
        }
    }

    public void DecreaseRemainMissile()
    {
        remainMissileText.text = "남은 미사일 수 : " + transform.gameObject.GetComponent<Tank>().missiles.Count ;
        if (transform.gameObject.GetComponent<Tank>().missiles.Count == 0)
        {
            Debug.Log(transform.gameObject.GetComponent<Tank>().missiles.Count);
            StartCoroutine(WaitForFinish(2));
        }
    }

    private IEnumerator WaitForFinish(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _uiManager.OnGameFinished(currentScore, true);
    }
    
}
