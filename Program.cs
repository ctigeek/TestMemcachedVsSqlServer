using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private const string shortString = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean bibendum pellentesque elit eget ornare. Donec pellentesque dignissim dolor, quis cursus felis laoreet vitae. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras ut mi placerat, feugiat nibh sed, mollis lorem. In sit amet bibendum odio, eu ultrices enim. Aliquam nec nunc eros. Aenean nulla diam, cursus sit amet pharetra quis, tempor ac dui. Vivamus eget ante sed risus bibendum ullamcorper id at erat. Ut placerat rutrum quam, eu posuere lectus pulvinar quis. Fusce ut efficitur urna, id pellentesque nunc. Nulla dolor lorem, cursus hendrerit lectus at, tincidunt varius ante. Curabitur varius ultrices urna non pellentesque. Suspendisse nec lectus sed tellus semper sagittis. Phasellus ultricies nulla quis est gravida iaculis. Donec aliquam imperdiet rhoncus. Pellentesque bibendum sapien non arcu laoreet, eget porttitor velit posuere. Cras id consectetur nunc. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed ut arcu erat. Suspendisse leo arcu, interdum in elit sodales, hendrerit feugiat lectus. Donec ultrices lectus mi, nec malesuada nisl consectetur at. Quisque eget tellus purus. Vestibulum sagittis dapibus nulla, vitae hendrerit lorem gravida a. In ut mattis nunc. Etiam placerat velit tortor, molestie tincidunt.";

        private const string longString = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sit amet dolor non diam condimentum finibus ac quis neque. Ut sed hendrerit velit. Vivamus eget magna odio. Cras mattis cursus nisi ac viverra. Proin a leo sit amet nisi fermentum pharetra. Nunc lectus massa, eleifend ut diam in, hendrerit iaculis lacus. Quisque tempor libero magna, sed tempus libero elementum sed. Cras consequat tortor a ornare tristique. Donec condimentum nec ligula sit amet posuere. Integer vulputate massa nunc, et lacinia sapien molestie vel. Integer a lectus ac ligula faucibus eleifend fermentum a tortor. Nulla nec erat bibendum mauris tincidunt tincidunt id id mauris. Nunc sit amet rhoncus justo. Sed tempor nisl id gravida auctor. Phasellus euismod augue vitae orci vestibulum, ut dignissim tellus tempor. Ut nec nisi quis quam tristique rhoncus.
Aenean vestibulum porttitor bibendum. In finibus metus ut nibh iaculis tempus. Fusce vel blandit justo. Aliquam eu nunc ipsum. Quisque in risus sapien. Nam in nulla quis enim bibendum convallis vel eu mauris. Donec et sapien efficitur, pretium lectus at, interdum diam. Sed convallis odio eu elit venenatis tincidunt.
Donec placerat, risus id laoreet scelerisque, nisl ante luctus erat, vel pharetra turpis mauris eget tortor. Quisque eu lorem sit amet purus iaculis venenatis et eget felis. Vivamus auctor dolor ut venenatis luctus. Integer non orci laoreet, cursus lectus id, porttitor nisl. In libero odio, lacinia non dictum quis, vulputate ac dolor. Nulla sodales accumsan metus a tempor. Sed eu interdum dolor. Nulla velit nunc, mattis id orci vel, varius commodo lorem. Sed rhoncus dignissim magna, in aliquet sapien cursus eget. Phasellus maximus blandit condimentum. Duis blandit orci quis lorem viverra venenatis. Vestibulum gravida pretium feugiat. Donec nisi nisi, finibus eget tempor vel, convallis commodo risus.
In quis ipsum scelerisque, molestie purus in, pellentesque metus. Fusce vitae dui ut nunc ullamcorper dignissim eget eu elit. Vivamus rutrum mi turpis, quis laoreet dui vehicula tincidunt. Cras elementum lobortis lacinia. Phasellus odio augue, ullamcorper id erat eget, vestibulum tempor nibh. Curabitur finibus scelerisque urna vitae commodo. Morbi tristique sem et ligula pulvinar suscipit. Sed vel condimentum mi, id accumsan massa. Integer vestibulum neque hendrerit fringilla pellentesque. Pellentesque eleifend vestibulum erat, id tempor erat gravida in. Vestibulum suscipit ligula quis finibus molestie.
In hac habitasse platea dictumst. Pellentesque laoreet imperdiet lacus a molestie. Phasellus ante tellus, sagittis quis lobortis vitae, viverra eget libero. Sed pulvinar felis non mauris consectetur fringilla. Donec blandit vulputate nibh. Pellentesque faucibus orci vitae nunc euismod tempor. Mauris condimentum congue dolor, nec tincidunt nunc sodales quis. Mauris nunc odio, mattis sit amet tortor vel, suscipit rhoncus sem. Ut semper dui non diam pellentesque ultricies. Donec quis velit nibh. Curabitur ultricies sagittis lacus nec vulputate. Aenean mattis sem lectus, nec varius purus mollis vel. Aliquam commodo orci in nulla fermentum tristique. Nullam egestas congue arcu quis facilisis.
Suspendisse vel lectus luctus, rhoncus diam ac, pharetra lacus. Ut luctus malesuada erat sed scelerisque. Quisque nec ex pulvinar, eleifend urna vitae, consequat lectus. Praesent sollicitudin nunc massa, eget vestibulum lectus vulputate eget. Etiam non sapien vitae est sollicitudin accumsan. Donec condimentum dui non nibh mollis, eget elementum libero suscipit. Nulla tincidunt odio neque, at finibus sem dictum et. Fusce consectetur placerat odio ultricies scelerisque. Sed tempus lorem lorem, vitae rhoncus orci consectetur eu. Aenean blandit ultrices ligula, vehicula lacinia nunc vehicula convallis. Nullam a lorem dolor. In interdum vel purus sit amet eleifend. Nullam non mi ac purus bibendum sollicitudin vel at mauris. Vivamus ac tortor finibus, fringilla lectus vitae, euismod erat. Praesent mattis felis felis, ac congue ipsum malesuada quis. Etiam sed diam non arcu pulvinar placerat.
Cras eget neque et purus eleifend aliquam id quis arcu. Maecenas aliquam tempor accumsan. Quisque mollis ante tempus dignissim suscipit. Aliquam a blandit elit, sit amet elementum mauris. In risus urna, tincidunt a arcu a, sodales aliquam tellus. Quisque mi urna, ultrices id vulputate scelerisque, porttitor congue diam. Suspendisse sagittis nibh nulla, id tempus nisl imperdiet at. Ut non gravida ante, id maximus eros. Maecenas molestie eros non magna consectetur, vel auctor dolor convallis. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nunc ac diam nec mi venenatis tristique in vitae enim. Sed mollis neque commodo, varius quam sed, porta metus. Maecenas rhoncus orci iaculis tristique sodales. Aliquam erat volutpat. Morbi lacinia nibh ut arcu euismod, in commodo lectus venenatis.
Vivamus venenatis eleifend elit, maximus mollis diam vehicula vel. Quisque auctor, augue nec mattis congue, metus ex auctor nunc, non feugiat lacus neque sed erat. Phasellus euismod lacus accumsan libero posuere, suscipit mollis ante tristique. Vestibulum interdum eget nunc eget sodales. Donec nec urna a nisi vulputate sollicitudin. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce ut mollis massa. Curabitur ultricies convallis eros, nec fermentum turpis cursus lobortis. Donec in massa nunc. Maecenas ullamcorper ipsum purus, non bibendum metus consequat at. Nulla sagittis metus sit amet odio viverra vestibulum. Fusce molestie cursus risus id condimentum. Proin eu dolor in magna tempor tempus eget eget quam.
Etiam tincidunt ligula eu arcu tincidunt venenatis. Praesent ultrices turpis non felis ullamcorper efficitur. Etiam id diam velit. Mauris laoreet, tellus in viverra ultricies, eros odio maximus orci, non scelerisque mauris lectus ultrices sem. Nunc et arcu tincidunt, dapibus mi quis, mattis quam. Cras vel dictum dui. Vivamus et vehicula tortor. Morbi sollicitudin aliquam consequat. Vivamus lacus justo, maximus vitae nisi eget, euismod molestie est.
Sed quis vestibulum augue, vel suscipit quam. Fusce a urna dignissim, eleifend arcu vitae, gravida ante. Aenean sodales facilisis velit, vitae sodales eros ornare non. Duis finibus nisi in fringilla malesuada. Fusce felis enim, lacinia sit amet accumsan nec, semper ut enim. Duis faucibus orci arcu, id eleifend lectus gravida quis. Sed egestas odio eget ante dapibus condimentum. Aliquam suscipit, ligula ac posuere luctus, ante lacus hendrerit leo, non placerat neque metus bibendum nisl. Interdum et malesuada fames ac ante ipsum primis in faucibus. Maecenas justo mi, bibendum non lacus sit amet, volutpat iaculis risus. Curabitur elementum volutpat nulla, quis pretium tellus aliquam vel. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut sit amet venenatis augue, eget tincidunt ante. Pellentesque sit amet consectetur turpis. Aenean in pellentesque quam. Donec vulputate sed neque ut imperdiet.
Fusce sed ex mollis, auctor nunc sit amet, ullamcorper felis. Suspendisse potenti. Donec et ligula ac tellus vestibulum mattis. Morbi pellentesque eget nulla ac eleifend. Aliquam id elementum ligula. Duis dignissim lectus vitae nunc lobortis posuere. Cras gravida, turpis quis tempor ullamcorper, lorem ipsum finibus est, vel gravida diam mauris ac sapien. Curabitur lacinia ante luctus convallis luctus. Phasellus nec dapibus dui.
Vestibulum iaculis mauris sapien, quis pretium ante imperdiet sit amet. Nunc suscipit felis est, id tincidunt nunc mattis et. Maecenas in neque arcu. Sed dapibus risus sed massa condimentum, sed gravida sem faucibus. Nullam porta felis a hendrerit ultricies. Cras non imperdiet nisl, quis aliquam dolor. Praesent fermentum at tortor eget scelerisque. Nulla metus orci, lobortis eget molestie sit amet, eleifend quis nisi. Aenean sed consectetur risus, sit amet pellentesque odio. Integer nec risus eu metus eleifend feugiat.
Ut auctor, mi a lacinia euismod, odio libero tempor nunc, non semper ligula enim at metus. Duis leo turpis, consequat nec orci sit amet, porta facilisis tortor. Nulla ac justo nunc. Vivamus commodo ac tellus id sollicitudin. Nulla ut lacinia arcu. Sed ornare tortor sit amet blandit congue. Proin dictum id tortor ut cursus. Sed a convallis leo, rutrum maximus metus. Duis luctus ex lacus, quis gravida augue sodales aliquet.
Sed tincidunt rhoncus est, at posuere massa ornare id. Nam ullamcorper augue sed nulla tristique, ac faucibus mauris ultrices. Suspendisse ornare velit a arcu varius, eu hendrerit massa maximus. Vivamus nec est pulvinar, ultricies ipsum vitae, euismod lacus. Nullam sem libero, placerat quis aliquet vitae, scelerisque et libero. Curabitur ante tortor, consequat at turpis ut, sagittis dignissim lacus. Vivamus quis nisi maximus, pharetra elit eu, condimentum dui. Etiam at ullamcorper orci, ut porta lacus. Pellentesque efficitur turpis ex, et facilisis elit sollicitudin ac. Aenean a leo pellentesque, ultrices ipsum vitae, egestas mi. Aliquam eget dapibus enim. Donec molestie urna dolor, ut gravida lectus lobortis at.
Quisque sit amet placerat est. Nulla auctor, ipsum in congue ornare, massa arcu hendrerit risus, quis venenatis quam ante eget mi. Sed molestie lobortis urna, et tristique est accumsan eu. In semper elementum nunc sed porta. Praesent luctus elementum quam in aliquet. Donec bibendum velit ac sem iaculis congue. Nunc ante tortor, dictum non ex ac, hendrerit imperdiet leo. Sed pretium blandit metus ac varius. Sed enim arcu, ornare eget leo a, eleifend placerat tellus. Etiam vestibulum mi non cursus congue. Cras porttitor arcu diam, a vulputate mi venenatis et. Vestibulum vitae massa vitae velit gravida feugiat ut in sapien. Morbi odio quam, semper ut tincidunt quis, vestibulum sed tellus. Curabitur tempus in mi ut semper.
Aenean fermentum finibus imperdiet. Cras ut volutpat eros, at imperdiet odio. Quisque semper velit felis, sit amet pulvinar orci dapibus ac. Fusce condimentum nunc non tellus bibendum, non ullamcorper quam pharetra. Suspendisse potenti. Aliquam ut aliquet est. Phasellus mollis mauris at neque suscipit, in placerat tortor efficitur. Duis eleifend in felis eget rutrum. Integer venenatis at nibh sed ullamcorper. Mauris cursus non neque in suscipit.
Integer orci diam, tempus at vestibulum a, fermentum vel elit. Vivamus ut dignissim risus. In quis neque est. Aenean convallis commodo lacinia. Pellentesque sit amet mi et lectus laoreet iaculis. Praesent tempor congue sem et dictum. Curabitur ut metus id ante maximus euismod. Morbi vel sem tortor. Donec vitae malesuada est. Etiam id quam nec erat malesuada pellentesque semper sed lacus. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed nisl velit, sodales eu congue in, ullamcorper nec nulla.
Quisque et arcu ipsum. Integer aliquet erat eu nulla aliquet, sit amet mattis libero dapibus. Aenean scelerisque non lorem a consequat. Donec eget massa velit. Suspendisse eros nisl, sollicitudin posuere orci semper, mollis volutpat nisl. Vivamus pellentesque eu enim id lacinia. Aliquam sodales turpis erat, id egestas magna iaculis sit amet. In hac habitasse platea dictumst.
Vestibulum faucibus scelerisque libero ac dictum. Donec arcu enim, tincidunt et vestibulum sit amet, posuere et tortor. Duis venenatis lorem vel purus tempor, ut vestibulum lacus convallis. Curabitur at magna rhoncus, luctus velit quis, auctor neque. Pellentesque dignissim venenatis bibendum. Donec luctus iaculis ipsum non commodo. Sed vel pharetra diam. Curabitur eu tincidunt leo, porttitor vestibulum odio.
Nullam faucibus eget ipsum ut cursus. Mauris rutrum, ex vitae mollis eleifend, est ex volutpat velit, vitae mattis ipsum mi sit amet ligula. Nunc quis arcu felis. Nunc quam sapien, dapibus ac luctus et, bibendum quis arcu. Duis pretium purus nec neque sagittis, ut blandit mi bibendum. Vivamus dignissim tellus urna, id auctor arcu congue quis. Curabitur ac nisi sed nunc commodo malesuada ac ut sed.";

        static string stringToTest = longString;

        private static void Main(string[] args)
        {
            try
            {
                long result;
                int size = int.Parse(args[0]);
                result = TestMemcachedWrite(size);
                result = TestSqlCacheWrite(size);

                Thread.Sleep(50);
                result = TestMemcachedWrite(size);
                Debug.WriteLine("Memcached write: " + result.ToString());
                Console.WriteLine("Memcached write: " + result.ToString());

                result = TestSqlCacheWrite(size);
                Debug.WriteLine("Sql Memory Write: " + result.ToString());
                Console.WriteLine("Sql Memory Write: " + result.ToString());

                result = TestSqlDiskWrite(size);
                Debug.WriteLine("Sql Disk Write: " + result.ToString());
                Console.WriteLine("Sql Disk Write: " + result.ToString());

                result = TestSqlCacheWrite3(size);
                Debug.WriteLine("Sql Write3: " + result.ToString());
                Console.WriteLine("Sql Write3: " + result.ToString());

                result = TestMemcachedRead(size);
                Debug.WriteLine("Memcached read: " + result.ToString());
                Console.WriteLine("Memcached read: " + result.ToString());

                result = TestSqlCacheRead(size);
                Debug.WriteLine("Sql Read: " + result.ToString());
                Console.WriteLine("Sql Read: " + result.ToString());

                result = TestSqlDiskRead(size);
                Debug.WriteLine("Sql Disk Read: " + result.ToString());
                Console.WriteLine("Sql Disk Read: " + result.ToString());

                result = TestSqlBinaryCacheRead(size);
                Debug.WriteLine("Sql Binary Read: " + result.ToString());
                Console.WriteLine("Sql Binary Read: " + result.ToString());

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
                client.Store(StoreMode.Set, "asdfasdf" + i.ToString(), stringToTest);
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
                if (result != stringToTest) throw new Exception();
            }

            sw.Stop();
            client.Dispose();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheRead(int length)
        {
            var client = new SqlMemoryCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString());
                if (result != stringToTest) throw new Exception();
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlDiskRead(int length)
        {
            var client = new SqlDiskCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString());
                //if (result != stringToTest) throw new Exception();
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlBinaryCacheRead(int length)
        {
            var client = new SqlBinaryCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                var result = client.ExecuteGet("asdfasdf" + i.ToString());
                if (result != stringToTest) throw new Exception();
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheWrite(int length)
        {
            var client = new SqlMemoryCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store("asdfasdf" + i.ToString(), stringToTest);
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlDiskWrite(int length)
        {
            var client = new SqlDiskCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store("asdfasdf" + i.ToString(), stringToTest);
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private static long TestSqlCacheWrite3(int length)
        {
            var client = new SqlBinaryCacheClient("cache");

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < length; i++)
            {
                client.Store("asdfasdf" + i.ToString(), stringToTest);
            }

            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }

    public class SqlMemoryCacheClient
    {
        public readonly string ConnectionStringName;
        private readonly string connectionString;
        private readonly SHA256Managed hasher;

        public SqlMemoryCacheClient(string connectionStringName)
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
                return (string)result;
            }
        }
    }

    public class SqlDiskCacheClient
    {
        public readonly string ConnectionStringName;
        private readonly string connectionString;
        private readonly SHA256Managed hasher;

        public SqlDiskCacheClient(string connectionStringName)
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
                comm.Parameters.AddWithValue("body", value.Substring(0, 3950));
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

    public class SqlBinaryCacheClient
    {
        public readonly string ConnectionStringName;
        private readonly string connectionString;
        private readonly SHA256Managed hasher;

        public SqlBinaryCacheClient(string connectionStringName)
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
                var comm = new SqlCommand("SaveCacheItem3", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));
                var body = GetBytesFromString(value);
                comm.Parameters.AddWithValue("body", body);
                comm.Parameters.AddWithValue("expiration", DateTime.Now);
                comm.ExecuteNonQuery();
            }
        }

        public string ExecuteGet(string key)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var comm = new SqlCommand("RetrieveCacheItem3", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("uid", GetUidKey(key));

                var result = comm.ExecuteScalar();
                if (result == DBNull.Value) return null;
                return GetStringFromBytes((byte[])result);
            }
        }

        private byte[] GetBytesFromString(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var outputMemStream = new MemoryStream();
            if (bytes.Length > 7950)
            {
                using (var memStream = new MemoryStream(bytes, false))
                {
                    using (outputMemStream)
                    {
                        memStream.Seek(0, SeekOrigin.Begin);
                        using (var zip = new GZipStream(outputMemStream, CompressionLevel.Fastest))
                        {
                            memStream.CopyTo(zip);

                        }
                    }
                }
                return outputMemStream.ToArray();
            }
            else
            {
                return bytes;
            }
        }

        private string GetStringFromBytes(byte[] bytes)
        {
            if (bytes[0] == 0x1f && bytes[1] == 0x8b && bytes[2] == 8 && bytes[8] == 4 && (bytes[3] & bytes[4] & bytes[5] & bytes[6] & bytes[7]) == 0)
            {
                var outputStream = new MemoryStream();
                using (var memStream = new MemoryStream(bytes, false))
                {
                    using (outputStream)
                    {
                        using (var zip = new GZipStream(memStream, CompressionMode.Decompress))
                        {
                            zip.CopyTo(outputStream);

                        }
                    }
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
            else
            {
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
