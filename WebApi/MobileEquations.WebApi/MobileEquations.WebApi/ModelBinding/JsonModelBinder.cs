using System;
using System.Reflection;
using System.Threading.Tasks;
using DotNetCommon.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MobileEquations.Model;

namespace MobileEquations.WebApi.ModelBinding
{//https://stackoverflow.com/questions/41367602/upload-files-and-json-in-asp-net-core-web-api
    public class JsonModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            Type modelType = bindingContext.ModelType;
            object obj = modelType.CreateInstance();

            foreach (PropertyInfo prop in modelType.GetProperties())
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(prop.Name);
                if (valueProviderResult != ValueProviderResult.None)
                {
                    prop.SetValueWithTypeRespect(obj, valueProviderResult.FirstValue);
                }
            }

            bindingContext.Result = ModelBindingResult.Success(obj);
            return Task.CompletedTask;
        }
    }
}
