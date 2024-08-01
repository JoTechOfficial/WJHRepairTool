using System;
using System.Collections.Generic; // Add this for the Dictionary
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WJHRepairTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Set GPU information when the form is loaded
            GpuLabel.Text = "GPU: " + GetGPUName();

            // Set CPU information when the form is loaded
            string cpuInfo = ExecutePowerShellCommand("Get-WmiObject Win32_Processor | Select-Object -ExpandProperty Name");
            CpuLabel.Text = "CPU: " + cpuInfo;

            // Retrieve RAM information
            string ramInfo = GetTotalPhysicalMemory();
            RamLabel.Text = "Total System RAM: " + ramInfo;

            // Retrieve Windows build information
            string windowsVersion = GetWindowsBuildVersion();
            WindowsLabel.Text = "OS Version: " + windowsVersion;

            // Retrieve BIOS information using PowerShell
            string biosInfo = GetBiosInformation();
            BiosLabel.Text = "Mobo Name/BIOS Version: " + biosInfo;
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

        private string GetGPUName()
        {
            try
            {
                // Define the command to get GPU information
                string command = "wmic path win32_videocontroller get caption";

                // Create a process to execute the command
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + command,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                // Start the process and read the output
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Parse the output to get the GPU name
                string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length > 1)
                {
                    return lines[1].Trim(); // Return the GPU name, skipping the header
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                MessageBox.Show("Error retrieving GPU information: " + ex.Message);
            }

            return "GPU information not available";
        }

        private string ExecutePowerShellCommand(string command)
        {
            try
            {
                // Create a process to execute the PowerShell command
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-Command \"{command}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                // Start the process and read the output
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output.Trim();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                MessageBox.Show("Error retrieving information: " + ex.Message);
            }

            return "Information not available";
        }

        private string GetTotalPhysicalMemory()
        {
            try
            {
                // Get total physical memory (RAM) using Environment class
                var ram = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                return $"{ram / (1024 * 1024 * 1024)} GB"; // Convert bytes to GB
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                MessageBox.Show("Error retrieving RAM information: " + ex.Message);
            }

            return "RAM information not available";
        }

        private string GetWindowsBuildVersion()
        {
            try
            {
                // Retrieve OS version and build number
                Version osVersion = Environment.OSVersion.Version;
                string versionString = osVersion.ToString();  // e.g., "10.0.19045"
                string buildNumber = GetBuildNumber(); // Extract build number from OS version

                // Map build numbers to Windows versions
                string buildLabel = GetBuildLabel(buildNumber);

                return $"{versionString} (Build {buildLabel})";
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                MessageBox.Show("Error retrieving Windows build information: " + ex.Message);
            }

            return "Windows build information not available";
        }

        private string GetBuildNumber()
        {
            // Retrieve the build number from the OS version
            // Adjust this as needed based on how the build number is retrieved
            return Environment.OSVersion.Version.Build.ToString();
        }

        private string GetBuildLabel(string buildNumber)
        {
            // Define mappings for Windows versions
            var buildLabels = new Dictionary<string, string>
            {
                // Windows 10
    { "10240", "1507 (RTM)" },
    { "10586", "1511" },
    { "14393", "1607" },
    { "15063", "1703" },
    { "16299", "1709" },
    { "17134", "1803" },
    { "17763", "1809" },
    { "18362", "1903" },
    { "18363", "1909" },
    { "19041", "2004" },
    { "19042", "20H2" },
    { "19043", "21H1" },
    { "19044", "21H2" },
    { "19045", "22H2" },

    // Windows 11
    { "22000", "21H2" },
    { "22621", "22H2" },
    { "22631", "23H2" }, // Adjusted from "24H2" based on current information
            };

            // Try to find the build number in the dictionary
            if (buildLabels.TryGetValue(buildNumber, out string label))
            {
                return label;
            }

            return "Unknown Build";
        }

        private string GetBiosInformation()
        {
            string biosInfo = ExecutePowerShellCommand(@"
        $bios = Get-WmiObject Win32_BIOS
        $model = $bios.SMBIOSBIOSVersion

        $baseboard = Get-WmiObject Win32_BaseBoard
        $baseboardProduct = $baseboard.Product
        $baseboardProduct + ' ' + $model
    ");

            return biosInfo.Trim();
        }
    }
}
