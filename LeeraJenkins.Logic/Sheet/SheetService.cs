using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Sheet
{
    public class SheetService : ISheetService
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private UserCredential credential;

        public SheetsService GetSheetService()
        {
            var credentialsFile = Credentials.credentials;
            using (var stream = new MemoryStream(credentialsFile))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                ).Result;
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
            return service;
        }
    }
}
