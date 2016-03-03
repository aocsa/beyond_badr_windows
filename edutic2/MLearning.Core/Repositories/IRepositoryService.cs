
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Core.Repositories
{
    public interface IRepositoryService
    {
         Task InsertAsync<T>(T entity);
         Task InsertAsync<T>(T entity, Dictionary<string, string> parameters);
         Task DeleteAsync<T>(T entity);
         Task<List<T>> SearchForAsync<T>(Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate,Func<T,int> getID,bool cacheResult) where T : new();
         Task<List<T>> SearchForAsync<T>(Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters, bool cacheResult) ;
		Task<List<T>> SearchForAsync<T>(Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters, Func<T,int> getID, bool cacheResult)where T : new();
         IQueryable<T> SearchForQuery<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters);
         Task<List<T>> SearchForWithCacheAsync<T>(Expression<Func<T, bool>> predicate, string identifier) where T : new();
         Task<List<T>> GetAllAsync<T>();
         Task<IQueryable<T>> GetAllQuery<T>();

		Task<T> SearchForFirstAsync<T> (System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate, Func<T,int> getID, bool cacheResult) where T : new() ;

        /// <summary>
        /// Obtiene todas los registros de una tabla y los almacena en cache
        /// </summary>
        /// <typeparam name="T">Tipo de objetos almacenado</typeparam>
        /// <param name="identifier">Usado para identificar el recurso almacenado en cache</param>
        /// <returns>Lista de objetos</returns>
         Task<List<T>> GetAllWithCacheAsync<T>(string identifier);
         Task<T> GetByIdAsync<T>(int id);

         Task<List<T>> GetFirstNAsync<T>(int n);
         Task UpdateAsync<T>(T entity);


         Task GetResource(string resourceName, string containerName);
         Task DownloadResource(string url, string filename);

         Task<JToken> InvokeGetApi(string apiname, Dictionary<string, string> parameters);


         void InitLocalSyncTable();

         Task TryGetTableUpdates();

         Task SyncTableUpdates();

         Task<MobileServiceUser> LoginProvider(int provider, JObject access_token);
         //int Count<T>();
         void Logout();
        
    }
}
