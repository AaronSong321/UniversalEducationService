using HIT.UES.Exam;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES.Exam
{
    [NotMapped]
    public class AutoCheckRule
    {
        public virtual void CheckAnswer(StudentAnswerRecord record)
        {
            switch (record.SuperiorQuestion.ExamQuestionType)
            {
                case ExamQuestion.QuestionType.SingleChoice:
                case ExamQuestion.QuestionType.TrueFalse:
                    if (record.Answer == record.SuperiorQuestion.GetAnswerString())
                        record.GiveMaxScore(this);
                    else
                        record.GiveScore(this, 0);
                    break;
                case ExamQuestion.QuestionType.MultipleChoice:
                case ExamQuestion.QuestionType.DisorientedChoice:
                    record.GiveScore(this, CheckMultipleChoiceAnswer(record));
                    break;
            }
        }
        protected virtual double CheckMultipleChoiceAnswer(StudentAnswerRecord record)
        {
            var answer = record.Answer;
            var correct = record.SuperiorQuestion.GetAnswerString();
            ushort op = 0;
            foreach (var ans in answer)
            {
                if (correct.Contains(ans))
                    op++;
                else
                {
                    op = 0;
                    break;
                }
            }
            return record.MaxScore * 1.0f * op / correct.Length;
        }
    }
}
