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
    public class ApprovalController : ControllerBase
    {
        private ErrorHandler errorHandler = new ErrorHandler();
        private ApprovalService service = new ApprovalService();
        private Token token = new Token();

        [HttpPost("{id}/technical")] /// <sumary>: Aprovação Técnica
        public IActionResult TechnicalApproval(int id, [FromBody]RequestItem[] items)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            try
            {
                return Ok(service.TechnicalApproval(id, items, use_id, "update"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        /*[HttpPost("{id}/administrative")]
        public IActionResult AdministrativeApproval(int id)
        {
            try
            {
                current_status = status_service.CurrentStatus(id);

                // Verifica se o status atual do item é 'Aguardando Aprovação de Compra' (AAC)
                if(current_status == 5)
                {
                    // Altera do status --> Compra Aprovada (CA)
                    status_service.ChangeStatus(id, 7);
                    current_status = status_service.CurrentStatus(id);
                    response.data = current_status;
                }

                return Ok();
            }
            catch (Exception e)
            {
                
                return BadRequest(errorHandler.DealError(e));
            }
        } */

    }
}
