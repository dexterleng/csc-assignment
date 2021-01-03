using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Clarifai.Api;
using Clarifai.Channels;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CSCTask7.Controllers
{
    public class ReceiptController : ApiController
    {
        public async Task<IHttpActionResult> PostFormData()
        {
            // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/sending-html-form-data-part-2
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                return StatusCode(HttpStatusCode.UnsupportedMediaType);
            }

            MultipartFileData file;

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                file = provider.FileData.First();
            }
            catch (System.Exception e)
            {
                return InternalServerError();
            }

            var recognizedForm = await AzureReceiptRecognition(file.LocalFileName);
            var clarifaiGeneralModel = await ClarifaiGeneralModel(file.LocalFileName);

            return Ok(new
            {
                azureRecognizedForm = recognizedForm,
                clarifaiGeneralModel = clarifaiGeneralModel
            });
        }

        private async Task<RecognizedForm> AzureReceiptRecognition(String receipt)
        {
            var stream = File.Open(receipt, FileMode.Open);

            var endpoint = Config.AzureEndpoint;
            var apiKey = Config.AzureKey;
            var client = new FormRecognizerClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            var operation = await client.StartRecognizeReceiptsAsync(stream);

            await operation.WaitForCompletionAsync();


            if (!operation.HasValue)
            {
                return null;
            }

            var form = operation.Value.Single();
            
            return form;
        }

        private async Task<Output> ClarifaiGeneralModel(String receipt)
        {
            var stream = File.Open(receipt, FileMode.Open);
            var client = new V2.V2Client(ClarifaiChannel.Grpc());

            var metadata = new Metadata
            {
                {"Authorization", "Key " + Config.ClarifaiKey}
            };

            var response = await client.PostModelOutputsAsync(
                    new PostModelOutputsRequest()
                    {
                        // general model id
                        ModelId = "aaa03c23b3724a16a56b629203edc62c",
                        Inputs =
                        {
                            new List<Input>()
                            {
                                new Input()
                                {
                                    Data = new Data()
                                    {
                                        Image = new Image()
                                        {
                                            Base64 = await Google.Protobuf.ByteString.FromStreamAsync(stream)
                                        }
                                    }
                                }
                            }
                        }
                    },
                    metadata
            );

            if (response.Status.Code != Clarifai.Api.Status.StatusCode.Success) {
                throw new Exception("Post model outputs failed, status: " + response.Status.Description);
            }

            var output = response.Outputs.First();

            return output;
        }
    }
}
