using AutoMapper;
using CallApp.Application.DTOs.User;
using CallApp.Application.Exceptions;
using CallApp.Application.Features.Account.Commands;
using CallApp.Application.Wrappers;
using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Features.Account.Queries
{
    public class GetUserByIdAsyncQuery : IRequest<Response<GetUserResponse>>
    {
        public int Id { get; set; }
    }

    public class GetUserByIdAsyncQueryHandler : IRequestHandler<GetUserByIdAsyncQuery, Response<GetUserResponse>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetUserByIdAsyncQueryHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }
        public async Task<Response<GetUserResponse>> Handle(GetUserByIdAsyncQuery request, CancellationToken cancellationToken)
        {
            var user = await _uow.userRepository.GetById(request.Id);
            var result = _mapper.Map<GetUserResponse>(user);

            return new Response<GetUserResponse>(result);
        }
    }
}
