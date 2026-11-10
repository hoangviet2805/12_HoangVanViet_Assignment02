using System.Collections.Generic;
using System.Linq;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly FUNewsManagementContext _context;

        public TagRepository(FUNewsManagementContext context)
        {
            _context = context;
        }

        public IEnumerable<Tag> GetTags() => _context.Tags.ToList();

        public Tag? GetTagById(int id) => _context.Tags.FirstOrDefault(t => t.TagID == id);

        public void AddTag(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();
        }

        public void UpdateTag(Tag tag)
        {
            _context.Tags.Update(tag);
            _context.SaveChanges();
        }

        public void DeleteTag(Tag tag)
        {
            _context.Tags.Remove(tag);
            _context.SaveChanges();
        }
    }
}
