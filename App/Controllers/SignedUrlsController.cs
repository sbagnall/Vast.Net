using ImdCloud;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace App.Controllers
{
    public class SignedUrlsController : Controller
    {
        private readonly IIngestMedia ingestMedia;

        public SignedUrlsController(IIngestMedia ingestMedia)
        {
            this.ingestMedia = ingestMedia;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int fileSize, string fileName, string userToken, CancellationToken token)
        {
            var preSignedUrls = await ingestMedia.GeneratePreSignedURL(fileSize, fileName, userToken, token);

            return Ok(preSignedUrls);
        }
    }
}
