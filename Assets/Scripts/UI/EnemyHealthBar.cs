using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Slider slider;

    private Coroutine currentCoroutine;
    private EnemyController enemy;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    public void GotHit(int health)
    {
        slider.value = health;
    }

    public void Set(EnemyController givenController)
    {
        enemy = givenController;
        slider.maxValue = givenController.enemy.health;
        slider.value = givenController.health;
        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(Follow());
    }

    public void Stop()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        gameObject.SetActive(false);
    }

    private IEnumerator Follow()
    {
        while (true)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(enemy.transform.position);
            transform.position = screenPos + offset;
            yield return null;
        }
    }
}
