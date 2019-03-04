using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class ExamPaperGenerator: UESObject
    {
        public Exam SelectedExam { get; private set; }
        public Teacher ExamPaperCreator { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public ExamQuestionSet SelectedQuestionSet { get; private set; }
        public ExamPaperRule Rule { get; private set; }

        public ExamPaperGenerator(Exam exam, Teacher creator, ExamQuestionSet qs)
        {
            SelectedExam = exam;
            ExamPaperCreator = creator;
            SelectedQuestionSet = qs;
            LastModifyTime = DateTime.Now;
            Rule = null;
        }
        public void SetRule(ExamPaperRule rule) => Rule = rule;

        //public 
    }
}
