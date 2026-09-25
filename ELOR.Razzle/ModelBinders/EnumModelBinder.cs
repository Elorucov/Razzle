using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace ELOR.Razzle.ModelBinders
{
    // Binds enum request parameters from the string value declared via [JsonStringEnumMemberName]
    // on each enum member.
    public sealed class EnumModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Metadata.UnderlyingOrModelType.IsEnum ? new EnumModelBinder() : null;
        }
    }

    public sealed class EnumModelBinder : IModelBinder
    {
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, object>> _mapCache = new();

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(modelName);

            if (value == ValueProviderResult.None || string.IsNullOrWhiteSpace(value.FirstValue))
            {
                // Absent: leave at default; presence is enforced by validators where required.
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, value);

            var raw = value.FirstValue.Trim();
            var enumType = bindingContext.ModelMetadata.UnderlyingOrModelType;
            var map = GetMemberMap(enumType);

            object parsed = map.TryGetValue(raw, out var found) ? found : null;

            if (parsed is null)
            {
                bindingContext.ModelState.TryAddModelError(modelName, $"Must be {string.Join(", ", map.Keys)}");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(parsed);
            return Task.CompletedTask;
        }

        private static IReadOnlyDictionary<string, object> GetMemberMap(Type enumType)
        {
            return _mapCache.GetOrAdd(enumType, static t =>
            {
                var map = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var name in Enum.GetNames(t))
                {
                    var field = t.GetField(name)!;
                    var attr = (JsonStringEnumMemberNameAttribute)Attribute.GetCustomAttribute(field, typeof(JsonStringEnumMemberNameAttribute));

                    // fallback to the member's own name when no attribute is present.
                    var key = attr?.Name ?? name;
                    map[key] = Enum.Parse(t, name);
                }

                return map;
            });
        }
    }
}
