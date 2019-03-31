using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;

namespace HIT.UES.Exam
{
    [NotMapped]
    public abstract class ExamPaperRule : DatabaseType
    {
        public bool CheckTotalScore { get; private set; }
        //public Exam SelectedExam { get; }
        public Teacher ExamPaperCreator { get; private set; }
        //public List<ExamQuestionSet> QuestionSets { get; private set; }

        public ExamPaperRule(Teacher teacher, bool checkTotalScore = true)
        {
            ExamPaperCreator = teacher;
            CheckTotalScore = checkTotalScore;
        }

        public abstract bool CheckRuleValidity(ExamPaper paper, out string errorMessage);
        public void TurnOnTotalScoreCheck() => CheckTotalScore = true;
        public void TurnOffTotalScoreCheck() => CheckTotalScore = false;
    }
}
