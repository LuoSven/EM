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
ToPublic.Init = function () {UploadEAFile

    $("#IsPublic").change(function () {
        ToPublic.GetPublicId($(this));
    })
}

var UploadEAFile = {};
UploadEAFile.Selector = "#FileList"
UploadEAFile.TrHtml = "<tr data-id=\"{Id}\"><td class=\"taL\">{FileName}</td><td class=\"taL\">{CreateDate}</td><td class=\"taL\">  <a href=\"javascript:;\" onclick=\"UploadEAFile.DeleteFile({Id})\">删除</a> &nbsp;<a href=\"../ExpenseAccount/ViewFile/{Id}\" target=\"_blank\">查看</a></td></tr>";
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
    $("#remark").val("")
    $("#file").val("")
    layer.open({
        type: 1,
        title: "上传附件",
        closeBtn: 1,
        area: '516px',
        shadeClose: false,
        content: $('#FileField')
    });
}
UploadEAFile.Init = function () {
    var oFrm = document.getElementById('if');
    oFrm.onload = oFrm.onreadystatechange = function () {
        if (this.readyState && this.readyState != 'complete') return;
        else {
            var json = eval("(" + oFrm.contentWindow.document.body.innerText + ")");
            if (json.code) {
                    alert("上传成功")
                    $("#NoRow").remove();
                    $("#AddRow").before(UploadEAFile.TrHtml.format(json.model))
            }
            else {
                alert(json.message)
            }
        }
    }
}

var EADetail = {};
EADetail.Selector = "#DetailList"
EADetail.Flag = false;
EADetail.TrHtml = '<tr data-id="{Id}">'+
'                        <td class="taR">{CompanyName}</td>' +
'                        <td class="taR">{OccurDateName}</td>' +
'                        <td class="taR">{CateName}</td>' +
'                        <td class="taR money">{Money}</td>' +
'                        <td class="taR">{Remark}</td>' +
'                        <td class="taR  ">'+
'                                <a href="javascript:;" onclick="EADetail.DeleteDetail({Id})">删除</a>'+
'                                <a href="javascript:;" onclick="EADetail.Show({Id})">编辑</a>'+
'                            <a href="/ExpenseAccount/BrowseDetail/{Id}" target="_blank">查看</a>'+
'                        </td>'+
'                    </tr>';

EADetail.DeleteDetail = function (id) {
    $.ajax({
        url: "/expenseaccount/DeleteDetail",
        data: { Id: id },
        success: function (a) {
            if (a.code) {
                $("tr[data-id=" + id + "] ", $(EADetail.Selector)).remove();
                EADetail.Sum();
            }
        }
    })
}
EADetail.Show = function (id) {
    if (!EADetail.Flag)
    {
        EADetail.Flag = true;
        $('#DetailField').remove();
        id = id == undefined ? 0 : id;
        var title = id == 0 ? "新增明细" : "编辑明细";
        var url = "/expenseaccount/GetDetail";
        $.ajax({
            title: title,
            url: url,
            data: { Id: id },
            success: function (a) {

                $("body").append(a);
                $("#EditDetail").validate({
                    submitHandler: function (form) {
                        EADetail.Save()
                    },
                });
                layer.open({
                    type: 1,
                    title: this.title,
                    closeBtn: 1,
                    area: '516px',
                    shadeClose: false,
                    content: $('#DetailField')
                });

                EADetail.Flag = false;
            }
        })
    }

  
}
EADetail.Save = function () {
    $.ajax({
        url: "/expenseaccount/SaveDetail",
        data: $("#EditDetail").serialize(),
        type:"post",
        success: function (a) {
            if (a.code)
            {
                alert("保存成功")
                $("#NoDetailRow").remove();
                $("#DetailList *[data-id=" + a.model.Id + "]").remove();
                $("#AddDetailRow").before(EADetail.TrHtml.format(a.model))
                EADetail.Sum();
                //计算金额
            }
            else
                alert(a.message)
        }

    })
   

}
EADetail.Sum = function () {
    var sum = 0;
    $("#DetailList .money").each(function () {
        var account = $(this).html();
        var money = parseFloat(account);
        sum += money;
    })
    $("#SumMoney").val(sum)
}

var BeforeSave = function ()
{
    if(($("#IsPublic").is(":checked")||$("#EANumber").html().indexOf("对公")>0)&&$("tr[data-id] ", $(UploadEAFile.Selector)).length==0)
    {
        alert("对公报销必须上传附件！")
        return false;
    }
    if ($("#DetailList *[data-id]").length == 0)
    {
        alert("请填写报销明细！")
        return false;
    }
    return true;
}
var SuccessFunction = function (ob) {
    Global.Form.NewIframe("报销单列表", "expenseaccount_index", "expenseaccount/index");
    if (location.href.indexOf("add") != -1) {
        if (confirm("是否继续新增报销单？"))
            location.href = location.href;
        else {
            Global.Form.CloseIframe("expenseaccount_add");
        }
    }
    else {
        Global.Form.CloseIframe("editexpenseaccount_" + ob.model.Id);
    }
    return false
}

$(function () {
    ToPublic.Init();
    UploadEAFile.Init();
})
