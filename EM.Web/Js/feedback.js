Global.feedback = {};
Global.feedback.ob = $(".feedback-content")
Global.feedback.Init = function (ob) {
    $(ob).click(function () {
        Global.feedback.show(Global.feedback.ob);
    })
}
Global.feedback.show = function (ob) {
    $("#feedbackMessage", ob).val("")
     
    $.ajax({
        url: "/system/getfeedback",
        success: function (a) {
            layer.open({
                title: ['建议与反馈', 'font-size: 16px;color: #0058E3;'],
                type: 1,
                area: ['600px', '400px'], //宽高
                content: a
            });

            $('button', Global.feedback.ob).click(function () {
                Global.feedback.submit(Global.feedback.ob)
            })
        }
    })
    
}
Global.feedback.submit = function (ob) {
    $.ajax({
        url: "/system/addFeedBack",
        type:"post",
        data: {
            message: $("#feedbackMessage", ob).val(),
            url: Global.Iframe.GetNowIframe()[0].src,
        },
        success: function (a) {
            Global.Utils.ShowMessage('提交成功！感谢您的反馈！');
            $(".feedback-history").html(a);
        }
    })
    
}