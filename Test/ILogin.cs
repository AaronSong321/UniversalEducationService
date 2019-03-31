using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace HIT.UES.Server.ServiceDeclaration
{
    public static class LoginServiceHelper
    {
        #region Login Subsystem
        public const string RegisterUriTemplate = "ExamSystemService/Students/Register?name={name}&password={password}";
        public const string TeacherRegisterUriTemplate = "ExamSystemService/Teachers/Register?name={name}&password={password}";
        public const string LoginUriTemplate = "ExamSystemService/Students/Login?id={id}&password={password}";
        #endregion

        #region Exam Subsystem
        public const string ExamPrefix = "ExamSystemSerivice/Exams/";
        public const string GetAllExams = "ExamSystemService/Exams";
        public const string GetExam = "ExamSystemService/Exams/Exam?id={id}&indexWord={indexWord}";
        public const string CreateExam = ExamPrefix + "Exam?name={name}&creatorId={creatorId}&department={department}&indexWord={indexWord}&maxScore={maxScore}" +
            "&description={description}&allowSignInTime={allowSignInTime}&allowAttendTime={allowAttendTime}&examDuration={examDuration}&" +
            "studentDeadline={studentDeadline}&teacherDeadline={teacherDeadline}&scorePublic={scorePublic}&examPaperGeneration={epgd}";
        //public const string GetExamByFilter = "ExamSystemService/Exams/Exam?filter={filter}";
        #endregion
    }


    [ServiceContract(Name = "Login")]
    public interface ILoginService
    {
        #region Login Service
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = LoginServiceHelper.RegisterUriTemplate, BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Student Register(string name, string password, out string errorMessage);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = LoginServiceHelper.LoginUriTemplate, BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Student Login(int id, string password, out string errorMessage);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = LoginServiceHelper.TeacherRegisterUriTemplate, BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Teacher TeacherRegister(string name, string password, out string em);
        #endregion


        #region Exam Information Service
        [OperationContract]
        [WebGet(UriTemplate = LoginServiceHelper.GetAllExams, BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Exam.Exam> GetAllExams();

        [OperationContract]
        [WebGet(UriTemplate = LoginServiceHelper.GetExam, BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Exam.Exam> GetExam(int id, string indexWord);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = LoginServiceHelper.CreateExam, BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Exam.Exam CreateExam(string name, int creatorId, string department, string indexWord, ushort maxScore, string description, DateTime allowSignInTime,
            DateTime allowAttendTime, double examDuration, DateTime studentDeadline, DateTime teacherDeadline, DateTime scorePublic, DateTime epgd, out string em);
        


        /*
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Exams/", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void CreateExam(string name, Teacher creator, string department, string indexWord,
            string description, ushort maxScore = 100);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "Exams/", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void ModifyExam(Exam.Exam exam, string name, Teacher teacher, string department, string indexWord,
            string description, ushort maxScore);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "Exams/{id}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void ModifyDateTime(Exam.Exam exam, Teacher teacher, DateTime allowSignIn, DateTime allowAttend,
            float examDuration, DateTime studentsubmit,
            DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration);
        */
        #endregion
    }

}
