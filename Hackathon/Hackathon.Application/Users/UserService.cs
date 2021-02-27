using AutoMapper;
using Hackathon.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hackathon.Application.Users
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(
            IUserRepository userRepository,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var userAccounts = await _userRepository.GetAllAsync();

            return _mapper.Map<List<User>, List<UserDto>>(userAccounts);
        }

        public async Task<UserDto> GetAsync(Guid userId)
        {
            if (userId == default)
            {
                throw new ArgumentException("User id is invalid.", nameof(userId));
            }

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new Exception($"Could not find user with id = '{userId}'");
            }

            return _mapper.Map<User, UserDto>(user);
        }

        public async Task<UserDto> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email is invalid.", nameof(email));
            }

            var user = await _userRepository.FilterAsync(x =>
                x.NormalizedEmail.ToUpper().Equals(email.ToUpper()));
            if (user.Count == 0)
            {
                throw new Exception($"Could not find user with email = '{email}'");
            }

            return _mapper.Map<User, UserDto>(user[0]);
        }

        public async Task<IdentityResult> CreateAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            var existingUser =
                await _userRepository.FilterAsync(x => x.NormalizedEmail.ToUpper().Equals(userDto.Email.ToUpper()));
            if (existingUser.Count > 0)
            {
                throw new Exception($"User with email = '{userDto.Email}' already exists.");
            }

            var user = new User()
            {
                Id = userDto.Id != default ? userDto.Id : Guid.NewGuid(),
                Email = userDto.Email,
                UserName = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName
            };

            var identityResult = await _userManager.CreateAsync(user);

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    throw new Exception(error.Description);
                }
            }

            return identityResult;
        }

        public async Task UpdateAsync(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            if (userDto.Id == default)
            {
                throw new ArgumentException("User id is invalid.", nameof(userDto.Id));
            }

            var user = await _userRepository.GetAsync(userDto.Id);
            if (user == null)
            {
                throw new Exception($"Could not find user with id = '{userDto.Id}'.");
            }

            _mapper.Map(userDto, user);

            await _userManager.UpdateAsync(user);
        }

        public async Task RemoveAsync(Guid userId)
        {
            if (userId == default)
            {
                throw new ArgumentException("User id is invalid.", nameof(userId));
            }

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new Exception($"Could not find user with id = '{userId}'.");
            }

            await _userManager.DeleteAsync(user);
        }
    }
}