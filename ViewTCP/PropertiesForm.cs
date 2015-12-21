using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace ViewTCP
{
    public partial class PropertiesForm : Form
    {
       // public string PID { get; set; }
        public string processPath { get; set; }
        public Process p { get; set; }
        public PropertiesForm()
        {
            InitializeComponent();
            
        }
        private Icon IconFromFilePath(string filePath)
        {
            var result = (Icon)null;

            try
            {
                result = Icon.ExtractAssociatedIcon(filePath);
            }
            catch (System.Exception)
            {
                // swallow and return nothing. You could supply a default Icon here as well
            }

            return result;
        }
        private string returnPublisherName(string exeName) //exeName : full path
        {
            string sResult = "";
            X509Certificate xcert = null;
            try
            {
                xcert = X509Certificate.CreateFromSignedFile(exeName);
                string[] temp = xcert.Subject.Split(',');
                foreach (string s in temp)
                {
                    if (s.StartsWith("CN"))
                    {
                        sResult = s.Split('=')[1];
                    }
                }                    
            }
            catch (System.Exception) { sResult = "Unable to readDER-encoded signature."; }
            return sResult;
        }
    
        private void setFormInformation()
        {
            Bitmap _icon; Icon tempIcon;
            lblVersionValue.Text= p.MainModule.FileVersionInfo.FileVersion;
            lblProcessDesc.Text = p.MainModule.FileVersionInfo.FileDescription;
            edtFullPath.Text    = processPath;
            lblPublisher.Text   = returnPublisherName(edtFullPath.Text);
            tempIcon            = IconFromFilePath(edtFullPath.Text);
            edtFullPath.ReadOnly = true;
            try
            {
                _icon = new Icon(tempIcon, 48, 48).ToBitmap();
            } catch(ArgumentOutOfRangeException)
            {
                _icon = Bitmap.FromHicon(new Icon(tempIcon, new Size(48, 48)).Handle);
            }
            imgApplication.Image = _icon;
        }
        private void PropertiesForm_Shown(object sender, EventArgs e)
        {
            
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PropertiesForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnEndProcess_Click(object sender, EventArgs e)
        {
            p.Kill();
            this.Close();
        }

        private void PropertiesForm_Load(object sender, EventArgs e)
        {
            setFormInformation();
        }
    }
}
