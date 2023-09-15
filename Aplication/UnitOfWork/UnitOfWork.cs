using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplication.Repository;
using Dominio;
using Dominio.Interfaces;
using Persistencia.Data;

namespace Aplication.UnitOfWork;

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApiContext _context;
        private UserRepository _users;
        private RolRepository _rols;

        public UnitOfWork(ApiContext context)
        {
            _context = context; 
        }

        public IRolInterface Rols
        {
            get
            {
                if(_rols == null)
                {
                    _rols = new RolRepository(_context);
                }
                return _rols;
            }
        }
        public IUserInterface Users
        {
            get
            {
                if(_users == null)
                {
                    _users = new UserRepository(_context);
                }
                return _users;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public int Save()
        {
            return _context.SaveChanges();
        }
                public void Dispose()
        {
            _context.Dispose();
        }
    }
