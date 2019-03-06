using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES
{
    public static class Settings
    {
        //internal static void Log(string s) => Console.WriteLine(s);
        public static UESContext uesContext;
        public static void InitDatabase(string databaseName = "HIT.UES.Database")
        {
            uesContext = new UESContext(databaseName);
            //uesContext.Database.Log = Console.Write;

            uesContext.Administrators.Load();
            uesContext.Students.Load();
            uesContext.Teachers.Load();
            uesContext.Exams.Load();
            uesContext.ExamPapers.Load();
            uesContext.ExamPaperInstances.Load();
            uesContext.ExamQuestions.Load();
            uesContext.ExamQuestionSets.Load();
            
        }

        public static void SaveDataCreation(DatabaseType data)
        {
            uesContext.Entry(data).State = EntityState.Added;
            uesContext.SaveChanges();
        }
        public static void SaveDataRemove(DatabaseType data)
        {
            uesContext.Entry(data).State = EntityState.Deleted;
            uesContext.SaveChanges();
        }
        public static void SaveDataDetach(DatabaseType data)
        {
            uesContext.Entry(data).State = EntityState.Detached;
            uesContext.SaveChanges();
        }
        public static void SaveDataModification(DatabaseType data)
        {
            uesContext.Entry(data).State = EntityState.Modified;
            uesContext.SaveChanges();
        }
    }
}
