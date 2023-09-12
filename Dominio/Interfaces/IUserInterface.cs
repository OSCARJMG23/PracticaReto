using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;

namespace Dominio.Interfaces
{
    public interface IUserInterface : IGenericRepository<User>
    {
        Task<User> GetByUserNameAsync(string UserName);
        Task<User> GetByRefreshTokenAsync(string UserName);
    }
}