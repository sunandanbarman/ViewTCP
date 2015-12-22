using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewTCP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var os = Environment.OSVersion;
            if ((os.Version.Major >= 6) && (os.Version.Minor >= 0))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            } else
            {
                MessageBox.Show("Unable to launch ! Supported OS are Vista and above ...", "ViewTCP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
