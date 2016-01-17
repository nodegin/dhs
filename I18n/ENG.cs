using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHS.I18n
{
    class ENG
    {

        public static Dictionary<string, string> Msg = new Dictionary<string, string>()
        {
            // metro base
            {"ui.back", "Back"},
            {"ui.minimize", "Minimize"},
            {"ui.close", "Close"},
            {"ui.logout", "Logout"},
            {"ui.no-result", "No matching result"},
            {"ui.not-set", "(not set)"},
            {"ui.info", "Info"},
            {"ui.warn", "Warning"},
            {"ui.errr", "Error"},
            // login
            {"login.title", "Login"},
            {"login.staffid", "Staff ID"},
            {"login.passwd", "Password"},
            // main
            {"main.title", "Home"},
            {"main.menu.store-furnitures", "Store Furnitures"},
            {"main.category.management", "Management"},
            {"main.menu.delivery-report", "Send Delivery Report"},
            {"main.menu.reorder-report", "Send Reorder Report"},
            {"main.menu.sales-report", "Generate Sales Report"},
            {"main.menu.import-furnitures", "Import Furnitures"},
            // store furnitures
            {"stfu.action.reload", "Reload"},
            {"stfu.action.search", "Search"},
            {"stfu.action.order-cart", "Inventory orders"},
            {"stfu.action.shopping-cart", "Shopping cart"},
            {"stfu.qty", "Qty"},
            {"stfu.shelf", "Shelf"},
            {"stfu.date", "Date"},
            {"stfu.continue-order", "This funiture has out of stock from store.\nContinue order from inventory?"},
            {"stfu.out-of-stock", "This furniture has out of stock from both store and inventory."},
            // cart
            {"cart.checkout", "Checkout"},
            {"cart.order", "Order"},
            {"cart.action.clear-all", "Clear all"},
            {"cart.subtotal", "Subtotal"},
            {"cart.discounted", "Discounted"},
            {"cart.total", "Total amount"},
            {"cart.delivery", "Delivery item"},
            {"cart.cust-name", "Customer name"},
            {"cart.cust-addr", "Customer address"},
            {"cart.cust-phone", "Customer phone number"},
            {"cart.cash", "Cash"},
            {"cart.card", "Credit Card"},
            {"cart.shopping-cart-empty", "Shopping cart is empty"},
            {"cart.order-cart-empty", "Nothing to order"},
            {"cart.cust-info-empty", "Please enter customer informations."},
            {"cart.make-payment", "Make Payment"},
            {"cart.deposit", "Deposit amount"},
            {"cart.cash-in", "Cash in"},
            {"cart.change", "Change"},
            {"cart.pay-by-card", "Charge from credit card"},
            {"cart.payment-ok", "Payment successful"},
            {"cart.payment-card-phase-1", "Waiting for credit card"},
            {"cart.payment-card-phase-2", "Validating credit card"},
            {"cart.payment-card-phase-3", "Connecting to Payment Gateway"},
            {"cart.payment-card-phase-4", "Authorizing"},
            {"cart.mark-and-return", "Mark as paid and return menu"},
            {"cart.print-invoice", "Print invoice"},
            // furniture details + edit
            {"edit.title", "Edit Item Details"},
            {"edit.id", "ID"},
            {"edit.name", "Name"},
            {"edit.desc", "Description"},
            {"edit.dimen", "Dimension (inches)"},
            {"edit.price", "Price"},
            {"edit.discount", "Discount"},
            {"edit.quantity", "Quantity"},
            {"edit.rl", "Reorder Level"},
            {"edit.sl", "Shelf Location"},
            {"edit.category", "Category"},
            {"edit.subcategory", "Subcategory"},
            {"edit.photo", "Photo"},
            {"edit.date", "Date"},
            {"edit.btn-update", "Update"},
            {"edit.missing-info", "Please fill in all informations."},
            {"edit.delete-item", "Delete Item"},
            {"edit.confirm-delete", "Are you sure you want to delete this item?"},
            // import furnitures
            {"imfu.import-csv", "Import from CSV file"},
            {"imfu.any-category", "Any category"},
            {"imfu.any-subcategory", "Any subcategory"},
            {"imfu.records-inserted", "record(s) inserted"},
            {"imfu.records-updated", "record(s) updated"},
            {"imfu.records-failed", "record(s) failed"},
            {"imfu.invalid-csv", "Invalid CSV file"},
            {"imfu.import", "Import"},
            {"imfu.cancel", "Cancel"},
            // search store furnitures
            {"srch.title", "Search Store Furnitures"},
            {"srch.order-by", "Order by"},
            {"srch.asc", "(Ascending)"},
            {"srch.desc", "(Descending)"},
            {"srch.search", "Search"},
            {"srch.reset", "Reset"},
            {"srch.id-length-error", " must be 10 digit lengths"},
            {"srch.search-syntax-error", "Invalid search syntax."},
            // send delivery & reorder report
            {"snrp.send", "Send to Inventory Department"},
            {"snrp.choose-report", "Please choose the report which should be sent."},
            {"snrp.reorder-report-sent", "Reorder report have been sent"},
            {"snrp.empty-reorder", "Nothing to reorder"},
            // generate sales report
            {"gsrp.print", "Print report"},
            
        };

    }
}
