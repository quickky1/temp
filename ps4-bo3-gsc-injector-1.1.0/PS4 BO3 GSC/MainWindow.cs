using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using libdebug;

namespace PS4_BO3_GSC
{
    public partial class MainWindow : MetroForm
    {
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

        // Pick the payload matching the selected firmware. Classic per-firmware builds
        // cover 5.05 - 7.55; the PS4Debug-NG binary auto-detects 9.00+.
        private string choosePayloadFile()
        {
            string firmware = GetSelectedFirmware();
            string relativePath;
            switch (firmware)
            {
                case "5.05": relativePath = Path.Combine("5_05", "ps4debug.bin"); break;
                case "6.72": relativePath = Path.Combine("6_72", "ps4debug.bin"); break;
                case "7.02": relativePath = Path.Combine("7_02", "ps4debug.bin"); break;
                case "7.55": relativePath = Path.Combine("7_55", "ps4debug.bin"); break;
                default: relativePath = Path.Combine("GoldHEN-13.50", "ps4debug-ng_v1.3.2_release_2026-09-20.bin"); break;
            }

            string displayFirmware = firmware.StartsWith("unknown") ? "9.00+ (auto-detect)" : firmware;
            string bundled = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Payloads", relativePath);

            if (File.Exists(bundled) && new FileInfo(bundled).Length > 0)
            {
                var result = MetroFramework.MetroMessageBox.Show(this,
                    "Send payload for firmware " + displayFirmware + "?\r\n\r\n" + bundled + "\r\n\r\nThe 9.00+ binary auto-detects the firmware when it loads. Sending bytes to GoldHEN's listener does not guarantee it starts; check the PS4 notification and then attach.",
                    "Send payload", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

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

        private async void connectPS4Button_Click(object sender, EventArgs e)
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

            connectionStatusLabel.Text = "Sending payload...";
            connectionStatusLabel.ForeColor = Color.DarkOrange;
            connectPS4Button.Enabled = false;
            Cursor = Cursors.WaitCursor;
            try
            {
                SendResult result = await Task.Run(() => SendPayloadAndVerify(host, port, payloadPath));
                connectionStatusLabel.Text = result.StatusText;
                connectionStatusLabel.ForeColor = result.StatusColor;
                MetroFramework.MetroMessageBox.Show(this, result.Message, result.Title, MessageBoxButtons.OK, result.Icon);
            }
            finally
            {
                Cursor = Cursors.Default;
                connectPS4Button.Enabled = true;
            }
        }

        private sealed class SendResult
        {
            public string Title;
            public string Message;
            public MessageBoxIcon Icon;
            public string StatusText;
            public Color StatusColor;
        }

        // Sends the payload, tolerating GoldHEN's habit of closing the connection
        // right after receiving (which used to look like a failure), then verifies
        // the payload is actually listening on the PS4Debug port (744).
        private SendResult SendPayloadAndVerify(string host, int port, string payloadPath)
        {
            string fileName = Path.GetFileName(payloadPath);
            bool fullySent = false;
            string sendError = null;

            try
            {
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.ReceiveTimeout = 8000;
                    sock.SendTimeout = 30000;
                    var connect = sock.BeginConnect(host, port, null, null);
                    if (!connect.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(8)))
                        throw new IOException("Timed out connecting to " + host + ":" + port + ".");
                    sock.EndConnect(connect);

                    using (var stream = new FileStream(payloadPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        byte[] chunk = new byte[65536];
                        int count;
                        while ((count = stream.Read(chunk, 0, chunk.Length)) > 0)
                        {
                            int offset = 0;
                            while (offset < count)
                            {
                                int sent = sock.Send(chunk, offset, count - offset, SocketFlags.None);
                                if (sent <= 0) throw new IOException("Connection closed before the full payload was sent.");
                                offset += sent;
                            }
                        }
                    }
                    fullySent = true;
                    try { sock.Shutdown(SocketShutdown.Send); }
                    catch { /* GoldHEN usually closes first once it has the bytes; ignore */ }
                }
            }
            catch (Exception ex)
            {
                sendError = ex.Message;
            }

            if (!fullySent)
            {
                return new SendResult
                {
                    Title = "Payload transfer failed",
                    Message = "Could not send " + fileName + " to " + host + ":" + port + ".\n\n" + sendError +
                              "\n\nCheck: PS4 on and jailbroken, GoldHEN BinLoader enabled, IP correct, same network.",
                    Icon = MessageBoxIcon.Error,
                    StatusText = "Payload transfer failed",
                    StatusColor = Color.Red
                };
            }

            // All bytes left the PC. Give the payload a moment to start, then check 744.
            bool listening = false;
            DateTime deadline = DateTime.UtcNow.AddSeconds(15);
            while (DateTime.UtcNow < deadline)
            {
                if (IsPayloadListening(host)) { listening = true; break; }
                Thread.Sleep(1000);
            }

            if (listening)
            {
                string firmware = null;
                try { firmware = QueryFirmwareVersion(host); }
                catch { /* classic 5.05-7.55 payloads don't answer the firmware query */ }

                return new SendResult
                {
                    Title = "Payload running",
                    Message = fileName + " was received and the payload is answering on port 744" +
                              (firmware != null ? " (firmware " + firmware + ")" : "") +
                              ".\n\nYou can attach now.",
                    Icon = MessageBoxIcon.Information,
                    StatusText = "Payload running" + (firmware != null ? " (FW " + firmware + ")" : ""),
                    StatusColor = Color.Green
                };
            }

            return new SendResult
            {
                Title = "Sent, not confirmed",
                Message = "All bytes of " + fileName + " were sent to " + host + ":" + port +
                          ", but nothing is answering on port 744 yet.\n\nCheck the PS4 notification: if the payload didn't start, wait a few seconds and press Detect Firmware, or resend.",
                Icon = MessageBoxIcon.Warning,
                StatusText = "Sent - payload not answering",
                StatusColor = Color.DarkOrange
            };
        }

        private bool IsPayloadListening(string host)
        {
            try
            {
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    var connect = sock.BeginConnect(host, 744, null, null);
                    if (!connect.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2))) return false;
                    sock.EndConnect(connect);
                    return true;
                }
            }
            catch { return false; }
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
                    "Could not read the firmware from the PS4 (port 744).\r\n\r\n" + ex.Message + "\r\n\r\nAuto-detect needs the PS4Debug-NG payload running (9.00+). For 5.05 - 7.55 classic payloads, pick the firmware from the list manually.\r\n\r\nOtherwise: send the payload first and confirm the PS4 shows its notification, then try Detect Firmware again.",
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
        // Reply: uint16 firmware as major*100+minor (NOT BCD), e.g. 900 -> "9.00",
        // 1350 -> "13.50", 505 -> "5.05". Verified against debugger/source/fw.c.
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
                ushort raw = BitConverter.ToUInt16(reply, 0);
                if (raw == 0xFFFF)
                    throw new IOException("Payload could not determine the firmware version.");
                int major = raw / 100;
                int minor = raw % 100;
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

        private int FindBO3Pid()
        {
            // Re-resolve BO3's pid fresh every time: the pid captured when "Attach"
            // was pressed goes stale if the game was restarted since.
            foreach (libdebug.Process process in ps4.GetProcessList().processes)
            {
                if (process.name == "eboot.bin")
                {
                    attachedProcess = process;
                    return process.pid;
                }
            }
            throw new Exception("BO3 (eboot.bin) is not running. Start the game, press Attach BO3, then dump again.");
        }

        private async void DumpMemoryButton_Click(object sender, EventArgs e)
        {
            if (ps4 == null || !ps4.IsConnected)
            {
                MessageBox.Show(this, "Send the payload and attach to BO3 first.", "Not attached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int pid;
            try
            {
                pid = FindBO3Pid();
            }
            catch (Exception ex)
            {
                connectionStatusLabel.Text = "Process Not Found";
                connectionStatusLabel.ForeColor = Color.Red;
                MessageBox.Show(this, ex.Message, "BO3 not found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    await Task.Run(() => DumpProcessMemory(root, pid));
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
            ProcessMap map;
            try
            {
                map = ps4.GetProcessMaps(pid);
            }
            catch (Exception ex)
            {
                throw new IOException("Could not read BO3's memory map from the payload: " + ex.Message +
                    " (is the PS4Debug-NG payload still running on port 744?)", ex);
            }

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
                const ulong maxSaneRegion = 16UL * 1024 * 1024 * 1024; // 16 GB sanity cap

                foreach (var entry in map.entries)
                {
                    if (entry == null || entry.end <= entry.start) continue;

                    // PS4/FreeBSD VM_PROT_READ = 0x1.
                    if ((entry.prot & 1) == 0)
                    {
                        manifest.WriteLine("SKIP no-read-protection name={0} start=0x{1:X} end=0x{2:X} prot=0x{3:X}", entry.name, entry.start, entry.end, entry.prot);
                        continue;
                    }

                    ulong length = entry.end - entry.start;
                    if (length > maxSaneRegion)
                    {
                        manifest.WriteLine("SKIP insane-region-size name={0} start=0x{1:X} end=0x{2:X} length={3} prot=0x{4:X}", entry.name, entry.start, entry.end, length, entry.prot);
                        continue;
                    }

                    string safeName = String.Concat((entry.name ?? "mapping").Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
                    if (String.IsNullOrWhiteSpace(safeName)) safeName = "mapping";
                    string fileName = String.Format("region_{0:D4}_{1}_{2:X}_{3:X}.bin", regionIndex++, safeName, entry.start, entry.end);
                    string filePath = Path.Combine(outputDirectory, fileName);

                    long written = 0;
                    long failedBytes = 0;

                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, chunkSize))
                    {
                        for (ulong offset = 0; offset < length; offset += (ulong)chunkSize)
                        {
                            int count = (int)Math.Min((ulong)chunkSize, length - offset);
                            byte[] bytes = null;
                            Exception lastError = null;
                            // Retry transient read failures before zero-filling the chunk.
                            for (int attempt = 0; attempt < 3 && bytes == null; attempt++)
                            {
                                try
                                {
                                    byte[] chunk = ps4.ReadMemory(pid, entry.start + offset, count);
                                    if (chunk == null || chunk.Length != count) throw new IOException("Short read");
                                    bytes = chunk;
                                }
                                catch (Exception readEx)
                                {
                                    lastError = readEx;
                                }
                            }
                            if (bytes != null)
                            {
                                fs.Write(bytes, 0, bytes.Length);
                            }
                            else
                            {
                                // Preserve address-to-file alignment by zero-filling failed reads.
                                fs.Write(new byte[count], 0, count);
                                failedBytes += count;
                                manifest.WriteLine("READ-FAIL file={0} address=0x{1:X} length={2} error={3}", fileName, entry.start + offset, count, (lastError != null ? lastError.Message : "unknown").Replace("\r", " ").Replace("\n", " "));
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
 