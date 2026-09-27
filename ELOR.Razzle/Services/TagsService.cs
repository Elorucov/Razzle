using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Mappings;
using ELOR.Razzle.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ELOR.Razzle.Services
{
    public class TagsService
    {
        private readonly UserSession _session;
        private readonly RazzleMapper _mapper;

        public TagsService(UserSession session, RazzleMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public async Task<uint> AddAsync(TagAddRequest request)
        {
            if (await _session.DB.Tags.AnyAsync(t => t.Name == request.Name && t.Type == request.Type))
                throw ServiceException.AlreadyExists();

            Tag tag = new Tag
            {
                Name = request.Name,
                Type = request.Type.Value
            };
            await _session.DB.Tags.AddAsync(tag);
            await _session.DB.SaveChangesAsync();
            return tag.Id;
        }

        // TODO: filter by type
        public async Task<APIList<TagDTO>> GetAsync(TagsGetRequest request)
        {
            var result = await GetInternalAsync(request.Ids);
            return new APIList<TagDTO> { Count = result.count, Items = _mapper.ToDto(result.tags) };
        }

        public async Task<(int count, List<Tag> tags)> GetInternalAsync(List<uint> ids)
        {
            var query = _session.DB.Tags.AsNoTracking();
            if (ids.Count > 0) query = query.Where(t => ids.Contains(t.Id));

            var count = await query.CountAsync();
            var items = await query.OrderBy(t => t.Id).ToListAsync();
            return (count, items);
        }
    }
}
