
(function ($) {
    $.fn.extend({
        multiSelect: function () {
            Global.Table.MultiSelect(this);
        },
        selectedVals:function(){
            return Global.Table.GetValues(this)
           
        },
        selectedRows: function () {
            return Global.Table.GetRows(this)
        }
    })
})(jQuery);

Global.Table = {};
Global.Table.Data = {};
Global.Table.Init = function () {
    $("table").each(function () {
        $(this).multiSelect();
    })
}
//获取表所对应的对象
Global.Table.GetData = function (tableOb) {
    var TableId = tableOb.attr("id") == undefined ? "table" : tableOb.attr("id");
    if (Global.Table.Data[TableId] == undefined)
    {
        Global.Table.Data[TableId] = {};
    }
    return  Global.Table.Data[TableId];
};
Global.Table.MultiSelect = function (tableOb) {
    var TableId = tableOb.attr("id") == undefined ? "table" : tableOb.attr("id");

    Global.Table.Data[TableId] = {};
    //if (Global.Table.Data[TableId] == undefined)
    //{
    //    Global.Table.Data[TableId] = {};
    //}
    Global.Log(Global.Table.Data[TableId]);
    Global.Table.BindData(tableOb, Global.Table.Data[TableId]);
    var $SelectAll = $("input[type=checkbox]:eq(0)", tableOb);
    var $SelectList = $("input[type=checkbox]:gt(0)", tableOb);
    $SelectAll.click(function () {
        var Checkboxs = $("input[type=checkbox]", tableOb);
        var Trs = $("tbody:eq(0)>tr", tableOb);
        var Ids = Trs.attrs("data-id");
        //全选按钮选中状态
        var IsAllSelected = $(this).is(":checked")
        //全选，全不选
        Checkboxs.prop("checked", IsAllSelected);
        $(Ids).each(function () {
            Global.Table.Data[TableId][this] = IsAllSelected;
        })
    })
    $SelectList.each(function () {
        //绑定点击事件
        $(this).click(function () {
            var $CheckBox = $(this);
            var IsSelected = $CheckBox.is(":checked");
            //父级tr
            var $ParentRow = $CheckBox.parents("tr");
            //id名称
            var DateId = $ParentRow.attr("data-id");
            Global.Table.Data[TableId][DateId] = IsSelected;

            //子集取消选中的时候要把全选的那个也取消
            if (!IsSelected)
            {
                //移除全选的选中状态
                $SelectAll.prop("checked", false);
            }
            
            Global.Log(Global.Table.Data[TableId]);
        })
    })
}
//翻页后重新绑定数据
Global.Table.BindData = function (tableOb,data) {
    
    $("tbody:eq(0)", tableOb)
    for (key in data)
    {
        if(data[key])
        {
            $("*[data-id=" + key + "] input[type=checkbox]", tableOb).prop("checked", true);
        }
    }
}
//获取选中的值
Global.Table.GetValues = function (tableOb) {
    var  data=Global.Table.GetData(tableOb);
    var Ids=[];
    for (key in data)
    {
        if(data[key])
        {
            Ids.push(key);
        }
    }
    return  Ids
}
//获取选中的行
Global.Table.GetRows = function (tableOb) {
    var Ids = Global.Table.GetValues(tableOb);
    //空对象，仅仅为了push
    var Rows = $("#2313123213");
    $(Ids).each(function () {
        Rows.push($("*[data-id=" + this+"]", tableOb));
    })
    return Rows;
};