using AutoMapper;
using CallApp.Application.Exceptions;
using CallApp.Application.Wrappers;
using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories.User;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Features.Account.Commands
{
    public class RegisterAsyncCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IDNumber { get; set; }
        public string Password { get; set; }
    }

    public class RegisterAsyncCommandHandler : IRequestHandler<RegisterAsyncCommand, Response<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public RegisterAsyncCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(RegisterAsyncCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newUser = _mapper.Map<UserEntity>(request);
                var userProfile = _mapper.Map<UserProfileEntity>(request);
                var user = await _uow.userRepository.GetAllWhereAsync(x => x.Email == newUser.Email.ToLower());
                if (user.Any()) throw new ApiException("User Already Registered");
                newUser.UserName = request.Email;
                newUser.Email = request.Email.ToLower();
                await _uow.userRepository.CreateUser(newUser, request.Password);
                await _uow.userProfileRepository.CreateUserProfile(userProfile, newUser.Id);
                return new Response<bool>(true, "User succesfully registered");
            }
            catch (Exception ex)
            {
                throw new ApiException("Something went wrong");
            }
        }
    }
}
