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
        [MaxLength(30), Required]
        public string PersonName { get; private set; }
        [MinLength(6),MaxLength(12),Required]
        public string Password { get; private set; }
        //public List<MessageBoard> MessageBoardSubscriped { get; }
        //public List<Message> MessageReceived { get; }
        //public List<Course> CourseAttended { get; }
        public List<Exam.Exam> GetExamRegistered() //{ get; private set; }
            => (from b in Settings.uesContext.Exams where b.SignedInStudents.Contains(this) select b).ToList();

        public Student()
        {

        }
        public Student(string name, string password)
        {
            PersonName = name;
            Password = password;
        }

        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
    }
}