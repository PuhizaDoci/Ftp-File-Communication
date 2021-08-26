using FtpFileCommunication.Service;
using System.Web.Http;

namespace FtpFileCommunication.Controllers
{
    public class FtpController : ApiController
    {
        private readonly FtpService ftpService;
        public FtpController()
        {
            ftpService = new FtpService();
        }

        [Route("api/execute")]
        public IHttpActionResult Execute()
        {
            try
            {
                ftpService.SendToFtp();
                return Ok();
            }
            catch (FluentFTP.FtpException exF)
            {
                return BadRequest(exF.Message);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}