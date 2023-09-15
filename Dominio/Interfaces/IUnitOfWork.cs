using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IUnitOfWork
    {
        IRolInterface Rols {get;}
        IUserInterface Users {get;}

        Task<int> SaveAsync();
    }
}