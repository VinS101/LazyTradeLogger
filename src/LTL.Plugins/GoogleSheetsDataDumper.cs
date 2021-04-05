using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using NLog;
using LTL.Business;
using static LTL.Common.EnumExtensions;

namespace LTL.Plugins
{
    /// <summary>
    /// Plugin responsible for dumping trade data into Google Sheets!
    /// </summary>
    public class GoogleSheetsDataDumper : IDataDumper
    {
        /// <summary>
        /// The file token.json stores the user's access and refresh tokens, and is created
        //    automatically when the authorization flow completes for the first time.
        /// </summary>
        private const string tokenPath = "token1.json";
        private readonly ILogger logger;
        private readonly string spreadsheetId;
        private readonly SheetsService service;
        private static UserCredential credential;
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };

        public GoogleSheetsDataDumper(string applicationName, ILogger logger, string googleSheetsCredsFilePath, string spreadsheetId)
        {
            this.logger = logger;
            this.spreadsheetId = spreadsheetId;
            logger.Debug("Initializing the {0} and setting up dependencies...", typeof(GoogleSheetsDataDumper));
            logger.Debug("SpreadsheetId = {0}", spreadsheetId);
            
            UsingGoogleSheetsCredsFileStream(googleSheetsCredsFilePath, (googleSheetsCredsFilePath, stream) 
                => AuthenthicateToGoogleSheets(googleSheetsCredsFilePath, stream));

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });

            logger.Debug("Google sheets data dumper is initialized!");
        }

        private void UsingGoogleSheetsCredsFileStream(string googleSheetsCredsFilePath, Func<FileStream,string,UserCredential> authenticate)
        {
            try
            {
                using (var stream = new FileStream(googleSheetsCredsFilePath, FileMode.Open, FileAccess.Read))
                {
                    logger.Debug("Authenthicating with Google sheets!");
                    credential = authenticate(stream, tokenPath);
                }   
            }
            catch (System.IO.FileNotFoundException exception)
            {
                logger.Error(exception, $"The Google Sheets creds file is not found: {googleSheetsCredsFilePath}");
                throw;
            }
        }

        private UserCredential AuthenthicateToGoogleSheets(FileStream stream, string tokenPath) => GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(tokenPath, true)).Result;

        public void Dump(TradeDataDto fields, Dictionary<string, object> parameters)
        {
            logger.Debug("Preparing to do a dump to google sheets!");

            // Define request parameters.
            string range = parameters["targetGoogleSheetsRange"].ToString();

            ValueRange requestBody = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new object[]
                    {
                        fields.Ticker,
                        fields.Strategy.UserFriendlyDescription().ToUpper(),
                        fields.OpenDate.ToString(),
                        fields.ExpiryDate.Date.ToShortDateString(),
                        fields.Quantity.ToString(),
                        fields.Delta.ToString(),
                        string.Empty,
                        fields.Price.ToString(),
                        fields.Underlying.ToString(),
                        fields.ShortPutStrike.ToString(),
                        fields.LongPutStrike.ToString(),
                        fields.ShortCallStrike.ToString(),
                        fields.LongCallStrike.ToString(),
                        fields.TotalCredit.ToString(),
                        fields.TotalDebit.ToString(),
                        fields.MaxRisk.ToString(),
                        fields.DTE.ToString(),
                        fields.RiskRewardRatio.HasValue? fields.RiskRewardRatio.ToString() : "N/A",
                        fields.TradeStatus.GetEnumName(),
                        fields.DaysInTrade.ToString(),
                    }
                }
            };

            SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;  // TODO: Update placeholder value.
            SpreadsheetsResource.ValuesResource.AppendRequest request = service.Spreadsheets.Values.Append(requestBody, spreadsheetId, range);
            request.ValueInputOption = valueInputOption;

            AppendValuesResponse response = request.Execute();

            logger.Debug(JsonConvert.SerializeObject(response));

            logger.Debug("Completed dumping data into google sheets!");
        }
    }
}