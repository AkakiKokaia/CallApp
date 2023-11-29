using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories.User
{
    public class UserProfileRepository : GenericRepository<UserProfileEntity>, IUserProfileRepository
    {
        private readonly CallAppDBContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public UserProfileRepository(CallAppDBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IUnitOfWork unitOfWork) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateUserProfile(UserProfileEntity request, int id)
        {
            var userProfile = new UserProfileEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IDNumber = request.IDNumber,
                UserId = id,
            };

            await _unitOfWork.userProfileRepository.Add(userProfile);
        }
    }
}
