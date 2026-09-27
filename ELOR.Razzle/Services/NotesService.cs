using ELOR.Razzle.Data.Entities;
using ELOR.Razzle.DTO.Requests;
using ELOR.Razzle.Mappings;
using ELOR.Razzle.Services.Infrastructure;
using System.Runtime.InteropServices;

namespace ELOR.Razzle.Services
{
    public sealed class NotesService
    {
        private readonly UserSession _session;
        private readonly RazzleMapper _mapper;
        private readonly TagsService _tags;

        public NotesService(UserSession session, RazzleMapper mapper, TagsService tags)
        {
            _session = session;
            _mapper = mapper;
            _tags = tags;
        }

        // TODO:
        // 1. check and write taskId after realizing TasksService
        // 2. возможность прописать настоящее время createdAt в случае,
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

            Note note = new Note
            {
                Text = request.Text,
                CreatedAt = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

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

            // TODO: check taskId

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
    }
}
