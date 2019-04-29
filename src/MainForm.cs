// https://github.com/dotnet-state-machine/stateless
// https://www.hanselman.com/blog/Stateless30AStateMachineLibraryForNETCore.aspx

using System;
using System.Windows.Forms;

namespace TestStateless
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
      var bug = new BugTracker.BugStateMachine("Incorrect stock count");
      bug.Assign("Joe");
      bug.Defer();
      bug.Assign("Sam");
      bug.Assign("Harold");
      bug.Close();

      Console.WriteLine();
      Console.WriteLine("State Machine:");
      Console.WriteLine(bug.ToDotGraph());
    }
  }
}
