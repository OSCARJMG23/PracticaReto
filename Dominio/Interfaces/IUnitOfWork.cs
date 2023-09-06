using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IUnitOfWork
    {
        IUserInterface Users {get;}
        IRolInterface Rols {get;}
        Task<int> SaveAsync();
    }
}