// <auto-generated />
#pragma warning disable CS0105
using MasterMemory.Validation;
using MasterMemory;
using MessagePack;
using System.Collections.Generic;
using System.Net;
using System;
using VIRCE_server.DataBase;
using VIRCE_server.MasterMemoryDataBase.Tables;

namespace VIRCE_server.MasterMemoryDataBase
{
   public sealed class DatabaseBuilder : DatabaseBuilderBase
   {
        public DatabaseBuilder() : this(null) { }
        public DatabaseBuilder(MessagePack.IFormatterResolver resolver) : base(resolver) { }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<RoomServerInfo> dataSource)
        {
            AppendCore(dataSource, x => x.RoomId, System.Collections.Generic.Comparer<byte>.Default);
            return this;
        }

        public DatabaseBuilder Append(System.Collections.Generic.IEnumerable<UserData> dataSource)
        {
            AppendCore(dataSource, x => x.GlobalUserId, System.Collections.Generic.Comparer<ushort>.Default);
            return this;
        }

    }
}