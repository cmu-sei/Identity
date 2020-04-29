// Copyright 2020 Carnegie Mellon University. 
// Released under a MIT (SEI) license. See LICENSE.md in the project root. 

using Identity.Clients.Abstractions;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService (
            IHttpContextAccessor context
        ){
            _context = context;
        }

        private readonly IHttpContextAccessor _context;
        public string Id
        {
            get
            {
                return _context.HttpContext.User.GetSubjectId();
            }
        }
        public string Name
        {
            get
            {
                return _context.HttpContext.User.GetSubjectName();
            }
        }
        public bool IsPrivileged
        {
            get
            {
                return _context.HttpContext.User.IsPrivileged();
            }
        }
        public bool IsAdmin
        {
            get
            {
                return _context.HttpContext.User.IsInRole(AppConstants.AdminRole);
            }
        }
    }
}
