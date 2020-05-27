using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voartec.Helpers;
using Voartec.Models;
using Voartec.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Voartec.Cryptography;

namespace Voartec.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersPermissionsController : ControllerBase
    {
        private UserPermissionService service = new UserPermissionService();
        private Token token = new Token();
        private ErrorHandler errorHandler = new ErrorHandler();

        [HttpGet]
        public IActionResult Get(string param = "{}")
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.List(param, use_id, "read"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet("autocomplete")]
        public IActionResult AutoComplete(string search)
        {

            var param = new { text_search = search, row_per_page = 50 };

            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.List(JsonConvert.SerializeObject(param), use_id, "read").data);
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }

        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.Get(id, use_id, "read"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserPermission obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(errorHandler.DealErrorModelState(ModelState.SelectMany(v => v.Value.Errors.Select(e => e.Exception))));
            }

            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            if (obj.per_id != 0)
            {
                return BadRequest("Para alteração, utilize o método put.");
            }

            try
            {
                return Ok(service.Save(obj, use_id, "create"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody]UserPermission obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(errorHandler.DealErrorModelState(ModelState.SelectMany(v => v.Value.Errors.Select(e => e.Exception))));
            }

            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            if (obj.per_id == 0)
            {
                return BadRequest("Para inclusão, utilize o método post.");
            }

            try
            {
                return Ok(service.Save(obj, use_id, "update"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.Delete(id, use_id, "delete"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpPut("savecheck")]
        public IActionResult Savecheck([FromBody]dynamic obj)
        {

            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.SaveChecked(obj, use_id, "update"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }

        }

        [HttpPut("applyperfil")]
        public IActionResult Applyperfil([FromBody]dynamic obj)
        {

            int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            try
            {
                return Ok(service.ApplyPerfil(obj, use_id, "applyperfil"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }

        }

    }
}