using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GoogleAnalyticsService
{
    class Program
    {
        static string clientId = "INSERT CLIENTID HERE";
        static string clientSecret = "INSERT CLIENT SECRET HERE";
        static string gaUser = "INSERT USER HERE";
        static string gaApplication = "My Project";
        static string oauthTokenFilestorage = "MyOAuthStorage";
        private static string profileId = "ga:INSERT PROFILEIDHERE";

        private static AnalyticsService analyticsService;

        private static void Main(string[] args)
        {   
            //Analytics Service Setup         
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                new[] {AnalyticsService.Scope.AnalyticsReadonly},
                gaUser,
                CancellationToken.None,
                new FileDataStore(oauthTokenFilestorage)
                ).Result;

            var service = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = gaApplication
            });

            //The basic request to Google Analytics
            string start = new DateTime(2014, 1, 1).ToString("yyyy-MM-dd");
            string end = new DateTime(2014, 1, 10).ToString("yyyy-MM-dd");
            var query = service.Data.Ga.Get(profileId, start, end, "ga:visitors");

            query.Dimensions = "ga:visitCount, ga:date, ga:visitorType";
            query.Filters = "ga:visitorType==New Visitor";
            query.SamplingLevel = DataResource.GaResource.GetRequest.SamplingLevelEnum.HIGHERPRECISION;

            var response = query.Execute();

            Console.WriteLine("Entries in result: {0}", response.TotalResults);
            Console.WriteLine("You had : {0} new visitors from {1} to {2}"
                , response.TotalsForAllResults.First(), start, end);
            Console.WriteLine("Has more data: {0}", response.NextLink == null);
            Console.WriteLine("Sample data: {0}", response.ContainsSampledData);

            Console.ReadLine();

        }
    }
}
