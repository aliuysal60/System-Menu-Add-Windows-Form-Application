using System;
using System.Windows.Forms;


namespace testApp.Demonstrations.MessageFilter
{
  public partial class MainForm : Form
  {
    #region Fields

    private SystemMenu _menu;

    #endregion

    #region Constructors

    public MainForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Text = Application.ProductName;

      _menu = new SystemMenu(this);
      _menu.AddCommand("&Defaults...", this.ShowDefaultsDialog, true);
      _menu.AddCommand("&Properties...", this.ShowPropertiesDialog, false);
      _menu.AddCommand("&About...", this.ShowAboutDialog, true);
    }

    private void ShowAboutDialog()
    {
      using (AboutDialog dialog = new AboutDialog())
      {
        dialog.ShowDialog(this);
      }
    }

    private void ShowDefaultsDialog()
    {
      MessageBox.Show("Fake defaults window.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowPropertiesDialog()
    {
      MessageBox.Show("Fake properties window.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
  }
}
