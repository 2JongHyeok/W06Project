using System.Collections.Generic;
using UnityEngine;

public class ForgeManger : MonoBehaviour
{
    [Header("Branch Configuration")]
    public MainBranchSO[] mainBranches;

    private void Start()
    {
        // Ensure Managers is initialized (if needed by other systems)
        var _ = Managers.Instance;
    }

    
}
