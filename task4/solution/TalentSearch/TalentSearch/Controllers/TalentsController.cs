using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;
using System.Web.Http.Results;
using Newtonsoft.Json;
using System.Text;

namespace TalentSearch.Controllers
{
    public class TalentsController : ApiController
    {
        
		// GET: api/Talents
        public HttpResponseMessage Get()
        {
			string data = File.ReadAllText(HttpContext.Current.Server.MapPath("~") + "searchTalent\\data.json");

			var response = Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(data, Encoding.UTF8, "application/json");
			return response;
        }

    }
}
