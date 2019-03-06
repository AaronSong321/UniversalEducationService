using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class ExamModule: UESModule
    {
        public ExamPaperInstance GenerateDummyInstance(ExamPaper paper, Student student)
            => throw new NotImplementedException("");
        public ExamPaperInstance StartExam(Exam exam, Student student, out string errorMessage)
            => exam.StartExam(student, out errorMessage);
        public (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double, string) StartExamPaperInstance
            (ExamPaperInstance instance, Student student)
            => instance.StartExamPaperInstance(student);
        public List<QuestionChooseRecord> GetQuestions(Student student, ExamPaper paper)
            => paper.QuestionSet;
        public (List<QuestionChooseRecord>, List<StudentAnswerRecord>, double) GetOngoingExamState
            (ExamPaperInstance instance, Student student, out string errorMessage)
            => instance.GetCurrentOngoingExamState(student, out errorMessage);
        public void StoreAnswer(StudentAnswerRecord record, Student student, string answer, out string errorMessage)
            => record.StoreAnswer(student, answer, out errorMessage);
        public void StudentSubmit(ExamPaperInstance instance, Student student, out string errorMessage)
            => instance.StudentSubmit(student, out errorMessage);
    }
}
