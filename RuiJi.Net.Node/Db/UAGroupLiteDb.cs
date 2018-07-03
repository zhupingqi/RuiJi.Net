using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuiJi.Net.Node.Db
{
    public class UAGroupLiteDb
    {

        public static List<UAGroupModel> GetModels()
        {
            using (var db = new LiteDatabase(@"LiteDb/UAGroups.db"))
            {
                var col = db.GetCollection<UAGroupModel>("uAGroups");

                return col.FindAll().ToList();
            }
        }

        public static UAGroupModel Get(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAGroups.db"))
            {
                var col = db.GetCollection<UAGroupModel>("uAGroups");

                return col.FindOne(m => m.Id == id);
            }
        }

        public static int AddOrUpdate(UAGroupModel group)
        {
            var result = group.Id;
            using (var db = new LiteDatabase(@"LiteDb/UAGroups.db"))
            {
                var col = db.GetCollection<UAGroupModel>("uAGroups");
                
                if (group.Id == 0)
                {
                    group.Name = group.Name.Trim();

                    result = col.Insert(group).AsInt32;
                }
                else
                {
                    col.Update(group);
                }
            }
            return result;
        }

        public static bool Remove(int id)
        {
            using (var db = new LiteDatabase(@"LiteDb/UAGroups.db"))
            {
                var col = db.GetCollection<UAGroupModel>("uAGroups");

                col.Delete(id);
            }
            return true;
        }
    }
}
