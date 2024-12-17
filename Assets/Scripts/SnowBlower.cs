using System.Collections;
using UnityEngine;

public class SnowBlower : MonoBehaviour
{
    public GameObject snowballPrefab; // Префаб снежка
    public Transform firePoint; // Точка, откуда будет вылетать снежок
    public float fireRate = 1f; // Скорость стрельбы (время между выстрелами)
    public float snowballForce = 10f; // Сила выстрела

    private bool canFire = true; // Флаг, разрешающий стрельбу
    public int maxAmmo = 10;

    private int currentAmmo;
    private bool isReloading;

    void Start()
    {
        currentAmmo = maxAmmo;
        StartCoroutine(HandleShooting());
    }

    private IEnumerator HandleShooting()
    {
        while (true)
        {
            if (Input.GetButton("Fire1") && canFire)
            {
                FireSnowball();
                currentAmmo--;
                UIManager.Instance.UpdateAmmoUI(currentAmmo, maxAmmo);

                if (currentAmmo <= 0)
                {
                    StartCoroutine(Reload());
                }

                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void FireSnowball()
    {
        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = snowball.GetComponent<Rigidbody>();
        if (rb != null) rb.AddForce(firePoint.forward * 10f, ForceMode.Impulse);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        UIManager.Instance.ShowReloadText(true);
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        UIManager.Instance.ShowReloadText(false);
        UIManager.Instance.UpdateAmmoUI(currentAmmo, maxAmmo);
        isReloading = false;
    }
}
