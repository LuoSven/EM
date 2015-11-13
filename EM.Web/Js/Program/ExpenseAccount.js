var ToPublic = {}
ToPublic.OldValue = "";
ToPublic.GetPublicId = function ($this) {
    var $EANumber = $("#EANumber")
    if ($this.is(":checked")) {
        ToPublic.OldValue = $EANumber.val();
        $EANumber.attr("readOnly", "readOnly")
        $.ajax({
            url: "/expenseaccount/GetNewPublicId",
            success: function (a) {
                if (a.code) {
                    $EANumber.val(a.EANumber);
                }
            }
        })
    }
    else {
        $EANumber.removeAttr("readOnly");
        $EANumber.val(ToPublic.OldValue);
    }
};
ToPublic.Init = function () {

    $("#IsPublic").change(function () {
        ToPublic.GetPublicId($(this));
    })
}

var UploadEAFile = {};
UploadEAFile.Selector = "#FileList"
UploadEAFile.TrHtml = "<tr data-id=\"{Id}\"><td class=\"taL\">{FileName}</td><td class=\"taL\">{CreateDate}</td><td class=\"taL\">  <a href=\"javascript:;\" onclick=\"UploadEAFile.DeleteFile({Id})\">删除</a> &nbsp;<a href=\"ExpenseAccount/ViewFile/{Id}\" target=\"_blank\">查看</a></td></tr>";
UploadEAFile.UploaderHtml = "<form target=\"if\" enctype=\"multipart/form-data\" id=\"AddFile\" name=\"AddFile\" method=\"post\" action=\"../../Upload/PostExpenseAccountFile/{Id}\"><input type=\"file\" name=\"file\" id=\"file\" class=\"wp100 hp100 pAbs \" style=\"top:0px;opacity:0\" /></form>";
UploadEAFile.UpdateFiles = function (ob) {
    var Ids = [];
    var Result = false;
    $("tr[data-id] ", $(UploadEAFile.Selector)).each(function () { Ids.push($(this).attr("data-id")) })
    if (Ids.length == 0)
        Result= true;
    $.ajax({
        url: "/expenseaccount/UpdateFiles",
        data: { Ids: Ids.join(","), ExpenseAccountId: ob.model.Id },
        type:"post",
        success: function (a) {
        }
    })
    if (location.href.indexOf("add") != -1)
    {
        if (confirm("是否继续新增报销单？"))
            location.href = location.href;
        else {
            Global.Form.NewIframe("报销单列表", "expenseaccount_index", "expenseaccount/index");
            Global.Form.CloseIframe("expenseaccount_add");
        }
    }
    else
    {
        Global.Form.NewIframe("报销单列表", "expenseaccount_index", "expenseaccount/index");
        Global.Form.CloseIframe("EditExpenseAccount_" + ob.model.Id);
    }
    return Result
};
UploadEAFile.DeleteFile = function (id) {
    $.ajax({
        url: "/expenseaccount/DeleteFile",
        data: { Id: id },
        success: function (a) {
            if(a.code)
            {
                $("tr[data-id=" + id + "] ", $(UploadEAFile.Selector)).remove();
            }
        }
    })
};
UploadEAFile.Show = function () {
    layer.open({
        type: 1,
        title: "上传附件",
        closeBtn: 1,
        area: '516px',
        shadeClose: true,
        content: $('#AddField')
    });
}
UploadEAFile.Init = function () {
    //$("#AddFileForm").append(UploadEAFile.UploaderHtml.format({ Id: $("#Id").val() }));
    var oFrm = document.getElementById('if');
    oFrm.onload = oFrm.onreadystatechange = function () {
        if (this.readyState && this.readyState != 'complete') return;
        else {
            var json = eval("(" + oFrm.contentWindow.document.body.innerText + ")");
            if (json.code) {
                alert("上传成功")
                {
                    $("#NoRow").remove();
                    $("#AddRow").before(UploadEAFile.TrHtml.format(json.model))
                }
            }
            else {
                alert(jsob.message)
            }
        }
    }
}

var BeforeSave = function ()
{
    if($("#IsPublic").is(":checked")&&$("tr[data-id] ", $(UploadEAFile.Selector)).length==0)
    {
        alert("对公报销必须上传附件！")
        return false;
    }
    return true;
}
$(function () {
    ToPublic.Init();
    UploadEAFile.Init();
})
