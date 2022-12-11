using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using WebApiCrystalReport.Models;
using System.Web.Http.Cors;

namespace WebApiCrystalReport.Controllers
{
    [RoutePrefix("api/reports")]
    public class ReportsController : ApiController
    {
        private CoreCrudEntities model = new CoreCrudEntities();
        [AllowAnonymous]
        [Route("GenerateProductListReport")]
        [HttpGet]
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public HttpResponseMessage GenerateProductListReport()
        {
            var rd = new ReportDocument();

            rd.Load(Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Reports/Product_List.rpt")));
            rd.SetDataSource(model.Products.Select(p => new {
                Id=p.Id,
                Name=p.Name,
                Category=p.Category.Name,
                Description=p.Description


            }).ToList());

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(ms.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Product_List.pdf"
                };
            result.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            return result;
        }
    }
}
