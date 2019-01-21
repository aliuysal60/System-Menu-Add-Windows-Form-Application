using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace testApp.Demonstrations.MessageFilter
{
  internal sealed class SystemMenu : IMessageFilter
  {
    #region Externals

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    #endregion

    #region Constants

    private const int MF_SEPARATOR = 0x800;

    private const int MF_STRING = 0x0;

    private const int WM_SYSCOMMAND = 0x112;

    #endregion

    #region Fields

    private List<Action> _actions = new List<Action>();

    private int _lastId = 0;

    private Form _owner;

    #endregion

    #region Constructors

    /// <summary>
    /// Initialises a new instance of the <see cref="SystemMenu"/> class for the specified
    /// <see cref="Form"/>.
    /// </summary>
    /// <param name="owner">The window for which the system menu is expanded.</param>
    public SystemMenu(Form owner)
    {
      if (owner == null)
      {
        throw new ArgumentNullException(nameof(owner));
      }

      _owner = owner;

      owner.FormClosed += this.FormClosedHandler;

      Application.AddMessageFilter(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a command to the system menu.
    /// </summary>
    /// <param name="text">The displayed command text.</param>
    /// <param name="action">The action that is executed when the user clicks on the command.</param>
    /// <param name="separatorBeforeCommand">Indicates whether a separator is inserted before the command.</param>
    public void AddCommand(string text, Action action, bool separatorBeforeCommand)
    {
      IntPtr systemMenuHandle;
      int id;

      systemMenuHandle = GetSystemMenu(_owner.Handle, false);

      id = ++_lastId;

      if (separatorBeforeCommand)
      {
        AppendMenu(systemMenuHandle, MF_SEPARATOR, 0, string.Empty);
      }

      AppendMenu(systemMenuHandle, MF_STRING, id, text);

      _actions.Add(action);
    }

    private void FormClosedHandler(object sender, FormClosedEventArgs e)
    {
      Application.RemoveMessageFilter(this);

      _actions = null;

      _owner.FormClosed -= this.FormClosedHandler;
      _owner = null;
    }

    private bool OnSysCommandMessage(ref Message msg)
    {
      bool result;
      int commandId;

      commandId = msg.WParam.ToInt32();
      result = commandId > 0 && commandId <= _lastId;

      Debug.WriteLine("System menu command: " + commandId);

      if (result)
      {
        _actions[commandId - 1].Invoke();
      }

      return result;
    }

    #endregion

    #region IMessageFilter Interface

    bool IMessageFilter.PreFilterMessage(ref Message m)
    {
      bool result;

      //Debug.WriteLine(m);

      if (m.Msg == WM_SYSCOMMAND && m.HWnd == _owner.Handle)
      {
        result = this.OnSysCommandMessage(ref m);
      }
      else
      {
        result = false; // allow the message to continue being processed
      }

      return result;
    }

    #endregion
  }
}
