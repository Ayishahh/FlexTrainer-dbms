using Project;
using System.Windows.Forms;

namespace FlexTrainer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            ApplicationConfiguration.Initialize();
            try
            {
                DatabaseHelper.EnsureServerConfigured();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new LogIn());
        }
    }
}