using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleEngine.API.Base;
using RuleEngine.API.Model;
using RuleEngine.Data.Entity;


namespace RuleEngine.API.Controllers
{
    public class FieldsController : BaseController
    {
        [HttpGet]
        public async Task<ApiReturn> Get()
        {
            try
            {
                var fields = await Db.Fields.Include(a => a.Type).ToListAsync();

                return new ApiReturn
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = AutoMapper.Mapper.Map<List<MField>>(fields),
                    Errors = null,
                    Message = "Data listed successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiReturn
                {
                    Success = false,
                    Code = ApiStatusCode.InternalServerError,
                    Data = null,
                    Errors = null,
                    Message = ex.Message,
                    InternalMessage = ex.StackTrace
                };
            }
        }

        [HttpPost]
        public async Task<ApiReturn> Post([FromBody] MField value)
        {
            try
            {
                var field = AutoMapper.Mapper.Map<DField>(value);

                Db.Fields.Add(field);
                await Db.SaveChangesAsync();

                return new ApiReturn
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = AutoMapper.Mapper.Map<MField>(field),
                    Errors = null,
                    Message = "Field added successfully."
                };
            }
            catch (Exception ex)
            {
                return new ApiReturn
                {
                    Success = false,
                    Code = ApiStatusCode.InternalServerError,
                    Data = null,
                    Errors = null,
                    Message = ex.Message,
                    InternalMessage = ex.StackTrace
                };
            }
        }
    }
}
