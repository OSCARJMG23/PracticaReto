using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;
using Dominio.Interfaces;
using Persistencia.Data;
using Microsoft.EntityFrameworkCore;

namespace Aplication.Repository;

    public class UserRepository : GenericRepository<User>,IUserInterface
    {
        private readonly ApiContext _context;

        public UserRepository(ApiContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                    .Include(u =>u.Rols)
                    .Include(u => u.RefreshTokens)
                    .FirstOrDefaultAsync(u =>u.RefreshTokens.Any(t => t.Token == refreshToken));
        }
        public async Task<User> GetByUserNameAsync(string username)
        {
            return await _context.Users
                                .Include(u =>u.Rols)
                                .Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.UserName.ToLower()== username.ToLower());
        }
        
    }
