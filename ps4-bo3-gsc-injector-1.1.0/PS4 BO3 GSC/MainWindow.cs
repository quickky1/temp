using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using libdebug;

namespace PS4_BO3_GSC
{
    public partial class MainWindow : MetroForm
    {
        public static Socket _psocket;
        public static bool pDConnected;

        private PS4DBG ps4;
        private Process attachedProcess;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            ps4IpTextBox.Text = Properties.Settings.Default.ps4ip;
            ps4PortTextBox.Text = Properties.Settings.Default.ps4Port;
            if (string.IsNullOrWhiteSpace(ps4PortTextBox.Text))
                ps4PortTextBox.Text = "9090";
        }

        // Prefer the supplied PS4Debug-NG payload. If it is not beside the app, allow selecting a compatible payload manually.
        private string choosePayloadFile()
        {
            string bundled = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Payloads", "ps4debug-ng_v1.3.2_release_2026-09-20.bin");

            if (File.Exists(bundled) && new FileInfo(bundled).Length > 0)
            {
                var result = MetroFramework.MetroMessageBox.Show(this,
                    "Use the bundled PS4Debug-NG v1.3.2 payload?\r\n\r\n" + bundled + "\r\n\r\nThis payload auto-detects the PS4 firmware when it loads (supports 3.xx - 13.xx). Sending it to GoldHEN's listener does not guarantee it starts; check the PS4 notification and then attach.",
                    "Send bundled PS4Debug-NG payload", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes) return bundled;
                if (result == DialogResult.Cancel) return null;
            }

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select firmware-compatible PS4 payload";
                dialog.Filter = "PS4 payloads (*.bin;*.elf)|*.bin;*.elf|All files (*.*)|*.*";
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog(this) != DialogResult.OK) return null;

                string ext = Path.GetExtension(dialog.FileName);
                if (!ext.Equals(".bin", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".elf", StringComparison.OrdinalIgnoreCase))
                {
                    MetroFramework.MetroMessageBox.Show(this, "Choose a .bin or .elf payload.", "Unsupported payload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (new FileInfo(dialog.FileName).Length == 0)
                {
                    MetroFramework.MetroMessageBox.Show(this, "The selected payload is empty.", "Invalid payload", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                return dialog.FileName;
            }
        }

        public static bool Connect2PS4(string ip, string port)
        {
            try
            {
                _psocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _psocket.ReceiveTimeout = 3000;
                _psocket.SendTimeout = 3000;
                _psocket.Connect(new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(port)));
                pDConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                pDConnected = false;
                return false;
            }
        }

        private void connectPS4Button_Click(object sender, EventArgs e)
        {
            string host = ps4IpTextBox.Text.Trim();
            string portText = ps4PortTextBox.Text.Trim();
            IPAddress address;
            int port;

            if (!IPAddress.TryParse(host, out address) || address.AddressFamily != AddressFamily.InterNetwork)
            {
                connectionStatusLabel.Text = "Invalid PS4 IP";
                connectionStatusLabel.ForeColor = Color.Red;
                MetroFramework.MetroMessageBox.Show(this, "Enter the PS4 IPv4 address.", "Invalid IP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(portText, out port) || port < 1 || port > 65535)
            {
                connectionStatusLabel.Text = "Invalid port";
                connectionStatusLabel.ForeColor = Color.Red;
                MetroFramework.MetroMessageBox.Show(this, "Enter a valid TCP port. GoldHEN BinLoader is typically 9090.", "Invalid port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string payloadPath = choosePayloadFile();
            if (string.IsNullOrEmpty(payloadPath))
            {
                connectionStatusLabel.Text = "Payload selection cancelled";
                connectionStatusLabel.ForeColor = Color.DarkOrange;
                return;
            }

            Properties.Settings.Default.ps4ip = host;
            Properties.Settings.Default.ps4Port = port.ToString();
            Properties.Settings.Default.Save();

            try
            {
                connectionStatusLabel.Text = "Sending payload bytes...";
                connectionStatusLabel.ForeColor = Color.DarkOrange;

                if (!Connect2PS4(host, port.ToString())) throw new IOException("Could not connect to the payload listener.");

                using (var stream = new FileStream(payloadPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] chunk = new byte[65536];
                    int count;
                    while ((count = stream.Read(chunk, 0, chunk.Length)) > 0)
                    {
                        int offset = 0;
                        while (offset < count)
                        {
                            int sent = _psocket.Send(chunk, offset, count - offset, SocketFlags.None);
                            if (sent <= 0) throw new IOException("Connection closed before the full payload was sent.");
                            offset += sent;
                        }
                    }
                }

                _psocket.Shutdown(SocketShutdown.Send);
                _psocket.Close();
                pDConnected = false;

                connectionStatusLabel.Text = "Payload bytes sent — verify on PS4";
                connectionStatusLabel.ForeColor = Color.YellowGreen;

                MetroFramework.MetroMessageBox.Show(this, "Payload bytes were sent. This confirms transfer only, not payload execution or firmware compatibility. Check the PS4's status, then use Attach BO3.", "Transfer complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err)
            {
                try { if (_psocket != null) _psocket.Close(); } catch { }
                pDConnected = false;
                connectionStatusLabel.Text = "Payload transfer failed";
                connectionStatusLabel.ForeColor = Color.Red;
                MetroFramework.MetroMessageBox.Show(this, err.Message, "Payload transfer failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void detectFirmwareButton_Click(object sender, EventArgs e)
        {
            string host = ps4IpTextBox.Text.Trim();
            IPAddress address;

            if (!IPAddress.TryParse(host, out address) || address.AddressFamily != AddressFamily.InterNetwork)
            {
                connectionStatusLabel.Text = "Invalid PS4 IP";
                connectionStatusLabel.ForeColor = Color.Red;
                MetroFramework.MetroMessageBox.Show(this, "Enter the PS4 IPv4 address.", "Invalid IP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string firmware;
            try
            {
                firmware = QueryFirmwareVersion(host);
            }
            catch (Exception ex)
            {
                connectionStatusLabel.Text = "Firmware detect failed";
                connectionStatusLabel.ForeColor = Color.Red;
                MetroFramework.MetroMessageBox.Show(this,
                    "Could not read the firmware from the PS4 (port 744).\r\n\r\n" + ex.Message + "\r\n\r\nSend the payload first and confirm the PS4 shows its notification, then try Detect Firmware again.",
                    "Firmware detection failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!firmwareComboBox.Items.Contains(firmware))
                firmwareComboBox.Items.Add(firmware);
            firmwareComboBox.SelectedItem = firmware;

            connectionStatusLabel.Text = "Firmware: " + firmware;
            connectionStatusLabel.ForeColor = Color.Green;
        }

        private string GetSelectedFirmware()
        {
            if (firmwareComboBox != null && firmwareComboBox.SelectedItem != null)
                return firmwareComboBox.SelectedItem.ToString();
            return "unknown (not detected)";
        }

        // Asks a running PS4Debug-NG payload (port 744) for the console firmware.
        // Request: 12-byte header (magic 0xFFAABBCC, CMD_FW_VERSION 0xBD000500, datalen 0).
        // Reply: uint16 firmware in BCD, e.g. 0x1350 -> "13.50", 0x900 -> "9.00".
        private static string QueryFirmwareVersion(string host)
        {
            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.ReceiveTimeout = 4000;
                sock.SendTimeout = 4000;
                sock.Connect(new IPEndPoint(IPAddress.Parse(host), 744));

                byte[] request = new byte[12];
                BitConverter.GetBytes(0xFFAABBCCu).CopyTo(request, 0);
                BitConverter.GetBytes(0xBD000500u).CopyTo(request, 4);
                BitConverter.GetBytes(0u).CopyTo(request, 8);
                SendAll(sock, request);

                byte[] reply = ReceiveAll(sock, 2);
                ushort bcd = BitConverter.ToUInt16(reply, 0);
                int major = ((bcd >> 12) & 0xF) * 10 + ((bcd >> 8) & 0xF);
                int minor = ((bcd >> 4) & 0xF) * 10 + (bcd & 0xF);
                return major.ToString() + "." + minor.ToString("D2");
            }
        }

        private static void SendAll(Socket sock, byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int sent = sock.Send(buffer, offset, buffer.Length - offset, SocketFlags.None);
                if (sent <= 0) throw new IOException("Connection closed while sending.");
                offset += sent;
            }
        }

        private static byte[] ReceiveAll(Socket sock, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int received = sock.Receive(buffer, offset, count - offset, SocketFlags.None);
                if (received <= 0) throw new IOException("Connection closed before the reply arrived.");
                offset += received;
            }
            return buffer;
        }

        private void attachBo3Button_Click(object sender, EventArgs e)
        {
            try
            {
                ps4 = new PS4DBG(ps4IpTextBox.Text);
                ps4.Connect();
            }
            catch
            {
                connectionStatusLabel.Text = "Connection Failed";
                connectionStatusLabel.ForeColor = Color.Red;
                return;
            }
            if (!ps4.IsConnected)
            {
                connectionStatusLabel.Text = "Connection Failed";
                connectionStatusLabel.ForeColor = Color.Red;
                return;
            }

            bool foundProcess = false;
            foreach (libdebug.Process process in ps4.GetProcessList().processes)
            {
                if (process.name == "eboot.bin")
                {
                    attachedProcess = process;
                    foundProcess = true;
                    break;
                }
            }

            if (!foundProcess)
            {
                connectionStatusLabel.Text = "Process Not Found";
                connectionStatusLabel.ForeColor = Color.Red;
                return;
            }

            connectionStatusLabel.Text = "Connected + Attached";
            connectionStatusLabel.ForeColor = Color.Green;
            ps4.Notify(222, "Attached to BO3 - ready to dump!");
        }

        private async void DumpMemoryButton_Click(object sender, EventArgs e)
        {
            if (ps4 == null || attachedProcess == null || !ps4.IsConnected)
            {
                MessageBox.Show(this, "Send the payload and attach to BO3 first.", "Not attached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose an empty folder for the BO3 process memory dump. Dump size can be several GB.";
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                string root = Path.Combine(dialog.SelectedPath, "BO3_CUSA02290_memory_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(root);
                Cursor = Cursors.WaitCursor;

                try
                {
                    await Task.Run(() => DumpProcessMemory(root, attachedProcess.pid));
                    MessageBox.Show(this, "Dump finished. Send the created BO3_CUSA02290_memory_* folder (zip it first).\r\nSee dump_manifest.txt for captured and skipped ranges.", "Dump complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Dump stopped: " + ex.Message + "\r\nAny completed region files and the manifest are retained.", "Dump error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void DumpProcessMemory(string outputDirectory, int pid)
        {
            // PS4Debug-NG maps are virtual address ranges. Dump only mappings with the
            // read permission bit set; record failures instead of aborting the entire dump.
            var map = ps4.GetProcessMaps(pid);

            string manifestPath = Path.Combine(outputDirectory, "dump_manifest.txt");
            using (var manifest = new StreamWriter(manifestPath, false, Encoding.UTF8))
            {
                manifest.WriteLine("BO3 process memory dump");
                manifest.WriteLine("Process: eboot.bin");
                manifest.WriteLine("PID: " + pid);
                manifest.WriteLine("PS4 firmware: " + GetSelectedFirmware());
                manifest.WriteLine("Created: " + DateTime.Now.ToString("O"));
                manifest.WriteLine("PS4Debug-NG readable mappings only; chunk read failures are marked as gaps.");
                manifest.WriteLine("Address ranges are virtual addresses. Files are raw bytes, one file per mapping, with unreadable chunks zero-filled and described below.");
                manifest.WriteLine();

                int regionIndex = 0;
                const int chunkSize = 1024 * 1024;

                foreach (var entry in map.entries)
                {
                    if (entry == null || entry.end <= entry.start) continue;

                    // FreeBSD/PS4 protection flags use bit 1 (PROT_READ) for readable mappings.
                    if ((entry.prot & 1) == 0)
                    {
                        manifest.WriteLine("SKIP no-read-protection name={0} start=0x{1:X} end=0x{2:X} prot=0x{3:X}", entry.name, entry.start, entry.end, entry.prot);
                        continue;
                    }

                    ulong length = entry.end - entry.start;
                    string safeName = String.Concat((entry.name ?? "mapping").Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
                    if (String.IsNullOrWhiteSpace(safeName)) safeName = "mapping";
                    string fileName = String.Format("region_{0:D4}_{1}_{2:X}_{3:X}.bin", regionIndex++, safeName, entry.start, entry.end);
                    string filePath = Path.Combine(outputDirectory, fileName);

                    long written = 0;
                    long failedBytes = 0;

                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, chunkSize))
                    {
                        for (ulong offset = 0; offset < length; offset += chunkSize)
                        {
                            int count = (int)Math.Min((ulong)chunkSize, length - offset);
                            try
                            {
                                byte[] bytes = ps4.ReadMemory(pid, entry.start + offset, count);
                                if (bytes == null || bytes.Length != count) throw new IOException("Short read");
                                fs.Write(bytes, 0, bytes.Length);
                            }
                            catch (Exception readEx)
                            {
                                // Preserve address-to-file alignment by zero-filling failed reads.
                                fs.Write(new byte[count], 0, count);
                                failedBytes += count;
                                lock (manifest)
                                    manifest.WriteLine("READ-FAIL file={0} address=0x{1:X} length={2} error={3}", fileName, entry.start + offset, count, readEx.Message.Replace("\r", " ").Replace("\n", " "));
                            }
                            written += count;
                        }
                    }

                    manifest.WriteLine("REGION file={0} name={1} start=0x{2:X} end=0x{3:X} length={4} prot=0x{5:X} captured={6} zeroFilledFailed={7}", fileName, entry.name, entry.start, entry.end, length, entry.prot, written, failedBytes);
                    manifest.Flush();
                }

                manifest.WriteLine("END " + DateTime.Now.ToString("O"));
            }
        }

        private void connectPS4Button_EnabledChanged(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = Color.FromArgb(211, 211, 211);
        }
    }
}
