using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    public class Exam : DatabaseType
    {
        #region Static Error Strings
        public static string NotCreator = "You are not the creator of this exam, so you cannot grant other teachers the " +
            "authority of creating exam papers of this exam.";
        public static string NoExamineAuthority = "You do not have the examine authority of this exam, so you cannot check the " +
            "answers of the exam papers of this exam.";
        public static string NotDepartmentAdmin = "You are not the administrator of your department. Get the authority, or" +
            "you cannot create an exam nor modify the information of an exam.";
        public static string NotSignedIn = "You have not signed in for the exam.";
        #endregion

        #region Basic Properties
        [Key]
        public int ExamID { get; private set; }
        [MaxLength(30), Required]
        public string ExamName { get; private set; }
        public Teacher Creator { get; private set; }
        public Teacher LastOperator { get; private set; }
        [MaxLength(30)]
        public string Department { get; private set; }
        [MaxLength(50)]
        public string IndexWord { get; private set; }
        public ushort MaxScore { get; private set; }
        public string Description { get; private set; }
        #endregion 

        #region DateTime Records
        public DateTime CreationTime { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public DateTime AllowSignInTime { get; private set; }
        public DateTime AllowAttendTime { get; private set; }
        public double ExamDuration { get; private set; }
        public DateTime StudentSubmitDeadline { get; private set; }
        public DateTime TeacherSubmitDeadline { get; private set; }
        public DateTime ScorePublicTime { get; private set; }
        public DateTime ExamPaperGenerationDeadline { get; private set; }
        #endregion

        #region List Part
        /// <summary>
        /// Recording teachers who are authorized to create exam papers and select questions and add to the papers
        /// </summary>
        public virtual List<Teacher> AuthorizedPaperCreator { get; private set; }
        /// <summary>
        /// Recording teachers who are authorized to check the answers of the exam paper instances of this exam
        /// </summary>
        public virtual List<Teacher> AuthorizedPaperExaminor { get; private set; }
        /// <summary>
        /// Containing all the exam paper of this exam, whether completed or not
        /// </summary>
        public virtual List<ExamPaper> ExamPapers { get; }
        public virtual List<ExamPaperInstance> StudentPapers { get; }
        public virtual List<Student> SignedInStudents { get; }
        #endregion

        #region Create and Modify
        public Exam()
        {

        }
        public Exam(string name, Teacher creator, string department, string indexWord, string description, ushort maxScore, DateTime allowSignIn, DateTime allowAttend, double examDuration,
            DateTime studentsubmit, DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration)
        {
            ExamName = name;
            Creator = creator;
            Department = department;
            IndexWord = indexWord;
            Description = description;
            MaxScore = maxScore;
            LastOperator = creator;

            AuthorizedPaperCreator = new List<Teacher>
            {
                creator
            };
            AuthorizedPaperExaminor = new List<Teacher> { creator };
            ExamPapers = new List<ExamPaper>();
            StudentPapers = new List<ExamPaperInstance>();
            SignedInStudents = new List<Student>();

            AllowSignInTime = allowSignIn;
            AllowAttendTime = allowAttend;
            ExamDuration = examDuration;
            StudentSubmitDeadline = studentsubmit;
            TeacherSubmitDeadline = teacherSubmit;
            ScorePublicTime = scorePublic;
            ExamPaperGenerationDeadline = examPaperGeneration;
            CreationTime = DateTime.Now;
            LastModifyTime = CreationTime;
        }
        public static Exam CreateExam(string name, Teacher creator, string department, string indexWord, string description, ushort maxScore, DateTime allowSignIn, DateTime allowAttend, double examDuration,
            DateTime studentsubmit, DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration, out string em)
        {
            if (!creator.DepartmentAdminAuthority)
            {
                em = NotDepartmentAdmin;
                return null;
            }
            else
            {
                var e = new Exam(name, creator, department, indexWord, description, maxScore, allowSignIn, allowAttend, examDuration, studentsubmit, teacherSubmit, scorePublic, examPaperGeneration);
                creator.AddtoExamCreation(e);
                Settings.SaveDataCreation(e);
                em = null;
                return e;
            }
        }
        public void ModifyExam(string name, Teacher teacher, string department, string indexWord, string description,
            ushort maxScore, DateTime allowSignIn, DateTime allowAttend, double examDuration,
            DateTime studentsubmit, DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration, out string em)
        {
            if (teacher.DepartmentAdminAuthority)
            {
                ExamName = name;
                Department = department;
                IndexWord = indexWord;
                Description = description;
                MaxScore = maxScore;
                LastModifyTime = DateTime.Now;
                LastOperator = teacher;
                AllowSignInTime = allowSignIn;
                AllowAttendTime = allowAttend;
                ExamDuration = examDuration;
                StudentSubmitDeadline = studentsubmit;
                TeacherSubmitDeadline = teacherSubmit;
                ScorePublicTime = scorePublic;
                ExamPaperGenerationDeadline = examPaperGeneration;
                LastOperator = teacher;
                LastModifyTime = DateTime.Now;
                Settings.SaveDataModification(this);
                em = null;
            }
            else
                em = NotDepartmentAdmin;
        }
        #endregion

        #region Authority
        public string AddToPaperAdmin(Teacher creator, Teacher teacher)
        {
            if (creator == Creator)
            {
                AuthorizedPaperCreator.Add(teacher);
                AuthorizedPaperExaminor.Add(teacher);
                Settings.SaveDataModification(this);
                return null;
            }
            else
            {
                return NotCreator;
            }
        }
        public bool HasAdminAuthority(Teacher teacher) => AuthorizedPaperCreator.Contains(teacher);
        public string RemoveFromPaperAdmin(Teacher creator, Teacher teacher)
        {
            if (creator == Creator)
            {
                AuthorizedPaperCreator.Remove(teacher);
                AuthorizedPaperExaminor.Remove(teacher);
                Settings.SaveDataModification(this);
                return null;
            }
            else
                return NotCreator;
        }
        public string AddToPaperExaminor(Teacher creator, Teacher teacher)
        {
            if (creator == Creator)
            {
                if (AuthorizedPaperExaminor.Contains(teacher))
                    return NotCreator;
                else
                {
                    AuthorizedPaperExaminor.Add(teacher);
                    Settings.SaveDataModification(this);
                    return null;
                }
            }
            else
                return NotCreator;
        }
        public bool HasExamineAuthority(Teacher teacher) => AuthorizedPaperExaminor.Contains(teacher);
        public string RemoveFromPaperExaminor(Teacher creator, Teacher teacher)
        {
            if (creator == Creator)
            {
                if (AuthorizedPaperExaminor.Contains(teacher))
                {
                    AuthorizedPaperExaminor.Remove(teacher);
                    Settings.SaveDataModification(this);
                    return null;
                }
                else
                    return $"Teacher {teacher} does not have this authority.";
            }
            else
                return NotCreator;
        }
        #endregion

        #region Query Exams
        public static List<Exam> GetAllExams()
        {
            using (var context = new UESContext())
            {
                context.Exams.Load();
                return context.Exams.ToList();
            }
        }
        public static List<Exam> GetExam(Predicate<Exam> filter)
        {
            using (var context = new UESContext())
            {
                context.Exams.Load();
                var query = from b in context.Exams where filter(b) select b;
                return query.ToList();
            }
        }
        public static List<Exam> GetExam(string indexWord)
            => GetExam((exam) => exam.ExamName.Contains(indexWord) || exam.IndexWord.Contains(indexWord));
        #endregion

        #region Sign in and Start an exam
        public void SignInForExam(Student student, out string errorMessage)
        {
            if (SignedInStudents.Contains(student))
            {
                errorMessage = "You haev already signed in.";
            }
            else if (DateTime.Now < AllowSignInTime || DateTime.Now > StudentSubmitDeadline)
            {
                errorMessage = $"This is not the time you can sign in. Start time = {AllowSignInTime}, finish time = " +
                    $"{StudentSubmitDeadline}, current time = {DateTime.Now}.";
            }
            else
            {
                SignedInStudents.Add(student);
                //student.ExamRegistered.Add(this);
                Settings.SaveDataModification(this);
                errorMessage = null;
            }
        }
        public ExamPaperInstance StartExam(Student student, out string errorMessage)
        {
            if (!SignedInStudents.Contains(student))
            {
                errorMessage = NotSignedIn;
            }
            else if (DateTime.Now < AllowAttendTime || DateTime.Now > StudentSubmitDeadline)
            {
                errorMessage = $"This is not the time you can attend the exam. Start time = {AllowAttendTime}, finish" +
                    $" time = {StudentSubmitDeadline}, current time = {DateTime.Now}";
            }
            else
            {
                var chosenPaper = ExamPapers[new Random().Next(0, ExamPapers.Count)];
                var k = chosenPaper.GenerateExamPaperInstance(student, out errorMessage);
                if (k == null)
                {
                    return null;
                }
                StudentPapers.Add(k);
                Settings.SaveDataModification(this);
                k.StartExamPaperInstance();
                return k;
            }
            return null;
        }
        #endregion

        #region Exam Papers and Instances
        public List<ExamPaper> GetAvaiablePapers(Teacher teacher)
        {
            if (!HasAdminAuthority(teacher))
            {
                return null;
            }
            else
            {
                return (from b in ExamPapers where b.Creator == teacher || b.Finished select b).ToList();
            }
        }
        public List<ExamPaperInstance> GetExamPaperInstances(Teacher teacher)
        {
            if (HasExamineAuthority(teacher))
                return StudentPapers;
            else
                return null;
        }
        public List<ExamPaperInstance> GetFinishedInstances(Teacher teacher)
        {
            if (HasAdminAuthority(teacher))
            {
                return (from b in StudentPapers where b.StudentSubmitted && !b.TeacherSubmitted select b).ToList();
            }
            else
                return null;
        }
        #endregion

        #region Inherited and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        public override int GetHashCode() => ExamID;
        public override bool Equals(object obj)
        {
            if (obj is Exam exam)
                return exam.ExamID == ExamID;
            else return false;
        }
        public override string ToString() => $"Exam {ExamName}";
        #endregion
    }
}
