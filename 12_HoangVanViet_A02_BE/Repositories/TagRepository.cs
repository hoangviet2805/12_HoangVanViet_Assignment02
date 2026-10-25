using System.Collections.Generic;
using _12_HoangVanViet_A02_BE.DAOs;
using _12_HoangVanViet_A02_BE.Models;

namespace _12_HoangVanViet_A02_BE.Repositories
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<Tag> GetTags() => TagDAO.Instance.GetTags();
        public Tag? GetTagById(int id) => TagDAO.Instance.GetTagById(id);
        public void AddTag(Tag tag) => TagDAO.Instance.AddTag(tag);
        public void UpdateTag(Tag tag) => TagDAO.Instance.UpdateTag(tag);
        public void DeleteTag(Tag tag) => TagDAO.Instance.DeleteTag(tag);
    }
}
