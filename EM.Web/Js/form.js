
Global.Form = {};
Global.Form.FileUploader = function () {
    var b_version = navigator.appVersion
    var version = b_version.split(";")
    var browser = navigator.appName;
    var trim_Version = version[1].replace(/[ ]/g, "");
    //批次把上传按钮的样式改掉，并且绑定事件
    $(".file").each(function () {
        //生成控件和样式
        //IE8前面不能套其他样式
        if (browser == "Microsoft Internet Explorer" && trim_Version == "MSIE8.0") {
            $(this).before('<input type="text"  style=" float: left;" readonly="readOnly" />')
            $(this).attr("style", "width: 70px; float: left;")
            //获取载入时默认的文件名并且显示到前面的输入框
            var displayValue = $(this).attr("displayValue");
            if (displayValue != undefined) {
                displayValues = displayValue.split("/");
                displayValue = displayValues[displayValues.length - 1];
                $(this).prev().val(displayValue);
            }
            $(this).live('change', function () {
                var ImgPath = this.value.split("/");
                var ImgName = ImgPath[ImgPath.length - 1];
                $(this).attr("displayValue", ImgName);
                $(this).prev().val(ImgName);
            });
        }
        else {
            var id = this.id + "FileCover";
            $(this.parentNode).append('<button type="button" class="fileCover btn btn-default btn-xs" id="' + id + '">本地上传</button>')
            $(this).appendTo($('#' + id));
            $('#' + id).before('<input type="text"  readonly="readOnly" />')
            //获取载入时默认的文件名并且显示到前面的输入框
            var displayValue = $(this).attr("displayValue");
            if (displayValue != undefined) {
                displayValues = displayValue.split("/");
                displayValue = displayValues[displayValues.length - 1];
                $(this.parentNode).prev().val(displayValue);
            }


            $(this).live('change', function () {
                var ImgPath = this.value.split("/");
                var ImgName = ImgPath[ImgPath.length - 1];
                $(this).attr("displayValue", ImgName);
                $(this.parentNode).prev().val(ImgName);
            });
        }

    })
}
Global.Form.DatePicker = function () {
    //时间插件目前用my97来做
    //$(".datepicker").focus(function () {
    //    WdatePicker({ el: this.id })
    //});
}
Global.Form.DeleteConfirm = function (url) {
    if (confirm("确认删除？"))
        location.href = url;
}
Global.Form.GetRandomCode = function () {
    var chars = ['_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];

    var n = 16;
    var res = "";
    res += chars[Math.ceil(Math.random() * (chars.length - 13)) + 12];
    for (var i = 0; i < n - 1 ; i++) {
        var id = Math.ceil(Math.random() * chars.length - 1);
        res += chars[id];
    }
    return res;
}
Global.Form.GetKeyWords = function (content) {
    //抓取关键词算法
    content.split("。")
}
Global.Form.AutoTextarea = function () {
    $("textarea").each(function () {
        var t = this.scrollHeight + "px";
        this.style.height = t;
        this.style.maxHeight = t;
        this.style.minHeight = t;
    })
}
Global.Form.AjaxSearchForm = function (FormJqOb, TargetJqOb,SubmitJqOb) {
    var alert = Global.Utils.ShowMessage
    SubmitJqOb.click(function () {
        $.ajax({
            type: "get",
            url: FormJqOb.attr("action"),
            data: FormJqOb.serialize(),
            success: function (a) {
                TargetJqOb.html(a)
            },
            error: function (a, b, c) {
                Global.Log(a);
                Global.Log(b);
                Global.Log(c);
                alert("服务器发生错误，请联系工作人员")
            },
            complete: function () {
            }
        })
    })
}
Global.Form.AjaxBodyForm = function (FormJqOb, Url,SuccessFunction) {
    var alert = Global.Utils.ShowMessage
    FormJqOb.submit(function () {
        $.ajax({
            type: "post",
            url: FormJqOb.attr("action"),
            data: FormJqOb.serialize(),
            success: function (a) {
                if(SuccessFunction!=undefined&&typeof(SuccessFunction)=="function")
                    SuccessFunction(a);
                if (a.code) {
                    alert("保存成功！");
                    if (Url != undefined)
                    {
                        if (Url != "")
                            location.href = Url;
                        else
                            location.href = location.href;

                    }
                }
                else
                    alert(a.message);
            },
            error: function (a, b, c) {
                Global.Log(a);
                Global.Log(b);
                Global.Log(c);
                alert("服务器发生错误，请联系工作人员")
            },
        })
        return false;
    })
}
Global.Form.NewIframe = function (name,id,url) {
    window.parent.Global.Iframe.OpenIframe(name, id, url, false)
}
Global.Form.FilterAction=function(actions)
{
    console.log(actions)
    var List = {};
    $("*[data-commond]").each(function () {
        List[$(this).attr("data-commond").toLowerCase()] = 0;;
    })
    for (var i in List) {
        if (actions[i] == undefined)
            $("*[data-commond="+i+"]").remove();
    }

}