using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using PersonelAccounting.Model.Core;

namespace PersonelAccounting.Data.Core
{
    public class Repository<T> : IRepository<T> where T : class, IIdentifier
    {

        protected DbContext Context { get; set; }
        internal DbSet<T> EntitySet { get; set; }

        public Repository(DbContext context)
        {
            Context = context;
            EntitySet = context.Set<T>();
        }

        private static Tuple<string, object[]> PrepareArguments(string storedProcedure, object parameters)
        {
            var parameterNames = new List<string>();
            var parameterParameters = new List<object>();
            if (parameters != null)
            {
                foreach (PropertyInfo propertyInfo in parameters.GetType().GetProperties())
                {
                    string name = "@" + propertyInfo.Name;
                    object value = propertyInfo.GetValue(parameters, null);

                    parameterNames.Add(name);
                    parameterParameters.Add(new SqlParameter(name, value ?? DBNull.Value));
                }
            }
            if (parameterNames.Count > 0)
                storedProcedure += " " + string.Join(", ", parameterNames);
            return new Tuple<string, object[]>(storedProcedure, parameterParameters.ToArray());
        }

        public virtual IEnumerable<T> ExecuteProcedure(string procedureName, object parameters)
        {
            var arguments = PrepareArguments(procedureName, parameters);
            return Context.Database.SqlQuery<T>(arguments.Item1, arguments.Item2);
        }

        public virtual IQueryable<T> GetAll()
        {
            return EntitySet.AsQueryable();
        }


        public T GetById(int id)
        {
            T entity = EntitySet.Find(id);
            return entity;
        }

        public virtual T InsertOrUpdate(T entity)
        {
            T result = null;
            if (((IIdentifier)entity).Id == 0)
            {
                result = EntitySet.Add(entity);
            }
            else
            {
                return Update(entity);
            }
            return result;
        }

        public virtual T Delete(T entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                EntitySet.Attach(entityToDelete);
            }
            return EntitySet.Remove(entityToDelete);
        }

        public virtual T Update(T entityToUpdate)
        {
            try
            {
                T result = EntitySet.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientValues = (T)entry.Entity;
                var databaseValues = (T)entry.GetDatabaseValues().ToObject();

                throw ex;
            }
        }
    }
}
