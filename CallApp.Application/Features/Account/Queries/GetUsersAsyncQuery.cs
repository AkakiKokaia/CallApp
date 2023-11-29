using AutoMapper;
using CallApp.Application.DTOs.User;
using CallApp.Application.Wrappers;
using CallApp.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Features.Account.Queries
{
    public class GetUsersAsyncQuery : IRequest<Response<List<GetUserResponse>>>
    {
    }
    public class GetUsersAsyncQueryHandler : IRequestHandler<GetUsersAsyncQuery, Response<List<GetUserResponse>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUsersAsyncQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public async Task<Response<List<GetUserResponse>>> Handle(GetUsersAsyncQuery request, CancellationToken cancellationToken)
        {
            var user = _uow.userRepository.GetAll().Include(x => x.UserProfiles);
            var result = _mapper.Map<List<GetUserResponse>>(user);

            return new Response<List<GetUserResponse>>(result);
        }
    }
}
