import { Link, useNavigate } from "react-router-dom";
import { FiTrash2, FiMinus, FiPlus, FiShoppingBag, FiArrowRight } from "react-icons/fi";
import { useCart } from "../context/CartContext";
import "./Cart.css";

const fmt = (n) => new Intl.NumberFormat("vi-VN").format(n) + "đ";

function Cart() {
  const { items, updateQty, removeItem, totalPrice } = useCart();
  const navigate = useNavigate();

  if (items.length === 0) {
    return (
      <div className="cart-empty">
        <FiShoppingBag className="cart-empty-icon" />
        <h2>Giỏ hàng trống</h2>
        <p>Bạn chưa có sản phẩm nào trong giỏ hàng.</p>
        <Link to="/shop" className="cart-btn-shop">Tiếp tục mua sắm</Link>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <div className="cart-container">
        <h1 className="cart-title">
          Giỏ hàng <span>({items.length} sản phẩm)</span>
        </h1>

        <div className="cart-layout">
          {/* Danh sách sản phẩm */}
          <div className="cart-items">
            <div className="cart-items-header">
              <span>Sản phẩm</span>
              <span>Số lượng</span>
              <span>Thành tiền</span>
              <span></span>
            </div>

            {items.map((item) => (
              <div className="cart-item" key={item.productId}>
                <div className="cart-item-info">
                  <div className="cart-item-img">
                    <img
                      src={item.imageUrl || "https://placehold.co/72x72?text=SP"}
                      alt={item.name}
                      onError={(e) => { e.target.src = "https://placehold.co/72x72?text=SP"; }}
                    />
                  </div>
                  <div>
                    <p className="cart-item-name">{item.name}</p>
                    <p className="cart-item-price">{fmt(item.price)}</p>
                  </div>
                </div>

                <div className="cart-item-qty">
                  <button className="qty-btn" onClick={() => updateQty(item.productId, item.quantity - 1)}>
                    <FiMinus />
                  </button>
                  <span className="qty-num">{item.quantity}</span>
                  <button className="qty-btn" onClick={() => updateQty(item.productId, item.quantity + 1)}>
                    <FiPlus />
                  </button>
                </div>

                <p className="cart-item-subtotal">{fmt(item.price * item.quantity)}</p>

                <button className="cart-item-remove" onClick={() => removeItem(item.productId)}>
                  <FiTrash2 />
                </button>
              </div>
            ))}
          </div>

          {/* Tóm tắt đơn hàng */}
          <aside className="cart-summary">
            <h2 className="cart-summary-title">Tóm tắt đơn hàng</h2>

            <div className="cart-summary-row">
              <span>Tạm tính</span>
              <span>{fmt(totalPrice)}</span>
            </div>
            <div className="cart-summary-row">
              <span>Phí vận chuyển</span>
              <span className="cart-free">Miễn phí</span>
            </div>
            <hr className="cart-divider" />
            <div className="cart-summary-row total">
              <span>Tổng cộng</span>
              <strong>{fmt(totalPrice)}</strong>
            </div>

            <button className="cart-btn-checkout" onClick={() => navigate("/checkout")}>
              Tiến hành đặt hàng <FiArrowRight />
            </button>
            <Link to="/shop" className="cart-btn-continue">← Tiếp tục mua sắm</Link>
          </aside>
        </div>
      </div>
    </div>
  );
}

export default Cart;
