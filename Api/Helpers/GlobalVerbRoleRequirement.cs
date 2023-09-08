using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Api.Helpers;

    public class GlobalVerbRoleRequirement : IAuthorizationRequirement
    {
        public bool IsAllowed(string role, string verb)
        {
            if(string.Equals("Administrator", role, StringComparison.OrdinalIgnoreCase)) return true;

            if(string.Equals("Gerente", role, StringComparison.OrdinalIgnoreCase)) return true;


            if(string.Equals("Empleado", role, StringComparison.OrdinalIgnoreCase)&& string.Equals("GET", verb,StringComparison.OrdinalIgnoreCase ))
            {
                return true;
            }

            if(string.Equals("Camper", role, StringComparison.OrdinalIgnoreCase)&& string.Equals("GET", verb,StringComparison.OrdinalIgnoreCase ))
            {
                return true;
            }
            if(string.Equals("Cliente", role, StringComparison.OrdinalIgnoreCase)&& string.Equals("GET", verb,StringComparison.OrdinalIgnoreCase ))
            {
                return true;
            }
            return false;
        }
    }
