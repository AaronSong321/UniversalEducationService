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
    public class CourseChapter : DatabaseType, IOrderedDataContainer<CourseSection>
    {
        #region Fields and Properties
        public int CourseChapterID { get; private set; }
        public string ChapterName { get; set; }
        public string IndexWord { get; set; }
        public int Order { get; set; }
        public Teacher Creator { get; private set; }
        public Teacher LastModifyTeacher { get; private set; }
        public DateTime LastModifyTime { get; private set; }
        public Course SuperiorCourse { get; private set; }
        public List<CourseSection> InferiorSection { get; }
        #endregion

        #region Constructors and basic information
        public CourseChapter()
        {
            LastModifyTime = DateTime.Now;
            InferiorSection = new List<CourseSection>();
        }
        public CourseChapter(string name, string indexWord, int order, Teacher creator, Course superior)
        {
            ChapterName = name;
            IndexWord = indexWord;
            Order = order;
            Creator = creator;
            SuperiorCourse = superior;
            LastModifyTime = DateTime.Now;
            LastModifyTeacher = creator;
            InferiorSection = new List<CourseSection>();
        }

        public void ModifyChapter(string name, string indexWord, int order, Teacher teacher)
        {
            ChapterName = name;
            IndexWord = indexWord;
            Order = order;
            LastModifyTeacher = teacher;
            LastModifyTime = DateTime.Now;
        }
        #endregion

        #region Inherited and Implemented Members
        public override string CastObjectToJson()
            => JsonConvert.SerializeObject(this, new JsonSerializerSettings
            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        public override XmlDocument CastObjectToXml()
            => JsonConvert.DeserializeXmlNode(CastObjectToJson());
        public void ExchangeOrder(CourseSection data1, CourseSection data2)
        {
            if (!InferiorSection.Contains(data1))
                throw new InvalidOperationException(Course.DoesnotContain(this, data1));
            if (!InferiorSection.Contains(data2))
                throw new InvalidOperationException(Course.DoesnotContain(this, data2));
            else
            {
                int t = data1.Order;
                data1.Order = data2.Order;
                data2.Order = t;
                Settings.SaveDataModification(data1);
                Settings.SaveDataModification(data2);
            }
        }
        public void ChangeOrder(CourseSection data, ushort newOrder)
        {
            if (!InferiorSection.Contains(data))
                throw new InvalidOperationException(Course.DoesnotContain(this, data));
            else
            {
                data.Order = newOrder;
                Settings.SaveDataModification(data);
            }
        }
        public override string ToString()
            => $"Chapter {Order}: {ChapterName}";
        public override int GetHashCode()
            => CourseChapterID;
        public override bool Equals(object obj)
        {
            if (obj is CourseChapter chapter)
                return CourseChapterID == chapter.CourseChapterID;
            else return false;
        }

        #endregion
    }
}
