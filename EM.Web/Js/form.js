
Global.Form = {};
Global.Form.Init = function () {
    Global.Form.DatePicker();
    Global.Form.FileUploader();
    Global.Form.AjaxSearchForm($("#SearchForm"), $("#List"))
    $('form[data-role=form]').each(function(){
        var  $form=$(this);
        var url = $form.attr("data-url");
        Global.Form.Valid($form, url)
    })
}
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
    $(".datepicker").focus(function () {
        WdatePicker({ el: this.id })
    });
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
Global.Form.AjaxSearchForm = function (FormJqOb, TargetJqOb) {
    var alert = Global.Utils.ShowMessage
    FormJqOb.submit(function () {
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
        return false;
    })
}
Global.Form.AjaxBodyForm = function (FormJqOb, Url,sf) {
    var alert = Global.Utils.ShowMessage
    var modelField = FormJqOb.attr("data-field");
    modelField = modelField == undefined || modelField == "" ? "model" : "modelName";
    var Data = {};
    Data[modelField] = FormJqOb.serializeJson()
    $("*[data-role=body]").each(function () {
        var bodyField = $(this).attr("data-field");
        var dataList = [];
        $("*[data-id]", this).each(function () {
            dataList.push((this).attr("data-id"));
        })
        Data[bodyField] = dataList;
    })
  
        $.ajax({
            type: "post",
            url: FormJqOb.attr("action"),
            data: Data,
            success: function (a) {
                var FunctionResult = true;
                if (a.code) {
                    if (sf!=undefined&& sf.isFunction())
                        FunctionResult = eval(sf)(a);
                    if (FunctionResult)
                    {
                        alert("保存成功！");
                        if (Url != undefined) {
                            if (Url != "")
                                location.href = Url;
                            else
                                location.href = location.href;

                        }
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
}
Global.Form.NewIframe = function (name,id,url) {
    window.parent.Global.Iframe.OpenIframe(name, id, url, false, true)
}
Global.Form.CloseIframe = function (id) {
    window.parent.Global.Iframe.CloseIframe(id)
}
Global.Form.DeleteFlag = 0;
Global.Form.Delete=function(url,ob,isComfirm)
{
    var alert = Global.Utils.ShowMessage
    if (!isComfirm)
    {
        $(ob).html("确认删除？")
        $(ob).attr("onclick",'Global.Form.Delete("'+url+'",this, true)')
        setTimeout(function () {
            if (Global.Form.DeleteFla = 0)
            {
                $(ob).html("删除")
                $(ob).attr("onclick", 'Global.Form.Delete("' + url + '",this, false)')
            }
        },1500)
    }

    if (isComfirm) {
        if (Global.Form.DeleteFlag)
            $(ob).focus();
        Global.Form.DeleteFlag = 1;
        $(ob).html("正在删除中")
        $.ajax({
            url: url,
            success: function (a) {
                Global.Form.DeleteFlag = 0
                if(typeof a =="string")
                    a=JSON.parse(a)
                if (!a.code) 
                  alert(a.message)
                $("form").submit();
            }

        })
    }
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
Global.Form.Valid = function (ob, url) {
    $(ob).validate({
        submitHandler:function(form)
        {
            var BeforeResult = true;
            var $form = $(ob);
            var SuccessFunction = $(ob).attr("data-success");
            var BeforeSaveFunction = $(ob).attr("data-beforesave");
            if (BeforeSaveFunction != undefined && BeforeSaveFunction.isFunction())
                BeforeResult = eval(BeforeSaveFunction)();
            if (BeforeResult)
                Global.Form.AjaxBodyForm($form, url, SuccessFunction);
        },
        unhighlight: function (element, errorClass) {
            var $errorMessage = $(".errorMessage", $(element).parent())
            $errorMessage.remove();
            $(element).removeClass("error")
        },
        errorPlacement: function (error, element) {
            var errorHtml = error.html();
            var $errorMessage=$(".errorMessage", $(element).parent())
            if($errorMessage.length>0)
            {
                $errorMessage.html(errorHtml)
            }
            else
            {
                $(element).parent().append("<a class=\"errorMessage\">" + errorHtml + "</a>")
            }
      
    } 
    })
}