using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class ExamSubsystem: UESSubsystem
    {
        public ExamInformationModule InformationModule { get; }
        public ExamQuestionModule QuestionModule { get; private set; }
        public ExamPaperModule @ExamPaperModule { get; private set; }
        public ExamModule @ExamModule { get; private set; }
        public AnswerCheckModule @AnswerCheckModule { get; private set; }
        public ExamScoreModule ScoreModule { get; private set; }

        public ExamSubsystem()
        {
            InformationModule = new ExamInformationModule();
            QuestionModule = new ExamQuestionModule();
            @ExamPaperModule = new ExamPaperModule();
            @ExamModule = new ExamModule();
            @AnswerCheckModule = new AnswerCheckModule();
            ScoreModule = new ExamScoreModule();
        }
    }
}
