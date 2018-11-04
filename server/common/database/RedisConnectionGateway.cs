using BookSleeve;
using LoESoft.Core.config;
using System;
using System.Net.Sockets;

namespace LoESoft.Core.database
{
    public sealed class RedisConnectionGateway
    {
        private const string RedisConnectionFailed = "Redis connection failed.";
        private RedisConnection _connection;
        private static volatile RedisConnectionGateway _instance;

        private static readonly object SyncLock = new object();
        private static readonly object SyncConnectionLock = new object();

        public static RedisConnectionGateway Current
        {
            get
            {
                if (_instance == null)
                    lock (SyncLock)
                        if (_instance == null)
                            _instance = new RedisConnectionGateway();

                return _instance;
            }
        }

        private RedisConnectionGateway() => _connection = GetNewConnection();

        private static RedisConnection GetNewConnection()
            => new RedisConnection(host: Settings.REDIS_DATABASE.HOST, port: Settings.REDIS_DATABASE.PORT, ioTimeout: Settings.REDIS_DATABASE.IO_TIMEOUT,
                  password: Settings.REDIS_DATABASE.PASSWORD == "" ? null : Settings.REDIS_DATABASE.PASSWORD, maxUnsent: Settings.REDIS_DATABASE.MAX_UNSENT,
                  allowAdmin: Settings.REDIS_DATABASE.ALLOW_ADMIN, syncTimeout: Settings.REDIS_DATABASE.SYNC_TIMEOUT);

        public RedisConnection GetConnection()
        {
            lock (SyncConnectionLock)
            {
                if (_connection == null)
                    _connection = GetNewConnection();

                if (_connection.State == RedisConnectionBase.ConnectionState.Opening)
                    return _connection;

                if (_connection.State == RedisConnectionBase.ConnectionState.Closing || _connection.State == RedisConnectionBase.ConnectionState.Closed)
                {
                    try
                    { _connection = GetNewConnection(); }
                    catch (Exception ex) { throw new Exception(RedisConnectionFailed, ex); }
                }

#pragma warning disable CS0618 // O tipo ou membro é obsoleto
                if (_connection.State == RedisConnectionBase.ConnectionState.Shiny)
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
                {
                    try
                    { _connection.Wait(_connection.Open()); }
                    catch (SocketException ex) { throw new Exception(RedisConnectionFailed, ex); }
                }

                return _connection;
            }
        }
    }
}