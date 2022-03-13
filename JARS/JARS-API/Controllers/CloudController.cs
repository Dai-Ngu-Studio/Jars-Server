using Google.Cloud.Storage.V1;
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
                return Ok($"{GoogleStorage}{gObject.Bucket}/{gObject.Name}");
            }
            catch (Exception)
            {
                return BadRequest(FaultyBody);
            }
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            return Ok();
        }
    }
}
