import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { FiSearch, FiShoppingCart, FiUser, FiHeart, FiMenu } from "react-icons/fi"; // Import các icon hiện đại
import "./Header.css";

function Header() {
  const [searchQuery, setSearchQuery] = useState("");
  const navigate = useNavigate();

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      // Điều hướng sang trang shop kèm câu lệnh query tìm kiếm (tùy bạn cấu hình ở trang Shop)
      navigate(`/shop?search=${searchQuery}`);
    }
  };

  return (
    <header className="at-header">
      <div className="at-header-container">
        
        {/* 1. LOGO THƯƠNG HIỆU */}
        <div className="at-logo">
          <Link to="/">
            AT<span>Design</span>
          </Link>
        </div>

        {/* 2. THANH TÌM KIẾM (SEARCH BAR) */}
        <form className="at-search-bar" onSubmit={handleSearchSubmit}>
          <input
            type="text"
            placeholder="Tìm kiếm nội thất hiện đại..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="submit" aria-label="Search">
            <FiSearch className="search-icon" />
          </button>
        </form>

        {/* 3. MENU ĐIỀU HƯỚNG */}
        <nav className="at-nav-menu">
          <Link to="/">Trang chủ</Link>
          <Link to="/shop">Sản phẩm</Link>
          <Link to="/about">Giới thiệu</Link>
          <Link to="/contact">Liên hệ</Link>
        </nav>

        {/* 4. CỤM ICON CHỨC NĂNG */}
        <div className="at-header-actions">
          {/* Nút yêu thích */}
          <Link to="/wishlist" className="action-item" title="Yêu thích">
            <FiHeart />
            <span className="action-badge">0</span>
          </Link>

          {/* Giỏ hàng */}
          <Link to="/cart" className="action-item" title="Giỏ hàng">
            <FiShoppingCart />
            <span className="action-badge cart-count">3</span> {/* Số 3 ví dụ cho số sản phẩm trong giỏ */}
          </Link>

          {/* Tài khoản / Đăng nhập */}
          <Link to="/login" className="action-item account-link" title="Tài khoản">
            <FiUser />
            <span className="account-text">Đăng nhập</span>
          </Link>

          {/* Icon Menu phụ cho giao diện điện thoại */}
          <button className="mobile-menu-toggle" aria-label="Toggle Menu">
            <FiMenu />
          </button>
        </div>

      </div>
    </header>
  );
}

export default Header;