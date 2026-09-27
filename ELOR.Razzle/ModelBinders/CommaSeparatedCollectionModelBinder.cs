using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Globalization;

namespace ELOR.Razzle.ModelBinders
{
    // <summary>
    /// Binds an array or generic collection parameter from a single comma-separated string,
    /// for example, "1,2,3" to an array of uint[] { 1, 2, 3 }
    /// </summary>
    public sealed class CommaSeparatedCollectionModelBinder(Type modelType, Type elementType) : IModelBinder
    {
        private static readonly char _separator = ',';

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var underlyingType = Nullable.GetUnderlyingType(elementType) ?? elementType;
            var isNullableElement = underlyingType != elementType;
            var converter = TypeDescriptor.GetConverter(underlyingType);

            var boxedItems = new List<object>();
            var hasError = false;

            foreach (var raw in valueProviderResult.Values)
            {
                if (string.IsNullOrEmpty(raw))
                    continue;

                foreach (var token in raw.Split(_separator, StringSplitOptions.None))
                {
                    var trimmed = token.Trim();

                    if (trimmed.Length == 0)
                    {
                        if (isNullableElement)
                        {
                            boxedItems.Add(null);
                            continue;
                        }

                        hasError = true;
                        bindingContext.ModelState.TryAddModelError(
                            modelName,
                            $"'{modelName}' contains an empty value; expected a comma-separated list of {underlyingType.Name}.");
                        continue;
                    }

                    try
                    {
                        boxedItems.Add(converter.ConvertFromString(null, CultureInfo.InvariantCulture, trimmed));
                    }
                    catch (Exception ex) when (ex is FormatException or NotSupportedException or ArgumentException or OverflowException)
                    {
                        hasError = true;
                        bindingContext.ModelState.TryAddModelError(
                            modelName,
                            $"The value '{trimmed}' in '{modelName}' is not a valid {underlyingType.Name}.");
                    }
                }
            }

            if (hasError)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var typedArray = Array.CreateInstance(elementType, boxedItems.Count);
            for (var i = 0; i < boxedItems.Count; i++)
                typedArray.SetValue(boxedItems[i], i);

            // T[] parameters get the array as-is; List<T>/IList<T>/ICollection<T>/IEnumerable<T>
            // parameters get a List<T> built from it (its constructor accepts any IEnumerable<T>,
            // and T[] implements that interface).
            object model = modelType.IsArray
                ? typedArray
                : Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), typedArray);

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Activates <see cref="CommaSeparatedCollectionModelBinder"/> for parameters/properties
    /// that are an array or a simple generic collection of "simple" element type (string, numeric, e.t.c)
    /// </summary>
    public sealed class CommaSeparatedCollectionModelBinderProvider : IModelBinderProvider
    {
        private static readonly Type[] SupportedCollectionGenericDefinitions =
        [
            typeof(List<>),
            typeof(IList<>),
            typeof(ICollection<>),
            typeof(IEnumerable<>),
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>),
        ];

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // Never touch [FromBody] models
            if (context.BindingInfo.BindingSource is { } source && source.CanAcceptDataFrom(BindingSource.Body))
                return null;

            var modelType = context.Metadata.ModelType;
            var elementType = GetElementType(modelType);

            if (elementType is null)
                return null;

            var underlyingType = Nullable.GetUnderlyingType(elementType) ?? elementType;

            // string converts "from itself" trivially; every other supported type needs a
            // TypeConverter capable of parsing a string (covers all BCL primitives + enums + Guid).
            if (underlyingType != typeof(string) &&
                !TypeDescriptor.GetConverter(underlyingType).CanConvertFrom(typeof(string)))
            {
                return null;
            }

            return new CommaSeparatedCollectionModelBinder(modelType, elementType);
        }

        private static Type GetElementType(Type modelType)
        {
            if (modelType.IsArray)
                return modelType.GetElementType();

            if (modelType.IsGenericType &&
                SupportedCollectionGenericDefinitions.Contains(modelType.GetGenericTypeDefinition()))
            {
                return modelType.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
