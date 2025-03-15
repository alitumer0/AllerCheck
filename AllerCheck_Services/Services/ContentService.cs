using AllerCheck_Core.Entities;
using AllerCheck_Data.Repositories.Interfaces;
using AllerCheck_Services.Services.Interfaces;

namespace AllerCheck_Services.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;

        public ContentService(IContentRepository contentRepository)
        {
            _contentRepository = contentRepository;
        }

        public void AddContent(Content content)
        {
            _contentRepository.Add(content);
            _contentRepository.SaveChanges();
        }

        public void DeleteContent(int id)
        {
            var content = _contentRepository.GetById(id);
            if (content != null)
            {
                _contentRepository.Delete(content);
                _contentRepository.SaveChanges();
            }
        }

        public List<Content> GetAllContents()
        {
            return _contentRepository.GetAll().ToList();
        }

        public Content GetContentById(int id)
        {
            return _contentRepository.GetById(id);
        }

        public void UpdateContent(Content content)
        {
            _contentRepository.Update(content);
            _contentRepository.SaveChanges();
        }
    }
} 