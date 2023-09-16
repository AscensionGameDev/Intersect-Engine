using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Intersect.Server.Web.RestApi.Payloads;

public partial class ModelBinderProvider<TModel, TModelBinder> : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(TModel))
        {
            return new BinderTypeModelBinder(typeof(TModelBinder));
        }

        return null;
    }
}