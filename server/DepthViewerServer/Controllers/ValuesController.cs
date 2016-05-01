using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DepthViewerServer.Controllers
{
    [RoutePrefix("api/v1/values")]
    public class ValuesController : ApiController
    {
        [Route("get")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("get")]
        [HttpGet]
        public string Get([FromUri] int id)
        {
            return "value"+id;
        }

        [Route("post")]
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        [Route("put")]
        [HttpPut]
        public void Put(int id, [FromBody]string value)
        {
        }

        [Route("delete")]
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}
