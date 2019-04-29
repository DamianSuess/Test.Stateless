//https://github.com/dotnet-state-machine/stateless/tree/dev/example

using System;
using Stateless;
using Stateless.Graph;

namespace TestStateless.BugTracker
{
  public class BugStateMachine
  {
    private readonly StateMachine<State, Trigger> _machine;

    /// <summary>Used when a trigger requires a payload</summary>
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;

    private readonly string _title;

    private string _assignee;

    public BugStateMachine(string title)
    {
      _title = title;

      // Instantiate a new state machine with opened state
      _machine = new StateMachine<State, Trigger>(State.Open);

      // Instantiate a new trigger w/ a param
      _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

      // Configure the open state
      _machine.Configure(State.Open)
              .Permit(Trigger.Assign, State.Assigned);

      // Configure the assigned state
      _machine.Configure(State.Assigned)
              .SubstateOf(State.Open)
              .OnEntryFrom(_assignTrigger, OnAssigned)
              .PermitReentry(Trigger.Assign)
              .Permit(Trigger.Close, State.Closed)
              .Permit(Trigger.Defer, State.Deferred)
              .OnExit(OnDeassigned);
    }

    public bool CanAssign => _machine.CanFire(Trigger.Assign);

    public void Close()
    {
      _machine.Fire(Trigger.Close);
    }

    public void Assign(string assignee)
    {
      _machine.Fire(_assignTrigger, assignee);
    }

    public void Defer()
    {
      _machine.Fire(Trigger.Defer);
    }

    public string ToDotGraph()
    {
      return UmlDotGraph.Format(_machine.GetInfo());
    }

    private void OnDeassigned()
    {
      SendEmailToAsignee("You've been unassigned from the task");
    }

    private void OnAssigned(string assignee)
    {
      if (_assignee != null && assignee != _assignee)
        SendEmailToAsignee("Don't forget to help employee");

      _assignee = assignee;
      SendEmailToAsignee("You own it");
    }

    private void SendEmailToAsignee(string message)
    {
      Console.WriteLine($"{_assignee}, RE: {_title}: {message}");
    }
  }
}
