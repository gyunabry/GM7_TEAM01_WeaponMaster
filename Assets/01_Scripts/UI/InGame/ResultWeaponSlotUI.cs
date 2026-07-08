using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultWeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponLevelText;
    [SerializeField] private TMP_Text weaponDamageText;

    public void Set(ResultWeaponData weaponData)
    {
        if (weaponData == null)
        {
            Clear();
            return;
        }

        if (weaponIcon != null)
        {
            weaponIcon.sprite = weaponData.icon;
            weaponIcon.enabled = weaponData.icon != null;
        }

        SetText(weaponLevelText, $"Lv. {weaponData.level}");
        SetText(weaponDamageText, weaponData.damage.ToString("N0"));
    }

    public void Clear()
    {
        if (weaponIcon != null)
        {
            weaponIcon.sprite = null;
            weaponIcon.enabled = false;
        }

        SetText(weaponLevelText, string.Empty);
        SetText(weaponDamageText, string.Empty);
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }
}
