using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.DTO.Responses;
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

        // TODO: make extension for paginated queries with "where" filters
        public async Task<TasksGetResponse> GetAsync(TasksGetRequest request)
        {
            var query = _session.DB.Tasks
                .Include(t => t.TagTasks)
                .Include(t => t.CompletionNote).AsNoTracking();

            if (request.TagIds?.Count > 0)
                query = query.Where(n => n.TagTasks.Any(tn => request.TagIds.Contains(tn.TagId)));

            int count = await query.CountAsync();

            if (request.Offset > 0) query = query.Skip(request.Offset);
            if (request.Count > 0) query = query.Take(request.Count);

            var result = await query.ToListAsync();

            // Tags

            List<uint> mentionedTagIds = new List<uint>();

            foreach (var note in result)
            {
                var tagIds = note.TagTasks.Select(n => n.TagId);
                mentionedTagIds.AddRange(tagIds);
            }
            mentionedTagIds = mentionedTagIds.Distinct().ToList();

            List<Tag> mentionedTags = null;
            if (mentionedTagIds.Count > 0)
            {
                var tagsResult = await _tags.GetInternalAsync(mentionedTagIds);
                mentionedTags = tagsResult.tags;
            }

            // Notes that mentioned in completed notes

            List<Note> mentionedNotes = result.SelectMany(t => t.Notes ?? [])
                .DistinctBy(n => n.Id).ToList();

            return new TasksGetResponse
            {
                Count = count,
                Items = _mapper.ToDto(result),
                Tags = _mapper.ToDto(mentionedTags),
                Notes = _mapper.ToDto(mentionedNotes)
            };
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
