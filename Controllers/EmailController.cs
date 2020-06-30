using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Net.Http;
//using System.Web.Http;


namespace SendEmail.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        
        private readonly ILogger<EmailController> _logger;

        protected IConfiguration Configuration { get; }

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.Configuration = configuration;
        }

        [HttpPost]
        [Route("contactus")]
        public async Task<IActionResult> ContactUs(string mailBody)
        {
            try
            {
                var apiKey = this.Configuration.GetValue<string>("SendGridApi");
                
                var client = new SendGridClient(apiKey);

                // Send a Single Email using the Mail Helper with convenience methods and initialized SendGridMessage object
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(this.Configuration.GetValue<string>("FromEmail")),
                    Subject = this.Configuration.GetValue<string>("ContactSubject")
                };
                Content mailContent = new Content("text/html", mailBody);
                msg.AddTo(new EmailAddress(this.Configuration.GetValue<string>("ToEmail")));
                msg.AddContents(new List<Content>() { mailContent });
                var response = await client.SendEmailAsync(msg);
                return Ok(response.StatusCode);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("applyjob")]
        public async Task<IActionResult> ApplyJob(string mailBody)
        {
            try
            {
                var apiKey = this.Configuration.GetValue<string>("SendGridApi");

                var client = new SendGridClient(apiKey);

                // Send a Single Email using the Mail Helper with convenience methods and initialized SendGridMessage object
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(this.Configuration.GetValue<string>("FromEmail"),"no-reply"),
                    Subject = this.Configuration.GetValue<string>("ApplySubject")
                };
                Content mailContent = new Content("text/html", mailBody);
                msg.AddTo(new EmailAddress(this.Configuration.GetValue<string>("ToEmail")));
                msg.AddContents(new List<Content>() { mailContent });
               
                var file = Request.Form.Files[0].OpenReadStream();
                string files;
                using (MemoryStream ms = new MemoryStream())
                {
                    string fileName = Request.Form.Files[0].FileName;
                    file.CopyTo(ms);
                    var bytes = ms.ToArray();
                    files = Convert.ToBase64String(bytes);
                    msg.AddAttachment(fileName, files);
                }
                
                var response = await client.SendEmailAsync(msg);
                return Ok(response.StatusCode);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }
            
        }
    }
}
