using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Mappings;
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

        public async Task<APIList<TagDTO>> GetAsync()
        {
            var query = _session.DB.Tags.AsNoTracking();

            var count = await query.CountAsync();
            var items = await query.OrderBy(t => t.Id).ToListAsync();
            return new APIList<TagDTO> { Count = count, Items = _mapper.ToDto(items) };
        }
    }
}
