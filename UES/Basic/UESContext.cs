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
    public class UESContext : DbContext
    {
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Exam.Exam> Exams { get; set; }
        public virtual DbSet<ExamPaper> ExamPapers { get; set; }
        public virtual DbSet<ExamPaperInstance> ExamPaperInstances { get; set; }
        public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }
        public virtual DbSet<ExamQuestionSet> ExamQuestionSets { get; set; }
        //public DbSet<ExamPaperRule> ExamPaperRules { get; set; }


        public UESContext(string databaseName): base(databaseName)
        {
            //var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
        public UESContext(): base()
        {
            //var instance = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }
}
