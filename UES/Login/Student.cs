using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using HIT.UES.Exam;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HIT.UES.Login
{
    public class Student : DatabaseType
    {
        [Key]
        public int StudentID { get; private set; }
        [MaxLength(30)]
        public string PersonName { get; private set; }
        //public List<MessageBoard> MessageBoardSubscriped { get; }
        //public List<Message> MessageReceived { get; }
        //public List<Course> CourseAttended { get; }
        [NotMapped]
        public List<Exam.Exam> ExamRegistered { get; }

        public Student()
        {
            ExamRegistered = new List<Exam.Exam>();
        }
        public Student(string name)
        {
            PersonName = name;
            ExamRegistered = new List<Exam.Exam>();
        }

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}