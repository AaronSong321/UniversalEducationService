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
    public class CourseSection : DatabaseType, IOrderedDataContainer<CourseContent>
    {
        public int CourseSectionID { get; private set; }
        public string SectionName { get; set; }
        public CourseChapter SuperiorChapter { get; private set; }
        public int Order { get; set; }
        public List<CourseContent> Contents { get; private set; }
        public Teacher LastModifyTeacher { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        
        public CourseSection()
        {
            Contents = new List<CourseContent>();
        }
        public CourseSection(string name, CourseChapter superior, Teacher creator, int order)
        {
            SectionName = name;
            SuperiorChapter = superior;
            Order = order;
            LastModifyTeacher = creator;
            LastModifyTime = DateTime.Now;
            Contents = new List<CourseContent>();
        }

        #region Inherited and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        public void ExchangeOrder(CourseContent data1, CourseContent data2)
        {
            if (!Contents.Contains(data1))
                throw new InvalidOperationException(Course.DoesnotContain(this, data1));
            else if (!Contents.Contains(data2))
                throw new InvalidOperationException(Course.DoesnotContain(this, data2));
            else
            {
                int temp = data1.Order;
                data1.Order = data2.Order;
                data2.Order = temp;
                Settings.SaveDataModification(data1);
                Settings.SaveDataModification(data2);
            }
        }
        public void ChangeOrder(CourseContent data, ushort newOrder)
        {
            if (!Contents.Contains(data))
                throw new InvalidOperationException(Course.DoesnotContain(this, data));
            else
            {
                data.Order = newOrder;
                Settings.SaveDataModification(data);
            }
        }
        public override string ToString()
            => $"Section {SuperiorChapter.Order}.{Order}: {SectionName}";
        public override int GetHashCode()
            => CourseSectionID;
        public override bool Equals(object obj)
        {
            if (obj is CourseSection section)
                return CourseSectionID == section.CourseSectionID;
            else return false;
        }

        #endregion
    }
}
