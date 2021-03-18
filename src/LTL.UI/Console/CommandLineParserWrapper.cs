using System;
using System.Collections.Generic;
using CommandLine;
using ILogger = NLog.ILogger;
using System.ComponentModel.DataAnnotations;

namespace LTL.UI.Console
{
    /// <summary>
    /// Responsible for parsing console user input
    /// </summary>
    public class CommandLineParserWrapper
    {
        private readonly ILogger logger;
        private readonly Action<RequiredFields> postParsingAction;
        private readonly Action<string> writeToCustomer;
        private readonly Action<string> writeToCustomerInRed;

        public CommandLineParserWrapper(ILogger logger, Action<RequiredFields> postParsingAction, Action<string> writeToCustomer, Action<string> writeToCustomerInRed)
        {
            this.logger = logger;
            this.postParsingAction = postParsingAction;
            this.writeToCustomer = writeToCustomer;
            this.writeToCustomerInRed = writeToCustomerInRed;
        }

        /// <summary>
        /// Parses input, processes the trade and logs it into the destination target
        /// </summary>
        /// <param name="inputArguments">Parameters passed into the console</param>
        /// <returns>Exit code for the console. 0 means success. 1 means validation error. 2 Means general error</returns>
        public ExitCode ParseUserInput(string[] inputArguments)
        {
            ExitCode exitCode = ExitCode.Success;
            logger.Debug("Parsing user input...");
            writeToCustomer("Welcome to Options Trades Logger! Please enter your trade!");
            try
            {
                var parser = Parser.Default; //Subject to change. I should be able to replace parsers.

                parser.ParseArguments<RequiredFields>(inputArguments)
                    .WithParsed(postParsingAction)
                    .WithNotParsed(NotParsed);
            }
            catch (ValidationException ValidationException)
            {
                logger.Error(ValidationException.ToString());
                writeToCustomerInRed($"Validation failure: {ValidationException.Message}");
                exitCode = ExitCode.ValidationError;
            }
            catch (Exception exception)
            {
                writeToCustomer("Oops, ran into an error.");
                logger.Error(exception.ToString());
                exitCode = ExitCode.GeneralError;
            }
            return exitCode;
        }

        private static void NotParsed(IEnumerable<Error> obj)
        {
            // TODO: Initiate a loop if the parsing is failed, so that we keep asking users for input? Or not? Need some research.
            throw new ValidationException("Parsing failed. Please ensure you have provided all the required input parameters.");
        }
    }
}
