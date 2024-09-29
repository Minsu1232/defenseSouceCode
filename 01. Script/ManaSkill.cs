using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ManaSkill : Skill
{
    public float manaCost; // ��ų �ߵ��� �ʿ��� ����
    public float manaRegenRate; // �ð��� ���� ȸ����
    public float currentMana; // ���� ����
    public float maxMana; // �ִ� ����
    public Image mpBar; // ������ UI
    
 
    public override void ActivateManaSkill(CharacterInfo caster, GameObject target)
    {
        if (currentMana >= manaCost)
        {
            base.ActivateManaSkill(caster, target);
            currentMana -= manaCost;
            UpdateManaBar(); // ���� ��� �� MP �� ������Ʈ
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
        UpdateManaBar(); // ���� ȸ�� �� MP �� ������Ʈ
    }

    public virtual void UpdateManaBar()
    {
        if (mpBar != null)
        {
            mpBar.fillAmount = currentMana / maxMana; // ���� ������ ���� fillAmount ������Ʈ
        }
    }
}
