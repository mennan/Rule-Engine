using RuleEngine.API.Model;
using RuleEngine.Data.Entity;

namespace RuleEngine.API.Config
{
    public static class Maps
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.Initialize(config =>
            {
                config.CreateMap<MRule, DRule>().ReverseMap();
                config.CreateMap<MRuleObject, DRule>().ReverseMap();
                config.CreateMap<MField, DField>().ReverseMap();
                config.CreateMap<MType, DType>().ReverseMap();
            });
        }
    }
}
