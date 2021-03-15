using System;
using System.Collections.Generic;
using CommandLine;
using Newtonsoft.Json;
using LTL.Business;
using LTL.Common;
using LTL.Plugins;
using ILogger = NLog.ILogger;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LTL.UI.Console
{

    public class Program
    {
        private static ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private static string SpreadSheetId = "1PNK_qz6XUwWfMd6cV5mbrd21L7bcNRB6PweLYJSCWqs";
        private static string GoogleSheetsCredsFilePath;
        private static readonly string ApplicationName = "Options Trades Logger";
        private static readonly string TargetGoogleSheetsRange = "Automated-logs";

        /// <summary>
        /// Main entry point of the console application
        /// </summary>
        /// <param name="args">Parameters passed into the console</param>
        /// <returns>Exit code for the console. 0 means success. 1 means validation error. 2 Means general error</returns>
        public static int Main(string[] args)
        {
            CreateHostBuilder(args).Build();

            logger.Info("Starting console application");
            logger.Debug("Arguments passed: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(args));

            var exitCode = (int)ProcessLoggingRequest(args);
            
            //await host.StopAsync(); // TODO: What is this for?
            return exitCode;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    logger.Debug("Start up: Configuring the app...");
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    // TODO: Inject dependencies and configuration values. This is awesome!
                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    IConfigurationRoot configurationRoot = configuration.Build();

                    GoogleSheetsCredsFilePath = configurationRoot.GetSection(nameof(GoogleSheetsCredsFilePath)).Value;

                    logger.Debug("App configured successfully!");
                    logger.Debug("GoogleSheetsCredsFilePath = {GoogleSheetsCredsFilePath}", GoogleSheetsCredsFilePath);
                });

        /// <summary>
        /// Parses input, processes the trade and logs it into the destination target
        /// </summary>
        /// <param name="inputArguments">Parameters passed into the console</param>
        /// <returns>Exit code for the console. 0 means success. 1 means validation error. 2 Means general error</returns>
        private static ExitCode ProcessLoggingRequest(string[] inputArguments)
        {
            ExitCode exitCode = ExitCode.Success;
            logger.Debug("Parsing user input...");
            WriteToCustomer("Welcome to Options Trades Logger! Please enter your trade!");
            try
            {
                var parser = Parser.Default; //Subject to change. I should be able to replace parsers.

                parser.ParseArguments<RequiredFields>(inputArguments)
                    .WithParsed(OrchestrateLoggingOperation)
                    .WithNotParsed(NotParsed);
            }
            catch (ValidationException ValidationException)
            {
                logger.Error(ValidationException.ToString());
                WriteToCustomerInRed($"Validation failure: {ValidationException.Message}");
                exitCode = ExitCode.ValidationError;
            }
            catch (Exception exception)
            {
                WriteToCustomer("Oops, ran into an error.");
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

        /// <summary>
        /// Root composition for the LTL.Console application. Responsible for instantiating dependencies and orchestration of the logging operation.
        /// </summary>
        /// <param name="fields"></param>
        private static void OrchestrateLoggingOperation(RequiredFields fields)
        {
            logger.Debug("Parsing successful. Validating fields...");

            // Manual root composition. Good for now until we use Ioc container!
            var TradeDataMapper = new TradeDataDtoMapper((strategy) => StrategyParser.ParseStrategy(strategy));
            TradeDataDto tradeDataDto = new TradeDataDto();
            TradeDataMapper.Map(fields, tradeDataDto);

            IParserValidator validator = new ParserValidator();
            validator.Validate(tradeDataDto);

            logger.Debug("Parsed input is valid. Proceeding...");
            logger.Debug("Valid, raw fields:{fields}", JsonConvert.SerializeObject(fields), null);

            IFieldCalculator fieldCalculator = new FieldCalculator(
                new DateTimeProvider(),
                logger);
            TradeDataDto calculatedTradeData = fieldCalculator.ComputeCalculatedFields(tradeDataDto);
            logger.Debug("Calculated fields:{fields}", JsonConvert.SerializeObject(fields), null);

            // Step 1: initialize and authenthicate the data dumper (Subject to change. Can be changed to just genrate a CSV or visually display in the UI)
            IDataDumper dataDumper = new GoogleSheetsDataDumper(
                ApplicationName,
                logger,
                GoogleSheetsCredsFilePath,
                SpreadSheetId);

            // Step 2: Dump the information into the desired target.
            dataDumper.Dump(
                calculatedTradeData,
                new Dictionary<string, object>
                { { "targetGoogleSheetsRange", TargetGoogleSheetsRange }
                });

            WriteToCustomer("Logging operation completed!");
        }

        public static void WriteToCustomer(string message) => System.Console.WriteLine(message);
        public static void WriteToCustomerInRed(string message)
        {
            var originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            WriteToCustomer(message);
            System.Console.ForegroundColor = originalColor;
        }
    }
}
