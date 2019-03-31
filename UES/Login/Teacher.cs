using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Exam;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Login
{
    public class Teacher : Student
    {
        //public int TeacherID { get; private set; }
        public bool DepartmentAdminAuthority { get; private set; }
        //public List<MessageBoard> MessageBoardCreated { get; }
        [NotMapped]
        public List<Exam.Exam> ExamCreated
        {
            get
            {
                if (ExamCreated == null)
                {
                    ExamCreated = (from exam in Settings.uesContext.Exams where exam.Creator == this select exam).ToList();
                }
                return ExamCreated;
            }
            set => ExamCreated = value;
        }
        [NotMapped]
        public List<Exam.Exam> ExamAdministrated { get; }
        [NotMapped]
        public List<Exam.Exam> ExamExamined { get; }
        [NotMapped]
        public List<ExamQuestionSet> QuestionSetCreated { get; }
        [NotMapped]
        public List<ExamQuestionSet> QuestionSetAdministrated { get; }
        [NotMapped]
        public List<ExamQuestion> QuestionCreated { get; }
        [NotMapped]
        public List<ExamPaperRule> PaperRuleCreated { get; }
        [NotMapped]
        public List<Course.Course> Courses { get; }

        public Teacher()
        {
            DepartmentAdminAuthority = false;
            /*
            ExamCreated = new List<Exam.Exam>();
            ExamAdministrated = new List<Exam.Exam>();
            ExamExamined = new List<Exam.Exam>();
            QuestionSetCreated = new List<ExamQuestionSet>();
            QuestionSetAdministrated = new List<ExamQuestionSet>();
            QuestionCreated = new List<ExamQuestion>();
            PaperRuleCreated = new List<ExamPaperRule>();
            */
        }
        public Teacher(string name, string password) : base(name, password)
        {
            DepartmentAdminAuthority = false;
            ExamCreated = new List<Exam.Exam>();
            ExamAdministrated = new List<Exam.Exam>();
            ExamExamined = new List<Exam.Exam>();
            QuestionSetCreated = new List<ExamQuestionSet>();
            QuestionSetAdministrated = new List<ExamQuestionSet>();
            QuestionCreated = new List<ExamQuestion>();
            PaperRuleCreated = new List<ExamPaperRule>();
        }

        #region Authority
        internal void GetDepartmentAdminAuthority() => DepartmentAdminAuthority = true;
        internal void LoseDepartmentAdminAuthority() => DepartmentAdminAuthority = false;

        internal void GetAdministrateAuthority(Exam.Exam exam) => ExamAdministrated.Add(exam);
        internal bool HasAdministrateAuthority(Exam.Exam exam) => ExamAdministrated.Contains(exam);
        internal void LoseAdministrateAuthority(Exam.Exam exam) => ExamAdministrated.Remove(exam);
        internal void GetExamineAuthority(Exam.Exam exam) => ExamExamined.Add(exam);
        internal bool HasExamineAuthority(Exam.Exam exam) => ExamExamined.Contains(exam);
        internal void LoseExamineAuthority(Exam.Exam exam) => ExamExamined.Remove(exam);
        #endregion
        internal void AddtoExamCreation(Exam.Exam exam) => ExamCreated.Add(exam);

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public override int GetHashCode() => StudentID;
        public override bool Equals(object obj)
        {
            if (obj is Teacher teacher)
                return teacher.StudentID == StudentID;
            else
                return false;
        }
    }
}
