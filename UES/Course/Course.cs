using HIT.UES.Login;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HIT.UES.Course
{
    public class Course : DatabaseType, IOrderedDataContainer<CourseChapter>
    {
        public int CourseID { get; }
        public string CourseName { get; private set; }
        public string IndexWord { get; private set; }
        public Teacher Creator { get; }
        public DateTime CreationTime { get; }
        public DateTime LastModifyTime { get; private set; }
        public string Description { get; set; }
        public virtual List<Teacher> AuthroizedTeacher { get; }
        public List<CourseChapter> Chapters { get; }



        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());

        public void ChangeOrder(CourseChapter data, ushort newOrder)
        {
            throw new NotImplementedException();
        }

        public void ExchangeOrder(CourseChapter data1, CourseChapter data2)
        {
            throw new NotImplementedException();
        }
    }
}
