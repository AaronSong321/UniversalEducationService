using System;
using System.Collections.Generic;
using HIT.UES.Exam;
using HIT.UES.Login;
using HIT.UES.Server.ServiceDeclaration;

namespace HIT.UES.Server.StandardServiceProvider
{
    public static class ServiceController
    {
        public static UESSystem system = new UESSystem();
    }


    public class LoginService: ILoginService
    {
        private LoginModule loginModule;
        private ExamInformationModule examInformationModule;

        public LoginService()
        {
            loginModule = ServiceController.system.LoginSubsystem.Login;
            examInformationModule = ServiceController.system.ExamSubsystem.InformationModule;
        }

        #region Exam Information Service
        public List<Exam.Exam> GetExam(Predicate<Exam.Exam> filter)
        {
            return examInformationModule.GetExam(filter);
        }

        public List<Exam.Exam> GetExam(int id, string indexWord)
        {
            var a = GetExam(exam => exam.ExamID == id);
            var b = GetExam(indexWord);
            a.AddRange(b);
            return a;
        }

        public List<Exam.Exam> GetExam(string indexWord)
        {
            return examInformationModule.GetExam(indexWord);
        }

        public List<Exam.Exam> GetAllExams()
        {
            return examInformationModule.GetAllExams();
        }
        #endregion


        #region Login Service
        public Student Login(int id, string password, out string errorMessage)
        {
            return loginModule.Login(id, password, out errorMessage);
        }

        public Student Register(string name, string password, out string errorMessage)
        {
            return loginModule.Register(name, password, out errorMessage);
        }

        public Teacher TeacherRegister(string name, string password, out string em)
        {
            return loginModule.TeacherRegister(name, password, out em);
        }
        #endregion 

    }
}
