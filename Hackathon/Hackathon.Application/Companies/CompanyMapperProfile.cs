using AutoMapper;
using Hackathon.Domain.CompanyAggregate;

namespace Hackathon.Application.Companies
{
    public class CompanyMapperProfile : Profile
    {
        public CompanyMapperProfile()
        {
            CreateMap<Company, CompanyDto>();
            CreateMap<CompanyDto, Company>();
        }
    }
}