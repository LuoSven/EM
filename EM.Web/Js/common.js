Global.Utils = {};
Global.Utils.CheckEmail = function CheckEmail(emailtoCheck) {
    var regExp = /^[^@^\s]+@[^\.@^\s]+(\.[^\.@^\s]+)+$/;
    if (emailtoCheck.match(regExp))
        return true;
    else
        return false;
}
Global.Utils.DoPointFix = function CheckEmail(emailtoCheck) {
    var tText = Value;
    if (tText != null && !isNaN(tText)) {
        var num = new Number(tText);
        tText = num.toFixed(pFixPoint); //  四舍五入        
        $('#' + pClientID).val(tText);
    }
    return tText;
}
Global.Utils.ClearAllCookie = function () {
    var keys = document.cookie.match(/[^ =;]+(?=\=)/g);
    if (keys) {
        for (var i = keys.length; i--;)
            document.cookie = keys[i] + '=0;expires=' + new Date(0).toUTCString() + "; path=/";
    }
}
Global.Utils.ShowMessage = function (msg) {
    var iconId=1;
    if (msg.indexOf("成功")==-1)
    {
        iconId = 5;
    }
    layer.msg(msg, {
        icon: iconId,
        time: 2000,
    });
}
Global.Utils.ConfirmMessage = function (msg) {
    var iconId = 1;
    if (msg.indexOf("成功") == -1) {
        iconId = 5;
    }
    layer.msg(msg, {
        icon: iconId,
        time: 2000,
    });
}
Global.Utils.RandomColor = function () {
    return '#' + ('00000' + (Math.random() * 0x1000000 << 0).toString(16)).slice(-6);
}
Global.Utils.ChangeColor = function (rgbColor, span, length, type ){
    var rgb = rgbColor.replace("#", "");
    var r = parseInt(rgb.substr(0, 2), 16);
    var g = parseInt(rgb.substr(2, 2), 16);
    var b = parseInt(rgb.substr(4, 2), 16);
    for (var j = 0; j < length; j++) {
        if (type == "change")//不同颜色渐变。总是中间值开始变，然后到下限或者上限后再到的值开始变
        {

        }
        var rs = r.toString(16).substr(0, 2);
        var gs = g.toString(16).substr(0, 2);
        var bs = b.toString(16).substr(0, 2);
        var colorTemp = "#" + rs + gs + bs;
        colorList.push(colorTemp);
    }
    if (type == "change")
    {
    }
    return '#' + ('00000' + (Math.random() * 0x1000000 << 0).toString(16)).slice(-6);
}
Global.Log = function (ob)
{
    try
    {
        console.log(ob)
    }
    catch(e)
    {

    }
}
Global.GetBrowser = function () {
    var Sys = {};
    var ua = navigator.userAgent.toLowerCase();
    var browserVer;
    (browserVer = ua.match(/msie ([\d.]+)/)) ? Sys.ie = formatVer(browserVer) :
        (browserVer = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = formatVer(browserVer) :
            (browserVer = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = formatVer(browserVer) :
                (browserVer = ua.match(/opera.([\d.]+)/)) ? Sys.opera = formatVer(browserVer) :
                    (browserVer = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = formatVer(browserVer) : 0;
    function formatVer(val) {
        return parseInt(val[1].split(".")[0])
    }
    return Sys;
}
//系统提示
Global.Utils.AlertMessage = function () {
    if (Global.Utils.AlertFlag)
        return;
    var messageFormat = '<div id="systemAlertMessage_{Id}" class="alertMessage p5 w300 pFx"><div class="alertMessage-title taC size14 wp100 fl fwB mb5"><span class="glyphicon glyphicon-info-sign pr5" aria-hidden="true"></span>消息<span class="glyphicon glyphicon-remove-circle fr size18 fL18 curp" aria-hidden="true"></span></div><div class="alertMessage-sender">{Sender}</div><div class="alertMessage-content size14">{Message}</div></div>';
    Global.Utils.AlertFlag=1,
    $.ajax({
        url: "/system/alertmessage?t=" + new Date().getTime(),
        success:function(a)
        {

            if (a.messages.length > 0)
            {
                for (var i = 0; i < a.messages.length; i++) {
                    var message = a.messages[i];
                    if (message.MessagType == 1) {
                        var html = messageFormat.format(message);
                        $("body").append(html);
                        var alertMessage = $("#systemAlertMessage_" + message.Id);
                        alertMessage.animate({ opacity: 1, bottom: "5px" },2000,"swing")
                        $(".glyphicon-remove-circle", alertMessage).click(function () {
                            $(this).parents(".alertMessage").animate({ opacity: 0, bottom: "300px" }, 1000, "swing", function () { $(".alertMessage:last").remove() })
                        })
                    }
                    else {
                        layer.confirm(message.Message, {
                            btn: ['确认'],
                            title: '系统通知'
                        }, function (a) {
                            layer.close(a);
                        });
                    }
                }
            }
            Global.Utils.AlertFlag = 0;
            
        }
    })
}
//原型拓展
Global.Expand = function () {
    //为array加上contains的方法
    Array.prototype.contains = function (item) {
        for (i = 0; i < this.length; i++) {
            if (this[i] == item) { return true; }
        }
        return false;
    };
    //用于查询对象数组中对象的某些值，同时支持对已查询属性进行重命名，若查询的属性不在改数组中，则该属性返回为undefined
    Array.prototype.select = function (args) {
        var newItems = [];
        if (typeof (args) === "object" && arguments.length === 1) {//传入查询的参数为对象时的处理方式
            for (var i = 0, imax = this.length; i < imax; i++) {
                var item = {};
                for (var key in args) {
                    if (args[key] !== undefined) {
                        item[key] = this[i][key] === undefined ? "undefined" : this[i][key];
                    }
                }
                newItems.push(item);
            }
        } else if (typeof (args) === "string" && arguments.length === 1) {//传入参数为字符串，且只有一个参数的处理方式
            for (var i = 0, imax = this.length; i < imax; i++) {
                var item = {};
                var keys = args.split(',');
                for (var k = 0, kmax = keys.length; k < kmax; k++) {
                    var iKey = keys[k].split("as");
                    if (iKey.length === 1) {
                        item[iKey[0].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                    } else {
                        item[iKey[1].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                    }
                }
                newItems.push(item);
            }
        } else {//传入的参数是多个字符串的处理方式
            for (var i = 0, imax = this.length; i < imax; i++) {
                var item = {};
                for (var j = 0, jmax = arguments.length; j < jmax; j++) {
                    if (arguments[j] !== undefined) {
                        var iKey = arguments[j].split("as");
                        if (iKey.length === 1) {
                            item[iKey[0].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                        } else {
                            item[iKey[1].trim()] = this[i][iKey[0].trim()] === undefined ? "undefined" : this[i][iKey[0].trim()];
                        }
                    }
                }
                newItems.push(item);
            }
        }
        returnnewItems;
    }
    Array.prototype.last = function () {
        return this[this.length - 1];
    }
    //
    Array.prototype.removeIndex = function (dx) {
        if (isNaN(dx) || dx > this.length) { return false; }
        for (var i = 0, n = 0; i < this.length; i++) {
            if (this[i] != this[dx]) {
                this[n++] = this[i]
            }
        }
        this.length -= 1
    }
    Array.prototype.remove = function (ob) {
        this.removeIndex(this.indexOf(ob));
        return this;
    }
    //根据jq对象返回对象的属性列表
    Array.prototype.attrs = function (attr) {
        var attrs = [];
        for (var i = 0; i < this.length; i++)
        {
            var jqobject = $(this[i]);
            var prop=jqobject.attr(attr);
            if(prop!=undefined)
            {
                attrs.push(prop)
            }

        }
        return attrs;
    }
    //为num加上变成百分比的方法
    Number.prototype.toPercent = function () {
        return (Math.round(this * 10000) / 100).toFixed(2) + '%';
    }
    //为string加上replaceAll的方法
    String.prototype.replaceAll = function (reallyDo, replaceWith, ignoreCase) {
        if (!RegExp.prototype.isPrototypeOf(reallyDo)) {
            return this.replace(new RegExp(reallyDo, (ignoreCase ? "gi" : "g")), replaceWith);
        } else {
            return this.replace(reallyDo, replaceWith);
        }
    }
    //为string加上format的方法
    String.prototype.format = function (args) {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length == 1 && typeof (args) == "object") {
                for (var key in args) {
                    if (args[key] != undefined) {
                        var reg = new RegExp("({" + key + "})", "g");
                        result = result.replace(reg, args[key]);
                    }
                }
            }
            else {
                for (var i = 0; i < arguments.length; i++) {
                    if (arguments[i] != undefined) {
                        //var reg = new RegExp("({[" + i + "]})", "g");//这个在索引大于9时会有问题，谢谢何以笙箫的指出
                        var reg = new RegExp("({)" + i + "(})", "g");
                        result = result.replace(reg, arguments[i]);
                    }
                }
            }
        }
        return result;
    }

    //原生方法增强
    String.prototype.Trim = function () {
        return this.replace(/(^\s*)|(\s*$)/g, "");
    }
    //判断是否匹配手机
    String.prototype.isMobile = function () {
        var val = this;
        if (!val.match(/^1[3|4|5|7|8][0-9]\d{4,8}$/) || val.length != 11 || val == "") {
            return false;
        } else {
            return true;
        }
    }
    //判断是否匹配邮箱
    String.prototype.isEmail = function () {
        var val = this;
        if (!val.match(/^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$/) || val == "") {
            return false;
        } else {
            return true;
        }
    }
    //判断是否匹配字符a-z0-9A-Z
    String.prototype.isWords = function () {
        var val = this;
        if (val.match(/[A-Za-z0-9]+$/) != val || val == "") {
            return false;
        } else {
            return true;
        }
    }
    //判断是否为纯数字
    String.prototype.isNumber = function () {
        var val = this;
        if (val.match(/\d+$/) != val || val == "") {
            return false;
        } else {
            return true;
        }
    }
    //判断数据类型
    String.prototype.getType = function () {
        var _t, o = this;
        return ((_t = typeof (o)) == "object" ? o == null && "null" || Object.prototype.toString.call(o).slice(8, -1) : _t).toLowerCase();
    };

    Date.prototype.format = function (fmt) { //author: meizz   
        var o = {
            "M+": this.getMonth() + 1,                 //月份   
            "d+": this.getDate(),                    //日   
            "h+": this.getHours(),                   //小时   
            "m+": this.getMinutes(),                 //分   
            "s+": this.getSeconds(),                 //秒   
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
            "S": this.getMilliseconds()             //毫秒   
        };
        if (/(y+)/.test(fmt))
            fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt))
                fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    };
    String.prototype.isFunction = function () {
      
        if (eval(this.valueOf()) != undefined && typeof (eval(this.valueOf())) == "function")
            return true;
        return false;
    };

    (function ($) {
        $.fn.serializeJson = function () {
            var serializeObj = {};
            var array = this.serializeArray();
            var str = this.serialize();
            $(array).each(function () {
                if (serializeObj[this.name]) {
                    if ($.isArray(serializeObj[this.name])) {
                        serializeObj[this.name].push(this.value);
                    } else {
                        serializeObj[this.name] = [serializeObj[this.name], this.value];
                    }
                } else {
                    serializeObj[this.name] = this.value;
                }
            });
            return serializeObj;
        };
        $.fn.attrs = function (attr) {
            var attrs = [];
            for (var i = 0; i < this.length; i++) {
                var jqobject = $(this[i]);
                var prop = jqobject.attr(attr);
                if (prop != undefined) {
                    attrs.push(prop)
                }

            }
            return attrs
        };
  
    })(jQuery);
}
Global.UiBlockAll = function () {
    $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);
}

Global.Expand();
