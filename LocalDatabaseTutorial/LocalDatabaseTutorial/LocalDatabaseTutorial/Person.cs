using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace LocalDatabaseTutorial
{
    public class Person
    {
        [PrimaryKey, AutoIncrement]
        public int? ID { get; set; } = null;
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
