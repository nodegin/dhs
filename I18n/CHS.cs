using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHS.I18n
{
    class CHS
    {

        public static Dictionary<string, string> Msg = new Dictionary<string, string>()
        {
            // metro base
            {"ui.back", "返回"},
            {"ui.minimize", "隐藏"},
            {"ui.close", "关闭"},
            {"ui.logout", "注销"},
            {"ui.no-result", "没有匹配的结果"},
            {"ui.not-set", "(未设置)"},
            {"ui.info", "通知"},
            {"ui.warn", "警告"},
            {"ui.errr", "错误"},
            // login
            {"login.title", "登录"},
            {"login.staffid", "职员ID"},
            {"login.passwd", "密码"},
            // main
            {"main.title", "主页"},
            {"main.menu.store-furnitures", "店内家品"},
            {"main.category.management", "监控"},
            {"main.menu.delivery-report", "发送速运报告"},
            {"main.menu.reorder-report", "发送补货清单"},
            {"main.menu.sales-report", "生成销售报表"},
            {"main.menu.import-furnitures", "输入新家品"},
            // store furnitures
            {"stfu.action.reload", "刷新"},
            {"stfu.action.search", "搜索"},
            {"stfu.action.order-cart", "货品订单"},
            {"stfu.action.shopping-cart", "购物车"},
            {"stfu.qty", "数量"},
            {"stfu.shelf", "位置"},
            {"stfu.date", "日期"},
            {"stfu.continue-order", "店内已经没有这件家品的存货了\n是否要从货仓进行订购?"},
            {"stfu.out-of-stock", "店内及货仓都已经没有这件家品的存货了"},
            // cart
            {"cart.checkout", "结帐"},
            {"cart.order", "订购"},
            {"cart.action.clear-all", "清空所有"},
            {"cart.subtotal", "小计"},
            {"cart.discounted", "折扣"},
            {"cart.total", "合计"},
            {"cart.delivery", "需要运送"},
            {"cart.cust-name", "客户名称"},
            {"cart.cust-addr", "客户地址"},
            {"cart.cust-phone", "客户电话"},
            {"cart.cash", "现金"},
            {"cart.card", "信用卡"},
            {"cart.shopping-cart-empty", "购物车目前为空"},
            {"cart.order-cart-empty", "订购清单目前为空"},
            {"cart.cust-info-empty", "请输入客户的所有资料"},
            {"cart.make-payment", "支付"},
            {"cart.deposit", "订金"},
            {"cart.cash-in", "收取"},
            {"cart.change", "找赎"},
            {"cart.pay-by-card", "使用信用卡扣费"},
            {"cart.payment-ok", "支付成功"},
            {"cart.payment-card-phase-1", "请插入信用卡"},
            {"cart.payment-card-phase-2", "正在验证信用卡"},
            {"cart.payment-card-phase-3", "连接到支付网关"},
            {"cart.payment-card-phase-4", "授权中"},
            {"cart.mark-and-return", "标记为已支付并返回"},
            {"cart.print-invoice", "列印发票"},
            // furniture details + edit
            {"edit.title", "修改家品资料"},
            {"edit.id", "编号"},
            {"edit.name", "家品名"},
            {"edit.desc", "简介"},
            {"edit.dimen", "尺寸 (吋)"},
            {"edit.price", "价格"},
            {"edit.discount", "折扣率"},
            {"edit.quantity", "存货"},
            {"edit.rl", "补货于"},
            {"edit.sl", "位置"},
            {"edit.category", "类别"},
            {"edit.subcategory", "子类别"},
            {"edit.photo", "照片"},
            {"edit.date", "日期"},
            {"edit.btn-update", "更新"},
            {"edit.missing-info", "请输入所有资料。"},
            {"edit.delete-item", "删除家品"},
            {"edit.confirm-delete", "您是否确定要删除这件家品？"},
            // import furnitures
            {"imfu.import-csv", "从 CSV 汇入"},
            {"imfu.any-category", "任何类别"},
            {"imfu.any-subcategory", "任何子类别"},
            {"imfu.records-inserted", "笔纪录新增了"},
            {"imfu.records-updated", "笔纪录更新了"},
            {"imfu.records-failed", "笔纪录失败了"},
            {"imfu.invalid-csv", "无效的 CSV 档案"},
            {"imfu.import", "输入"},
            {"imfu.cancel", "取消"},
            // search store furnitures
            {"srch.title", "搜索店内家品"},
            {"srch.order-by", "排序"},
            {"srch.asc", "(升序)"},
            {"srch.desc", "(降序)"},
            {"srch.search", "搜索"},
            {"srch.reset", "重设"},
            {"srch.id-length-error", "必须为 10 位长"},
            {"srch.search-syntax-error", "无效的搜索语法"},
            // send delivery report
            {"sdrp.send", "发送到库存部门"},
            {"sdrp.choose-report", "请选择要发送的报告"},
            {"snrp.reorder-report-sent", "补货清单已发送"},
            {"snrp.empty-reorder", "暂无需要补货的家品"},
            // generate sales report
            {"gsrp.print", "列印销售报表"},
        };

    }
}
