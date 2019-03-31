using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Login
{
    public class LoginSubsystem: UESSubsystem
    {
        public LoginModule Login { get; }
        public GrantingModule Granting { get; }

        public LoginSubsystem()
        {
            Login = new LoginModule();
            Granting = new GrantingModule();
        }
    }

    public class LoginModule: UESModule
    {
        public Student Login(int id, string password, out string errorMessage)
        {
            //Student ans = null;
            foreach (var a in Settings.uesContext.Students)
            {
                if (a.StudentID == id)
                {
                    if (a.Password == password)
                    {
                        errorMessage = null;
                        return a;
                    }
                    else
                    {
                        errorMessage = $"User {id} is a student, but password is incorrect.";
                        goto returnNull;
                    }
                }
            }
            foreach (var a in Settings.uesContext.Teachers)
            {
                if (a.StudentID == id)
                {
                    if (a.Password == password)
                    {
                        errorMessage = null;
                        return a;
                    }
                    else
                    {
                        errorMessage = $"User {id} is a teacher, but password is incorrect.";
                        goto returnNull;
                    }
                }
            }

            errorMessage = $"User {id} not found in Settings.uesContext.Students and Settings.uesContext.Teachers";
        returnNull:
            return null;
        }

        public Student Register(string name, string passowrd, out string errorMessage)
        {
            Student s = new Student(name, passowrd);
            Settings.SaveDataCreation(s);
            errorMessage = null;
            return s;
        }
        public Teacher TeacherRegister(string name, string password, out string em)
        {
            Teacher tea = new Teacher(name, password);
            Settings.SaveDataCreation(tea);
            em = null;
            return tea;
        }
    }


    public class GrantingModule: UESModule
    {
        public void GrantDepartmentAdminAuthority(Administrator admin, Teacher teacher, out string em)
            => admin.GrantDepartmentAdminAuthority(teacher, out em);
        public void RecallDepartmentAdminAuthority(Administrator admin, Teacher teacher, out string em)
            => admin.RecallDepartmentAdminAuthority(teacher, out em);

    }
}
