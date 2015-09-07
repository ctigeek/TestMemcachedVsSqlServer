using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace TestSqlVsMemcached
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                int size = int.Parse(args[0]);
                var result = TestMemcachedWrite(size);
                result = TestSqlCacheWrite(size);

                Thread.Sleep(50);
                result = TestMemcachedWrite(size);
                Debug.WriteLine("Memcached write: " + result.ToString());
                Console.WriteLine("Memcached write: " + result.ToString());

                result = TestSqlCacheWrite(size);
                Debug.WriteLine("Sql Write: " + result.ToString());
                Console.WriteLine("Sql Write: " + result.ToString());

                result = TestSqlCacheWrite2(size);
                Debug.WriteLine("Sql Write2: " + result.ToString());
                Console.WriteLine("Sql Write2: " + result.ToString());

                result = TestMemcachedRead(size);
                Debug.WriteLine("Memcached read: " + result.ToString());
                Console.WriteLine("Memcached read: " + result.ToString());

                result = TestSqlCacheRead(size);
                Debug.WriteLine("Sql Read: " + result.ToString());
                Console.WriteLine("Sql Read: " + result.ToString());

                result = TestSqlCacheRead2(size);
                Debug.WriteLine("Sql Read2: " + result.ToString());
                Console.WriteLine("Sql Read2: " + result.ToString());
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            Console.WriteLine("Press any key....");
            Console.ReadLine();
        }


        private static long TestMemcachedWrite(int length)
        {
            var client = new MemcachedClient();

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store(StoreMode.Set, "asdfasdf" + i.ToString(), "This is a value being stored into memcached.");
            }

            sw.Stop();
            client.Dispose();
            return sw.ElapsedMilliseconds;
        }

        private static long TestMemcachedRead(int length)
        {
            var client = new MemcachedClient();

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString()).Value.ToString();
                if (result != "This is a value being stored into memcached.") throw new Exception();
            }

            sw.Stop();
            client.Dispose();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheRead(int length)
        {
            var client = new SqlCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString());
                if (result != "This is a value being stored into memcached.") throw new Exception();
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheRead2(int length)
        {
            var client = new SqlCacheClient2("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString());
                if (result != "This is a value being stored into memcached.") throw new Exception();
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheWrite(int length)
        {
            var client = new SqlCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store("asdfasdf" + i.ToString(), "This is a value being stored into memcached.");
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheWrite2(int length)
        {
            var client = new SqlCacheClient2("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store("asdfasdf" + i.ToString(), "This is a value being stored into memcached.");
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }

    public class SqlCacheClient
    {
        public readonly string ConnectionStringName;
        private readonly string connectionString;
        private readonly SHA256Managed hasher;

        public SqlCacheClient(string connectionStringName)
        {
            this.ConnectionStringName = connectionStringName;
            connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            hasher = new SHA256Managed();
        }

        private Guid GetUidKey(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            var hash = hasher.ComputeHash(bytes);
            var guid = new Guid(hash.Take(16).ToArray());
            return guid;
        }

        public void Store(string key, string value)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand("SaveCacheItem", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));
                comm.Parameters.AddWithValue("body", value);
                comm.Parameters.AddWithValue("expiration", DateTime.Now);
                comm.ExecuteNonQuery();
            }
        }

        public string ExecuteGet(string key)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand("RetrieveCacheItem", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));

                var result = comm.ExecuteScalar();
                if (result == DBNull.Value) return null;
                return (string) result;
            }
        }
    }

    public class SqlCacheClient2
    {
        public readonly string ConnectionStringName;
        private readonly string connectionString;
        private readonly SHA256Managed hasher;

        public SqlCacheClient2(string connectionStringName)
        {
            this.ConnectionStringName = connectionStringName;
            connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString;
            hasher = new SHA256Managed();
        }

        private Guid GetUidKey(string key)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            var hash = hasher.ComputeHash(bytes);
            var guid = new Guid(hash.Take(16).ToArray());
            return guid;
        }

        public void Store(string key, string value)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand("SaveCacheItem2", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));
                comm.Parameters.AddWithValue("body", value);
                comm.Parameters.AddWithValue("expiration", DateTime.Now);
                comm.ExecuteNonQuery();
            }
        }

        public string ExecuteGet(string key)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand("RetrieveCacheItem2", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));

                var result = comm.ExecuteScalar();
                if (result == DBNull.Value) return null;
                return (string)result;
            }
        }
    }

}
