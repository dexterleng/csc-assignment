using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using task5.Models;

namespace task5.Controllers
{
	class S3Uploader
	{
		readonly string bucketName;
		private AmazonS3Client client;

		public S3Uploader()
		{
			this.bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET");
			this.client = new AmazonS3Client();
		}

		public async static Task<BitlyResponse> GenerateBitly(string url)
		{
			string bitlyAuth = Environment.GetEnvironmentVariable("BITLY_TOKEN");

			string json = JsonConvert.SerializeObject(new {
				long_url = url
			});

			using (HttpClient c = new HttpClient())
			{
				c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bitlyAuth);

				var res = await c.PostAsync("https://api-ssl.bitly.com/v4/shorten", new StringContent(json, Encoding.UTF8, "application/json"));
				res.EnsureSuccessStatusCode();

				string body = await res.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<BitlyResponse>(body);
			}

		}

		public async Task<string> Upload(string filename, Stream content)
		{
			try
			{
				var a = await client.ListBucketsAsync();
				var fileTransferUtility = new TransferUtility(client);

				await fileTransferUtility.UploadAsync(content, bucketName, filename);
				return $"https://s3.amazonaws.com/{bucketName}/{filename}";
			}
			catch (AmazonS3Exception e)
			{
				Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
				throw;
			}
			catch (Exception e)
			{
				Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
				throw;
			}
		}

	}

	// aws credentials must be placed in ~/.aws/credentials
    public class UploadController : ApiController
    {
		// mb to bytes
		readonly int maxSize = 2 * (int)Math.Pow(10, 6);

		// upload single image to aws and shorten link
		[HttpPost]
		public async Task<IHttpActionResult> UploadImage()
		{
			var file = HttpContext.Current.Request.Files?[0];

			if (file != null && file.ContentLength > 0)
			{
				string filename = Path.GetFileName(file.FileName);

				if (file.ContentLength > maxSize)
					return BadRequest($"File exceeds size of {maxSize}");

				if (!file.ContentType.StartsWith("image"))
					return BadRequest("Submitted file must be an image");

				try
				{
					string resourceUrl = await new S3Uploader().Upload($"{DateTimeOffset.Now.ToUnixTimeSeconds()}_{filename}", file.InputStream);
					BitlyResponse bitly = await S3Uploader.GenerateBitly(resourceUrl);
					return Ok(bitly.Link);
				}
				catch (Exception e)
				{
					return InternalServerError(e);
				}

				//string contents = new StreamReader(file.InputStream).ReadToEnd();
				//Console.WriteLine(contents);
			}

			return BadRequest("Invalid file provided");

		}
    }
}
