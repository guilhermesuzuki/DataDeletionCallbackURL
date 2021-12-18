using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DataDeletionCallbackURL.Controllers
{
    public class SocialNetworkController : Controller
    {
        [HttpPost]
        public ActionResult FacebookDelete()
        {
            string signed_request = Request.Form["signed_request"];

            if (!String.IsNullOrEmpty(signed_request))
            {
                var split = signed_request.Split('.');

                if (string.IsNullOrWhiteSpace(split[0]) == false)
                {
                    int mod4 = split[0].Length % 4;
                    if (mod4 > 0) split[0] += new string('=', 4 - mod4);

                    split[0] = split[0]
                        .Replace('-', '+')
                        .Replace('_', '/');
                }

                if (string.IsNullOrWhiteSpace(split[1]) == false)
                {
                    int mod4 = split[1].Length % 4;
                    if (mod4 > 0) split[1] += new string('=', 4 - mod4);

                    split[1] = split[1]
                        .Replace('-', '+')
                        .Replace('_', '/');
                }

                var dataRaw = Encoding.UTF8.GetString(Convert.FromBase64String(split[1]));

                // JSON object
                var json = JObject.Parse(dataRaw);

                var appSecretBytes = Encoding.UTF8.GetBytes("Put your APP Secret here");
                var hmac = new System.Security.Cryptography.HMACSHA256(appSecretBytes);
                var expectedHash = Convert.ToBase64String(hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(signed_request.Split('.')[1])))
                    .Replace('-', '+')
                    .Replace('_', '/');

                if (expectedHash != split[0])
                {
                    return BadRequest();
                }

                //*********************
                //Delete your data here
                //*********************

                if (json != null)
                {
                    return Json(new {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        url = Url.Action("FacebookInfo", "SocialNetwork", new { id = json.GetValue("user_id").ToString() }, HttpContext.Request.Scheme),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        confirmation_code = json.GetValue("user_id").ToString(),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    });
                }
            }

            //bad request
            return BadRequest();
        }

        [HttpGet]
        public ActionResult FacebookInfo(string id) 
        {
            return View();
        }
    }
}
