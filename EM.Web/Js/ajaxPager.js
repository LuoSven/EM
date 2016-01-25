var AjaxPager = {};
AjaxPager.Bind = function () {
    var $AjaxPager = $("div[data-ajaxpager=true]");
    if ($AjaxPager[0] != undefined) {
        var pager = AjaxPager.GetPager($AjaxPager);
        $(" a", $AjaxPager).each(function () {
            $(this).click(function () {
                var $this = $(this)
                var Url = $this.attr("href")
                if (Url != "" && Url != undefined) {
                    $("#" + pager.loadingid).css("display", "block")
                    $.ajax({
                        url: Url,
                        type: pager.type,
                        data: $("#" + pager.formId).serialize(),
                        success: function (a) {
                             pager = AjaxPager.GetPager($("div[data-ajaxpager=true]"));
                            $("#" + pager.loadingid).css("display", "none")
                            $("#" + pager.target).html(a)
                            AjaxPager.Bind();
                            window.scrollTo(0, 0);
                            //绑定报表全选事件
                            Global.Table.Init();
                        }
                    })
                    return false;
                }
               
            })
        })
    }
}
AjaxPager.GetPager = function ($AjaxPager) {
    var pager = {};
    pager.type = $AjaxPager.attr("data-ajax-method");
    pager.loadingid = $AjaxPager.attr("data-ajax-loadingid");
    pager.formId = $AjaxPager.attr("data-ajax-form-id");
    pager.target = $AjaxPager.attr("data-ajax-targetid");
    pager.type = pager.type == "" ? "get" : pager.type;
    return pager
}
    

