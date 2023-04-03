using System.Diagnostics;
using System.Text;

namespace AssistedTerminal; 

public class Bash {
    private Process _bashProcess = null!;
    private readonly StringBuilder _outputBuilder;

    public Bash() {
        _outputBuilder = new StringBuilder();
        StartBashProcess();
    }

    private void StartBashProcess() {
        _bashProcess = new Process {
            StartInfo = new ProcessStartInfo {
                FileName = "/bin/bash",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _bashProcess.OutputDataReceived += (sender, e) => {
            if (e.Data != "END_OF_CMD") {
                Console.WriteLine("@ " + e.Data);
            }
            _outputBuilder.AppendLine(e.Data);
        };
        _bashProcess.ErrorDataReceived += (sender, e) => {
            Console.WriteLine("@ " + e.Data);
            _outputBuilder.AppendLine(e.Data);
        };

        _bashProcess.Start();
        _bashProcess.BeginOutputReadLine();
        _bashProcess.BeginErrorReadLine();
    }

    public string ExecuteCommand(string cmd, int waitTimeInsteadOfWaitForEndOfCmdMsg = -1) {

        if (waitTimeInsteadOfWaitForEndOfCmdMsg == -1) {
            _outputBuilder.Clear();
            _bashProcess.StandardInput.WriteLine(cmd + "; echo END_OF_CMD");

            while (true) {
                try {
                    while (!_outputBuilder.ToString().Contains("END_OF_CMD")) {
                        Thread.Sleep(100);
                    }
                    break;
                }
                catch (Exception) {
                    // Ignore
                }
            }

            string result = _outputBuilder.ToString();
            int endIndex = result.IndexOf("END_OF_CMD", StringComparison.Ordinal);
            return result[..endIndex].Trim();
        }
        else {
            _bashProcess.StandardInput.WriteLine(cmd);
            Thread.Sleep(waitTimeInsteadOfWaitForEndOfCmdMsg);
            return _outputBuilder.ToString();
        }
        
    }

    public void Dispose() {
        _bashProcess.StandardInput.WriteLine("exit");
        _bashProcess.WaitForExit();
        _bashProcess.Close();
    }
}