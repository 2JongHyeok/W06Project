using System.Collections.Generic;
using UnityEngine;

public class ForgeManger : MonoBehaviour
{
    [Header("Branch Configuration")]
    public MainBranchSO[] mainBranches;
    // public Dictionary<ForgeId, BaseForgeSO> forgeDictionary = new Dictionary<ForgeId, BaseForgeSO>();
    public Dictionary<MainBranchType, MainBranchSO> MainBranch = new Dictionary<MainBranchType, MainBranchSO>();
    public Dictionary<SubBranchType, SubBranchSO> SubBranch = new Dictionary<SubBranchType, SubBranchSO>();
    public Dictionary<SubBranchType, SubBranchSO> LockedSubBranch = new Dictionary<SubBranchType, SubBranchSO>();
    private int[] currnetForgeLevel = new int[System.Enum.GetValues(typeof(ForgeId)).Length];

    private void Start()
    {
        for (int i = 0; i < mainBranches.Length; i++)
        {
            MainBranch.Add(mainBranches[i].branchType, mainBranches[i]);
            FindSubBranches(mainBranches[i].subBranches);
        }
        foreach (var item in SubBranch)
        {
            Debug.Log(item.Key + " / " + item.Value.subBranchType);
        }
        for (int i = 0; i < currnetForgeLevel.Length; i++)
        {
            currnetForgeLevel[i] = 0;
        }
        foreach (var item in LockedSubBranch)
        {
            Debug.Log("LockedSubBranch: " + item.Key + " / " + item.Value.subBranchType);
        }
        GetForgeSO(ForgeId.MainCannonAtkDamage, 0);
    }
    //서브 브랜치 탐색용 재귀 함수
    private void FindSubBranches(SubBranchSO[] subBranches)
    {
        foreach (var subBranch in subBranches)
        {
            if (!SubBranch.ContainsKey(subBranch.subBranchType))
            {
                SubBranch.Add(subBranch.subBranchType, subBranch);
            }
            for (int j = 0; j < subBranch.baseForgeSOs.Length; j++)
            {
                // forgeDictionary.Add(subBranch.baseForgeSOs[j].ForgeId, subBranch.baseForgeSOs[j]);
                if (subBranch.baseForgeSOs[j].postSubBranches != null && subBranch.baseForgeSOs[j].postSubBranches.Length > 0)
                {
                    foreach (var postSubBranch in subBranch.baseForgeSOs[j].postSubBranches)
                    {
                        if (!LockedSubBranch.ContainsKey(postSubBranch.subBranchType))
                        {
                            LockedSubBranch.Add(postSubBranch.subBranchType, postSubBranch);
                        }
                    }
                    FindSubBranches(subBranch.baseForgeSOs[j].postSubBranches);
                }
            }
        }
    }
    //특정 레벨의 강화 가져오기
    public BaseForgeSO GetForgeSO(ForgeId forgeId, int level)
    {
        if (SubBranch.Count == 0) return null;
        if (SubBranch.ContainsKey((SubBranchType)(int)forgeId))
        {
            SubBranchSO subBranch = SubBranch[(SubBranchType)(int)forgeId];
            Debug.Log(subBranch.baseForgeSOs[level].upgradeName);
            return subBranch.baseForgeSOs[level];
        }
        return null;
    }
    //강화할 때 호출
    public void ForgeApply(BaseForgeSO forgeSO)
    {
        forgeSO?.Apply();
        if(forgeSO.postSubBranches != null && forgeSO.postSubBranches.Length > 0)
        {
            //언락검출
        }
        currnetForgeLevel[(int)forgeSO.forgeId]++;
    }
}
