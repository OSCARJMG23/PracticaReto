using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dominio.Entities
{
    public class UserRol:BaseEntity
    {
        public int UsuarioId { get; set; }
        public User User {get;set;}
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}