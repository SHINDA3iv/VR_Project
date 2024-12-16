using System.Collections;
using UnityEngine;

public class SnowBlower : MonoBehaviour
{
    public GameObject snowballPrefab; // Префаб снежка
    public Transform firePoint; // Точка, откуда будет вылетать снежок
    public float fireRate = 1f; // Скорость стрельбы (время между выстрелами)
    public float snowballForce = 10f; // Сила выстрела

    private bool canFire = true; // Флаг, разрешающий стрельбу

    void Update()
    {
        // Проверяем, нажата ли кнопка для стрельбы (например, левая кнопка мыши или кнопка на контроллере)
        if (Input.GetButtonDown("Fire1") && canFire)
        {
            FireSnowball();
        }
    }

    void FireSnowball()
    {
        // Создаем снежок на точке выстрела
        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, firePoint.rotation);

        // Применяем силу к снежку
        Rigidbody rb = snowball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * snowballForce, ForceMode.VelocityChange);
        }

        // Запускаем корутину для перезарядки
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        canFire = false; // Запрещаем стрелять
        yield return new WaitForSeconds(fireRate); // Ждем некоторое время, чтобы перезарядиться
        canFire = true; // Разрешаем стрельбу
    }
}
