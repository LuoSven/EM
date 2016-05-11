(function ($) {
    $.fn.moveStopEvent = function (callback) {
        return this.each(function () {
            var x = 0,
             y = 0,
             x1 = 0,
             y1 = 0,
             isRun = false,
             si,
             self = this;
            var sif = function () {
                si = setInterval(function () {
                    if (x == x1 && y == y1) {
                        clearInterval(si);
                        isRun = false;
                        callback && callback.call(self);
                    }
                    x = x1;
                    y = y1;
                }, 30);
            }
            $(this).mousemove(function (e) {
                x1 = e.pageX;
                y1 = e.pageY;
                !isRun && sif(), isRun = true;
            }).mouseout(function () {
                clearInterval(si);
                isRun = false;
            });
        });
    }
})(jQuery);
Global.feedback = {};
Global.feedback.isOnMoving = 0;
Global.feedback.justMove = 0;
Global.feedback.innerPosition = {};
Global.feedback.ob = $(".feedback-content")
Global.feedback.Init = function (ob) {
    $("span", ob).click(function () {
        console.log("1")
        if (!Global.feedback.isOnMoving && !Global.feedback.justMove)
            Global.feedback.show(Global.feedback.ob);
    })
    Global.feedback.getCahcePos(ob)
    Global.feedback.confirmMoving(ob);
    Global.feedback.moving(ob);

}
Global.feedback.updateCahcePos = function (pos) {
    if (window.localStorage)
        window.localStorage["feedbackPos"] = JSON.stringify(pos)
}
Global.feedback.getCahcePos = function (ob) {
    if (window.localStorage) {
        if (window.localStorage["feedbackPos"]) {
            var pos = eval('(' + window.localStorage["feedbackPos"] + ')')
            $(ob).css("left", pos.x + "px")
            $(ob).css("top", pos.y + "px")
        }
    }
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

Global.feedback.confirmMoving = function (ob) {
    $(ob).mousedown(function () {
        Global.feedback.isOnMoving = 1;
        Global.feedback.innerPosition.x = $(ob).position().left
        Global.feedback.innerPosition.y = $(ob).position().top
        $(ob).css("cursor", "move")
    }).mouseup(function (e) {
        Global.feedback.updateCahcePos({ x: parseInt($(ob).css("left")), y: parseInt($(ob).css("top")) })
        Global.feedback.isOnMoving = 0

        console.log("2")
        $(ob).css("cursor", "pointer")
        e.stopPropagation()

    })
}
Global.feedback.moving = function (ob) {
    $(document).mousemove(function (e) {

        if (Global.feedback.isOnMoving) {
            Global.feedback.justMove = 1;
            var xx = e.pageX || e.originalEvent.x || e.originalEvent.layerX || 0;
            var yy = e.pageY || e.originalEvent.y || e.originalEvent.layerY || 0;
            $(ob).css("left", (xx - 80) + "px")
            $(ob).css("top", (yy - 20) + "px")
        }
    }).moveStopEvent(function () {
        console.log("3")
        setTimeout(function () {
            Global.feedback.justMove = 0;
        }, 1000)
    });
}
Global.feedback.submit = function (ob) {
    $.ajax({
        url: "/system/addFeedBack",
        type: "post",
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