using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Com0Com.CSharp
{
    public interface ICmdRunner
    {
        string[] RunCommandGetStdOut(string workingDir, string command, string args);
        Task<string[]> RunCommandGetStdOutAsync(string workingDir, string command, string args);
    }

    public class CmdRunner : ICmdRunner
    {
        public string[] RunCommandGetStdOut(string workingDir, string command, string args)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = workingDir,
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.Start();

            while (!proc.HasExited)
            {
                Thread.Sleep(100);
            }

            if (proc.ExitCode != 0)
                throw new ApplicationException($"Exit code of {proc.ExitCode} received when running '{command} {args}'");

            var ret = new List<string>();
            while (!proc.StandardOutput.EndOfStream)
            {
                ret.Add(proc.StandardOutput.ReadLine());
            }

            return ret.ToArray();
        }

        public async Task<string[]> RunCommandGetStdOutAsync(string workingDir, string command, string args)
        {
            return await Task.Run(() => RunCommandGetStdOut(workingDir, command, args));
        }
    }
}
