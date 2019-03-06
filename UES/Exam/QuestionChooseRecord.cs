using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HIT.UES.Login;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIT.UES.Exam
{
    public class QuestionChooseRecord : DatabaseType
    {
        public int QuestionChooseRecordID { get; private set; }
        public ExamPaper SuperiorExamPaper { get; private set; }
        public int QuestionOrder { get; private set; }
        public int MaxScore { get; private set; }
        public bool ChooseFinished { get; private set; }
        public ExamQuestion Question { get; private set; }

        public QuestionChooseRecord()
        {
            ChooseFinished = false;
        }
        public QuestionChooseRecord(ExamPaper paper, int order, int score, ExamQuestion question)
        {
            SuperiorExamPaper = paper;
            QuestionOrder = order;
            MaxScore = score;
            Question = question;
            ChooseFinished = false;
        }
        public QuestionChooseRecord(ExamPaper paper, int score, ExamQuestion question)
        {
            SuperiorExamPaper = paper;
            QuestionOrder = (ushort)(paper.GetQuestionNumber() + 1);
            MaxScore = score;
            Question = question;
            ChooseFinished = false;
        }
        public QuestionChooseRecord(ExamPaper paper, QuestionChooseRecord record)
        {
            SuperiorExamPaper = paper;
            QuestionOrder = (ushort)(paper.GetQuestionNumber() + 1);
            MaxScore = record.MaxScore;
            Question = record.Question;
            ChooseFinished = false;
        }
        public void ModifyOrder(int order) => QuestionOrder = order;

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}
