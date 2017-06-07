using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RuleEngine.API.Base;
using RuleEngine.API.Helper;
using RuleEngine.API.Model;
using RuleEngine.Data.Entity;

namespace RuleEngine.API.Controllers
{
    public class Person
    {
        public string surname { get; set; }
    }

    public class RulesController : BaseController
    {
        [HttpGet]
        public async Task<ApiReturn> Get()
        {
            try
            {
                var rules = await Db.Rules.ToListAsync();

                return new ApiReturn<List<MRuleObject>>
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = AutoMapper.Mapper.Map<List<MRuleObject>>(rules),
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

        [HttpGet("{id}")]
        public async Task<ApiReturn> Get(Guid id)
        {
            try
            {
                var rule = await Db.Rules.FirstOrDefaultAsync(a => a.RuleId == id);

                if (rule == null)
                    return new ApiReturn
                    {
                        Success = false,
                        Code = ApiStatusCode.NotFound,
                        Data = null,
                        Message = "Rule not found."
                    };

                return new ApiReturn<MRuleObject>
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = AutoMapper.Mapper.Map<MRuleObject>(rule),
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
        public async Task<ApiReturn> Post([FromBody] MRuleObject value)
        {
            try
            {
                var rule = new DRule
                {
                    Name = value.Name,
                    Filter = value.Filter,
                    Content = value.Content
                };

                Db.Rules.Add(rule);
                await Db.SaveChangesAsync();

                return new ApiReturn
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = AutoMapper.Mapper.Map<MRule>(rule),
                    Errors = null,
                    Message = "Data added successfully."
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

        [HttpPost("{id}")]
        public ApiReturn Post(Guid id, [FromBody] MRuleRun value)
        {
            try
            {
                const string buildQueryMethodName = "BuildQuery";
                const string toListMethodName = "ToList";
                const BindingFlags toListBindingFlags = (BindingFlags.Public | BindingFlags.Static);

                var ruleContent = JsonConvert.DeserializeObject<FilterRule>(value.RuleContent);
                var fields = GetFields(ruleContent);
                var tempType = RuleEngineTypeBuilder.CompileResultType(fields);
                var listGenericType = typeof(List<>);
                var dataType = listGenericType.MakeGenericType(tempType);
                var content = JsonConvert.DeserializeObject(value.Data, dataType);
                var genericBuildQueryMethod = typeof(QueryBuilder).GetMethod(buildQueryMethodName).MakeGenericMethod(tempType);
                var queryResult = genericBuildQueryMethod.Invoke(null, new[] { content, ruleContent, false, null });
                var enumerableToListMethod = typeof(Enumerable).GetMethod(toListMethodName, toListBindingFlags);
                var genericToListMethod = enumerableToListMethod.MakeGenericMethod(tempType);
                var filteredResult = genericToListMethod.Invoke(null, new[] { queryResult });

                return new ApiReturn
                {
                    Success = true,
                    Code = ApiStatusCode.Success,
                    Data = filteredResult,
                    Errors = null,
                    Message = "Data listed successfully.",
                    InternalMessage = ""
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

        private List<FilterRuleField> GetFields(FilterRule rule)
        {
            var fields = new List<FilterRuleField>();

            if (rule == null)
                return fields;

            if (!string.IsNullOrEmpty(rule.Field) && !string.IsNullOrEmpty(rule.Type))
            {
                fields.Add(new FilterRuleField
                {
                    Field = rule.Field,
                    Type = GetFieldType(rule.Type)
                });
            }

            if (rule.Rules == null || rule.Rules.Count <= 0)
                return fields;

            foreach (var subRule in rule.Rules)
            {
                fields.Add(new FilterRuleField
                {
                    Field = subRule.Field,
                    Type = GetFieldType(subRule.Type)
                });

                if (subRule.Rules == null || subRule.Rules.Count <= 0)
                    continue;

                foreach (var childRule in subRule.Rules)
                {
                    fields.AddRange(GetFields(childRule));
                }
            }

            return fields;
        }

        private string GetFieldType(string typeName)
        {
            Type type = null;

            switch (typeName)
            {
                case "integer":
                    type = typeof(int);
                    break;
                case "double":
                    type = typeof(double);
                    break;
                case "string":
                    type = typeof(string);
                    break;
                case "date":
                case "datetime":
                    type = typeof(DateTime);
                    break;
                case "boolean":
                    type = typeof(bool);
                    break;
            }

            return type != null ? type.FullName : string.Empty;
        }
    }
}