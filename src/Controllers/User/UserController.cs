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
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Voartec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserService service = new UserService();
        private ErrorHandler errorHandler = new ErrorHandler();

        [HttpGet("{id}")] /// <summary> --> obtém um usuário
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

        [HttpPost] /// <summary> --> cria um usuário
        public IActionResult Post([FromBody]User user)
        {
            try
            {
                return Ok(service.Post(user));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [AllowAnonymous]
        [HttpPost("authenticate")] /// <summary> --> autentica um usuário
        public IActionResult Authenticate([FromBody]Login login)
        {
            try
            {
                return Ok(service.Authenticate(login.username, login.password, login.system_flag));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }

        }

        [HttpGet("validaToken")]
        public IActionResult valida_token()
        {
            //int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);

            try
            {
                ObjResult objResult = new ObjResult();
                objResult.resultStatus = "success";
                objResult.data = "token validado com suecesso.";
                return Ok(objResult);
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }
        }

        [HttpGet]
        public IActionResult List(string param = "{}")
        {
            //int use_id = token.GetIdUserToken(Request.Headers["Authorization"]);
            int use_id = 1;
            try
            {
                return Ok(service.List(param, use_id, "read"));
            }
            catch (Exception e)
            {
                return BadRequest(errorHandler.DealError(e));
            }

        }
    }
}