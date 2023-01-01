using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceScript : MonoBehaviour
{
    public Text sChronometre;
    public Text sMunitions;
    public Text sScore;
    float chronometre;
    int score;
    int munition;

    private void Awake()
    {
        chronometre = 0;
        score = 0;
        munition = 0;
    }

    private void Update()
    {
        chronometre += Time.deltaTime;

        sChronometre.text = chronometre.ToString("F2");
        sScore.text = score.ToString();
        if (munition == 0) sMunitions.text = "";
        else sMunitions.text = munition.ToString();
    }

    public void AddScore(int bonus)
    {
        score += bonus;
    }

    public void SetMunition(int mun)
    {
        munition = mun;
    }
}
