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
    alert(msg);
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
}
Global.UiBlockAll = function () {
    $(document).ajaxStart($.blockUI).ajaxStop($.unblockUI);
}

Global.Expand();
