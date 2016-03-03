using Cirrious.CrossCore;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Core.DownloadCache;
using Core.Session;
using Microsoft.WindowsAzure.MobileServices;

using MLearning.Core.Entities;
using MLearning.Core.Entities.json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Classes;
using MLearning.Core.Configuration;
using Cirrious.MvvmCross.Plugins.File;
using MLearning.Core.Classes;
using MLearningDB;
using System.Diagnostics;




namespace Core.Repositories
{
	public class WAMSRepositoryService : IRepositoryService
	{

		public  MobileServiceClient MobileService;
		private ISQLiteConnection _liteConnection;


		private int _takeNRows = 500;

		public bool UseLocalDBWhenOffline { get; private set; }


		public WAMSRepositoryService(ISQLiteConnectionFactory factory)
		{

			UseLocalDBWhenOffline = true;
			MobileService = new MobileServiceClient("https://eduticservice2016.azure-mobile.net/", "EHmeLHXJUiWhHBHYYAoOtsYJLwSZWh33");
			//MobileService = new MobileServiceClient("https://mlearningservice.azure-mobile.net/", "xIAzBqsUDUutvnCTruCwpCozkkyNkj33");
			//	MobileService = new MobileServiceClient("https://eduticservice.azure-mobile.net/", "dbKHHcwqYgLERWaOCVSHfccSQIWSKv93");
			_liteConnection = factory.Create(Constants.LocalDbName);


			//When App starts

			TryGetTableUpdates();

			int i = 0;

			i++;


			TryGetTableUpdates();


		}
		public WAMSRepositoryService()
		{

			//    CurrentPlatform.Init();
			MobileService = new MobileServiceClient("https://eduticservice2016.azure-mobile.net/", "EHmeLHXJUiWhHBHYYAoOtsYJLwSZWh33");
			//MobileService = new MobileServiceClient("https://mlearningservice.azure-mobile.net/", "xIAzBqsUDUutvnCTruCwpCozkkyNkj33");
			//MobileService = new MobileServiceClient("https://eduticservice.azure-mobile.net/", "dbKHHcwqYgLERWaOCVSHfccSQIWSKv93");



			UseLocalDBWhenOffline = false;

			//TryGetTableUpdates();
		}

		public async Task<MobileServiceUser> LoginProvider(int provider, JObject access_token)
		{
			if (provider == 3)
				return await MobileService.LoginAsync(MobileServiceAuthenticationProvider.Facebook, access_token);
			else
				return null;
		}
		public void Logout()
		{
			MobileService.Logout();
		}
		public async System.Threading.Tasks.Task InsertAsync<T>(T entity)
		{

			try
			{
				await MobileService.GetTable<T>().InsertAsync(entity);


			}
			catch (MobileServiceInvalidOperationException e)
			{

				throw;
			}
			catch (NullReferenceException nref)
			{
				Mvx.Trace(nref.Message);
			}


		}

		public async Task InsertAsync<T>(T entity, Dictionary<string, string> parameters)
		{
			await MobileService.GetTable<T>().InsertAsync(entity,parameters);
		}

		public async System.Threading.Tasks.Task DeleteAsync<T>(T entity)
		{
			try
			{         

				await MobileService.GetTable<T>().DeleteAsync(entity);
			}
			catch (MobileServiceInvalidOperationException e)
			{

				throw e;
			}
		}


		public List<T> SearchForLocalTable<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T:new()
		{
			return _liteConnection.Table<T>().Where(predicate).ToList();
		}

		public List<T> SearchForLocalTable<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate,int toSkip,int toTake) where T : new()
		{
			return _liteConnection.Table<T>().Where(predicate).Skip(toSkip).Take(toTake).ToList();
		}


		public List<T> SearchForLocalTable<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate,Dictionary<string, string> p) where T : new()
		{
			return _liteConnection.Table<T>().Where(predicate).ToList();
			//result = await MobileService.GetTable<T>().Take(1000).Where(predicate).WithParameters(parameters).IncludeTotalCount().ToListAsync();
			//return _liteConnection.Table<T>().Where(predicate).Where(t=>t.id == ).ToList();
		}



		int CreateTableInLocalDB<T>()
		{


			bool has_property = typeof(T).HasProperty("id_pk");
			CreateFlags flag;

			if (has_property)
			{
				flag = CreateFlags.None;
			}
			else
			{
				flag = CreateFlags.ImplicitPK;
			}


			return  _liteConnection.CreateTable<T>(flag);
		}

		public  bool TableExists(string tableName)
		{

			string text = "SELECT count(*) FROM sqlite_master WHERE type = 'table' AND name = '"+tableName+"'";
			ISQLiteCommand cmd = _liteConnection.CreateCommand(text);
			int r = cmd.ExecuteScalar<Int32>();

			return (r != 0);
		}


		//Call everytime the App starts
		public async Task SyncTableUpdates()
		{
			try {
				var result = await MobileService.GetTable<table_update>().Where(t=>t.id>0).LoadAllAsync();
				Debug.WriteLine ("********* &&&&& " + result.Count);
				// var result = await MobileService.GetTable<table_update>().Take(_takeNRows).ToListAsync();
				_liteConnection.DeleteAll<table_update>();
				foreach (var item in result)
				{
					//Convert to UTC
					item.updated_at = item.updated_at.ToUniversalTime();
					_liteConnection.Insert(item);
				}
			} catch (Exception e) {

				throw(e);
			}
		}


		public async Task TryGetTableUpdates()
		{

			if (TableExists("table_update")==false)
			{
				int r = CreateTableInLocalDB<table_update>();

			}

			await SyncTableUpdates();

		}


		async Task<DateTime> TableHasUpdate<T>()
		{
			string classname = typeof(T).Name;

			DateTime lastCloudUpdate = new DateTime();
			DateTime lastSync = new DateTime();

			//Create table in localDB if it doesnt exist

			if(TableExists("table_sync")==false)
				InitLocalSyncTable();
			//  int r =  _liteConnection.CreateTable<T>(CreateFlags.ImplicitPK);
			int r = CreateTableInLocalDB<T>();

			//var result = await MobileService.GetTable<table_update>().Where(t => t.table_name == classname).ToListAsync();



			var result = _liteConnection.Table<table_update>().Where(t => t.table_name == classname).ToList();

			if (result.Count > 0)
			{
				lastCloudUpdate = result.FirstOrDefault().updated_at;             
			}

			table_sync tb = _liteConnection.Table<table_sync>().Where(t => t.table_name == classname).FirstOrDefault();

			if (tb != null)
			{
				lastSync = tb.synced_at;
			}


			if (lastSync < lastCloudUpdate)
				return lastSync;

			return DateTime.MaxValue;

		}


		/// <summary>
		/// Get Cloud rows updated since lastSync and store them in LocalDB
		/// </summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="lastSync">Date limit</param>
		/// <param name="getID">Function to get Template ID</param>
		async Task SyncLocalTable<T>(DateTime lastSync,Func<T,int> getID) where T:new()
		{

			// Trying to get all rows in the table T
			//LastSynt Funcionality not Working YET------ Reason: Delete needs extra coding
			var result = await MobileService.GetTable<T>().WithParameters(new Dictionary<string, string> { { "lastsync", lastSync.ToString("yyyy-MM-ddTHH:mm:sszzz", DateTimeFormatInfo.InvariantInfo) } }).LoadAllAsync();
			//  var result = await MobileService.GetTable<T>().Take(_takeNRows).WithParameters(new Dictionary<string, string> { { "lastsync", lastSync.ToString("yyyy-MM-ddTHH:mm:sszzz", DateTimeFormatInfo.InvariantInfo) } }).ToListAsync();






			string classname = typeof(T).Name;

			_liteConnection.DeleteAll<T>();

			/*       //If id exists in local db delete 
			foreach (var item in result)
			{                
				_liteConnection.Delete(item);
			}*/

			//Insert all

			var storage = Mvx.Resolve<IMvxFileStore>();



			//   _liteConnection.Execute(string.Format("PRAGMA temp_store_directory = '{0}';", storage.NativePath(string.Empty)));
			_liteConnection.BeginTransaction();


			foreach (var tuple in result)
			{
				_liteConnection.Insert(tuple);
			}

			_liteConnection.Commit();



			//Update sync table


			var table_s = _liteConnection.Table<table_sync>().Where(t => t.table_name == classname).FirstOrDefault();

			if (table_s != null)
			{
				table_s.synced_at = DateTime.UtcNow;

				_liteConnection.Update(table_s);
			}
			else
			{
				Mvx.Trace("Table name not found in table_sync. Check SyncLocalTable Method line on WAMSRepositoryService");
				throw new NullReferenceException();
			}




		}


		//If cacheResult is true, save the results in localDB and use synchronization 
		public async Task<List<T>> SearchForAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate,Func<T,int> getID, bool cacheResult) where T : new()
		{
			List<T> result;

			try
			{

				//If want to cache result

				if (cacheResult)
				{

					//Check synchronization dates
					DateTime lastSync =  await TableHasUpdate<T>();

					if (lastSync != DateTime.MaxValue)
					{
						//Have Update
						//Get results from lastSyncDate and save them to DB if cacheResult its true
						//TODO: Filter with predicate
						await SyncLocalTable<T>(lastSync, getID);


					}


					//No Update, use local
					result = SearchForLocalTable<T>(predicate);

				}
				else
				{
					result = await MobileService.GetTable<T>().Take(_takeNRows).Where(predicate).ToListAsync();


				}




			}
			catch (WebException e)
			{
				if (UseLocalDBWhenOffline)
				{
					result = SearchForLocalTable<T>(predicate);
				}
				else
				{
					throw e;
				}
			}


			return result;



		}




		//If cacheResult is true, save the results in localDB and use synchronization 
		public async Task<T> SearchForFirstAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate,Func<T,int> getID, bool cacheResult) where T : new()
		{
			List<T> result;

			try
			{

				//If want to cache result

				if (cacheResult)
				{

					//Check synchronization dates
					DateTime lastSync =  await TableHasUpdate<T>();

					if (lastSync != DateTime.MaxValue)
					{
						//Have Update
						//Get results from lastSyncDate and save them to DB if cacheResult its true
						//TODO: Filter with predicate
						await SyncLocalTable<T>(lastSync, getID);

					}
					//No Update, use local
					result = SearchForLocalTable<T>(predicate);

				}
				else
				{
					result = await MobileService.GetTable<T>().Take(_takeNRows).Where(predicate).ToListAsync();

					int c = result.Count;


				}




			}
			catch (WebException e)
			{
				if (UseLocalDBWhenOffline)
				{
					result = SearchForLocalTable<T>(predicate);
				}
				else
				{
					throw e;
				}
			}


			return result.FirstOrDefault();



		}

		// My IMPLEMENTATION


		public async Task<List<T>> SearchForAsync<T> (Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters, Func<T, int> getID, bool cacheResult) where T : new()
		{
			List<T> result;

			try
			{

				//If want to cache result

				if (cacheResult)
				{

					//Check synchronization dates
					DateTime lastSync =  await TableHasUpdate<T>();

					if (lastSync != DateTime.MaxValue)
					{
						//Have Update
						//Get results from lastSyncDate and save them to DB if cacheResult its true
						//TODO: Filter with predicate
						await SyncLocalTable<T>(lastSync, getID);

					}

					//No Update, use local
					result = SearchForLocalTable<T>(predicate,parameters);

				}
				else
				{
					result = await MobileService.GetTable<T>().Take(1000).Where(predicate).WithParameters(parameters).IncludeTotalCount().ToListAsync();

				}




			}
			catch (WebException e)
			{
				if (UseLocalDBWhenOffline)
				{
					result = SearchForLocalTable<T>(predicate);
				}
				else
				{
					throw e;
				}
			}


			return result;



		}







		public async Task<List<T>> SearchForAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters, bool cacheResult)
		{



			List<T> toReturn ;
			try
			{
				toReturn = await MobileService.GetTable<T>().Take(1000).Where(predicate).WithParameters(parameters).IncludeTotalCount().ToListAsync();
			}
			catch (Exception e)
			{
				throw e;
			}

			return toReturn;

		}

		public IQueryable<T> SearchForQuery<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Dictionary<string, string> parameters)
		{
			return MobileService.GetTable<T>().Where(predicate).WithParameters(parameters).IncludeTotalCount().Query;


		}



		public async Task<List<T>> SearchForAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, Func<T, DateTime> getLastUpdate, Func<T, int> getID, bool cacheResult,int skip,int take) where T : new()
		{
			List<T> result;

			try
			{

				//If want to cache result

				if (cacheResult)
				{

					//Check synchronization dates
					DateTime lastSync = await TableHasUpdate<T>();

					if (lastSync != DateTime.MaxValue)
					{
						//Have Update
						//Get results from lastSyncDate and save them to DB if cacheResult its true
						//TODO: Filter with predicate
						await SyncLocalTable<T>(lastSync, getID);

					}

					//No Update, use local
					result = SearchForLocalTable<T>(predicate,skip,take);

				}
				else
				{
					result = await MobileService.GetTable<T>().Take(_takeNRows).Where(predicate).ToListAsync();

				}




			}
			catch (WebException e)
			{
				if (UseLocalDBWhenOffline)
				{
					result = SearchForLocalTable<T>(predicate);
				}
				else
				{
					throw e;
				}
			}


			return result;



		}

		public async Task<List<T>> GetAllAsync<T>()
		{
			return  await MobileService.GetTable<T>().Select(x => x).ToListAsync();
		}

		public async Task<IQueryable<T>> GetAllQuery<T>()
		{
			var t = MobileService.GetTable<T>().IncludeTotalCount().Select(x => x);
			var w = await t.ToEnumerableAsync();
			//w.Start(); w.Wait();
			var c = t.RequestTotalCount;
			return t.Query;
		}

		public async Task<T> GetByIdAsync<T>(int id)
		{
			return await MobileService.GetTable<T>().LookupAsync(id);
		}


		public async System.Threading.Tasks.Task<List<T>> GetFirstNAsync<T>(int n)
		{
			return await MobileService.GetTable<T>().Take(n).ToListAsync();
		}

		public async System.Threading.Tasks.Task GetResource(string resourceName, string containerName)
		{

			try
			{
				List<Core.Entities.Resource> result =  await SearchForAsync<Core.Entities.Resource>(r => r.LocalPath == resourceName && r.ContainerName == containerName,(it)=>DateTime.UtcNow,it=>0,false);
				foreach (var r in result)
				{
					await DownloadResource(r.SasQueryString, r.LocalPath);
				}
			}
			catch (MobileServiceInvalidOperationException e)
			{
				string m = e.Message;

			}






		}


		public async Task DownloadResource(string url, string filename)
		{          

			using (var client = new HttpClient())
			{
				var content = await client.GetByteArrayAsync(new Uri(url));
				//   var storage = Mvx.Resolve<IMvxFileStore>();

				// storage.WriteFile(filename, content);
			}
		}


		public async Task<JToken> InvokeGetApi(string apiname, Dictionary<string, string> parameters)
		{


			return await MobileService.InvokeApiAsync(apiname, System.Net.Http.HttpMethod.Get, parameters);


			//return  await MobileService.InvokeApiAsync<T>(apiname, System.Net.Http.HttpMethod.Get, null);

		}


		public async Task<List<T>> SearchForWithCacheAsync<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string identifier) where T : new()
		{
			List<T> list = new List<T>() ;
			WebException connectionError = null;

			CacheService service = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);
			try
			{
				list = await SearchForAsync<T>(predicate, (it) => DateTime.UtcNow, it => 0, false);



				await service.cacheObjectList<T>(identifier, list);
			}
			catch (WebException e)
			{
				connectionError = e;
			}


			if (connectionError != null)
			{
				bool ok;
				string localpath;

				var localResource = await service.tryReadFileFromLocalCache(CacheService.GetStringWithPrefix(identifier));
				var bytes = localResource.Bytes;
				ok = localResource.InCache;
				localpath = localResource.LocalPath;

				if (ok)
				{
					list = service.deserializeObjectListFromBytes<List<T>>(bytes);
				}
				else
				{
					throw connectionError;
				}
			}

			return list;

		}


		public async Task UpdateAsync<T>(T entity)
		{
			await MobileService.GetTable<T>().UpdateAsync(entity);
		}


		public async Task<List<T>> GetAllWithCacheAsync<T>(string identifier)
		{
			List<T> list = new List<T>();
			WebException connectionError = null;
			CacheService service = CacheService.Init(SessionService.GetCredentialFileName(), Constants.PreferencesFileName, Constants.LocalDbName);
			try
			{
				list = await GetAllAsync<T>();



				await service.cacheObjectList<T>(identifier, list);
			}
			catch (WebException e)
			{
				connectionError = e;
			}


			if (connectionError != null)
			{
				bool ok;
				string localpath;
				//var localResource = await service.tryReadFileFromLocalCache(CacheService.GetStringWithPrefix(identifier));
				var localResource = await service.tryReadFileFromLocalCache(CacheService.GetStringWithPrefix(identifier));
				var bytes = localResource.Bytes;
				ok = localResource.InCache;
				localpath = localResource.LocalPath;

				if (ok)
				{
					list = service.deserializeObjectListFromBytes<List<T>>(bytes);
				}
				else
				{
					throw connectionError;
				}
			}

			return list;
		}

		public void InitLocalSyncTable()
		{
			Assembly myassembly = GetType().GetTypeInfo().Assembly;


			int r =   _liteConnection.CreateTable<table_sync>();




			HashSet<string> names = new HashSet<string>();
			foreach (TypeInfo tp in myassembly.DefinedTypes)
			{

				if (tp.DeclaredProperties.Any(p => p.Name == "id"))
				{
					names.Add(tp.Name);
				}

			}

			foreach (var item in names)
			{
				_liteConnection.Insert(new table_sync { table_name = item });
			}

		}

		public async Task<DataPage<T>> GetPage<T>(Expression<Func<T, bool>> predicate, int skip, int take)
		{
			var query = MobileService.GetTable<T>().Where(predicate).Skip(skip).Take(take).IncludeTotalCount();
			var results = await query.ToEnumerableAsync();
			DataPage<T> p = new DataPage<T>();
			p.totalCount = ((ITotalCountProvider)results).TotalCount;
			p.data = await query.ToListAsync();
			return p;
		}

		/*public async Task<long> Count<T>()
        {
            var query = MobileService.GetTable<T>().Take(1).IncludeTotalCount();
            var results = await query.ToEnumerableAsync();
            return ((ITotalCountProvider)results).TotalCount;
        }*/

	}
}
