﻿using JWTAuthentication.Const;
using JWTAuthentication.Context;
using JWTAuthentication.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JWTAuthentication.Services
{
    public class RoleAuthenticationService : IRoleAuthorizationService
    {
        private readonly AuthContext _context;

        public RoleAuthenticationService(AuthContext context)
        {
            _context = context;
        }

        public async Task<bool> IsAuthorized (int roleId, string moduleName, string actionName)
        {
            try
            {
                int AuthenticationSettingId = await _context.AuthenticationSetting.Where(x=>x.Action ==  actionName 
                                              && x.ModuleName == moduleName)
                                              .Select(x=>x.Id).FirstOrDefaultAsync();

                return (_context.RoleAuthentication.Any(x => x.RoleId == roleId
                        && x.AuthenticationSettingId == AuthenticationSettingId));
            }
            catch
            {
                return false;
            }
        }
    }
}
