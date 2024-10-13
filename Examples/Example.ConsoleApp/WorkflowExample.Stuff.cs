namespace Example.ConsoleApp;

public enum WorkflowStates { New, Revision, Approving, Approved, Rejected }
public enum WorkflowActions { Update, ToApproving, Approve, Reject, ToRevision }

public class WorkflowEntity
{
    public string Name { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public WorkflowStates State { get; set; }
}


