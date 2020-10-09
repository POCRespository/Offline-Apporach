using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace LocalDatabaseTutorial
{
   public class Database
    {
        private const int V = 1;
        readonly SQLiteAsyncConnection _database;
        private readonly object person;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Person>().Wait();
        }
        public Task<List<Person>> GetPeopleASync()
        {
            return _database.Table<Person>().ToListAsync();
        }
       
        public Task<List<Person>> GetPeopleForPosting()
        {
            return _database.Table<Person>().ToListAsync();
        }
        public Task<int> savePersonAsync(Person person)
        {
            if (person.ID != 0)
            {
                return _database.UpdateAsync(person);
            }
            else
            {
                return _database.InsertAsync(person);
            }
        }
        public async Task<int> Post(List<Person> persons)
        {
            
                try
                {                
                 return  await  _database.InsertAllAsync(persons);
                    
                }
                catch(Exception e)
                {
                throw e;
                }
            //return _database.UpdateAsync(person);

        }
          
           
        }


    }

