import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { FiMapPin, FiFileText, FiCheckCircle, FiAlertCircle, FiShoppingBag } from "react-icons/fi";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import orderService from "../services/orderService";
import "./Checkout.css";

const fmt = (n) => new Intl.NumberFormat("vi-VN").format(n) + "đ";

function Checkout() {
  const { items, totalPrice, clearCart } = useCart();
  const { user, isLoggedIn } = useAuth();
  const navigate = useNavigate();

  const [shippingAddress, setShippingAddress] = useState(user?.address || "");
  const [notes, setNotes] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [orderId, setOrderId] = useState(null);

  // Chưa đăng nhập
  if (!isLoggedIn) {
    return (
      <div className="checkout-gate">
        <FiShoppingBag className="checkout-gate-icon" />
        <h2>Vui lòng đăng nhập để đặt hàng</h2>
        <Link to="/login" state={{ from: "/checkout" }} className="checkout-btn-login">
          Đăng nhập ngay
        </Link>
      </div>
    );
  }

  // Giỏ trống
  if (items.length === 0 && !success) {
    return (
      <div className="checkout-gate">
        <FiShoppingBag className="checkout-gate-icon" />
        <h2>Giỏ hàng của bạn đang trống</h2>
        <Link to="/shop" className="checkout-btn-login">Tiếp tục mua sắm</Link>
      </div>
    );
  }

  // Đặt hàng thành công
  if (success) {
    return (
      <div className="checkout-success">
        <FiCheckCircle className="checkout-success-icon" />
        <h2>Đặt hàng thành công!</h2>
        <p>Đơn hàng <strong>#{orderId}</strong> của bạn đã được ghi nhận.<br />Chúng tôi sẽ liên hệ xác nhận sớm nhất.</p>
        <div className="checkout-success-actions">
          <Link to="/profile" className="checkout-btn-orders">Xem đơn hàng của tôi</Link>
          <Link to="/shop" className="checkout-btn-continue">Tiếp tục mua sắm</Link>
        </div>
      </div>
    );
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!shippingAddress.trim()) {
      setError("Vui lòng nhập địa chỉ nhận hàng."); return;
    }
    setLoading(true); setError("");
    try {
      const res = await orderService.checkout({
        shippingAddress,
        notes,
        items: items.map(i => ({ productId: i.productId, quantity: i.quantity })),
      });
      setOrderId(res.orderId);
      clearCart();
      setSuccess(true);
    } catch (err) {
      setError(err?.response?.data?.message || "Đặt hàng thất bại, vui lòng thử lại.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="checkout-page">
      <div className="checkout-container">
        <h1 className="checkout-title">Xác nhận đơn hàng</h1>

        <div className="checkout-layout">
          {/* Form bên trái */}
          <form className="checkout-form" onSubmit={handleSubmit}>
            {error && (
              <div className="checkout-error">
                <FiAlertCircle /> {error}
              </div>
            )}

            {/* Thông tin người nhận */}
            <div className="checkout-card">
              <h3 className="checkout-card-title">
                <FiMapPin /> Thông tin người nhận
              </h3>

              <div className="checkout-row">
                <div className="checkout-field">
                  <label>Họ và tên</label>
                  <input type="text" value={user?.fullName || ""} readOnly className="checkout-input readonly" />
                </div>
                <div className="checkout-field">
                  <label>Số điện thoại</label>
                  <input type="text" value={user?.phone || ""} readOnly className="checkout-input readonly" />
                </div>
              </div>

              <div className="checkout-field">
                <label>Địa chỉ nhận hàng <span className="req">*</span></label>
                <input
                  type="text"
                  className="checkout-input"
                  placeholder="Số nhà, tên đường, phường/xã, quận/huyện, tỉnh/thành..."
                  value={shippingAddress}
                  onChange={e => { setShippingAddress(e.target.value); setError(""); }}
                />
                <p className="checkout-field-hint">
                  Địa chỉ mặc định từ hồ sơ — bạn có thể sửa trực tiếp bên trên.
                </p>
              </div>
            </div>

            {/* Ghi chú */}
            <div className="checkout-card">
              <h3 className="checkout-card-title">
                <FiFileText /> Ghi chú đơn hàng
              </h3>
              <textarea
                className="checkout-input checkout-textarea"
                rows={3}
                placeholder="Giao giờ hành chính, gọi trước khi giao, để tại cổng..."
                value={notes}
                onChange={e => setNotes(e.target.value)}
              />
            </div>

            <button type="submit" className="checkout-btn-submit" disabled={loading}>
              {loading ? <span className="checkout-spinner" /> : "Xác nhận đặt hàng"}
            </button>
          </form>

          {/* Tóm tắt đơn bên phải */}
          <aside className="checkout-summary">
            <h3 className="checkout-summary-title">Đơn hàng ({items.length} sản phẩm)</h3>

            <div className="checkout-product-list">
              {items.map(item => (
                <div className="checkout-product" key={item.productId}>
                  <div className="checkout-product-img">
                    <img
                      src={item.imageUrl || "https://placehold.co/56x56?text=SP"}
                      alt={item.name}
                      onError={e => { e.target.src = "https://placehold.co/56x56?text=SP"; }}
                    />
                    <span className="checkout-qty-badge">{item.quantity}</span>
                  </div>
                  <div className="checkout-product-info">
                    <p className="checkout-product-name">{item.name}</p>
                    <p className="checkout-product-price">{fmt(item.price)} × {item.quantity}</p>
                  </div>
                  <p className="checkout-product-sub">{fmt(item.price * item.quantity)}</p>
                </div>
              ))}
            </div>

            <hr className="checkout-divider" />
            <div className="checkout-total-row"><span>Tạm tính</span><span>{fmt(totalPrice)}</span></div>
            <div className="checkout-total-row"><span>Vận chuyển</span><span className="free">Miễn phí</span></div>
            <hr className="checkout-divider" />
            <div className="checkout-total-row grand"><span>Tổng cộng</span><strong>{fmt(totalPrice)}</strong></div>
          </aside>
        </div>
      </div>
    </div>
  );
}

export default Checkout;
