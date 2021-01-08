using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace bruhhhhhhh
{
	public partial class WeatherServiceForm : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			UriBuilder url = new UriBuilder();
			url.Scheme = "https";// Same as "http://"

			url.Host = "api.data.gov.sg/v1";
			url.Path = "environment/24-hour-weather-forecast";// change to v2
			//url.Query = "q=china&format=xml&num_of_days=5&key=x35ahuadjhmdp5rb75ddw2ha";

			//Make a HTTP request to the global weather web service
			string res = MakeRequest(url.ToString());
			if (res != null)
			{
				Response.ContentType = "application/json";
				Response.Write(res);

			}
			else
			{
				Response.ContentType = "text/html";
				Response.Write("<h2> error  accessing web service </h2>");
			}
		}

		private string MakeRequest(string requestUrl)
		{
			try
			{
				var request = WebRequest.Create(requestUrl) as HttpWebRequest;
				var response = request.GetResponse() as HttpWebResponse;

				var serializer = new JsonSerializer();
				using (var reader = new StreamReader(response.GetResponseStream()))
					return reader.ReadToEnd();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
