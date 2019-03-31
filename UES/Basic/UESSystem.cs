using HIT.UES.Exam;
using HIT.UES.Login;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIT.UES
{
    public class UESSystem
    {
        public LoginSubsystem LoginSubsystem;
        public ExamSubsystem ExamSubsystem;
        public UESSystem()
        {
            LoginSubsystem = new LoginSubsystem();
            ExamSubsystem = new ExamSubsystem();
        }

        #region in test
        public UESContext context;
        public string databaseName;
        public void SaveDataCreation(DatabaseType data)
        {
            context.Entry(data).State = EntityState.Added;
            context.SaveChanges();
        }
        public void SaveDataRemove(DatabaseType data)
        {
            context.Entry(data).State = EntityState.Deleted;
            context.SaveChanges();
        }
        public void SaveDataDetach(DatabaseType data)
        {
            context.Entry(data).State = EntityState.Detached;
            context.SaveChanges();
        }
        public void SaveDataModification(DatabaseType data)
        {
            context.Entry(data).State = EntityState.Modified;
            context.SaveChanges();
        }
        #endregion
    }
}
