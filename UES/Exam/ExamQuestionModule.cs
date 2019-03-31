using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIT.UES.Exam;

namespace HIT.UES.Exam
{
    public class ExamQuestionModule: UESModule
    {
        #region QuestionSet
        public ExamQuestionSet CreateQuestionSet(Teacher creator, string name, string indexWord, out string errorMessage)
            => ExamQuestionSet.CreateExamQuestionSet(name, indexWord, creator, out errorMessage);
        public void ModifyQuestionSet(ExamQuestionSet questionSet, string name, string indexWord,
            Teacher teacher, out string errorMessage)
            => questionSet.ModifyQuestionSet(name, indexWord, teacher, out errorMessage);
        public List<ExamQuestionSet> GetAllQuestionSets()
            => ExamQuestionSet.GetAllQuestionSets();
        public List<ExamQuestionSet> GetQuestionSet(Predicate<ExamQuestionSet> filter)
            => ExamQuestionSet.GetQuestionSet(filter);
        public List<ExamQuestionSet> GetQuestionSet(string indexWord)
            => ExamQuestionSet.GetQuestionSet(indexWord);
        public List<ExamQuestion> GetAllQuestions(ExamQuestionSet questionSet, Teacher teacher, out string errorMessage)
        {
            List<ExamQuestion> ans;
            (ans, errorMessage) = questionSet.GetAllExamQuestions(teacher);
            return ans;
        }
        public List<ExamQuestion> GetQuestion(ExamQuestionSet questionSet, Teacher teacher, 
            Predicate<ExamQuestion> filter, out string errorMessage)
        {
            List<ExamQuestion> ans;
            (ans, errorMessage) = questionSet.GetExamQuestion(filter, teacher);
            return ans;
        }
        public List<ExamQuestion> GetQuestion(ExamQuestionSet questionSet, Teacher teacher, string indexWord, 
            out string errorMessage)
        {
            List<ExamQuestion> ans;
            (ans, errorMessage) = questionSet.GetExamQuestion(indexWord, teacher);
            return ans;
        }
        #endregion

        #region Authority
        public void AddToOperators(ExamQuestionSet set, Teacher creator, Teacher teacher, out string errorMessage)
            => set.AddToOperators(creator, teacher, out errorMessage);
        public bool HasOperateAuthority(ExamQuestionSet set, Teacher teacher)
            => set.HasOperateAuthority(teacher);
        public void RemoveFromOperators(ExamQuestionSet set, Teacher creator, Teacher teacher, out string errorMessage)
            => set.RemoveFromOperators(creator, teacher, out errorMessage);
        public void AddToUsers(ExamQuestionSet set, Teacher creator, Teacher teacher, out string errorMessage)
            => set.AddToUsers(creator, teacher, out errorMessage);
        public bool HasUseAuthority(ExamQuestionSet set, Teacher teacher)
            => set.HasUseAuthority(teacher);
        public void RemoveFromUsers(ExamQuestionSet set, Teacher creator, Teacher teacher, out string errorMessage)
            => set.RemoveFromUsers(creator, teacher, out errorMessage);
        #endregion

        #region MultipleChoiceQuestion
        public MultipleChoiceQuestion CreateMultipleChoiceQuestion(Teacher creator, ExamQuestionSet superiorSet,
            string explanation, string indexWord, ExamQuestion.QuestionType type, ushort optionNumber,
            ushort correctOptionNumber, out string errorMessage)
        {
            MultipleChoiceQuestion ans;
            (ans, errorMessage) = MultipleChoiceQuestion.CreateMultipleChoiceQuestion(creator,
                superiorSet, explanation, indexWord, type, optionNumber, correctOptionNumber);
            return ans;
        }
        public void SetQuestion(MultipleChoiceQuestion question, string trunk, string optionA, string optionB, 
            string optionC, string optionD, string optionE, Teacher teacher, out string errorMessage)
            => question.SetQuestion(trunk, optionA, optionB, optionC, optionD, optionE, teacher, out errorMessage);
        public bool CheckCorrectAnswerValidity(MultipleChoiceQuestion question, string answer, out string invalidity)
            => question.CheckAnswerValidity(answer, out invalidity);
        public bool SetMultipleChoiceAnswer(MultipleChoiceQuestion question, string answer, Teacher teacher,
            out string invalidity)
            => question.SetMultipleChoiceAnswer(answer, teacher, out invalidity);
        #endregion

        #region TrueFalseQuestion
        public TrueFalseQuestion CreateTrueFalseQuestion(Teacher creator, ExamQuestionSet superiorSet, string explanation,
            string indexWord, out string errorMessage)
            => TrueFalseQuestion.CreateTrueFalseQuestion(creator, superiorSet, explanation, indexWord, out errorMessage);
        public TrueFalseQuestion CreateTrueFalseQuestion(Teacher creator, TrueFalseQuestion other, out string errorMessage)
            => TrueFalseQuestion.CreateTrueFalseQuestion(creator, other, out errorMessage);
        public void SetAnswer(TrueFalseQuestion question, bool answer, Teacher teacher, out string errorMessage)
            => question.SetAnswer(answer, teacher, out errorMessage);
        public void SetQuestion(TrueFalseQuestion question, string trunk, Teacher teacher, out string errorMessage)
            => question.SetQuestion(trunk, teacher, out errorMessage);
        #endregion

        #region FreeResponseQuestion
        public FreeResponseQuestion CreateFreeResponseQuestion(Teacher teacher, ExamQuestionSet superiorSet,
            string indexWord, out string errorMessage)
            => FreeResponseQuestion.CreateFreeResponseQuestion(teacher, superiorSet, null, indexWord, out errorMessage);
        public FreeResponseQuestion CreateFreeResponseQuestion(Teacher teacher, FreeResponseQuestion other,
            out string errorMessage)
            => FreeResponseQuestion.CreateFreeResponseQuestion(teacher, other, out errorMessage);
        public void SetAnswer(FreeResponseQuestion question, string answer, Teacher teacher, out string errorMessage)
            => question.SetAnswer(answer, teacher, out errorMessage);
        #endregion

        #region ExamQuestion
        public void ModifyExamQuestion(ExamQuestion question, string explanation, string indexWord,
            Teacher teacher, out string errorMessage)
            => question.ModifyExamQuestion(explanation, indexWord, teacher, out errorMessage);
        public void ConfirmQuestionCreation(ExamQuestion question, Teacher teacher, out string errorMessage)
            => question.ConfirmQuestionCreation(teacher, out errorMessage);
        public string GetString(ExamQuestion.QuestionType type)
            => ExamQuestion.GetString(type);
        public string GetQuestionString(ExamQuestion question)
            => question.GetQuestionString();
        public string GetAnswerSstring(ExamQuestion question)
            => question.GetAnswerString();
        public void SetExplanation(ExamQuestion question, string explanation, Teacher teacher, out string errorMessage)
            => question.SetExplanation(explanation, teacher, out errorMessage);      
        #endregion
    }
}
