using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private int currentHealth;
    private int maxHealth;
    public Slider slider;
    public Text healthText;

    public void Start()
    {
    }

    public void initialize(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        healthText.text = maxHealth.ToString();
    }

    public void takeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        slider.value = currentHealth;
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    public void heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        slider.value = currentHealth;
        healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
