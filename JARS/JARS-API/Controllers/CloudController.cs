using FirebaseAdmin.Auth;
using Google.Analytics.Data.V1Beta;
using Google.Cloud.Storage.V1;
using JARS_API.BusinessModels;
using JARS_DAL.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace JARS_API.Controllers
{
    [Route("api/v1/cloud")]
    [ApiController]
    public class CloudController : ControllerBase
    {
        private const string UnreadableBody = "Request body is not readable.";
        private const string FaultyBody = "Request body is faulty.";
        private const string GoogleStorage = "https://storage.googleapis.com/";

        private ITransactionRepository _transactionRepository;
        public CloudController(ITransactionRepository repository)
        {
            _transactionRepository = repository;
        }

        /// <summary>
        /// Uploads an image represented by a Base-64 String.
        /// Format: data:image/{imageType};base64,{base-64-string}
        /// </summary>
        /// <returns>The URL for the image.</returns>
        [HttpPost("image")]
        [Authorize]
        public async Task<ActionResult> UploadImage()
        {
            string? body = null;
            if (Request.Body == null)
            {
                return BadRequest(UnreadableBody);
            }
            StreamReader streamReader = new StreamReader(Request.Body);
            try
            {
                if (Request.Body.CanSeek) Request.Body.Seek(0, SeekOrigin.Begin);
                if (Request.Body.CanRead) body = await streamReader.ReadToEndAsync();
            }
            catch (Exception)
            {
                return BadRequest(UnreadableBody);
            }
            if (body == null)
            {
                return BadRequest(UnreadableBody);
            }
            string? dataType = null;
            string? bodyData = null;
            string? fileExtension = null;
            try
            {
                string[] bodyParts = body.Split(',');
                if (bodyParts.Length == 2)
                {
                    bodyData = bodyParts[1];
                    string[] metadata = bodyParts[0].Split(';');
                    if (metadata.Length == 2)
                    {
                        string[] dataIdentifier = metadata[0].Split(':');
                        if (dataIdentifier.Length == 2)
                        {
                            dataType = dataIdentifier[1];
                            string[] type = dataType.Split('/');
                            if (type.Length == 2)
                            {
                                fileExtension = $".{type[1]}";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest(FaultyBody);
            }
            if (bodyData == null)
            {
                return BadRequest(FaultyBody);
            }
            try
            {
                string bucketName = "jars-c19f8.appspot.com";

                byte[] bodyByte = Convert.FromBase64String(bodyData);

                StorageClient storageClient = StorageClient.Create();
                MemoryStream stream = new MemoryStream(bodyByte);
                Google.Apis.Storage.v1.Data.Object gObject = storageClient.UploadObject(bucketName, $"{Guid.NewGuid()}{fileExtension}", dataType, stream);

                Dictionary<string, string> jsonResponse = new Dictionary<string, string>();
                jsonResponse.Add("imageUrl", $"{GoogleStorage}{gObject.Bucket}/{gObject.Name}");
                return Ok(jsonResponse);
            }
            catch (Exception)
            {
                return BadRequest(FaultyBody);
            }
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            BetaAnalyticsDataClient client = new BetaAnalyticsDataClientBuilder
            {
                CredentialsPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"),
            }.Build();

            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            List<string> dateRanges = new List<string>();
            for (int i = -13; i < 0; i++)
            {
                dateRanges.Add(DateTime.Now.AddDays(i).ToString("yyyy-MM-dd"));
            }

            RunReportRequest farRequest = new RunReportRequest
            {
                Property = "properties/" + 306844081,
                Dimensions = { new Dimension { Name = "city" }, },
                Metrics = {
                    new Metric { Name = "active28DayUsers" },
                    new Metric { Name = "newUsers" },
                    new Metric { Name = "totalUsers" },
                },
                DateRanges = {
                    new DateRange { StartDate = dateRanges[0], EndDate = dateRanges[1], Name = "13-12 days ago" },
                    new DateRange { StartDate = dateRanges[2], EndDate = dateRanges[3], Name = "11-10 days ago" },
                    new DateRange { StartDate = dateRanges[4], EndDate = dateRanges[5], Name = "9-8 days ago" },
                    new DateRange { StartDate = dateRanges[6], EndDate = dateRanges[7], Name = "7-6 days ago" },
                },
            };

            RunReportRequest closeRequest = new RunReportRequest
            {
                Property = "properties/" + 306844081,
                Dimensions = { new Dimension { Name = "city" }, },
                Metrics = {
                    new Metric { Name = "active28DayUsers" },
                    new Metric { Name = "newUsers" },
                    new Metric { Name = "totalUsers" },
                },
                DateRanges = {
                    new DateRange { StartDate = dateRanges[8], EndDate = dateRanges[9], Name = "5-4 days ago" },
                    new DateRange { StartDate = dateRanges[10], EndDate = dateRanges[11], Name = "3-2 days ago" },
                    new DateRange { StartDate = dateRanges[12], EndDate = currentDate, Name = "yesterday-today" },
                },
            };

            BatchRunReportsRequest requests = new BatchRunReportsRequest();
            requests.Requests.Add(farRequest);
            requests.Requests.Add(closeRequest);
            requests.Property = "properties/" + 306844081;

            var response = await client.BatchRunReportsAsync(requests);

            List<AnalyticsReport> farActive28Reports = new();
            List<AnalyticsReport> closeActive28Reports = new();

            List<AnalyticsReport> farNewReports = new();
            List<AnalyticsReport> closeNewReports = new();

            List<AnalyticsReport> farTotalReports = new();
            List<AnalyticsReport> closeTotalReports = new();


            foreach (var row in response.Reports[0].Rows)
            {
                if (row.DimensionValues[0].Value == "Ho Chi Minh City")
                {
                    farActive28Reports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[0].Value)));
                    farNewReports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[1].Value)));
                    farTotalReports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[2].Value)));
                }
            }

            foreach (var row in response.Reports[1].Rows)
            {
                if (row.DimensionValues[0].Value == "Ho Chi Minh City")
                {
                    closeActive28Reports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[0].Value)));
                    closeNewReports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[1].Value)));
                    closeTotalReports.Add(new AnalyticsReport(row.DimensionValues[1].Value, int.Parse(row.MetricValues[2].Value)));
                }
            }

            Dictionary<string, object> reports = new();

            farActive28Reports.Reverse();
            closeActive28Reports.Reverse();
            farActive28Reports.AddRange(closeActive28Reports);

            farNewReports.Reverse();
            closeNewReports.Reverse();
            farNewReports.AddRange(closeNewReports);

            farTotalReports.Reverse();
            closeTotalReports.Reverse();
            farTotalReports.AddRange(closeTotalReports);

            reports.Add("active28DayUsers", farActive28Reports);
            reports.Add("newUsers", farNewReports);
            reports.Add("totalUsers", farTotalReports);

            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("reports", reports);
            data.Add("todayTransactions", (await _transactionRepository.GetTransactionsFromDate(DateTime.Today)).Count());
            return Ok(data);
        }
    }
}
