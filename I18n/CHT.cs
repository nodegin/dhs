using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHS.I18n
{
    class CHT
    {

        public static Dictionary<string, string> Msg = new Dictionary<string, string>()
        {
            // metro base
            {"ui.back", "返回"},
            {"ui.minimize", "最小化"},
            {"ui.close", "關閉"},
            {"ui.logout", "登出"},
            {"ui.no-result", "找不到符合的結果"},
            {"ui.not-set", "(未設定)"},
            {"ui.info", "通知"},
            {"ui.warn", "警告"},
            {"ui.errr", "錯誤"},
            // login
            {"login.title", "登入"},
            {"login.staffid", "職員ID"},
            {"login.passwd", "密碼"},
            // main
            {"main.title", "主頁"},
            {"main.menu.store-furnitures", "店内傢俬"},
            {"main.category.management", "管理"},
            {"main.menu.delivery-report", "發送送遞報告"},
            {"main.menu.reorder-report", "發送補貨清單"},
            {"main.menu.sales-report", "產生銷售報表"},
            {"main.menu.import-furnitures", "輸入新傢俬"},
            // store furnitures
            {"stfu.action.reload", "重新整理"},
            {"stfu.action.search", "搜尋"},
            {"stfu.action.order-cart", "貨品訂單"},
            {"stfu.action.shopping-cart", "購物車"},
            {"stfu.qty", "數量"},
            {"stfu.shelf", "位置"},
            {"stfu.date", "日期"},
            {"stfu.continue-order", "店内已經沒有這件傢俬的存貨了\n是否要從貨倉進行訂購?"},
            {"stfu.out-of-stock", "店内及貨倉都已經沒有這件傢俬的存貨了"},
            // cart
            {"cart.checkout", "結帳"},
            {"cart.order", "訂購"},
            {"cart.action.clear-all", "全部清除"},
            {"cart.subtotal", "小計"},
            {"cart.discounted", "折扣"},
            {"cart.total", "總數"},
            {"cart.delivery", "需要送遞"},
            {"cart.cust-name", "客人名稱"},
            {"cart.cust-addr", "客人地址"},
            {"cart.cust-phone", "客人電話"},
            {"cart.cash", "現金"},
            {"cart.card", "信用卡"},
            {"cart.shopping-cart-empty", "購物車內沒有貨品"},
            {"cart.order-cart-empty", "沒有可以訂購的貨品"},
            {"cart.cust-info-empty", "請輸入客人的全部資料"},
            {"cart.make-payment", "付款"},
            {"cart.deposit", "訂金"},
            {"cart.cash-in", "收取"},
            {"cart.change", "找贖"},
            {"cart.pay-by-card", "使用信用卡付款"},
            {"cart.payment-ok", "付款成功"},
            {"cart.payment-card-phase-1", "請插入信用卡"},
            {"cart.payment-card-phase-2", "正在驗證信用卡"},
            {"cart.payment-card-phase-3", "連線至付款閘道"},
            {"cart.payment-card-phase-4", "授權中"},
            {"cart.mark-and-return", "標記為已付款並返回"},
            {"cart.print-invoice", "列印發票"},
            // furniture details + edit
            {"edit.title", "修改貨品資料"},
            {"edit.id", "編號"},
            {"edit.name", "貨品名"},
            {"edit.desc", "簡介"},
            {"edit.dimen", "尺寸 (吋)"},
            {"edit.price", "價格"},
            {"edit.discount", "折扣率"},
            {"edit.quantity", "存貨"},
            {"edit.rl", "補貨於"},
            {"edit.sl", "位置"},
            {"edit.category", "類別"},
            {"edit.subcategory", "子類別"},
            {"edit.photo", "照片"},
            {"edit.date", "日期"},
            {"edit.btn-update", "更新"},
            {"edit.missing-info", "請輸入所有資料."},
            {"edit.delete-item", "刪除貨品"},
            {"edit.confirm-delete", "您是否確定要刪除這件貨品?"},
            // import furnitures
            {"imfu.import-csv", "從 CSV 匯入"},
            {"imfu.any-category", "任何類別"},
            {"imfu.any-subcategory", "任何子類別"},
            {"imfu.records-inserted", "筆紀錄新增了"},
            {"imfu.records-updated", "筆紀錄更新了"},
            {"imfu.records-failed", "筆紀錄失敗了"},
            {"imfu.invalid-csv", "無效的 CSV 檔案"},
            {"imfu.import", "輸入"},
            {"imfu.cancel", "取消"},
            // search store furnitures
            {"srch.title", "搜尋店内傢俬"},
            {"srch.order-by", "排序"},
            {"srch.asc", "(升序)"},
            {"srch.desc", "(降序)"},
            {"srch.search", "搜尋"},
            {"srch.reset", "重設"},
            {"srch.id-length-error", "必須為 10 位長"},
            {"srch.search-syntax-error", "無效的搜尋語法"},
            // send delivery report
            {"sdrp.send", "發送至庫存部門"},
            {"sdrp.choose-report", "請選擇要發送的報告"},
            {"snrp.reorder-report-sent", "補貨清單已發送"},
            {"snrp.empty-reorder", "暫無需要補貨的貨品"},
            // generate sales report
            {"gsrp.print", "列印銷售報表"},
        };

    }
}
