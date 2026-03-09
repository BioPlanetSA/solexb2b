using System;
using SolEx.Hurt.Model.Enums;

namespace SolEx.Hurt.Model.AtrybutyKlas
{
    public class ApiUprawnioneRoleAttribute : Attribute
    {
        private RoleType[] _role = null;
        public RoleType[] role
        {
            get
            {
                if (_role == null)
                {
                    return new RoleType[] { RoleType.Administrator };
                }
                return _role;
            }
        }

        public ApiUprawnioneRoleAttribute(params RoleType[] roles ){
            _role = roles;            
        }
    }
}
