namespace Workflow.Entities;

public class SystemActionPreference
{
    public int RuleID {get; private set;}
    public int ActionID {get; private set;}
    public int ProjectID {get; private set;}
    public bool Enabled {get; set;}

    public SystemActionPreference(int ruleID, int actionID, int projectID)
    {
        RuleID = ruleID;
        ActionID = actionID;
        ProjectID = projectID;
        Enabled = true;
    }

    private SystemActionPreference()
    {
        
    }
}
