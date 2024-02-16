namespace Example.ConsoleApp;

public enum WorkflowStates { New, Revision, Approving, Approved, Rejected }
public enum WorkflowActions { Update, ToApproving, Approve, Reject, ToRevision }

public class ExampleEntity
{
    public string Name { get; set; } = string.Empty;
    public WorkflowStates State { get; set; }
    public int AuthorId { get; set; }
}