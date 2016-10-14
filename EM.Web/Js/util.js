var basePath;
String.prototype.startWith = function (c, b) {
    if (typeof c == "undefined" || this.length < c.length) {
        return false
    }
    var a = this.substr(0, c.length);
    if (b) {
        a = a.toLowerCase();
        c = c.toLowerCase()
    }
    if (a == c) {
        return true
    }
    return false
};
String.prototype.endWith = function (c, b) {
    if (typeof c == "undefined" || this.length < c.length) {
        return false
    }
    var a = this.substring(this.length - c.length);
    if (b) {
        a = a.toLowerCase();
        c = c.toLowerCase()
    }
    if (a == c) {
        return true
    }
    return false
};
String.prototype.trim = function () {
    return $.trim(this)
};
String.prototype.isNumber = function () {
    var a = /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/;
    return a.test(this)
};
String.prototype.toDate = function () {
    var b;
    var c = this;
    if (!this.isEmpty()) {
        var d = new RegExp("^[1-2]\\d{3}-(0?[1-9]||1[0-2])-(0?[1-9]||[1-2][1-9]||3[0-1])$");
        if (d.test(this)) {
            var a = c.split("-");
            b = new Date(a[0], a[1], a[2], 0, 0, 0)
        }
    }
    return b
};
String.prototype.toDateTime = function () {
    var b;
    var c = this;
    if (!this.isEmpty()) {
        var d = new RegExp("^[1-2]\\d{3}-(0?[1-9]||1[0-2])-(0?[1-9]||[1-2][0-9]||3[0-1]) ((0)?[0-9]||1[0-9]||2[0-4]):((0)?[0-9]||[1-5][0-9]):((0)?[0-9]||[1-5][0-9])$");
        if (d.test(this)) {
            var a = c.replace(/[^\d]/g, "-").split("-");
            b = new Date(a[0], a[1], a[2], a[3], a[4], a[5])
        }
    }
    return b
};
String.prototype.isDigit = function () {
    return /^\d+$/.test(this)
};
String.prototype.isEmpty = function () {
    var a = $.trim(this);
    return a == ""
};
String.prototype.isEmail = function () {
    var a = $.trim(this);
    var b = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$/;
    return b.test(a)
};
String.prototype.isPostalcode = function () {
    var a = $.trim(this);
    var b = /^[1-9]\d{5}$/;
    return b.test(a)
};
String.prototype.isPhone = function () {
    var a = $.trim(this);
    var b = /^(\d{3}\-)?(0\d{2,3}-)?[1-9]\d{6,7}$/;
    return b.test(a)
};
String.prototype.isMobile = function () {
    var a = $.trim(this);
    var b = /^1[3,5,8]{1}[0-9]{1}[0-9]{8}$/;
    return b.test(a)
};
String.prototype.isLetter = function () {
    var a = $.trim(this);
    var b = /^[A-Za-z]+$/;
    return b.test(a)
};
String.prototype.isChinese = function () {
    var a = $.trim(this);
    var b = /^[\u4e00-\u9fa5]+$/;
    return b.test(a)
};
String.prototype.isEnglishChinese = function () {
    var a = $.trim(this);
    var b = /^[A-Za-z\u4e00-\u9fa5 ]+$/;
    return b.test(a)
};
String.prototype.isLetterOrNumber = function () {
    var a = $.trim(this);
    var b = /^[A-Za-z0-9]+$/;
    return b.test(a)
};
String.prototype.isLetterOrNumberOrUnderline = function () {
    var a = $.trim(this);
    var b = /^\w+$/;
    return b.test(a)
};
String.prototype.isCurrency = function () {
    var a = $.trim(this);
    var b = /^\d+(\.\d{1,2})?$/;
    return b.test(a)
};
String.prototype.containIllegalChar = function (b) {
    var d = "[`~!@#$^&*()=|{}':;',\\[\\].<>/?]";
    if (b) {
        d = b
    }
    var a = $.trim(this);
    var c = new RegExp(d);
    return c.test(a)
};
String.prototype.getActualLength = function () {
    var b;
    var a = 0;
    for (b = 0; b < this.trim().length; b++) {
        if (this.charCodeAt(b) > 255) {
            a += 2
        } else {
            a++
        }
    }
    return a
};
String.prototype.replaceAll = function (a, c, b) {
    if (!RegExp.prototype.isPrototypeOf(a)) {
        return this.replace(new RegExp(a, (b ? "gi" : "g")), c)
    } else {
        return this.replace(a, c)
    }
};

function onKeyUpDigits(b) {
    var a = $(b).val();
    var c = a.replace(/[^\d]/g, "");
    $(b).val(c)
}
function onKeyUpLetter(b) {
    var a = $(b).val();
    var c = a.replace(/[^a-zA-Z]*/g, "");
    $(b).val(c)
}
function limitLength(c, a) {
    var b = $(c).val().trim();
    if (b.length > a) {
        $(c).val(b.substring(0, a))
    }
}
function makeAlertAndFocus(b, a) {
    alert(a);
    $(b).focus();
    $(b).select()
}
function wrapperHtml(a) {
    if (!a) {
        return a
    }
    return a.replace(/\</g, "&lt;").replace(/\>/g, "&gt;").replace(/'/g, "&#39").replace(/"/g, "&#34")
}
function wrapperEscape(b) {
    if (!b) {
        return b
    }
    var c = document.createElement("div");
    var a = document.createTextNode(b);
    c.appendChild(a);
    return c.innerHTML
}
function wrapperUnescape(b) {
    if (!b) {
        return b
    }
    var a = document.createElement("div");
    a.innerHTML = b;
    return a.innerText || a.textContent
}
function ajaxSubmit(a) {
    if (!("async" in a)) {
        a.async = true
    }
    if (!(a.url)) {
        alert("您没有设置提交的地址！");
        return
    }
    var c = "1";
    if (a.updateId) {
        c = "2"
    }
    var d = a.url;
    var e;
    if (a.form) {
        e = $("#" + a.form).serialize()
    }
    if (a.params && !($.isEmptyObject(a.params))) {
        if (e && e.length) {
            e = e + "&" + $.param(a.params)
        } else {
            e = $.param(a.params)
        }
    }
    if (!(e)) {
        e = "rand=" + Math.random()
    } else {
        e = e + "&rand=" + Math.random()
    }
    if (a.onBefore) {
        a.onBefore()
    }
    if (a.maskId) {
        var b = "正在执行操作，请稍候.....!";
        if (a.maskLabel) {
            b = a.maskLabel
        }
        $("#" + a.maskId).mask(b)
    }
    $.ajax({
        type: "POST",
        url: d,
        data: e,
        async: a.async,
        beforeSend: function (f) {
            f.setRequestHeader("CeduAjax", c)
        },
        success: function (f, g) {
            if (a.maskId) {
                $("#" + a.maskId).unmask()
            }
            if (a.updateId) {
                $("#" + a.updateId).html(f)
            }
            if (a.onSuccess) {
                a.onSuccess(f, g)
            }
        },
        error: function (f, h, g) {
            if (a.maskId) {
                $("#" + a.maskId).unmask()
            }
            if (a.onError) {
                a.onError(f, h, g)
            } else {
                if (f.status == 462) {
                    if (a.updateId) {
                        $("#" + a.updateId).html(f.responseText)
                    } else {
                        alert("系统处理出现错误，请联系系统管理员！")
                    }
                }
            }
        }
    });
    if (a.onAfter) {
        a.onAfter()
    }
}
function showValInfo(a) {
    if (a && !($.isEmptyObject(a))) {
        $.each(a, function (b, c) {
            $("#" + b + "_info").hide();
            $("#" + b + "_error").val(c).show()
        })
    }
}
function dialSuccess(a, b) {
    a("#qryFlg").val("1");
    if (b) {
        a.dialog.tips(b, 1, "succ.png")
    } else {
        a.dialog.tips("保存成功！", 1, "succ.png")
    }
}
function dialError(pJq, request, textStatus, errorThrown, failInfo) {
    if (request.status == 460 && request.responseText) {
        pJq.dialog.tips("业务处理不成功，请注意提示信息！", 1, "fail.png");
        var infos = eval("(" + request.responseText + ")");
        $.each(infos, function (key, value) {
            var key2 = key.replace(/\./g, "\\.").replace(/\[/g, "\\[").replace(/\]/g, "\\]");
            $("#" + key2 + "_info").hide();
            if (!$("#" + key2 + "_error").length) {
                createError(key)
            }
            $("#" + key2 + "_error").html(value).show()
        })
    } else {
        if (request.status == 461) {
            if (request.responseText == "-1") {
                pJq.dialog.tips("您没有修改任何数据！", 2, "fail.png")
            } else {
                if (request.responseText == "0") {
                    pJq.dialog.tips("保存失败！", 2, "fail.png")
                } else {
                    pJq.dialog.tips(request.responseText, 2, "fail.png")
                }
            }
        }
    }
}
function createError(b) {
    var a = $("<div/>").attr({
        id: b + "_error",
        generated: true
    }).addClass("formError");
    a.insertAfter($(':input[name="' + b + '"]'))
}
function ajaxPaging(d, f, b, a) {
    var e = {
        rand: Math.random(),
        pageNo: a
    };
    if (f) {
        var c = "#" + f;
        e = $(c).serialize();
        if (e) {
            e = e + "&pageNo=" + a
        } else {
            e = "pageNo=" + a
        }
    }
    $.ajax({
        type: "POST",
        url: d,
        data: e,
        success: function (g, h) {
            if (b) {
                $("#" + b).html(g)
            }
        }
    })
}
function postPersonRecord(b) {
    if (personRecord && personRecord.url) {
        var a = b.indexOf(".do");
        var c = b.substring(0, a);
        alert(c.replace("/", "_"));
        var d = {
            user: personRecord.userId,
            action: c.replace("/", "_")
        };
        $.ajax({
            type: "POST",
            url: personRecord.url,
            data: d
        })
    }
}
function submitTo(a) {
    document.forms[0].action = a;
    document.forms[0].submit()
}
function goTo(a) {
    window.location.href = a
}
function batchDelete(a, c) {
    var b = getValues(a);
    if (b == "") {
        alert("请选择要删除的数据项!");
        return false
    }
    if (confirm("您确定要删除选取的项吗?")) {
        c(b);
        return true
    }
    return false
}
function getValues(b) {
    var c = "";
    var a = 0;
    $("input[name='" + b + "']:checked").each(function () {
        if (a > 0) {
            c = c + ","
        }
        a++;
        c = c + $(this).val()
    });
    return c
}
function checkParent(f, c) {
    var b = "input[type='checkbox'][name='" + c + "']";
    var e = $(b);
    var d = document.getElementById(f);
    var a = true;
    if (d.type == "checkbox") {
        for (i = 0; i < e.length; i++) {
            if (!e[i].checked && !e[i].disabled) {
                a = false;
                break
            }
        }
        d.checked = a
    }
}
function checkChildren(c, b) {
    if (c.type == "checkbox") {
        var f = c.checked;
        var a = "input[type='checkbox'][name='" + b + "']";
        var e = $(a);
        var d = 0;
        for (d = 0; d < e.length; d++) {
            if (!e[d].disabled) {
                e[d].checked = f
            }
        }
    }
}
function renderSelect(d) {
    var f = {
        code: "id",
        label: "name",
        empty: [false, "", "请选择"],
        append: true,
        select: null,
        list: [],
        value: null
    };
    var b = $.extend(true, {}, f, d);
    if (typeof b.empty === "boolean") {
        b.empty = [b.empty, "", "请选择"]
    }
    var e = b.list;
    var a = typeof (b.select) == "string" ? $("#" + b.select) : b.select;
    if (!b.append) {
        a.empty()
    }
    if (b.empty[0]) {
        _selectAddOption(a[0], b.empty[1], b.empty[2])
    }
    if (e && e.length) {
        for (var c = 0; c < e.length; c++) {
            _selectAddOption(a[0], e[c][b.code], e[c][b.label])
        }
    }
    if (b.value != null) {
        a.val(b.value)
    }
    return a
}
function _selectAddOption(a, c, d) {
    var e = document.createElement("option");
    e.text = d;
    e.value = c;
    try {
        a.add(e, null)
    } catch (b) {
        a.add(e)
    }
};