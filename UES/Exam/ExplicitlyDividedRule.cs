using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    [NotMapped]
    public class ExplicitlyDividedRule : ExamPaperRule
    {
        [NotMapped]
        public class RuleEntry
        {
            public string IndexWord { get; private set; }
            public ushort QuestionNumber { get; private set; }
            public ExamQuestion.QuestionType Type { get; private set; }
            public ushort MaxScore { get; private set; }

            internal protected RuleEntry(string indexWord, ushort number, ExamQuestion.QuestionType type, ushort maxScore)
            {
                IndexWord = indexWord;
                QuestionNumber = number;
                Type = type;
                MaxScore = maxScore;
            }
            internal protected RuleEntry(RuleEntry entry)
            {
                IndexWord = entry.IndexWord;
                QuestionNumber = entry.QuestionNumber;
                Type = entry.Type;
                MaxScore = entry.MaxScore;
            }
            internal protected void ModifyRuleEntry(string indexWord, ushort number, ExamQuestion.QuestionType type,
                ushort maxScore)
            {
                IndexWord = indexWord;
                QuestionNumber = number;
                Type = type;
                MaxScore = maxScore;
            }
        }

        public List<RuleEntry> Entries { get; private set; }

        public ExplicitlyDividedRule(Teacher creator, bool checkTotalScore = true): base(creator, checkTotalScore)
        {
            Entries = new List<RuleEntry>();
        }
        
        public void AddNewRuleEntry(string indexWord, ushort number, ExamQuestion.QuestionType type, ushort maxScore)
        {
            Entries.Add(new RuleEntry(indexWord, number, type, maxScore));
        }
        public void ModifyRuleEntry(RuleEntry entry, string indexWord, ushort number, ExamQuestion.QuestionType type,
            ushort maxScore)
        {
            entry.ModifyRuleEntry(indexWord, number, type, maxScore);
        }
        public void DeleteRuleEntry(RuleEntry entry) => Entries.Remove(entry);
        public RuleEntry DuplicateRuleEntry(RuleEntry entry)
        {
            var en = new RuleEntry(entry);
            Entries.Add(en);
            return en;
        }

        public bool AcommadateRule(ExamQuestion question, RuleEntry entry)
        {
            return (question.IndexWord.Contains(entry.IndexWord) || question.GetQuestionString().Contains(entry.IndexWord))
                && question.SatisfyQuestionType(entry.Type);
        }
        
        public ushort ChooseQuestions(ExamPaper paper, ExamQuestionSet questionSet)
        {
            var questions = questionSet.QuestionSet;
            var myset = new List<ExamQuestion>();
            foreach (var q in questions)
                myset.Add(q);
            ushort ans = 0;
            foreach (var entry in Entries)
            {
                var tempset = new List<ExamQuestion>();
                foreach (var q in myset)
                    if (AcommadateRule(q, entry) && !paper.ContainsQuestion(q))
                        tempset.Add(q);
                if (tempset.Count == 0)
                    throw new InvalidOperationException($"The current question set does not contain enough" +
                        $"questions accomadating the current rule entry.");
                for (int i = 0; i < entry.QuestionNumber; i++)
                {
                    Random r = new Random();
                    var num = r.Next(0, tempset.Count);
                    paper.ChooseQuestion(tempset[num], entry.MaxScore);
                    tempset.Remove(tempset[num]);
                    ans++;
                }
            }
            return ans;
        }

        public override bool CheckRuleValidity(ExamPaper paper, out string errorMessage)
        {
            bool ans = true;
            errorMessage = null;
            if (CheckTotalScore)
            {
                float score = paper.CurrentScore;
                foreach (var question in Entries)
                    score += question.MaxScore * question.QuestionNumber;
                if (score != paper.MaxScore)
                {
                    ans = false;
                    errorMessage += $"Total score violation: current score is {paper.CurrentScore}, the total score after " +
                        $" new questions being chosen is {score}, unequal to the paper's max score {paper.MaxScore}\n";
                }
            }
            return ans;
        }

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}
