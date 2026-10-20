using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticleServiceReference;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace WEB_API.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly WcfArticeServiceClient client;

        public ArticleController()
        {
            client = new WcfArticeServiceClient();
        }

        [HttpPost("add/{email}")]
        [Authorize(Roles = "seller")]
        public ActionResult CreateArticle(string email, [FromBody] ArticleDto articleDto)
        {
            try
            {
                return Ok(client.AddArticleAsync(email, articleDto));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message + "Internal server error!");
            }
        }

        [HttpGet]
        [Authorize(Roles = "buyer")]
        public ActionResult GetArticles()
        {
            try
            {
                return Ok(client.GettArticlesAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpGet("get-articles-by-email/{email}")]
        [Authorize(Roles = "seller")]
        public ActionResult GetArticlesByEmailForSeller(string email)
        {
            try
            {
                return Ok(client.GetArticlesByEmailForSellerAsync(email));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpPost("get-articles-for-cart")]
        [Authorize(Roles = "buyer")]
        public ActionResult GetArticlesByEmailForSeller([FromBody] List<int> articles)
        {
            try
            {
                return Ok(client.GettArticlesForCartAsync(ConvertFromListToArray(articles)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "seller")]
        public ActionResult DeleteArticle(int id)
        {
            try
            {
                client.DeleteArticleByIdAsync(id);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpPatch("update")]
        [Authorize(Roles = "seller")]
        public ActionResult UpdateArticle([FromBody] ArticleDto articleDto)
        {
            try
            {
                return Ok(client.UpdateArticleAsync(articleDto));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error!");
            }
        }

        [HttpPost("upload-picture/{id}")]
        public ActionResult UploadPicture(IFormFile image, int id)
        {
            try
            {
                return Ok(client.UploadImageAsync(null, id));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private int[] ConvertFromListToArray(List<int> ids){
            int[] rez = new int[ids.Count];
            for (int i = 0; i < ids.Count(); i++) {
                rez[i] = ids[i];
            }

            return rez;
        }
    }
}
