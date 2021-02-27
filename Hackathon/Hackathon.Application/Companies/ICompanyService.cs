using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Companies
{
    public interface ICompanyService
    {
        Task<List<CompanyDto>> GetAllAsync();

        Task<CompanyDto> GetAsync(Guid companyId);

        Task<CompanyDto> GetByNameAsync(string name);

        Task<CompanyDto> CreateAsync(CompanyDto companyDto);

        Task UpdateAsync(CompanyDto companyDto);

        Task RemoveAsync(Guid companyId);
    }
}