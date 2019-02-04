#region

using LoESoft.GameServer.realm.terrain;
using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace LoESoft.GameServer.realm.mapsetpiece
{
    public abstract class MapSetPiece
    {
        public abstract int Size { get; }

        public abstract void RenderSetPiece(World world, IntPoint pos);

        public unsafe void LoadJson(void* world, string embeddedResource, IntPoint* pos, void* wmap)
        {
            if (embeddedResource == null)
                return;

            var resource = embeddedResource.Replace(".jm", "");
            var stream = typeof(RealmManager).Assembly.GetManifestResourceStream("LoESoft.GameServer.realm.mapsetpiece.maps." + resource + ".jm");

            if (stream == null)
                throw new ArgumentException("JSON map resource " + nameof(resource) + " not found!");

            FromWorldMap(new MemoryStream(Json2Wmap.Convert(GameServer.Manager.GameData, new StreamReader(stream).ReadToEnd())), world, pos, wmap);
        }

        private unsafe void FromWorldMap(Stream dat, void* world, IntPoint* pos, void* wmap)
        {
            var map = (GCHandle.FromIntPtr(new IntPtr(wmap)).Target as Wmap);
            map.Load(dat, 0);

            int w = map.Width, h = map.Height;

            pos->X = ((GCHandle.FromIntPtr(new IntPtr(world)).Target as World).Map.Width / 2) - (w / 2);
            pos->Y = ((GCHandle.FromIntPtr(new IntPtr(world)).Target as World).Map.Width / 2) - (w / 2);

            var ens = map.InstantiateEntities();

            foreach (var i in ens)
            {
                i.Move(i.X + pos->X, i.Y + pos->Y);
                (GCHandle.FromIntPtr(new IntPtr(world)).Target as World).EnterWorld(i);
            }
        }
    }
}