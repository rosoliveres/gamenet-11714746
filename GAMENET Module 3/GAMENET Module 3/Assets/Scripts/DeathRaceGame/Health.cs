using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float StartingHealth;
    public float CurrentHealth;

    public Image HealthBar;

    public bool IsDead = false;

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = StartingHealth;
        SetHealthBarFillAmount();
    }

    public void SetHealthBarFillAmount()
    {
        HealthBar.fillAmount = CurrentHealth / StartingHealth;
    }
}
