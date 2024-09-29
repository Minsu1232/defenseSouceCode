using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManaSkill : Skill
{
    public float manaCost; // 스킬 발동에 필요한 마나
    public float manaRegenRate; // 시간당 마나 회복량
    public float currentMana; // 현재 마나
    public float maxMana; // 최대 마나
    public Image mpBar; // 마나바 UI
    
 
    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        if (currentMana >= manaCost)
        {
            base.ActivateManaSkill(caster, target);
            currentMana -= manaCost;
            UpdateManaBar(); // 마나 사용 후 MP 바 업데이트
            Debug.Log($"{skillName} activated! Mana left: {currentMana}/{maxMana}");
            currentMana = 0;
        }
        else
        {
            Debug.Log($"{skillName} cannot be activated! Not enough mana.");
        }
    }

    public virtual void RegenerateMana()
    {
        currentMana += manaRegenRate * Time.deltaTime;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateManaBar(); // 마나 회복 후 MP 바 업데이트
    }

    public virtual void UpdateManaBar()
    {
        if (mpBar != null)
        {
            mpBar.fillAmount = currentMana / maxMana; // 현재 마나에 따른 fillAmount 업데이트
        }
    }
}
