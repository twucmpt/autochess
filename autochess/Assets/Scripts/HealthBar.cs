using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Entity entity;
    public GameObject healthBarContainer;
    public UnityEngine.UI.Image healthBar;

    void Update() {
        healthBarContainer.SetActive(entity.currentHealth < entity.maxHealth);
        healthBar.fillAmount = ((float)entity.currentHealth)/entity.maxHealth;
    }
}
