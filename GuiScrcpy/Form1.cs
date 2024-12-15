using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Hosting;
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
namespace GuiScrcpy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadAvailableDevices();
            InitializeUSBIndicator();
            InitializeWifiIndicator();
            IntializeWebIndicator();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2ShadowForm1.SetShadowForm(this);
            PopulateDevices();

        }

        //Finding the path for necessary tool for our software
        private string FindScrcpyPath()
        {
            //Define the path to scrcpy using a relative path
            string scrcpyPath = Path.Combine(Application.StartupPath, "tools", "scrcpy.exe");

            if (!File.Exists(scrcpyPath))
            {
                MessageBox.Show("Scrcpy executable not found. Please install Scrcpy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            return scrcpyPath;
        }
        private string FindADBPath()
        {
            // Define the path to ADB using a relative path
            string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");

            if (!File.Exists(adbPath))
            {
                MessageBox.Show("ADB executable not found. Please install ADB.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            return adbPath;
        }
        //supposed to exist in same path


        //start scrcpy section //


        private async void StartScrcpyAsync(string arguments, string recordFile)
        {
            //string scrcpyPath = @"D:\scrcpy3\scrcpy.exe"; // Path to scrcpy executable
            string scrcpyPath = Path.Combine(Application.StartupPath, "tools", "scrcpy.exe");
            if (!File.Exists(scrcpyPath))
            {
                lblStatus.Text = "Scrcpy executable not found. Please check the path.";
                return;
            }

            try
            {
                // Run Scrcpy in a background task
                await Task.Run(() =>
                {
                    Process process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = scrcpyPath,
                            Arguments = arguments,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.WaitForExit(); // Wait for Scrcpy to complete
                });

                lblStatus.Text = "Scrcpy has exited.";
                if (!string.IsNullOrEmpty(recordFile))
                {
                    if (File.Exists(recordFile))
                    {
                        lblStatusfile.Text = $"Recording Saved to: {recordFile}";
                        string resourcePath = Path.Combine(Application.StartupPath, "Resources", "button.png");
                        pictureRecord.Image = Image.FromFile(resourcePath);
                    }
                    else
                    {
                        lblStatusfile.Text = "Recording failed or file not found.";
                    }
                }

            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error running Scrcpy: {ex.Message}";
                if (!string.IsNullOrEmpty(recordFile))
                {
                    lblStatusfile.Text = "Recording failed due to an error.";
                }
            }


        }
        private void chkTurnOffScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTurnOffScreen.Checked)
            {
                labelshowInfo.Text = "this will turn off screen but mirroring is still on";
            }
           
        }
        private void btnStartScrcpy_Click_1(object sender, EventArgs e)
        {
            // Validate user inputs
            if (string.IsNullOrWhiteSpace(txtBitrate.Text) || string.IsNullOrWhiteSpace(txtResolution.Text))
            {
                lblStatus.Text = "Please enter bitrate and resolution.";
                return;
            }

            if (cmbDevices.SelectedItem == null)
            {
                lblStatus.Text = "Please select a device.";
                return;
            }

            // Prepare Scrcpy arguments
            string bitrate = txtBitrate.Text.Trim();       // Example: "8M"
            string resolution = txtResolution.Text.Trim(); // Example: "1920"
            string selectedDevice = cmbDevices.SelectedItem.ToString(); // Device serial

            string arguments = $"-s {selectedDevice} --video-bit-rate {bitrate} --max-size {resolution}";

            // Add FPS flags
            if (chk30FPS.Checked)
            {
                arguments += " --max-fps 30";
            }
            else if (chk60FPS.Checked)
            {
                arguments += " --max-fps 60";
            }
            else if (chkDefaultFPS.Checked)
            {
                arguments += " --max-fps 60"; // Default option
            }
            

            // Add turn-screen-off functionality
            if (chkTurnOffScreen.Checked)
            {

                arguments += " --turn-screen-off --stay-awake";
                string resourcePath = Path.Combine(Application.StartupPath, "Resources", "Stream_grey.png");
                piconoff.Image = Image.FromFile(resourcePath);
                
            }

            // Add recording functionality
            string recordFile = string.Empty;
            if (chkRecord.Checked)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                //recordFile = Path.Combine(@"D:\scrcpy3\recordings\", $"recording_{timestamp}.mkv");
                recordFile = Path.Combine(Application.StartupPath, "recordings", $"recording_{timestamp}.mkv");
                string resourcePath = Path.Combine(Application.StartupPath, "Resources", "button_red.png");
                pictureRecord.Image = Image.FromFile(resourcePath);
                arguments += $" -r \"{recordFile}\"";
                lblStatusfile.Text = "Recording Start";
            }


            

            // Start Scrcpy asynchronously
            StartScrcpyAsync(arguments, recordFile);

            // Update UI status
            lblStatus.Text = "Scrcpy is running...";
        }

        private void PopulateDevices()
        {
            try
            {
                // Path to adb.exe
                //string adbPath = @"D:\scrcpy3\adb.exe"; // Adjust path if needed
                string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");
                if (!File.Exists(adbPath))
                {
                    lblStatus.Text = "ADB executable not found!";
                    return;
                }

                // Run `adb devices` command
                Process process = new Process();
                process.StartInfo.FileName = adbPath;
                process.StartInfo.Arguments = "devices";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.OutputDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        Console.WriteLine($"Output: {args.Data}");
                    }
                };

                process.ErrorDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        Console.WriteLine($"Error: {args.Data}");
                    }
                };
                process.ErrorDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrWhiteSpace(args.Data))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = $"Scrcpy Error: {args.Data}";
                        });
                    }
                };


                // Parse the output
                cmbDevices.Items.Clear();
                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (line.Contains("device") && !line.Contains("List"))
                    {
                        string device = line.Split('\t')[0];
                        cmbDevices.Items.Add(device);
                    }
                }

                if (cmbDevices.Items.Count > 0)
                {
                    cmbDevices.SelectedIndex = 0; // Auto-select the first device
                    lblStatus.Text = "Devices loaded successfully.";
                    string resourcePath = Path.Combine(Application.StartupPath, "Resources", "usb_green.png");
                    picUsbIndicator.Image = Image.FromFile(resourcePath);
                    usbStatus.Image= Image.FromFile(resourcePath);
                    
                }
                else
                {
                    lblStatus.Text = "No devices found. Connect a device and try again.";
                    string resourcePath = Path.Combine(Application.StartupPath, "Resources", "usb_grey.png");
                    picUsbIndicator.Image = Image.FromFile(resourcePath);
                }
            }
            catch (Exception ex)
            {
                string resourcePath = Path.Combine(Application.StartupPath, "Resources", "usb_grey.png");
                picUsbIndicator.Image = Image.FromFile(resourcePath);
            }

        }

        //INDICATOR INITIALIZATION//

        private void InitializeUSBIndicator()
        {
            picUsbIndicator.SizeMode = PictureBoxSizeMode.StretchImage;
            string resourcePath = Path.Combine(Application.StartupPath, "Resources", "usb2-grey.png");
            picUsbIndicator.Image = Image.FromFile(resourcePath);
            lblStatus.Text = "No device connected."; // Default status
        }
        private void InitializeWifiIndicator()
        {
            picWifiIndicator.SizeMode = (PictureBoxSizeMode.StretchImage);
            string resourcePath = Path.Combine(Application.StartupPath, "Resources", "wifi-signal.png");
            picWifiIndicator.Image = Image.FromFile(resourcePath);
            lblConnectionStatus.Text = "available devices";
        }
        private void IntializeWebIndicator()
        {
            //picCameraIndicator.SizeMode =(PictureBoxSizeMode.StretchImage);
            string resourcePath = Path.Combine(Application.StartupPath, "Resources", "webcam-grey.png");
            //picCameraIndicator.Image = Image.FromFile(resourcePath);

        }

        //WIRELESS SECTION//

        private void LoadAvailableDevices()
        {
            cmbAvailableDevices.Items.Clear();
            string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");
            //string adbPath = @"D:\scrcpy3\adb.exe";
            try
            {
                // Command to list connected devices (ADB must be installed and in PATH)
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = "devices",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Split output by new lines and parse each line for device IPs
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    // Ignore the first line which is the header "List of devices attached"
                    if (line.Contains("List of devices attached") || string.IsNullOrWhiteSpace(line))
                        continue;

                    // Check if the line contains a valid device, expecting device info after the tab space
                    string[] parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1 && parts[1] == "device")
                    {
                        string deviceId = parts[0].Trim();

                        // If the device is a WiFi device, the deviceId should contain an IP address
                        if (deviceId.Contains(":"))
                        {
                            // Extract just the IP address (before the colon)
                            string ipAddress = deviceId.Split(':')[0];

                            // Add the IP address to the ComboBox, ensure no duplicate IPs
                            if (!cmbAvailableDevices.Items.Contains(ipAddress))
                            {
                                cmbAvailableDevices.Items.Add(ipAddress);
                            }
                        }
                    }
                }

                // If no devices found, show a placeholder message
                if (cmbAvailableDevices.Items.Count == 0)
                {
                    cmbAvailableDevices.Items.Add("No devices found.");
                    string resourcePath = Path.Combine(Application.StartupPath, "Resources", "wifi-xmark-grey.png");
                    picWifiIndicator.Image = Image.FromFile(resourcePath);
                    lblConnectionStatus.Text = "No Available Device Found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching devices: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



        private void btnStartScrcpyWireless_Click_1(object sender, EventArgs e)
        {

            string ipAddress = txtIpAddress.Text.Trim();

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                //MessageBox.Show("Please enter a valid IP address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"Starting scrcpy with IP: {ipAddress}"); // Debugging IP
            string resourcePath = Path.Combine(Application.StartupPath, "Resources", "wifi-signal-blue.png");
            picWifiIndicator.Image = Image.FromFile(resourcePath);
            

            try
            {
                //string scrcpyPath = @"D:\scrcpy3\scrcpy.exe"; // Path to scrcpy.exe
                string scrcpyPath = Path.Combine(Application.StartupPath, "tools", "scrcpy.exe");
                if (!File.Exists(scrcpyPath))
                {
                    //MessageBox.Show("scrcpy executable not found. Please check the path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = scrcpyPath,
                        Arguments = $"--tcpip={ipAddress}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                //MessageBox.Show($"Output: {output}");
                //MessageBox.Show($"Output: {output}\nError: {error}");

                if (output.Contains("connected"))
                {
                    lblConnectionStatus.Text = "Connected Successfully!";
                   
                }
                else
                {
                    lblConnectionStatus.Text = "Connection Failed!";
                   
                }
            }
            catch (Exception ex)
            {
                lblConnectionStatus.Text = $"Error: {ex.Message}";
            }

        }

        //DEVICES AVAILABLITY SECTION//
        private void cmbAvailableDevices_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbAvailableDevices.SelectedItem != null && cmbAvailableDevices.SelectedItem.ToString() != "No devices found.")
            {
                txtIpAddress.Text = cmbAvailableDevices.SelectedItem.ToString();
            }
        }
        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            LoadAvailableDevices();
        }

        //DEFAULT FPS SECTION//
        private void chkDefaultFPS_Click(object sender, EventArgs e)
        {
            if (chkDefaultFPS.Checked)
            {
                txtBitrate.Text = "16M"; // Set high default bitrate
                txtResolution.Text = "1920";
                txtBitrate.Enabled = false; // Optional: Disable manual editing
                txtResolution.Enabled = false;
                chk30FPS.Checked = false;
                chk60FPS.Checked = false;
                chkbtn1920R.Checked = false;
                chkbtn1024R.Checked = false;
                chk10M.Checked = false;
                chk16M.Checked = false;
                chk20M.Checked = false;
                labelshowInfo.Text = "Selecting Default will set high Bitrate and Resolution With 60Fps";
            }
            else
            {
                txtBitrate.Enabled = true; // Re-enable manual editing
                txtResolution.Enabled = true;
                txtBitrate.Text = ""; // Clear the bitrate when unchecked
                txtResolution.Text = "";
                labelshowInfo.Text = "";
            }
        }
        private void chk30FPS_Click(object sender, EventArgs e)
        {
           
            if (chk30FPS.Checked)
            {
                chk60FPS.Checked = false;
                chkDefaultFPS.Checked = false;
            }
        }
        private void chk60FPS_Click(object sender, EventArgs e)
        {
            if (chk60FPS.Checked)
            {
                chk30FPS.Checked = false;
                chkDefaultFPS.Checked = false;
            }
        }
        
      

        //CAMERA SECTION//
        private void cmbfacinFront_Click(object sender, EventArgs e)
        {
            string arguments = "--video-source=camera"; // Default video source is camera
            if (cmbfacinFront.Checked)
            {
                arguments += " --video-source=camera --camera-facing=front --camera-size=1920x1080";
                cmbfacingBack.Checked = false;
                labelshowInfo.Text = "fetching Front Camera";
            }


        }

        private void cmbfacingBack_Click(object sender, EventArgs e)
        {
            string arguments = "--video-source=camera"; // Default video source is camera
            if (cmbfacingBack.Checked)
            {
                arguments += " --camera-facing=back";
                cmbfacinFront.Checked = false;
                labelshowInfo.Text = "fetching Back Camera";
                
            }
        }

        private void btnStartCameraStream_Click(object sender, EventArgs e)
        {
            {
                //string scrcpyPath = @"D:\scrcpy3\scrcpy.exe"; // Path to scrcpy executable
                string scrcpyPath = Path.Combine(Application.StartupPath, "tools", "scrcpy.exe");
                if (!File.Exists(scrcpyPath))
                {
                    MessageBox.Show("scrcpy executable not found. Please check the path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string arguments = " --video-source=camera ";  // Start camera streaming

                if (chkAudioNo.Checked)
                {
                    arguments += " --no-audio"; // Append --no-audio if audio is disabled
                }
                else if (chkAudioYes.Checked)
                {
                    arguments += " --audio-source=mic"; // Optional: Explicitly enable microphone audio
                }
                else
                {
                    arguments += "--video-source=camera";
                }

                Process process = new Process();
                process.StartInfo.FileName = scrcpyPath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                labelshowInfo.Text = "This feature is supported only in Android 12 or 'Higher'";
                lblStatus.Text = "Camera stream started...";
                string resourcePath = Path.Combine(Application.StartupPath, "Resources", "webcam-blue.png");
                //picCameraIndicator.Image = Image.FromFile(resourcePath);
            }
        }


        private void usbStatus_Click(object sender, EventArgs e)
        {

        }

        private void txtBitrate_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtIpAddress_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {

        }

        

        private void btnReset_Click(object sender, EventArgs e)
        {

            // Show confirmation message
            var result = MessageBox.Show("Are you sure you want to reset the software?", "Confirm Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicked 'Yes', proceed with the reset
            if (result == DialogResult.Yes)
            {
                // Clear the ComboBox
                cmbDevices.Items.Clear();
                cmbDevices.SelectedIndex = -1; // Reset selection

                // Clear the TextBoxes
                txtBitrate.Clear();
                txtResolution.Clear();
                //txtDeviceIp.Clear();

                // Uncheck all FPS-related checkboxes
                chkDefaultFPS.Checked = false;
                chk30FPS.Checked = false;
                chk60FPS.Checked = false;
                chk10M.Checked=false;
                chk16M.Checked=false;
                chk20M.Checked=false;
                chkbtn1024R.Checked=false;
                chkbtn1920R.Checked=false;
                chkRecord.Checked=false;
                chkTurnOffScreen.Checked=false;
                cmbfacinFront.Checked=false;
                cmbfacingBack.Checked=false;
                chkAudioNo.Checked=false;
                chkAudioYes.Checked=false;
                labelshowInfo.Text = "";

                // Reset the status label
                lblStatus.Text = "Ready to start.";
                //lblstatusForWl.Text = ""; // Clear the wireless status label

                // Re-populate the devices list
                PopulateDevices();


            }
            else
            {
                // Optionally, you can add a message when the reset is canceled
                lblStatus.Text = "Reset canceled.";
            }

        }
        //FILE TRANSFER SECTION//
        private void radioBtnLowMode_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnUploadFile_Click_1(object sender, EventArgs e)
        {
            //string adbPath = @"D:\scrcpy3\adb.exe";
            string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");
            string destinationPath = "/sdcard/Download/";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select File to Upload";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sourceFilePath = openFileDialog.FileName;

                    try
                    {
                        Process process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = adbPath,
                                Arguments = $"push \"{sourceFilePath}\" {destinationPath}",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            }
                        };

                        process.Start();
                        process.WaitForExit();

                        MessageBox.Show("File uploaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnRefreshFiles.PerformClick(); // Refresh the file list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error uploading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnRefreshFiles_Click_1(object sender, EventArgs e)
        {
            //string adbPath = @"D:\scrcpy3\adb.exe";
            string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");
            string directoryPath = "/sdcard/Download/";

            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = $"shell ls {directoryPath}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string[] files = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                lvFiles.Items.Clear();

                foreach (string file in files)
                {
                    ListViewItem item = new ListViewItem(file);
                    lvFiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error listing files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownloadFile_Click_1(object sender, EventArgs e)
        {
            if (lvFiles.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a file to download.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedFile = lvFiles.SelectedItems[0].Text;
            string adbPath = Path.Combine(Application.StartupPath, "tools", "adb.exe");
            //string adbPath = @"D:\scrcpy3\adb.exe";
            string sourcePath = $"/sdcard/Download/{selectedFile}";
            string destinationPath = Path.Combine(Application.StartupPath, "downloads");
            string destinationDirectory = destinationPath;

            try
            {
                Process process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = adbPath,
                        Arguments = $"pull \"{sourcePath}\" \"{destinationDirectory}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                MessageBox.Show($"File downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
                                //Buttons 
        private void guna2CustomCheckBox1_Click(object sender, EventArgs e)
        {
            if (chk10M.Checked)
            {
                txtBitrate.Text = "10";
                chk16M.Checked = false;
                chk20M.Checked = false;
            }
        }

        private void guna2CustomCheckBox2_Click(object sender, EventArgs e)
        {
            if (chk16M.Checked) 
            {
                txtBitrate.Text ="16";
                chk10M.Checked = false;
                chk20M.Checked = false;
                chkDefaultFPS.Checked = false;
            }

        }

        private void chk20M_Click(object sender, EventArgs e)
        {
            if (chk20M.Checked)
            {
                txtBitrate.Text = "20";
                chk16M.Checked = false;
                chk10M.Checked = false;
                chkDefaultFPS.Checked = false;
            }
        }

        private void chkbtn1024R_Click(object sender, EventArgs e)
        {
            if (chkbtn1024R.Checked) 
            {
                txtResolution.Text = "1024";
                chkbtn1920R.Checked = false;
                chkDefaultFPS.Checked = false;
            }
        }

        private void chkbtn1920R_Click(object sender, EventArgs e)
        {
            if (chkbtn1920R.Checked)
            {
                txtResolution.Text = "1920";
                chkbtn1024R.Checked = false;
                chkDefaultFPS.Checked = false;
            }
        }

        private void chkAudioYes_Click(object sender, EventArgs e)
        {
            chkAudioNo.Checked = false;
        }

        private void chkAudioNo_Click(object sender, EventArgs e)
        {
            chkAudioYes.Checked = false;
        }

        private void ClickInfoLabel_Click(object sender, EventArgs e)
        {

        }

        private void picCameraIndicator_Click(object sender, EventArgs e)
        {

        }

        private void chkRecord_Click(object sender, EventArgs e)
        {

        }

        private void btnforhelp_Click(object sender, EventArgs e)
        {

        }

        private void lblConnectionStatus_Click(object sender, EventArgs e)
        {

        }
    }


}
