using System;
using System.Collections.Generic;
using LTL.Business;
using LTL.Common;
using LTL.Plugins;
using Newtonsoft.Json;
using ILogger = NLog.ILogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LTL.UI.Console
{
    public class Program
    {
        private static ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private static GoogleSheetsSettings _googleSheetsSettings;
        private static readonly string ApplicationName = "Options Trades Logger";

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

            var parser = new CommandLineParserWrapper(logger, OrchestrateLoggingOperation, WriteToCustomer, WriteToCustomerInRed);
            var exitCode = (int)parser.ParseUserInput(args);

            return exitCode;
        }

        /// <summary>
        /// Composes the dependencies of the application
        /// </summary>
        /// <param name="args">Parameters passed into the console</param>
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                logger.Debug("Start up: Configuring the app...");
                configuration.Sources.Clear();

                IHostEnvironment env = hostingContext.HostingEnvironment;

                // TODO: Inject dependencies and configuration values. This is awesome!
                configuration
                    .AddJsonFile("appsettings.json", optional : true, reloadOnChange : true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                IConfigurationRoot configurationRoot = configuration.Build();

                _googleSheetsSettings = configurationRoot.GetSection(nameof(GoogleSheetsSettings)).Get<GoogleSheetsSettings>();

                logger.Debug("App configured successfully!");
                logger.Debug("Google sheets settings: {settings}", JsonConvert.SerializeObject(_googleSheetsSettings));
            });

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
                _googleSheetsSettings.GoogleSheetsCredsFilePath,
                _googleSheetsSettings.GoogleSpreadSheetId);

            // Step 2: Dump the information into the desired target.
            dataDumper.Dump(
                calculatedTradeData,
                new Dictionary<string, object>
                { { "targetGoogleSheetsRange", _googleSheetsSettings.TargetGoogleSheetsRange }
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
