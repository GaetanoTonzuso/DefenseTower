using GameDevHQ.FileBase.Gatling_Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    // Position where the upgraded weapon will be spawned
    private Transform _spawnPos;

    // Reference to the weapon being upgraded
    private GameObject _oldWeapon;

    [SerializeField] private GameObject _panelTurret;   // UI panel for turret upgrade options
    [SerializeField] private GameObject _panelMissile;  // UI panel for missile upgrade options

    private void OnEnable()
    {
        // Listen for weapon upgrade requests
        EventService.Instance.OnUpdateWeapon.AddListener(UpgradeGatling);
        EventService.Instance.OnUpdateWeapon.AddListener(UpgradeMissile);
    }

    private void OnDisable()
    {
        // Remove listeners to avoid memory leaks
        EventService.Instance.OnUpdateWeapon.RemoveListener(UpgradeGatling);
        EventService.Instance.OnUpdateWeapon.RemoveListener(UpgradeMissile);
    }

    // Gatling upgrade settings
    [SerializeField] private GameObject _upgradeGatlingPrefab;  // Prefab of upgraded Gatling gun
    [SerializeField] private int _upgradeGatlingCost = 500;     // Price for upgrading Gatling gun

    // Missile upgrade settings
    [SerializeField] private GameObject _upgradeMissilePrefab;  // Prefab of upgraded missile launcher
    [SerializeField] private int _upgradeMissileCost = 850;     // Price for upgrading missile launcher

    // Called when a Gatling upgrade request is received (sets the target weapon info)
    public void UpgradeGatling(Transform spawnPos, GameObject oldWeapon)
    {
        _spawnPos = spawnPos;
        _oldWeapon = oldWeapon;
    }

    // Called when a Missile upgrade request is received (sets the target weapon info)
    public void UpgradeMissile(Transform spawnPos, GameObject oldWeapon)
    {
        _spawnPos = spawnPos;
        _oldWeapon = oldWeapon;
    }

    // Executes Gatling upgrade (UI button)
    public void UpgradeGatling()
    {
        // Safety check to ensure a weapon is selected
        if (_spawnPos == null || _oldWeapon == null)
        {
            Debug.LogWarning("UpgradeManager: spawnPos or oldWeapon not set!");
            return;
        }

        // Verify player has enough warfunds
        if (GameManager.instance.currentWarfunds >= _upgradeGatlingCost)
        {
            // Deduct currency
            GameManager.instance.RemoveWarfunds(_upgradeGatlingCost);

            // Hide upgrade panel on original weapon
            _oldWeapon.GetComponent<Gatling_Gun>().upgradePanel.SetActive(false);

            // Spawn upgraded weapon
            GameObject weapon = Instantiate(_upgradeGatlingPrefab, _spawnPos.position, Quaternion.identity);

            // Place new weapon under same parent as old weapon
            weapon.transform.SetParent(_oldWeapon.transform.parent);

            // Remove old weapon
            Destroy(_oldWeapon);

            // Reset stored values
            _spawnPos = null;
            _oldWeapon = null;
        }
    }

    // Executes Missile upgrade (UI button)
    public void UpgradeMissile()
    {
        // Ensure weapon info is available
        if (_spawnPos == null || _oldWeapon == null)
        {
            Debug.LogWarning("UpgradeManager: spawnPos or oldWeapon not set!");
            return;
        }

        // Check currency balance
        if (GameManager.instance.currentWarfunds >= _upgradeMissileCost)
        {
            GameManager.instance.RemoveWarfunds(_upgradeMissileCost);

            // Close missile upgrade UI panel
            _panelMissile.SetActive(false);

            // Spawn upgraded missile weapon
            GameObject weapon = Instantiate(_upgradeMissilePrefab, _spawnPos.position, Quaternion.identity);
            weapon.transform.SetParent(_oldWeapon.transform.parent);

            // Destroy previous weapon
            Destroy(_oldWeapon);

            // Reset stored info
            _spawnPos = null;
            _oldWeapon = null;
        }
    }

    // Called when player closes the upgrade UI panel without upgrading
    public void CancelAction()
    {
        _panelMissile.SetActive(false);
        _panelTurret.SetActive(false);
    }
}
