using FluentStateMachine;
using System;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

internal class WorkflowExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== WorkflowExample Start ===\n");

        var entity = new WorkflowEntity
        {
            Name = "test",
            State = WorkflowStates.New,
            AuthorId = 1,
        };

        var workflow = WorkflowBuilder.Build(entity);

        await workflow.TriggerAsync(WorkflowActions.ToApproving);
        await workflow.TriggerAsync(WorkflowActions.ToRevision, "correction required");
        await workflow.TriggerAsync(WorkflowActions.Update);
        await workflow.TriggerAsync(WorkflowActions.ToApproving);
        await workflow.TriggerAsync(WorkflowActions.Approve, "OK");

        Console.WriteLine($"Entity state is '{entity.State}'");

        Console.WriteLine($"Done\n");
    }

    class WorkflowBuilder
    {
        public static IStateMachine<WorkflowStates, WorkflowActions> Build(WorkflowEntity entity)
        {
            return new FsmBuilder<WorkflowStates, WorkflowActions>(entity.State)
                .OnFire(x => Console.WriteLine($">>> Trigger event '{x.Event}'"))
                .OnComplete(x => Console.WriteLine())
                .OnEnter(x =>
                {
                    entity.State = x.State;
                    Console.WriteLine($"State changed to '{x.State}' from '{x.PrevState}'");
                })

                // for all states
                .On(WorkflowActions.Update)
                    .Enable(x => IsAuthor(entity))
                    .Execute(x => Console.WriteLine($"Saving object '{entity.Name}'"))

                .State(WorkflowStates.New)
                    .On(WorkflowActions.ToApproving)
                        .Enable(x => IsAuthor(entity))
                        .JumpTo(WorkflowStates.Approving)

                .State(WorkflowStates.Revision)
                    .On(WorkflowActions.ToApproving)
                        .Enable(x => IsAuthor(entity))
                        .JumpTo(WorkflowStates.Approving)

                .State(WorkflowStates.Approving)
                    .OnEnter(x => Console.WriteLine($"Object '{entity.Name}' is awaiting approval"))
                    .On(WorkflowActions.Approve)
                        .Enable(x => IsApprover(entity))
                        .Execute(x => Console.WriteLine($"Send with comment: {x.Data}"))
                        .JumpTo(WorkflowStates.Approved)
                    .On(WorkflowActions.Reject)
                        .Enable(x => IsApprover(entity))
                        .JumpTo(WorkflowStates.Rejected)
                    .On(WorkflowActions.ToRevision)
                        .Enable(x => IsApprover(entity))
                        .Execute(x => Console.WriteLine($"Send with comment: {x.Data}"))
                        .JumpTo(WorkflowStates.Revision)

                .State(WorkflowStates.Approved)
                    .OnEnter(x => Console.WriteLine($"Object '{entity.Name}' was approved"))

                .State(WorkflowStates.Rejected)
                    .OnEnter(x => Console.WriteLine($"Object '{entity.Name}' was rejected"))

               .Build();
        }

        static bool IsAuthor(WorkflowEntity entity) => entity.AuthorId == 1;
        static bool IsApprover(WorkflowEntity entity) => true;
    }
}
