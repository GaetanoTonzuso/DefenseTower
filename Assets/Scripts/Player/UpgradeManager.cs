using GameDevHQ.FileBase.Gatling_Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private Transform _spawnPos;
    private GameObject _oldWeapon;

    [SerializeField] private GameObject _panelTurret;
    [SerializeField] private GameObject _panelMissile;

    private void OnEnable()
    {
        EventService.Instance.OnUpdateWeapon.AddListener(UpgradeGatling);
        EventService.Instance.OnUpdateWeapon.AddListener(UpgradeMissile);
    }

    private void OnDisable()
    {
        EventService.Instance.OnUpdateWeapon.RemoveListener(UpgradeGatling);
        EventService.Instance.OnUpdateWeapon.RemoveListener(UpgradeMissile);
    }

    [SerializeField] private GameObject _upgradeGatlingPrefab;
    [SerializeField] private int _upgradeGatlingCost = 500;

    [SerializeField] private GameObject _upgradeMissilePrefab;
    [SerializeField] private int _upgradeMissileCost = 850;

    public void UpgradeGatling(Transform spawnPos, GameObject oldWeapon)
    {
        _spawnPos = spawnPos;
        _oldWeapon = oldWeapon;
    }

    public void UpgradeMissile(Transform spawnPos, GameObject oldWeapon)
    {
        _spawnPos = spawnPos;
        _oldWeapon = oldWeapon;
    }

    public void UpgradeGatling()
    {
        if (_spawnPos == null || _oldWeapon == null)
        {
            Debug.LogWarning("UpgradeManager: spawnPos or oldWeapon not set!");
            return;
        }

        if (GameManager.instance.currentWarfunds >= _upgradeGatlingCost)
        {
            GameManager.instance.RemoveWarfunds(_upgradeGatlingCost);
            _oldWeapon.GetComponent<Gatling_Gun>().upgradePanel.SetActive(false);
            GameObject weapon = Instantiate(_upgradeGatlingPrefab, _spawnPos.position, Quaternion.identity);
            weapon.transform.SetParent(_oldWeapon.transform.parent);
            Destroy(_oldWeapon);

            _spawnPos = null;
            _oldWeapon = null;
        }
    }

    public void UpgradeMissile()
    {
        if (_spawnPos == null || _oldWeapon == null)
        {
            Debug.LogWarning("UpgradeManager: spawnPos or oldWeapon not set!");
            return;
        }

        if (GameManager.instance.currentWarfunds >= _upgradeMissileCost)
        {
            GameManager.instance.RemoveWarfunds(_upgradeMissileCost);
            _panelMissile.SetActive(false);
            GameObject weapon = Instantiate(_upgradeMissilePrefab, _spawnPos.position, Quaternion.identity);
            weapon.transform.SetParent(_oldWeapon.transform.parent);
            Destroy(_oldWeapon);

            _spawnPos = null;
            _oldWeapon = null;
        }
    }

    public void CancelAction()
    {
        _panelMissile.SetActive(false);
        _panelTurret.SetActive(false);
    }
}
