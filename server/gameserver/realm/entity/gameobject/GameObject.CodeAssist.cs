#region

using LoESoft.GameServer.realm.terrain;
using System;
using System.Xml.Linq;

#endregion

namespace LoESoft.GameServer.realm.entity
{
    partial class GameObject
    {
        public static bool GetStatic(XElement elem)
        {
            return elem.Element("Static") != null;
        }

        public static int? GetHP(XElement elem)
        {
            XElement n = elem.Element("MaxHitPoints");

            if (n != null)
                return Utils.FromString(n.Value);

            return null;
        }

        private static bool IsInteractive(ushort objType)
        {
            if (GameServer.Manager.GameData.ObjectDescs.TryGetValue(objType, out ObjectDesc desc))
            {
                if (desc.Class != null)
                    if (desc.Class == "Container"
                        || desc.Class.ContainsIgnoreCase("wall")
                        || desc.Class == "Merchant"
                        || desc.Class == "Portal")
                        return false;

                return !(desc.Static && !desc.Enemy && !desc.EnemyOccupySquare);
            }

            return false;
        }

        protected bool CheckHP()
        {
            try
            {
                if (Vulnerable && HP <= 0 && ObjectDesc != null && Owner != null)
                {
                    if (ObjectDesc.EnemyOccupySquare || ObjectDesc.OccupySquare)
                        Owner.Obstacles[(int)(X - 0.5), (int)(Y - 0.5)] = 0;

                    if (Owner.Map[(int)(X - 0.5), (int)(Y - 0.5)].ObjType == ObjectType)
                    {
                        WmapTile tile = Owner.Map[(int)(X - 0.5), (int)(Y - 0.5)].Clone();
                        tile.ObjType = 0;
                        Owner.Map[(int)(X - 0.5), (int)(Y - 0.5)] = tile;
                    }

                    Owner.LeaveWorld(this);

                    return false;
                }
            }
            catch (Exception) { }

            return true;
        }
    }
}