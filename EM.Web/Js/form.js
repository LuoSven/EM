
Global.Form = {};
Global.Form.Init = function () {
    Global.Form.DatePicker();
    Global.Form.DateSelecter();
    Global.Form.FileUploader();
    Global.Form.AjaxSearchForm($("#SearchForm"), $("#List"))
    $('form[data-role=form]').each(function(){
        var  $form=$(this);
        var url = $form.attr("data-url");
        Global.Form.Valid($form, url)
    })
    $('#loading').fadeOut();
    $('*[data-role=returnBtn]').click(function () {
        var name = $(this).attr("data-name");
        var url = $(this).attr("data-url");
        Global.Form.NewIframe(name,url.replace("/","_"),url)
    })
    Global.Form.HighChartStyle();
}
Global.Form.FileUploader = function () {
    var b_version = navigator.appVersion
    var version = b_version.split(";")
    var browser = navigator.appName;
    var trim_Version =version.length>1? version[1].replace(/[ ]/g, ""):'';
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
Global.Form.DateSelecter = function ()
{
    var dateTypes = ["全部","近七天", "近一月", "近三月", "近半年"];
    var html = "";
    for (var i = 0; i < dateTypes.length; i++) {
        html += "<option value={value} >{name}</option>".format({ value: i, name: dateTypes[i] });
    }
    $(".dateselecter").each(function () {
        var _this = $(this);
        var dataValue = _this.attr("data-value")
        _this.html(html).change(function () {
            var value = $(this).find("option:selected").val()-0;
            var SDate = new Date(), EDate = new Date();
            switch (value) {
                case 0:
                    SDate = new Date(1999, 12, 11);
                case 1:
                    SDate = new Date(EDate.getFullYear(), EDate.getMonth(), EDate.getDate() - 7)
                    break;
                case 2:
                    SDate = new Date(EDate.getFullYear(), EDate.getMonth() - 1, EDate.getDate())
                    break;
                case 3:
                    SDate = new Date(EDate.getFullYear(), EDate.getMonth() - 3, EDate.getDate())
                    break;
                case 4:
                    SDate = new Date(EDate.getFullYear(), EDate.getMonth() - 6, EDate.getDate())
                    break;
            };
            $(this).parent().find("#SDate").val(SDate.format("yyyy/MM/dd"))
            $(this).parent().find("#EDate").val(EDate.format("yyyy/MM/dd"))
        });

        if (dataValue != "" && !isNaN(dataValue)) {
            $("option[value=" + dataValue + "]", _this).attr("selected", "selected");
        }
         
    })
   


   
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
Global.Form.Auto3
Textarea = function () {
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
                Global.Form.FilterAction(action);
                //要重新绑定全选事件
                Global.Table.Init();
            },
            error: function (a, b, c) { 
                Global.Log(a.responseText);
                Global.Log(b);
                Global.Log(c);
                alert("服务器发生错误，请联系工作人员")
            },
            complete: function () {
            }
        })
        return false;
    });
    $("select", FormJqOb).each(function () {
        $(this).change(function () {
            FormJqOb.submit();
        })
    });
}
Global.Form.AjaxBodyForm = function (FormJqOb, Url,sf) {
    var alert = Global.Utils.ShowMessage
    var modelField = FormJqOb.attr("data-field");
    modelField = modelField == undefined || modelField == "" ? "model" : "modelName";
    var Data = {};
    $("*[data-role=body]").each(function () {
        var bodyField = $(this).attr("data-field");
        var dataList = [];
        $("*[data-id]", this).each(function () {
            dataList.push($(this).attr("data-id"));
        })
        Data[bodyField] = dataList.join(',');
    })
    var urlSearch = "?";
    for (var i in Data) {
        urlSearch += i + "=" + Data[i] + "&";
    }
    var url = FormJqOb.attr("action");
    url = url === "" || url == undefined ? "" : url;
   var layerIndex= layer.open({ type: 3, icon: -1, content: '<a style="font-size: 16px;font-weight: 700;color: #FFF;position: absolute;width: 100px;padding-top: 20px;">数据保存中</a>' })
   $.ajax({
           layerIndex:layerIndex,
            type: "post",
            url: url + urlSearch,
            data: FormJqOb.serialize(),
            success: function (a) {
                layer.close(this.layerIndex);
                var FunctionResult = true;
                if (a.code) {
                    if (sf != undefined && sf.isFunction())
                    {

                        FunctionResult = eval(sf)(a);
                        if (FunctionResult) {
                            alert("保存成功！");
                            if (Url != undefined) {
                                if (Url != "")

                                    Global.Form.CloseIframe();
                                else
                                    location.href = location.href;

                            }
                        }
                    }
                    else
                    {
                        alert("保存成功！");
                        if (Url != undefined) {
                            if (Url != "")
                                Global.Form.CloseIframe();
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
    window.parent.Global.Iframe.AutoIframeHeight();
}
Global.Form.CloseIframe = function (id) {
    if (id == undefined){
        window.parent.Global.Iframe.CloseIframeNow();
    }
    else {

        window.parent.Global.Iframe.CloseIframe(id)
    }
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
            if (Global.Form.DeleteFlag == 0)
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
                $("form[id=SearchForm]").submit();
            }

        })
    }
}
Global.Form.ConfirmFlag = 0;
Global.Form.Confirm = function (url, ob, isComfirm,message,process) {
    var alert = Global.Utils.ShowMessage
    if (!isComfirm) {
        $(ob).html("确认？")//  $(ob).html("确认" + message + "？")
        $(ob).attr("onclick", 'Global.Form.Confirm("' + url + '",this, true,"' + message + '","' + process + '")')
        setTimeout(function () {
            if (Global.Form.ConfirmFlag == 0) {
                $(ob).html(message)
                if(process != undefined)
                {
                    $(ob).attr("onclick", 'Global.Form.Confirm("' + url + '",this, false,"' + message + '","' + process + '")')
                } else {
                    $(ob).attr("onclick", 'Global.Form.Confirm("' + url + '",this, false,"' + message + '")');
                }
            }
        }, 1500)
    }

    if (isComfirm) {
        if (Global.Form.ConfirmFlag)
            $(ob).focus();
        Global.Form.ConfirmFlag = 1;


        if (process != undefined && process.isFunction())
        {
            $(ob).html(message)
            $(ob).attr("onclick", 'Global.Form.Confirm("' + url + '",this, false,"' + message + '","' + process + '")')
            BeforeResult = eval(process)(ob);
        }else
        {

            $(ob).html("正在" + message + "中")
            $.ajax({
                url: url,
                success: function (a) {
                    Global.Form.ConfirmFlag = 0
                    if (typeof a == "string")
                        a = JSON.parse(a)
                    if (!a.code)
                        alert(a.message)
                    $("form[id=SearchForm]").submit();
                }

            })
        }

    }
}
Global.Form.Confirms = function (url, tableJqOb, message,beforeCheck) {
    var alert = Global.Utils.ShowMessage;
    var Message = "当前选中了{length}项，确认" + message+"?";
    var Ids = tableJqOb.selectedVals();
    if (Ids.length == 0)
    {
        alert("未选中任何项！");
        return
    }
    if (beforeCheck != undefined)
    {
        var messageCheck = beforeCheck.call(tableJqOb,url)
        if (messageCheck != "")
        {
            alert(messageCheck);
            return;
        }
        
    } else
    {
        alert("请先完善数据验证！");
        return
    }
   
    Message = Message.format({ length: Ids.length});
    if (confirm(Message))
    {
        var ajaxConfig = {
            url: url,
            data: { Ids: Ids.join(",") },
            success: function (a) {
                if (typeof a == "string")
                    a = JSON.parse(a)
                if (!a.code)
                    alert(a.message)
                $("form[id=SearchForm]").submit();
            }
        }
        Global.Log(ajaxConfig);
        
        $.ajax(ajaxConfig)
    }
  

}
Global.Form.ToExcel=function(FormJqOb,url)
{
    location.href = url+"?" + FormJqOb.serialize();
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
Global.Form.HighChartStyle = function () {
    if (!typeof Highcharts == undefined )
    {
        Highcharts.createElement('link', {
            href: 'http://fonts.googleapis.com/css?family=Dosis:400,600',
            rel: 'stylesheet',
            type: 'text/css'
        }, null, document.getElementsByTagName('head')[0]);

        Highcharts.theme = {
            colors: ["#7cb5ec", "#f7a35c", "#90ee7e", "#7798BF", "#aaeeee", "#ff0066", "#eeaaee",
                "#55BF3B", "#DF5353", "#7798BF", "#aaeeee"],
            chart: {
                backgroundColor: null,
                style: {
                    fontFamily: "sans-serif"
                }
            },
            title: {
                style: {
                    fontSize: '16px',
                    fontWeight: 'bold',
                    textTransform: 'uppercase'
                }
            },
            tooltip: {
                borderWidth: 0,
                backgroundColor: 'rgba(219,219,216,0.8)',
                shadow: false
            },
            legend: {
                itemStyle: {
                    fontWeight: 'bold',
                    fontSize: '13px'
                }
            },
            xAxis: {
                gridLineWidth: 1,
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            yAxis: {
                minorTickInterval: 'auto',
                title: {
                    style: {
                        textTransform: 'uppercase'
                    }
                },
                labels: {
                    style: {
                        fontSize: '12px'
                    }
                }
            },
            plotOptions: {
                candlestick: {
                    lineColor: '#404048'
                }
            },


            // General
            background2: '#F0F0EA'

        };

        // Apply the theme
        Highcharts.setOptions(Highcharts.theme);
    }
   
}