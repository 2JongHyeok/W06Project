using System.Collections.Generic;
using UnityEngine;

public class ForgeManger : MonoBehaviour
{
    [Header("Branch Configuration")]
    public MainBranchSO[] mainBranches;
    // public Dictionary<ForgeId, BaseForgeSO> forgeDictionary = new Dictionary<ForgeId, BaseForgeSO>();
    public Dictionary<MainBranchType, MainBranchSO> MainBranch = new Dictionary<MainBranchType, MainBranchSO>();
    public Dictionary<SubBranchType, SubBranchSO> SubBranch = new Dictionary<SubBranchType, SubBranchSO>();

    private void Start()
    {
        for (int i = 0; i < mainBranches.Length; i++)
        {
            MainBranch.Add(mainBranches[i].branchType, mainBranches[i]);
            FindSubBranches(mainBranches[i].subBranches);
        }
        foreach(var item in SubBranch)
        {
            Debug.Log(item.Key + " / " + item.Value.subBranchType);
        }
    }

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
                if(subBranch.baseForgeSOs[j].postSubBranches != null && subBranch.baseForgeSOs[j].postSubBranches.Length > 0)
                {
                    FindSubBranches(subBranch.baseForgeSOs[j].postSubBranches);
                }
            }
        }
    }
}
