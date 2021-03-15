using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace LTL.UI.Console.Integration.Tests
{
    //https://www.codeproject.com/Articles/17652/How-to-Test-Console-Applications
    public class ProgramTests
    {
        private readonly ITestOutputHelper logger;

        public ProgramTests(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        [Theory]
        [InlineData("--ticker MSFT --strategy \"SHORT PUT\" --shortputstrike 230 --expiration 4/16/2022 --price 1.98 --underlying 231.60 --delta 0.3")]
        public void Main_WhenBasicInputIsProvided_ShouldReturnExitCode0(string commandLineArguments)
        {
            // ARRANGE
            int expectedExitCode = 0;

            // ACT
            var exitCode = StartOTLConsoleApp(commandLineArguments);

            // ASSERT
            Assert.Equal(expectedExitCode, exitCode);
        }

        /// <summary>
        /// Starts the LTL console app and waits for exit code
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns>The LTL Console exit code</returns>
        private int StartOTLConsoleApp(string arguments)
        {
            // Initialize process here
            Process proc = new Process();
            proc.StartInfo.FileName = "OTLConsoleDataCollector.exe";
            // add arguments as whole string
            proc.StartInfo.Arguments = arguments;

            // use it to start from testing environment
            proc.StartInfo.UseShellExecute = false;

            // redirect outputs to have it in testing console
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            // set working directory
            proc.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            const string logFileName = "app.log";

            if (File.Exists(logFileName))
            {
                File.Delete(logFileName);
            }

            // start and wait for exit
            proc.Start();
            proc.WaitForExit();

            logger.WriteLine(proc.StandardOutput.ReadToEnd());
            logger.WriteLine(proc.StandardError.ReadToEnd());

            return proc.ExitCode;
        }
    }
}