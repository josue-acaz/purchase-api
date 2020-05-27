using System;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Services;
using Serilog;
using Newtonsoft.Json;
using Voartec.Cryptography;

namespace Voartec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestItemController : ControllerBase
    {
        private RequestItemService item_service = new RequestItemService();
        private ItemStatusService status_service = new ItemStatusService();
        private Token token = new Token();
        private ErrorHandler errorHandler = new ErrorHandler();

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(item_service.GetById(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]RequestItem item)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            try
            {
                return Ok(item_service.Post(item, use_id, "create"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]RequestItem item)
        {
            //int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            int use_id = 1;

            try
            {
                return Ok(item_service.Update(id, item, use_id, "update"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(item_service.Delete(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet]
        public IActionResult List(string param="{}")
        {
            Console.WriteLine("Listando itens da requisição");
            try
            {
                return Ok(item_service.List(param));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                return Ok(item_service.Cancel(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet("{id}/requestDependencies")]
        public IActionResult GetRequestDependencies(int id)
        {
            Console.WriteLine("Obtendo as dependências da requisição...");
            try
            {
                return Ok(item_service.GetRequestDependencies(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet("{id}/itemsByRequest")]
        public IActionResult GetItemsByRequest(int id)
        {
            try
            {
                return Ok(item_service.GetItemsByRequest(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet("autocomplete")] /// <sumary> --> função autocompletar na busca de itens
        public IActionResult AutoComplete(string search)
        {
            Console.WriteLine("Listando resultados para item...");
            var param = new { text_search = search, row_per_page = 50 };

            try
            {
                return Ok(item_service.List(JsonConvert.SerializeObject(param)).data);
            }
            catch (Exception e)
            {
                
                return BadRequest(errorHandler.DealError(e));
            }
        }
    }
}
