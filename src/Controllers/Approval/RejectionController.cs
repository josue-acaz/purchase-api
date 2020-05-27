using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Voartec.Helpers;
using Voartec.Services;
using System.Threading.Tasks;
using System.Dynamic;
using Voartec.Models;
using Newtonsoft.Json.Linq;
using Voartec.Cryptography;
using Serilog;

namespace Voartec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RejectionController : ControllerBase
    {
        private ErrorHandler errorHandler = new ErrorHandler();
        private RejectionService service = new RejectionService();
        private Token token = new Token();

        [HttpPost("{id}/technical")] /// <sumary>: Aprovação Técnica
        public IActionResult TechnicalRejection(int id, [FromBody]RequestItem[] items)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            try
            {
                return Ok(service.TechnicalRejection(id, items, use_id, "update"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }
    }
}