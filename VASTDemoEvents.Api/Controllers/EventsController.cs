using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Linq;

namespace VASTDemoEvents.Api.Controllers
{
    public class EventsController : Controller
    {
        public EventsController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("PostgreSQL");

            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        private string InsertEventQuery = @"
insert into vast.events (logged, body, url, verb, referrer, eventname, campaign_id) values (now(), @body, @url, @verb, @referrer, @eventname, @campaign)
";

        private string InsertCampaignQuery = @"
insert into vast.campaigns (""name"", tag) values (@name, @tag)
";

        private string connectionString;

        [HttpGet, Route("tag")]
        public dynamic Tag([FromQuery] int? id = null)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                string tag;

                if (id == null)
                {
                    tag = conn.ExecuteScalar<string>("select tag from vast.campaigns order by id desc limit 1");
                }
                else
                {
                    tag = conn.ExecuteScalar<string>("select tag from vast.campaigns where id = @id desc limit 1",
                        new {id});
                }

                return Content(tag, "text/xml");
            }
        }

        [HttpGet, Route("")]
        public ContentResult Index()
        {
            return base.Content(System.IO.File.ReadAllText("./index.html"), "text/html");
        }

        [HttpGet, Route("player")]
        public ContentResult Player()
        {
            return base.Content(System.IO.File.ReadAllText("./player.html"), "text/html");
        }

        [HttpGet, HttpPost, HttpPut, Route("all")]
        public string SaveEvent([FromQuery] string eventname)
        {
            Request.EnableBuffering();

            //using (var stream = new StreamReader(Request.Body))
            //{
            var url = Request.GetDisplayUrl();
            var verb = Request.Method;
            var referrer = Request.Headers["Referer"].ToString();

            var body = string.Empty;

            foreach (var header in Request.Headers)
            {
                body += $"{header.Key} : {header.Value.ToString()}\n";
            }

            using (var conn = new NpgsqlConnection(connectionString))
            {
                var campaign = conn.ExecuteScalar<int>("select id from vast.campaigns order by 1 desc limit 1");

                conn.Execute(InsertEventQuery, new
                {
                    eventname,
                    body,
                    url,
                    verb,
                    referrer,
                    campaign
                });
            }
            //}

            return "";
        }

        public class Campaign
        {
            public string Tag { get; set; }
            public string Name { get; set; }
        }

        [HttpPost, Route("save")]
        public void Save([FromBody] Campaign request)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Execute(InsertCampaignQuery, new
                {
                    name = request.Name,
                    tag = request.Tag
                });
            }
        }

        [HttpGet, Route("read")]
        public IEnumerable<dynamic> ReadOut()
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                var campaign = conn.QueryFirst("select * from vast.campaigns order by 1 desc limit 1");

                return conn.Query<dynamic>("select * from vast.events where campaign_id = @id",
                        new {id = (int) campaign.id})
                    .Select(x => new
                    {
                        Name = campaign.name,
                        Logged = x.logged,
                        Url = x.url,
                        Referrer = x.referrer,
                        EventName = x.eventname
                    });
            }
        }
    }
}