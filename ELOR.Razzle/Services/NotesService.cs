using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.DTO.Responses;
using ELOR.Razzle.Mappings;
using ELOR.Razzle.Services.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ELOR.Razzle.Services
{
    public sealed class NotesService
    {
        private readonly UserSession _session;
        private readonly RazzleMapper _mapper;
        private readonly TagsService _tags;
        private readonly TasksService _tasks;

        public NotesService(UserSession session, RazzleMapper mapper, TagsService tags, TasksService tasks)
        {
            _session = session;
            _mapper = mapper;
            _tags = tags;
            _tasks = tasks;
        }

        // TODO:
        // 1. возможность прописать настоящее время createdAt в случае,
        //    если девайс попытался создать заметку оффлайн.
        public async Task<uint> CreateAsync(NoteCreateRequest request)
        {
            CashFlow cash = null;

            if (request.CashFlowType.HasValue)
            {
                if (request.Amount == 0) throw ServiceException.InvalidParam("amount", "required if cashFlowType is defined");

                cash = new CashFlow
                {
                    FlowType = request.CashFlowType.Value,
                    Amount = request.Amount
                };
            }

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

            // Checking task
            uint? taskId = null;
            if (request.TaskId > 0)
            {
                var task = await _tasks.GetInternalAsync(request.TaskId);
                if (task == null) throw ServiceException.NoTaskFound();
                taskId = task.Id;
            }

            Note note = new Note
            {
                Text = request.Text,
                TaskId = taskId,
                CreatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            await _session.DB.Notes.AddAsync(note);
            
            if (cash != null)
            {
                cash.Note = note;
                await _session.DB.CashFlows.AddAsync(cash);
            }

            if (attachedTags != null)
            {
                foreach (var tag in attachedTags)
                {
                    await _session.DB.TagNotes.AddAsync(new TagNote { 
                        Note = note,
                        TagId = tag.Id // because we use AsNoTracking in TagsService.GetInternalAsync
                    });
                }
            }

            await _session.DB.SaveChangesAsync();
            return note.Id;
        }

        // TODO: make extension for paginated queries with "where" filters
        public async Task<NotesGetResponse> GetAsync(NotesGetRequest request)
        {
            var query = _session.DB.Notes.Include(n => n.TagNotes).AsNoTracking();

            if (request.TagIds?.Count > 0) 
                query = query.Where(n => n.TagNotes.Any(tn => request.TagIds.Contains(tn.TagId)));

            if (request.TaskId > 0) query = query.Where(n => n.TaskId == request.TaskId);
            int count = await query.CountAsync();

            if (request.Offset > 0) query = query.Skip(request.Offset);
            if (request.Count > 0) query = query.Take(request.Count);

            var result = await query.ToListAsync();

            // Tags

            List<uint> mentionedTagIds = new List<uint>();

            foreach (var note in result)
            {
                var tagIds = note.TagNotes.Select(n => n.TagId);
                mentionedTagIds.AddRange(tagIds);
            }
            mentionedTagIds = mentionedTagIds.Distinct().ToList();

            List<Tag> mentionedTags = null;
            if (mentionedTagIds.Count > 0)
            {
                var tagsResult = await _tags.GetInternalAsync(mentionedTagIds);
                mentionedTags = tagsResult.tags;
            }

            // Tasks

            var mentionedTaskIds = result
                .Where(n => n.TaskId.HasValue).Select(n => n.TaskId.Value)
                .Distinct().ToList();

            List<TaskEntity> mentionedTasks = null;
            if (mentionedTaskIds.Count > 0)
            {
                var tasksResult = await _tasks.GetInternalAsync(mentionedTaskIds);
                mentionedTasks = tasksResult.tasks;
            }

            return new NotesGetResponse
            {
                Count = count,
                Items = _mapper.ToDto(result),
                Tags = _mapper.ToDto(mentionedTags),
                Tasks = _mapper.ToDto(mentionedTasks)
            };
        }
    }
}
