using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class ExamInformationModule: UESModule
    {
        public Exam CreateExam(string name, Teacher creator, string department, string indexWord, string description, ushort maxScore, DateTime allowSignIn, DateTime allowAttend, double examDuration,
            DateTime studentSubmit, DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration, out string em)
            => Exam.CreateExam(name, creator, department, indexWord, description, maxScore, allowSignIn, allowAttend, examDuration, studentSubmit,
                teacherSubmit, scorePublic, examPaperGeneration,out em);
        public void ModifyExam(Exam exam, string name, Teacher teacher, string department, string indexWord,
            string description, ushort maxScore, DateTime allowSignIn, DateTime allowAttend,
            float examDuration, DateTime studentSubmit, DateTime teacherSubmit, DateTime scorePublic, DateTime examPaperGeneration, out string em)
            => exam.ModifyExam(name, teacher, department, indexWord, description, maxScore, allowSignIn, allowAttend, examDuration, studentSubmit,
                teacherSubmit, scorePublic, examPaperGeneration, out em);
        public List<Exam> GetAllExams()
            => Exam.GetAllExams();
        public List<Exam> GetExam(Predicate<Exam> filter)
            => Exam.GetExam(filter);
        public List<Exam> GetExam(string indexWord) => Exam.GetExam(indexWord);
        public void SignInForExam(Student student, Exam exam, out string errorMessage)
            => exam.SignInForExam(student, out errorMessage);
        public void AddToPaperAdmin(Exam exam, Teacher creator, Teacher teacher, out string errorMessage)
            => errorMessage = exam.AddToPaperAdmin(creator, teacher);
        public bool HasAdminAuthority(Exam exam, Teacher teacher)
            => exam.HasAdminAuthority(teacher);
        public void RemoveFromPaperAdmin(Exam exam, Teacher creator, Teacher teacher, out string errorMessage)
            => errorMessage = exam.RemoveFromPaperAdmin(creator, teacher);
        public void AddToPaperExaminor(Exam exam, Teacher creator, Teacher teacher, out string errorMessage)
            => errorMessage = exam.AddToPaperExaminor(creator, teacher);
        public bool HasExamineAuthority(Exam exam, Teacher teacher)
            => exam.HasExamineAuthority(teacher);
        public void RemoveFromPaperExaminor(Exam exam, Teacher creator, Teacher teacher, out string em)
            => em = exam.RemoveFromPaperExaminor(creator, teacher);
    }
}
