dotnet build -c Release 
dotnet pack .\FluentStateMachine\ -c Release -o ..\_publish
dotnet pack .\FluentStateMachine.MediatR\ -c Release -o ..\_publish
