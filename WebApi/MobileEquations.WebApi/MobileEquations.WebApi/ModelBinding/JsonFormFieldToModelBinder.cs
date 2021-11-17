using System;
using System.Text.Json;
using System.Threading.Tasks;
using DotNetCommon.Constants;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MobileEquations.WebApi.ModelBinding
{
    public class JsonFormFieldToModelBinder : IModelBinder
    {//https://stackoverflow.com/questions/41367602/upload-files-and-json-in-asp-net-core-web-api
        private const string _jsonFormFieldName = "jsonString";

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            Type modelType = bindingContext.ModelType;
            string jsonString = bindingContext.ValueProvider.GetValue(_jsonFormFieldName).FirstValue;
            object obj = JsonSerializer.Deserialize(jsonString, modelType, JsonSerializationOptions.CaseInsensitive);

            bindingContext.Result = ModelBindingResult.Success(obj);
            return Task.CompletedTask;
        }
    }
}
