using AutoMapper;
using CallApp.Application.Exceptions;
using CallApp.Application.Wrappers;
using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Features.Account.Commands
{
    public class UpdateUserAsyncCommand : IRequest<Response<bool>>
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IDNumber { get; set; }
    }

    public class UpdateUserAsyncCommandHandler : IRequestHandler<UpdateUserAsyncCommand, Response<bool>>
    {

        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UpdateUserAsyncCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Response<bool>> Handle(UpdateUserAsyncCommand request, CancellationToken cancellationToken)
        {
            var user = await _uow.userProfileRepository.FindFirst(x => x.UserId == request.UserId);
            if (user == null) throw new ApiException("User doesn't exist");
            var userProfile = _mapper.Map(request, user);
            await _uow.userProfileRepository.Update(user, cancellationToken);

            return new Response<bool>(true, "User has been updated");
        }
    }
}
