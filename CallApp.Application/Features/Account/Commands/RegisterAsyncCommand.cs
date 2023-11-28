using AutoMapper;
using CallApp.Application.Exceptions;
using CallApp.Application.Resources;
using CallApp.Application.Wrappers;
using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories.User;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Localization;
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

    public class RegisterAsyncCommandValidator : CustomAbstractValidator<RegisterAsyncCommand>
    {
        public RegisterAsyncCommandValidator(IStringLocalizer<ValidationErrorMessages> localizer) 
        {
            RuleFor(v => v.Email)
                .NotEmpty()
                .WithMessage(x => localizer["EmailIncorrect"]);
        }
    }

    public class RegisterAsyncCommandHandler : IRequestHandler<RegisterAsyncCommand, Response<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<RegisterAsyncCommand> _localizer;

        public RegisterAsyncCommandHandler(IUnitOfWork uow, IMapper mapper, IStringLocalizer<RegisterAsyncCommand> localizer)
        {
            _uow = uow;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Response<bool>> Handle(RegisterAsyncCommand request, CancellationToken cancellationToken)
        {
            var newUser = _mapper.Map<UserEntity>(request);
            var user = await _uow.userRepository.GetAllWhereAsync(x => x.Email == newUser.Email.ToLower());
            if (user.Any()) throw new ApiException("User Already Registered");
            newUser.UserName = request.Email;
            newUser.Email = request.Email.ToLower();
            await _uow.userRepository.CreateUser(newUser, request.Password);
            return new Response<bool>(true, "User succesfully registered");
        }
    }
}
