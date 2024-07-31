using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WJHRepairTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SoundControlPanelbtn_Click(object sender, EventArgs e)
        {
            Process.Start("control", "/name Microsoft.Sound");
        }

        private void NetworkSettingsbtn_Click(object sender, EventArgs e)
        {
            Process.Start("control.exe", "ncpa.cpl");
        }

        private void BluetoothSettingsbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start ms-settings:bluetooth",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void StartupAppsbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start ms-settings:startupapps",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void InstalledAppsbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start ms-settings:appsfeatures",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void DiskManagementbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start diskmgmt.msc",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void UpdateWindowsbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start ms-settings:windowsupdate",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void DiskCleanupbtn_Click(object sender, EventArgs e)
        {
            Process.Start("cleanmgr.exe");
        }

        private void DefragmentDrivesbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start dfrgui",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }

        private void DeleteTempFilesbtn_Click(object sender, EventArgs e)
        {
            // Open Temp Folder For User To See
            ProcessStartInfo psiOpenTempFolder = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c start %TEMP%",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process to open the temp folder
            Process.Start(psiOpenTempFolder);

            // Delete Temp Files
            ProcessStartInfo psiDeleteTempFiles = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c rd /s /q %TEMP%",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process to delete temp files
            Process.Start(psiDeleteTempFiles);

            DateTime Tthen = DateTime.Now;
            do
            {
                Application.DoEvents();
            } while (Tthen.AddSeconds(3) > DateTime.Now);

            using (var successMessageBox = new Form())
            {
                successMessageBox.TopMost = true;
                MessageBox.Show(successMessageBox, "Temporary folder opened and files deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FixPrinterSpoolerbtn_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/k echo Stopping the print spooler service... && net stop spooler && echo Deleting printer spool files... && del %systemroot%\\System32\\spool\\printers\\* /Q && echo Restarting the print spooler service... && net start spooler && echo Done. Press any key to exit. && pause",
                UseShellExecute = true,  // Use the operating system shell to start the process
                CreateNoWindow = false,  // Create a visible window
                WindowStyle = ProcessWindowStyle.Normal  // Show the window in normal mode
            });
        }
    }
}