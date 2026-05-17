using UnityEngine;
using UnityEngine.UI;

public class WeaponUIManager : MonoBehaviour
{
    [SerializeField] private PlayerShooterScript playerShooter;
    [SerializeField] private Text weaponText;

    private void Update()
    {
        if (playerShooter == null || weaponText == null) return;

        WeaponData weapon = playerShooter.GetCurrentWeapon();

        if (weapon != null)
        {
            if (weapon.weaponID == "none")
            {
                weaponText.text = "ïêäÌ: Ç»Çµ";
            }
            else
            {
                weaponText.text = "ïêäÌ: " + weapon.weaponName;
            }
        }
    }
}