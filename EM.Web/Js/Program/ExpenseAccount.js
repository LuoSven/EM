
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
UploadEAFile.Browse = function (url) {
    layer.open({
        type: 2,
        title: '浏览附件',
        shadeClose: false,
        shade: false,
        maxmin: true,
        area: ['50%', '100%'],
        content: url //iframe的url
    });
}
UploadEAFile.Init = function () {
    var oFrm = document.getElementById('if');
    if (oFrm != null)
    {

        oFrm.onload = oFrm.onreadystatechange = function () {
            if (this.readyState && this.readyState != 'complete') return;
            else {
                var json = eval("(" + oFrm.contentWindow.document.body.innerText + ")");
                if (json.code) {
                    if ($(UploadEAFile.Selector + " *[data-id=" + json.model.Id + "]").length == 0) {

                        alert("上传成功")
                        $("#NoRow").remove();

                        $("#AddRow").before(UploadEAFile.TrHtml.format(json.model))
                    }
                }
                else {
                    alert(json.message)
                }
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

EADetail.Browse = function (id) {
    if (!EADetail.Flag) {
        EADetail.Flag = true;
        $('#DetailField').remove();
        var url = "/expenseaccount/BrowseDetail";
        $.ajax({
            url: url,
            data: { Id: id },
            success: function (a) {
                $("body").append(a);
                layer.open({
                    type: 1,
                    title: "查看报销明细",
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

var FailApprove = {};
FailApprove.Show=function (ob) {
    var html='<div class="messageContent w200  p5 pAbs " ><form><table class="wp100"><tbody><tr><td class="w40 taC">原因</td><td><textarea name="message" class="wp100"></textarea></td></tr><tr><td class=" taC">备注</td><td><textarea name="Note" class="wp100"></textarea></td></tr><tr><td class=" taC"></td><td><button type="button" class="btn btn-primary btn-xs">确认</button><button type="button" class="btn  btn-xs">取消</button></td></tr></tbody></table></form></div>'
    var offset = $(ob).offset()
    var id = $(ob).parents("tr").attr("data-id")
    $(".messageContent").css("top", offset.top + "px")
    $(".messageContent").css("left", offset.left-250 + "px")
    $(".messageContent").removeClass("hidden", 200)
    FailApprove.Init($(".messageContent"), id);
}
FailApprove.Init = function ($messageContent,id)
{
    $("form", $messageContent)[0].reset();
    $("#Id", $messageContent).val(id);
    $("#ApproveStatus", $messageContent).val(3);
    $(".btn-primary",$messageContent).off("click").click(function () {
        $.ajax({
            url: "/ExpenseAccount/UpdataApproveStatus",
            data:$("form",$messageContent).serialize(),
            success: function (a) {
                $("form[id=SearchForm]").submit();
                $(".messageContent").addClass("hidden", 200)
            }
        })
    })
    $(".btn-cancel", $messageContent).off("click").click(function () {
        $(".messageContent").addClass("hidden",200)
    })

}

var UploadApproveStatus = function () {
   
    var data = {};
    data["Id"] = $("#Id").val();
    data["ApproveStatus"] = $("#ApproveStatus option:selected").val();
    data["Message"] = $("#ApproveStatus option:selected").html().split(":").last();
    $.ajax({
        url: "/expenseaccount/UpdataApproveStatus",
        data: data,
        success: function (a) {
            alert("保存成功！")
            location.href = location.href;
        }
    })

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
    if (location.href.indexOf("add") != -1) {
        if (confirm("是否继续新增报销单？"))
            location.href = location.href;
        else {
            Global.Form.NewIframe("查看报销单", "expenseaccount_Browse_" + ob.model.Id, "/expenseaccount/Browse/" + ob.model.Id);
            Global.Form.CloseIframe("expenseaccount_add");

        }
    }
    else {
        Global.Form.NewIframe("查看报销单", "expenseaccount_Browse_" + ob.model.Id, "/expenseaccount/Browse/" + ob.model.Id);
        Global.Form.CloseIframe("editexpenseaccount_" + ob.model.Id);
    }
    return false
}

var MultiCheck = function (url) {
    url = url.toLowerCase();
    var status = this.selectedRows().attrs("data-status");
    var Message = "";
    switch (url) {
        case "/expenseaccount/deletes":
            if (status.contains('2') || status.contains('4'))
                Message = "选中项包含待确认或已确认项，不能删除";
            break;
        case "/expenseaccount/UpdataApproveStatuss?ApproveStatus=4":
            if (status.contains('4'))
                Message = "选中项包含已确认项，不能重复确认";
            if (status.contains('1') || status.contains('4'))
                Message = "选中项包含草稿箱，不能进行确认";
            break;
        case "/expenseaccount/UpdataApproveStatuss?ApproveStatus=3":
            if (status.contains('3'))
                Message = "选中项包含已退回项，不能重复确认";
            if (status.contains('1') || status.contains('4'))
                Message = "选中项包含草稿箱，不能进行退回";
            break;
        case "/expenseaccount/sumbitexpenseaccounts":
            if (status.contains('2'))
                Message = "选中项包含待确认项，请勿重复提交";
            if (status.contains('4'))
                Message = "选中项包含已确认项，不能进行提交";
            break;
        case "/expenseaccount/cancelexpenseaccounts":
            if (status.contains('1'))
                Message = "选中项包含草稿箱，请不能进行撤回";
            if (status.contains('4'))
                Message = "选中项包含已确认项，不能撤回";
            break;
    }
    return Message
}
$(function () {
    //ToPublic.Init();
    UploadEAFile.Init();

    
})
