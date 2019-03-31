using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
        public List<Exam.Exam> GetExam(Predicate<Exam.Exam> filter) => examInformationModule.GetExam(filter);

        public List<Exam.Exam> GetExam(int id, string indexWord)
        {
            var a = GetExam(exam => exam.ExamID == id);
            var b = GetExam(indexWord);
            a.AddRange(b);
            return a;
        }

        public List<Exam.Exam> GetExam(string indexWord) => examInformationModule.GetExam(indexWord);

        public List<Exam.Exam> GetAllExams() => examInformationModule.GetAllExams();

        public Exam.Exam CreateExam(string name, int creatorId, string department, string indexWord, ushort maxScore, string description, DateTime allowSignInTime,
            DateTime allowAttendTime, double examDuration, DateTime studentDeadline, DateTime teacherDeadline, DateTime scorePublic, DateTime epgd, out string em)
        {
            Teacher cuando;
            using (var cielo = new UESContext())
            {
                cielo.Teachers.Load();
                var me = (from b in cielo.Teachers where b.StudentID == creatorId select b).ToList();
                if (me.Count != 1)
                {
                    em = $"Teacher with id {creatorId} is not found in a temperate query context.";
                    return null;
                }
                else
                    cuando = me[0];
                
            }
            return examInformationModule.CreateExam(name, cuando, department, indexWord, description, maxScore, allowSignInTime, allowAttendTime, 
                examDuration, studentDeadline, teacherDeadline, scorePublic, epgd, out em);
        }
        #endregion


        #region Login Service
        public Student Login(int id, string password, out string errorMessage) => loginModule.Login(id, password, out errorMessage);

        public Student Register(string name, string password, out string errorMessage) => loginModule.Register(name, password, out errorMessage);

        public Teacher TeacherRegister(string name, string password, out string em) => loginModule.TeacherRegister(name, password, out em);
        #endregion 

    }
}
