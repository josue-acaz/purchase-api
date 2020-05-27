using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Voartec.Models;
using Voartec.Helpers;
using Voartec.Services;
using Voartec.Cryptography;
using Newtonsoft.Json;
using Serilog;

namespace Voartec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private RequestService service = new RequestService();
        private ErrorHandler errorHandler = new ErrorHandler();
        private Token token = new Token();

        [HttpGet("{id}")] /// <summary> --> obtém a requisição
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(service.GetById(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPost] /// <summary> --> cria a requisição
        public IActionResult Post([FromBody]Request request)
        {
            try
            {
                return Ok(service.Post(request));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("{id}")] /// <summary> --> atualiza a requisição
        public IActionResult Update(int id, [FromBody]Request request)
        {
            try
            {
                return Ok(service.Update(id, request));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpDelete("{id}")] /// <summary> --> exclui a requisição
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(service.Delete(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet] /// <summary> --> lista as requisições
        public IActionResult List(string param="{}")
        {
            try
            {
                return Ok(service.List(param));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("{id}/cancel")] /// <summary> --> cancela a requisição
        public IActionResult Cancel(int id)
        {
            try
            {
                return Ok(service.Cancel(id));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }
        
        [HttpPost("{id}/send")] /// <summary> --> envia a requisição
        public IActionResult Send(int id, [FromBody] Message msg)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            Console.WriteLine("ID usuário: " + use_id);

            try
            {
                return Ok(service.Send(id, use_id, "finish", msg));
            }
            catch (Exception e)
            {
                
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("{id}/reopen")] /// <summary> --> reabre a requisição
        public IActionResult ReopenRequest(int id)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            try
            {
                return Ok(service.ReopenRequest(id, use_id, "reopen"));
            }
            catch (Exception e)
            {
                
                return BadRequest(errorHandler.DealError(e));
            }
        }
    }
}
