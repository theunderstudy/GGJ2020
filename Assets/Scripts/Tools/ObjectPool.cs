using System.Collections.Generic;
using UnityEngine;
public class ObjectPool : Singleton<ObjectPool>
{

    // upgrade region
    public UpgradeBase[] UpgradePrefabs;
    private Dictionary<UpgradeTypes, Stack<UpgradeBase>> m_UpgradePool = new Dictionary<UpgradeTypes, Stack<UpgradeBase>>();
    private Stack<UpgradeBase> m_UpgradeStack;
    private UpgradeBase m_Upgrade;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < UpgradePrefabs.Length; i++)
        {
            for (int _count = 0; _count < 5; _count++)
            {
                AddUpgradeToDictionay(Instantiate(UpgradePrefabs[i]));
            }
        }
    }
    private void AddUpgradeToDictionay(UpgradeBase upgrade)
    {
        upgrade.transform.parent = transform;
        upgrade.transform.localPosition = Vector3.zero;
        upgrade.gameObject.SetActive(false);

        if (m_UpgradePool.TryGetValue(upgrade.UpgradeType, out m_UpgradeStack))
        {
            m_UpgradeStack.Push(upgrade);
        }
        else
        {
            m_UpgradeStack = new Stack<UpgradeBase>();
            m_UpgradeStack.Push(upgrade);
            m_UpgradePool.Add(upgrade.UpgradeType, m_UpgradeStack);
        }
    }

    private UpgradeBase GetUpgradePrefab(UpgradeTypes upgradetype)
    {
        for (int i = 0; i < UpgradePrefabs.Length; i++)
        {
            if (UpgradePrefabs[i].UpgradeType == upgradetype)
            {
                return UpgradePrefabs[i];
            }
        }

        return null;
    }
    private UpgradeBase GetUpgradeFromDictionary(UpgradeTypes upgradetype)
    {
        if (m_UpgradePool.TryGetValue(upgradetype, out m_UpgradeStack))
        {
            if (m_UpgradeStack.Count == 0)
            {
                for (int newUpgradeCount = 0; newUpgradeCount < 5; newUpgradeCount++)
                {
                    AddUpgradeToDictionay(Instantiate(GetUpgradePrefab(upgradetype)));
                }
            }
            m_Upgrade = m_UpgradeStack.Pop();
            return m_Upgrade;
        }

        return null;
    }


    public UpgradeBase GetUpgrade(UpgradeTypes upgradetype)
    {
        m_Upgrade = GetUpgradeFromDictionary(upgradetype);
        if (m_Upgrade == null)
        {
            return null;
        }
        m_Upgrade.gameObject.SetActive(true);
        m_Upgrade.ResetUpgrade();
        return m_Upgrade;
    }

    public void ReturnUpgrade(UpgradeBase returnedUpgrade)
    {
        AddUpgradeToDictionay(returnedUpgrade);
    }

}
