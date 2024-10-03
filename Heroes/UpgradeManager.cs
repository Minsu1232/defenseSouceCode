using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeSystem upgradeSystem;

    public void OnWarriorUpgradeButtonClicked()
    {
        upgradeSystem.Upgrade(CharacterData.AttackType.Warrior);
    }

    public void OnArcherUpgradeButtonClicked()
    {
        upgradeSystem.Upgrade(CharacterData.AttackType.Archer);
    }

    public void OnMagicUpgradeButtonClicked()
    {
        upgradeSystem.Upgrade(CharacterData.AttackType.Magic);
    }
}
