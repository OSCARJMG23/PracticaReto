using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;
using Dominio.Interfaces;
using Persistencia.Data;



namespace Aplication.Repository;

    public class RolRepository :GenericRepository<Rol>, IRolInterface
    {
        private readonly ApiContext _context;

        public RolRepository(ApiContext context) : base(context)
        {
            _context = context;
        }
    }
