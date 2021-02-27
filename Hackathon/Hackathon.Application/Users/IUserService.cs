using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Users
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();

        Task<UserDto> GetAsync(Guid userId);

        Task<UserDto> GetByEmailAsync(string email);

        Task<IdentityResult> CreateAsync(UserDto userDto);

        Task UpdateAsync(UserDto userDto);

        Task RemoveAsync(Guid userId);
    }
}