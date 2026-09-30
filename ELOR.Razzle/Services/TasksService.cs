using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Mappings;
using ELOR.Razzle.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ELOR.Razzle.Services
{
    public class TasksService
    {
        private readonly UserSession _session;
        private readonly RazzleMapper _mapper;
        private readonly TagsService _tags;

        public TasksService(UserSession session, RazzleMapper mapper, TagsService tags)
        {
            _session = session;
            _mapper = mapper;
            _tags = tags;
        }

        public async Task<uint> CreateAsync(TaskCreateRequest request)
        {
            // Checking tags.
            // If request have tag ids, but no one are found in DB,
            // we throwing an exception

            List<Tag> attachedTags = null;
            if (request.TagIds.Count > 0)
            {
                var tagsResult = await _tags.GetInternalAsync(request.TagIds);
                if (tagsResult.count == 0) throw ServiceException.NoTagsFound();
                attachedTags = tagsResult.tags;
            }

            TaskEntity task = new TaskEntity
            {
                Name = request.Name,
                CreatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            await _session.DB.Tasks.AddAsync(task);

            if (attachedTags != null)
            {
                foreach (var tag in attachedTags)
                {
                    await _session.DB.TagTasks.AddAsync(new TagTask
                    {
                        Task = task,
                        TagId = tag.Id // because we use AsNoTracking in TagsService.GetInternalAsync
                    });
                }
            }

            await _session.DB.SaveChangesAsync();
            return task.Id;
        }

        public async Task<TaskEntity> GetInternalAsync(uint id)
        {
            return await _session.DB.Tasks.FindAsync(id);
        }

        public async Task<(int count, List<TaskEntity> tasks)> GetInternalAsync(List<uint> ids)
        {
            var query = _session.DB.Tasks.AsNoTracking();
            if (ids.Count > 0) query = query.Where(t => ids.Contains(t.Id));

            var count = await query.CountAsync();
            var items = await query.OrderBy(t => t.Id).ToListAsync();
            return (count, items);
        }
    }
}
