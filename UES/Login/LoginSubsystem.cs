using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Login
{
    public class LoginSubsystem: UESSubsystem
    {
        public LoginModule Login;
        public GrantingModule Granting;
    }

    public class LoginModule: UESModule
    {
        public void InvokeRegister()
        {

        }
        public void InvokeLogin()
        {

        }
    }

    public enum GrantDepartmentAdminState { Success, DeficientOperatorAuthority, AlreadyAdmin, UnknownError }
    public enum RecallDepartmentAdminState { Success, DeficientOperatorAuthority, NotAdmin, UnknownError }
    public class GrantingModule: UESModule
    {
        public (GrantDepartmentAdminState, string) GrantDepartmentAdminAuthority()
        {
            return (GrantDepartmentAdminState.Success, "success");
        }
        public (RecallDepartmentAdminState, string) RecallDepartmentAdminAuthority()
        {
            return (RecallDepartmentAdminState.Success, "success");
        }
    }
}
