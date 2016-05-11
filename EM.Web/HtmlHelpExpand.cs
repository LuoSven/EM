using EM.Model.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace EM.Web
{
    public static class HtmlHelpExpand
    {
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, KeyValueGroupVM selectGroupList, object htmlAttributes)
        {
            var builder = new TagBuilder("select");
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            string optionFormat = "<option value=\"{0}\">{1}</option>";
            string optionSelectedFormat = "<option selected=\"selected\" value=\"{0}\">{1}</option>";
            string optionGroupFormat = "<optgroup label=\"{0}\">{1}</optgroup>";
            var selectFormat = "<select name=\"{0}\" >{1}</select>";
            ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            StringBuilder sb = new StringBuilder();
            var groupSb = new StringBuilder();
            foreach (var selectList in selectGroupList.List)
            {
                var optionSb = new StringBuilder();
                foreach (var option in selectList.Value)
                {
                    if (option.Selected)
                    {

                        optionSb.AppendFormat(optionSelectedFormat, option.Value, option.Text);
                    }
                    else
                    {

                        optionSb.AppendFormat(optionFormat, option.Value, option.Text);
                    }
                }
                groupSb.AppendFormat(optionGroupFormat, selectList.Key, optionSb.ToString());
            }
            //sb.AppendFormat(selectFormat, modelMetadata.PropertyName, groupSb.ToString());
            builder.MergeAttribute("name", modelMetadata.PropertyName);
            builder.InnerHtml = groupSb.ToString();
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));

        }
    }
}
