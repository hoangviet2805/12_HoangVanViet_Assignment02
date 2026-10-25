using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.DAOs
{
    public class TagDAO
    {
        private static TagDAO? _instance;
        private static readonly object _instanceLock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TagDAO();
                    }
                    return _instance;
                }
            }
        }

        public IEnumerable<Tag> GetTags()
        {
            using var context = new FUNewsManagementContext();
            return context.Tags.ToList();
        }

        public Tag? GetTagById(int id)
        {
            using var context = new FUNewsManagementContext();
            return context.Tags.FirstOrDefault(t => t.TagID == id);
        }

        public void AddTag(Tag tag)
        {
            using var context = new FUNewsManagementContext();
            context.Tags.Add(tag);
            context.SaveChanges();
        }

        public void UpdateTag(Tag tag)
        {
            using var context = new FUNewsManagementContext();
            context.Tags.Update(tag);
            context.SaveChanges();
        }

        public void DeleteTag(Tag tag)
        {
            using var context = new FUNewsManagementContext();
            context.Tags.Remove(tag);
            context.SaveChanges();
        }
    }
}
