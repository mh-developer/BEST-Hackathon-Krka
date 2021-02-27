using AutoMapper;
using Hackathon.Domain.CompanyAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(
            ICompanyRepository companyRepository,
            IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> CreateAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyDto));
            }

            var existingCompany = await _companyRepository.FilterAsync(x => x.Name == companyDto.Name);
            if (existingCompany.Count != 0)
            {
                throw new Exception("Company with same name already exists.");
            }

            var company = _mapper.Map<CompanyDto, Company>(companyDto);

            _companyRepository.Add(company);

            await _companyRepository.SaveChangesAsync();

            return _mapper.Map<Company, CompanyDto>(company);
        }

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();

            return _mapper.Map<List<Company>, List<CompanyDto>>(companies);
        }

        public async Task<CompanyDto> GetAsync(Guid companyId)
        {
            if (companyId == default)
            {
                throw new ArgumentException("Company id is invalid.", nameof(companyId));
            }

            var company = await _companyRepository.GetAsync(companyId);
            if (company == null)
            {
                throw new Exception($"Could not find company with id = '{companyId}'.");
            }

            return _mapper.Map<Company, CompanyDto>(company);
        }

        public async Task<CompanyDto> GetByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is invalid.", nameof(name));
            }

            var company = await _companyRepository.FilterAsync(x => x.Name.ToUpper().Equals(name.ToUpper()));
            if (company.Count == 0)
            {
                throw new Exception($"Could not find user with name = '{name}'");
            }

            return _mapper.Map<Company, CompanyDto>(company[0]);
        }

        public async Task RemoveAsync(Guid companyId)
        {
            if (companyId == default)
            {
                throw new ArgumentException("Company id is invalid.", nameof(companyId));
            }

            var company = await _companyRepository.GetAsync(companyId);
            if (company == null)
            {
                throw new Exception($"Could not find company with id = '{companyId}'.");
            }

            _companyRepository.Remove(company);

            await _companyRepository.SaveChangesAsync();
        }

        public async Task UpdateAsync(CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                throw new ArgumentNullException(nameof(companyDto));
            }

            if (companyDto.Id == default)
            {
                throw new ArgumentException("Company id is invalid.", nameof(companyDto.Id));
            }

            var company = await _companyRepository.GetAsync(companyDto.Id);
            if (company == null)
            {
                throw new Exception($"Could not find company with id = '{companyDto.Id}'.");
            }

            _mapper.Map(companyDto, company);

            await _companyRepository.SaveChangesAsync();
        }
    }
}