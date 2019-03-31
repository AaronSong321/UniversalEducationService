using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    public class ExamPaperModule: UESModule
    {
        #region Creation and Basic Information
        public ExamPaper CreateExamPaper(Exam superiorExam, Teacher creator, string indexWord,
            string description, float duration, ushort score, out string errorMessage)
            => ExamPaper.CreateExamPaper(superiorExam, creator, indexWord, description, duration, score, out errorMessage);
        public ExamPaper CreateExamPaper(Teacher teacher, ExamPaper other, out string errorMessage)
            => ExamPaper.DuplicateExamPaper(teacher, other, out errorMessage);
        public void ModifyExamPaper(ExamPaper paper, Teacher teacher, string indexWord, string description, uint duration,
            ushort score, out string errorMessage)
            => paper.ModifyExamPaper(teacher, indexWord, description, duration, score, out errorMessage);
        public bool CheckConsistency(ExamPaper paper, out string inconsistency)
            => paper.CheckConsistency(out inconsistency);
        public void SaveExamPaper(ExamPaper paper, Teacher creator)
            => paper.SaveExamPaper(creator);
        #endregion

        #region Need to add authority check
        public void ChooseQuestion(ExamPaper paper, ExamQuestion q, ushort score, ushort order)
            => paper.ChooseQuestion(q, score, order);
        public void ChooseQuestion(ExamPaper paper, ExamQuestion q, ushort score)
            => paper.ChooseQuestion(q, score);
        public void ContainsQuestion(ExamPaper paper, ExamQuestion q)
            => paper.ContainsQuestion(q);
        public int GetQuestionCount(ExamPaper paper)
            => paper.GetQuestionNumber();
        public float GetCurrentScore(ExamPaper paper)
            => paper.CurrentScore;
        public void ExchangeOrder(ExamPaper paper, QuestionChooseRecord question1, QuestionChooseRecord question2)
            => paper.ExchangeOrder(question1, question2);
        public void ChangeOrder(ExamPaper paper, QuestionChooseRecord question, ushort order)
            => paper.ChangeOrder(question, order);
        #endregion

        #region Exam Papers and Instances
        public List<ExamPaper> GetAvailableExamPapers(Exam exam, Teacher teacher)
            => exam.GetAvaiablePapers(teacher);
        #endregion

        #region Choose Questions and Create Exam Paper Rules
        public ExplicitlyDividedRule CreateExplicitlyDividedRule(Teacher creator, bool checkTotalScore = true)
            => new ExplicitlyDividedRule(creator, checkTotalScore);
        public void AddRuleEntry(ExplicitlyDividedRule rule, string indexWord, ushort number, 
            ExamQuestion.QuestionType typeName, ushort maxScore)
            => rule.AddNewRuleEntry(indexWord, number, typeName, maxScore);
        public void DuplicateRuleEntry(ExplicitlyDividedRule rule, ExplicitlyDividedRule.RuleEntry entry)
            => rule.DuplicateRuleEntry(entry);
        public void ModifyRuleEntry(ExplicitlyDividedRule rule, ExplicitlyDividedRule.RuleEntry entry,
            string indexWord, ushort number, ExamQuestion.QuestionType type,
            ushort maxScore) => rule.ModifyRuleEntry(entry, indexWord, number, type, maxScore);
        public List<ExplicitlyDividedRule.RuleEntry> GetRuleEntries(ExplicitlyDividedRule rule)
            => rule.Entries;
        public void DeleteRuleEntry(ExplicitlyDividedRule rule, ExplicitlyDividedRule.RuleEntry entry)
            => rule.DeleteRuleEntry(entry);
        //public void AutoGenerateExamPaper(ExamPaperGenerator generator)
        public bool CheckRuleValidity(ExplicitlyDividedRule rule, ExamPaper paper, out string invalidity)
            => rule.CheckRuleValidity(paper, out invalidity);
        public ushort ChooseQuestions(ExplicitlyDividedRule rule, ExamPaper paper, ExamQuestionSet questionSet)
            => rule.ChooseQuestions(paper, questionSet);
        #endregion

    }
}
