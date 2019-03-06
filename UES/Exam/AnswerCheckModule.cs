using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class AnswerCheckModule: UESModule
    {
        public List<ExamPaperInstance> GetExamPaperInstances(Exam exam, Teacher teacher)
            => exam.GetExamPaperInstances(teacher);
        public List<ExamPaperInstance> GetFinishedInstances(Exam exam, Teacher teacher)
            => exam.GetFinishedInstances(teacher);
        public double? CalculateTotalScore(ExamPaperInstance instance, out string errorMessage)
            => instance.CalculateTotalScore(out errorMessage);
        public void AutoCheckAnswer(StudentAnswerRecord record, AutoCheckRule rule)
            => rule.CheckAnswer(record);
        public void GiveScore(StudentAnswerRecord record, Teacher teacher, double score, out string errorMessage)
            => record.GiveScore(teacher, score, out errorMessage);
        public void SubmitExamPaperInstance(ExamPaperInstance instance, Teacher teacher, out string errorMessage)
            => instance.TeacherSubmit(teacher, out errorMessage);
        public AutoCheckRule CreateAutoCheckRule()
            => new AutoCheckRule();

    }
}
